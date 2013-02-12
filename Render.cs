using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace ModelViewer
{
	/// <summary>
	/// Class to contain the methods for rendering the scene.
	/// </summary>
	public class Render
	{
		private Device device;					// Copy of the rendering device.
		private Device oldDevice;
		private Effect effect;					// Copy of the effect class.
		private Texture renderedGlow;			// Texture to render the emissive portions to.
		private Texture renderedScene;		// Texture to render the regular scene (used only for blending with glow).
		private Texture shadowMap;				// Texture to render the shadow map to.
		private Texture shadowMap2;			// Texture to render the shadow map to.
		private Texture shadowMap3;			// Texture to render the shadow map to.
		private Texture lightTexture;
		private List<Model> mesh;				// A reference to the list of models.
		private RenderToSurface surfRender;	// Class to help render emissive passes.
		private RenderToSurface shadowHelp;	// Call to render to the shadow map.
		private EffectHandle prevTechnique;	// The previous technique for the shaders.
		private int width;						// Width of the screen.
		private int height;						// Height of the screen.
		private int shadowMapSize;				// Size of the shadow map
		private bool showLights;
		private GroundPlane groundPlane;
		private Material blackMat;
		private Material whiteMat;

		/// <summary>
		/// Constructor to initalize references and build the render targets.
		/// </summary>
		/// <param name="device">Copy of the rendering device.</param>
		/// <param name="effect">Copy of the effect class.</param>
		/// <param name="mesh">Reference of the list of models.</param>
		/// <param name="width">Width of the rendering panel.</param>
		/// <param name="height">Height of the rendering panel.</param>
		public Render(ref Device device, ref Effect effect, ref List<Model> mesh, ref GroundPlane groundPlane, int width, int height)
		{
			this.device = device;
			this.effect = effect;
			this.mesh = mesh;
			this.groundPlane = groundPlane;
			this.width = width;
			this.height = height;

			showLights = false;

			shadowMapSize = 512;

			blackMat = new Material();
			whiteMat = new Material();

			blackMat.Ambient = blackMat.Diffuse = blackMat.Emissive = Color.Black;
			whiteMat.Ambient = whiteMat.Diffuse = whiteMat.Emissive = Color.White;

			// Setup the RenderToSurface class and create the render target textures.
			surfRender = new RenderToSurface(device, width, height, Format.X8R8G8B8, true, DepthFormat.D16);
			renderedGlow = new Texture(device, width, height, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
			renderedScene = new Texture(device, width, height, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

			shadowHelp = new RenderToSurface(device, shadowMapSize, shadowMapSize, Format.X8R8G8B8, true, DepthFormat.D16);
			shadowMap = new Texture(device, shadowMapSize, shadowMapSize, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
			shadowMap2 = new Texture(device, shadowMapSize, shadowMapSize, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
			shadowMap3 = new Texture(device, shadowMapSize, shadowMapSize, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);

         //lightTexture = TextureLoader.FromFile(device, "lightbulb.png");

         MemoryStream loMS = new MemoryStream();
         global::ModelViewer.Properties.Resources.lightbulb.Save(loMS, System.Drawing.Imaging.ImageFormat.Png);
         loMS.Seek(0, 0);

         lightTexture = TextureLoader.FromStream(device, loMS);         
		}

		#region Set/Get Methods

		/// <summary>
		/// Set/Get method for the width.
		/// </summary>
		public int DeviceWidth
		{
			get
			{
				return width;
			}
			set
			{
				width = value;
			}
		}

		/// <summary>
		/// Set/Get method for the height.
		/// </summary>
		public int DeviceHeight
		{
			get
			{
				return height;
			}
			set
			{
				height = value;
			}
		}

		public bool Lights
		{
			get
			{
				return showLights;
			}
			set
			{
				showLights = value;
			}
		}

		public Device Device
		{
			set
			{
				oldDevice = device;
				device = value;

				RebuildRenderTargets(width, height);
				RebuildShadowMaps(shadowMapSize);
			}
		}

		public Effect Effect
		{
			set
			{
				effect = value;
			}
		}

		public int ShadowSize
		{
			get
			{
				return shadowMapSize;
			}
			set
			{
				shadowMapSize = value;

				RebuildShadowMaps(value);
			}
		}

		public RenderToSurface SceneRender
		{
			get
			{
				return surfRender;
			}
		}

		public RenderToSurface ShadowRender
		{
			get
			{
				return shadowHelp;
			}
		}

		#endregion

		/// <summary>
		/// Rebuilds the render targets after resizing.
		/// </summary>
		/// <param name="width">The new width of the rendering panel.</param>
		/// <param name="height">The new height of the rendering panel.</param>
		public void RebuildRenderTargets(int width, int height)
		{
			surfRender.Dispose();
			renderedGlow.Dispose();
			renderedScene.Dispose();

			surfRender = new RenderToSurface(device, width, height, Format.X8R8G8B8, true, DepthFormat.D16);
			renderedGlow = new Texture(device, width, height, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
			renderedScene = new Texture(device, width, height, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
		}

		public void RebuildShadowMaps(int shadowMapSize)
		{
			shadowHelp.Dispose();
			shadowMap.Dispose();
			shadowMap2.Dispose();
			shadowMap3.Dispose();

			shadowHelp = new RenderToSurface(device, shadowMapSize, shadowMapSize, Format.X8R8G8B8, true, DepthFormat.D16);
			shadowMap = new Texture(device, shadowMapSize, shadowMapSize, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
			shadowMap2 = new Texture(device, shadowMapSize, shadowMapSize, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
			shadowMap3 = new Texture(device, shadowMapSize, shadowMapSize, 1, Usage.RenderTarget, Format.X8R8G8B8, Pool.Default);
		}

		public void ReleaseRenderTextures()
		{
			if (!renderedGlow.Disposed)
				renderedGlow.Dispose();

			if (!renderedScene.Disposed)
				renderedScene.Dispose();

			if (!shadowMap.Disposed)
				shadowMap.Dispose();
			if (!shadowMap2.Disposed)
				shadowMap2.Dispose();
			if (!shadowMap3.Disposed)
				shadowMap3.Dispose();
		}

		/// <summary>
		/// Method to prepare the scene for rendering.
		/// </summary>
		/// <param name="background">Color of the background.</param>
		public void BeginRender(Color background)
		{
			device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, background, 1.0f, 0);
			device.BeginScene();
		}

		/// <summary>
		/// Method to signal the end of rendering so the frame can be displayed to the screen.
		/// </summary>
		public void EndRender()
		{
			device.EndScene();
			device.Present();
		}

		/// <summary>
		/// Renders the scene.
		/// </summary>
		public void RenderScene(Color background, List<LightObj> lights, Camera camera)
		{
			bool useGlow = false;

			// Sets the variables for the shader.
			SetVariables(lights, camera);
			//effect.Technique = "Test";

			for (int i = 0; i < mesh.Count; i++)
			{
				if (mesh[i].Emissive.Enable && !mesh[i].Emissive.Path.Equals(""))
				{
					useGlow = true;
					break;
				}
			}

			// If/Else statement to control rendering with emissive glow or not.
			if (useGlow)
				RenderGlow();
			else
				BeginRender(background);

			// If emissive glow is used, the base scene is rendered to a texture.
			if (useGlow)
			{
				surfRender.BeginScene(renderedScene.GetSurfaceLevel(0), device.Viewport);
				device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, background, 1.0f, 0);
			}

			if (groundPlane.Enabled)
				RenderModel((Model)groundPlane, lights[0].Shadows, 0);

			// Loops through all the models and renders each.
			for (int i = 0; i < mesh.Count; i++)
			{
				if (mesh[i].Enabled)
					RenderModel(mesh[i], lights[0].Shadows, 0);
			}

			if (showLights)
			{
				using (Sprite spriteobject = new Sprite(device))
				{
					foreach (LightObj light in lights)
					{
						if (!light.Enabled)
							continue;

						spriteobject.SetWorldViewRH(device.Transform.World, device.Transform.View);

						spriteobject.Begin(SpriteFlags.AlphaBlend | SpriteFlags.Billboard | SpriteFlags.SortTexture | SpriteFlags.ObjectSpace);
						//spriteobject.Transform = Matrix.Scaling(0.25f, 0.25f, 0.25f);
						spriteobject.Draw(lightTexture, Rectangle.Empty, Vector3.Empty, light.Direction, Color.White);
						spriteobject.End();
					}
				}
			}

			// If emissive glow is used, the textures are set to the shader and rendered to a sprite.
			if (useGlow)
			{
				surfRender.EndScene(Filter.None);

				effect.SetValue("renderedScene", renderedScene);

				BeginRender(background);

				using (Sprite spriteobject = new Sprite(device))
				{
					prevTechnique = effect.Technique;
					effect.Technique = "Bloom";

					effect.Begin(FX.None);

					spriteobject.Begin(SpriteFlags.None);

					effect.BeginPass(0);

					spriteobject.Draw(renderedGlow, Rectangle.Empty, new Vector3(0, 0, 0), new Vector3(0, 0, 0), Color.White);

					effect.EndPass();

					spriteobject.End();

					effect.End();

					effect.Technique = prevTechnique;
				}
			}

			EndRender();
		}

		/// <summary>
		/// Sets the variables within the shader code.
		/// </summary>
		/// <param name="light">List of lights used.</param>
		/// <param name="camera">The camera.</param>
		private void SetVariables(List<LightObj> light, Camera camera)
		{
			// Create an array of the 3 lights intensities.
			float[] lightIntensity = new float[3];
			int[] enableShadows = new int[3];
			int[] enableLights = new int[3];

			for (int i = 0; i < 3; i++)
			{
				lightIntensity[i] = light[i].Intensity;
				enableShadows[i] = Convert.ToInt32(light[i].Shadows);
				enableLights[i] = Convert.ToInt32(light[i].Enabled);
			}

			// Create an array of the 3 lights positions.
			Vector4[] lightPositions = new Vector4[3];
			for (int i = 0; i < 3; i++)
				lightPositions[i] = new Vector4(light[i].Direction.X, light[i].Direction.Y, light[i].Direction.Z, 1.0f);

			Vector4[] lightAmbient = new Vector4[3];
			Vector4[] lightDiffuse = new Vector4[3];
			Vector4[] lightSpecular = new Vector4[3];

			// Copy over the light colors to different vector4's
			for (int i = 0; i < 3; i++)
			{
				lightAmbient[i] = new Vector4(light[i].Ambient.R / 255.0f, light[i].Ambient.G / 255.0f, light[i].Ambient.B / 255.0f, 1.0f);
				lightDiffuse[i] = new Vector4(light[i].Diffuse.R / 255.0f, light[i].Diffuse.G / 255.0f, light[i].Diffuse.B / 255.0f, 1.0f);
				lightSpecular[i] = new Vector4(light[i].Specular.R / 255.0f, light[i].Specular.G / 255.0f, light[i].Specular.B / 255.0f, 1.0f);
			}

			// Set the world, project and view matrixes in the shader
			effect.SetValue("projMat", camera.Projection);
			effect.SetValue("viewMat", camera.View);
			effect.SetValue("worldMat", Matrix.Identity);
			
			// Set the emissive texture
			effect.SetValue("glowMap", renderedGlow);

			// Sets the light and camera's position in the shader.
			effect.SetValue("lightPos", lightPositions);
			effect.SetValue("enableLight", enableLights);
			effect.SetValue("enableShadows", enableShadows);
			effect.SetValue("eyePos", new float[] { camera.Position.X, camera.Position.Y, camera.Position.Z });

			// Setup the light information
			effect.SetValue("lightIntensity", lightIntensity);
			effect.SetValue("lightAmbientColor", lightAmbient);
			effect.SetValue("lightDiffuseColor", lightDiffuse);
			effect.SetValue("lightSpecularColor", lightSpecular);
		}

		/// <summary>
		/// Method to set the texture values within the effect class and render the model.
		/// </summary>
		/// <param name="num">The index into the list of models.</param>
		private void RenderModel(Model mesh, bool shadows, int num)
		{
			// Set the technique being used for this particular model.
			if (shadows)
			{
				if (mesh.Normal.Enable && !mesh.Normal.Path.Equals(""))
				{
					if (Manager.GetDeviceCaps(0, DeviceType.Hardware).PixelShaderVersion > new Version(2, 0))
						effect.Technique = "NormalMapShadows";
					else
					{
						MessageBox.Show("Error: Your graphics card does not support pixel shader 2.0b", "Error",
											 MessageBoxButtons.OK, MessageBoxIcon.Error);
						effect.Technique = "PerPixelShadows";
					}
				}
				else
					effect.Technique = "PerPixelShadows";
			}
			else
				effect.Technique = mesh.Normal.Technique;
			
			// Set the Ambient Occlusion texture.
			effect.SetValue(mesh.Ambient.EffectName, mesh.Ambient.Texture);
			effect.SetValue(mesh.Ambient.EffectBool, mesh.Ambient.Enable);

			// Set the Diffuse texture.
			effect.SetValue(mesh.Diffuse.EffectName, mesh.Diffuse.Texture);
			effect.SetValue(mesh.Diffuse.EffectBool, mesh.Diffuse.Enable);

			// Set the Normal/Parallax texture.
			effect.SetValue(mesh.Normal.EffectName, mesh.Normal.Texture);
			effect.SetValue(mesh.Normal.EffectBool, mesh.Normal.Enable);

			// Set the Specular texture.
			effect.SetValue(mesh.Specular.EffectName, mesh.Specular.Texture);
			effect.SetValue(mesh.Specular.EffectBool, mesh.Specular.Enable);

			// Begin rendering with the hlsl shader.
			int numPasses = effect.Begin(FX.None);

			for (int i = 0; i < numPasses; i++)
			{
				if (shadows)
				{
					switch (num)
					{
						case 0:
							effect.SetValue("shadowMapLight1", shadowMap);
							break;

						case 1:
							effect.SetValue("shadowMapLight2", shadowMap2);
							break;

						case 2:
							effect.SetValue("shadowMapLight3", shadowMap3);
							break;
					}
				}

				effect.BeginPass(i);

				// Draw the model.
				mesh.Mesh.DrawSubset(0);

				// End drawing with the hlsl shader.
				effect.EndPass();
			}

			effect.End();
		}

		/// <summary>
		/// Method to render the emissive pass to a render target.
		/// </summary>
		public void RenderGlow()
		{
			surfRender.BeginScene(renderedGlow.GetSurfaceLevel(0), device.Viewport);
			device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

			if (groundPlane.Enabled)
			{
				if (!groundPlane.Emissive.Enable)
					device.Material = blackMat;
				else
					device.Material = whiteMat;

				device.SetTexture(0, groundPlane.Emissive.Texture);
				groundPlane.Mesh.DrawSubset(0);
			}

			for (int i = 0; i < mesh.Count; i++)
			{
				if (!mesh[i].Enabled)
					continue;

				if(!mesh[i].Emissive.Enable)
					device.Material = blackMat;
				else
					device.Material = whiteMat;

				device.SetTexture(0, mesh[i].Emissive.Texture);
				mesh[i].Mesh.DrawSubset(0);
			}

			surfRender.EndScene(Filter.None);
		}

		/// <summary>
		/// Method to build shadow maps.
		/// </summary>
		public void RenderShadowMap(LightObj light, int num, Matrix projection)
		{
			Matrix lightMatrix = Matrix.LookAtRH(light.Direction, new Vector3(0, 0, 0), new Vector3(0, 1, 0)) * projection;

			switch (num)
			{
				case 0:
					shadowHelp.BeginScene(shadowMap.GetSurfaceLevel(0));//, device.Viewport);
					break;

				case 1:
					shadowHelp.BeginScene(shadowMap2.GetSurfaceLevel(0), device.Viewport);
					break;

				case 2:
					shadowHelp.BeginScene(shadowMap3.GetSurfaceLevel(0), device.Viewport);
					break;
			}

			device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);

			effect.Technique = "ShadowMap";
			effect.SetValue("maxDepth", light.Distance * 3);
			effect.SetValue("lightWVP", lightMatrix);

			effect.Begin(FX.None);
			effect.BeginPass(0);

			if (groundPlane.Enabled)
				groundPlane.Mesh.DrawSubset(0);

			for (int i = 0; i < mesh.Count; i++)
			{
				if (mesh[i].Enabled)
					mesh[i].Mesh.DrawSubset(0);
			}

			effect.EndPass();
			effect.End();

			shadowHelp.EndScene(Filter.Point);

//			shadowMap = shadowTest.

			switch (num)
			{
				case 0:
					effect.SetValue("shadowMapLight1", shadowMap);
					break;

				case 1:
					effect.SetValue("shadowMapLight2", shadowMap2);
					break;

				case 2:
					effect.SetValue("shadowMapLight3", shadowMap3);
					break;
			}
			
		}
	}
}
