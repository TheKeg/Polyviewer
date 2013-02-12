using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;


namespace ModelViewer
{
	/// <summary>
	/// Class to handle rendering and interaction with the camera and light classes.
	/// </summary>
	public class ResourceManager
	{
		private Camera camera;							// Class to store the camera information.
		private Device device;							// The rendering device
		private Effect effect;							// Class to handle shaders.
		private List<LightObj> lights;				// List of available lights.
		private List<Model> mesh;						// List of loaded models.
		private PresentParameters presentParams;	// Presentation parameters for initalizing the rendering device.
		private Render renderer;						// Class to render the scene.
		private GroundPlane groundPlane;
		
		/// <summary>
		/// Method to setup the rendering device.
		/// Also initalizes all other variables used.
		/// </summary>
		/// <param name="panel">Panel to be used for rendering.</param>
		public ResourceManager(Panel panel)
		{
			setupDevice(panel);

			camera = new Camera(ref device);
			camera.SetCamera((float)panel.Width, (float)panel.Height);

			mesh = new List<Model>();
			lights = new List<LightObj>();

			// Add two blank meshes. One for the ground plane and one for the first model loaded.
			groundPlane = new GroundPlane(device, effect);
			mesh.Add(new Model(device, effect));

			// Add the 3 lights.
			lights.Add(new LightObj(device, effect, true));
			lights.Add(new LightObj(device, effect, false));
			lights.Add(new LightObj(device, effect, false));

			renderer = new Render(ref device, ref effect, ref mesh, ref groundPlane, panel.Width, panel.Height);
		}

		#region Setup Methods

		/// <summary>
		/// Defines the presentation parameters.
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		private void setupPresentParams(int width, int height, Panel panel)
		{
			presentParams = new PresentParameters();
			presentParams.BackBufferWidth = width;
			presentParams.BackBufferHeight = height;
			presentParams.SwapEffect = SwapEffect.Discard;
			presentParams.Windowed = true;
			presentParams.AutoDepthStencilFormat = DepthFormat.D16;
			presentParams.BackBufferFormat = Format.X8R8G8B8;
			presentParams.EnableAutoDepthStencil = true;
			presentParams.DeviceWindowHandle = panel.Handle;
		}

		/// <summary>
		/// Creates the rendering device.
		/// </summary>
		/// <param name="panel"></param>
		public void setupDevice(Panel panel)
		{
			setupPresentParams(panel.Width, panel.Height, panel);
			
			// Creates the device.
			device = new Device(0, DeviceType.Hardware, panel.Handle, CreateFlags.HardwareVertexProcessing, presentParams);
			
			// Enables various renderstates.
			SetRenderState();
			setupEffect();

			device.DeviceReset += new EventHandler(device_DeviceReset);
			device.DeviceResizing += new System.ComponentModel.CancelEventHandler(device_DeviceResizing);
		}

		/// <summary>
		/// Loads the shader file from disk and sets the technique.
		/// </summary>
		private void setupEffect()
		{
			string errors;

			effect = Effect.FromString(device, global::ModelViewer.Properties.Resources.Shader, null, null, ShaderFlags.None, null, out errors);
	
			Console.WriteLine(errors);	// Prints out any errors, used for debugging.

			effect.Technique = "PerPixel";	// Sets the technique to be used.
		}

		/// <summary>
		/// Sets up the distance for the camera and light.
		/// </summary>
		public void setupZoom()
		{
			float zoomLevel = 0;
			float minValue = int.MaxValue;
			float maxValue = int.MinValue;
			float midPoint = 0;

			if (mesh[0].Mesh.NumberFaces > 0)
			{
				for (int i = 0; i < mesh.Count; i++)
				{
					if (!mesh[i].Enabled)
						continue;

					if (mesh[i].Min < minValue)
						minValue = mesh[i].Min;

					if (mesh[i].Max > maxValue)
						maxValue = mesh[i].Max;

					midPoint += mesh[i].calcMidPoint();
				}

				zoomLevel = (maxValue + Math.Abs(minValue)) * 2.0f;

				if (zoomLevel == 0.0f)
					zoomLevel = 1.0f;

				midPoint /= mesh.Count;

				camera.Offset = new Vector3(0, midPoint , 0);	// Offsets the camera to be in the middle of the object.
				camera.ZoomCamera(zoomLevel);											// Sets the camera's distance.

				for (int i = 0; i < 3; i++)
				{
					lights[i].Distance = zoomLevel * 2.0f;
				}

				groundPlane.Size = zoomLevel;
				groundPlane.MinValue = minValue;

				groundPlane.CreateGroundPlane(minValue, zoomLevel, 1.0f);
			}
		}

		/// <summary>
		/// Set The various values of the render state.
		/// </summary>
		private void SetRenderState()
		{
			device.RenderState.CullMode = Cull.None;
			device.RenderState.FillMode = FillMode.Solid;
			device.RenderState.Lighting = true;
			device.RenderState.ZBufferEnable = true;
			device.RenderState.ZBufferWriteEnable = true;
			device.RenderState.ShadeMode = ShadeMode.Gouraud;
			device.RenderState.AntiAliasedLineEnable = true;
			
		}

		/// <summary>
		/// Event handler for when the rendering device is resized.
		/// </summary>
		public void device_DeviceResizing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			adjustCamera(presentParams.BackBufferWidth, presentParams.BackBufferHeight);

			try
			{
				renderer.SceneRender.OnLostDevice();
				renderer.ShadowRender.OnLostDevice();

				renderer.SceneRender.Dispose();
				renderer.ShadowRender.Dispose();

				renderer.ReleaseRenderTextures();

				device.Reset(presentParams);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		/// <summary>
		/// Event handler for when the rendering device is lost.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void device_DeviceReset(object sender, EventArgs e)
		{
			try
			{
				device.TestCooperativeLevel();

				adjustCamera(presentParams.BackBufferWidth, presentParams.BackBufferHeight);

				if (presentParams.BackBufferWidth > 0 && presentParams.BackBufferHeight > 0)
				{
					renderer.RebuildRenderTargets(presentParams.BackBufferWidth, presentParams.BackBufferHeight);
				}

				renderer.RebuildShadowMaps(renderer.ShadowSize);

				SetRenderState();
			}
			catch (DeviceLostException)
			{
			}
			catch (DeviceNotResetException)
			{
				try
				{
					adjustCamera(presentParams.BackBufferWidth, presentParams.BackBufferHeight);

					device.Reset(presentParams);

					SetRenderState();
				}
				catch (DeviceLostException)
				{
				}
			}
		}

		#endregion

		#region Set/Get Methods

		/// <summary>
		/// Get Method for the rendering device.
		/// </summary>
		public Device Device
		{
			get
			{
				return device;
			}
			set
			{
				device = value;
			}
		}

		/// <summary>
		/// Get method for the effect file.
		/// </summary>
		public Effect Effect
		{
			get
			{
				return effect;
			}
		}

		public PresentParameters PresentParams
		{
			get
			{
				return presentParams;
			}
			set
			{
				presentParams = value;
			}
		}

		public bool ShowLights
		{
			get
			{
				return renderer.Lights;
			}
			set
			{
				renderer.Lights = value;
			}
		}

		/// <summary>
		/// Get method for the light
		/// </summary>
		public LightObj GetLight(int num)
		{
			return lights[num];
		}

		/// <summary>
		/// Set/Get method for the camera
		/// </summary>
		public Camera Camera
		{
			get
			{
				return camera;
			}
			set
			{
				camera = value;
			}
		}

		/// <summary>
		/// Set method for the model.
		/// </summary>
		public int ModelCount
		{
			get
			{
				return mesh.Count;
			}
		}

		public GroundPlane GroundPlane
		{
			get
			{
				return groundPlane;
			}
		}

		/// <summary>
		/// Method to get a model from the model list.
		/// </summary>
		/// <param name="num">Index into the list of models.</param>
		public Model getModel(int num)
		{
			return mesh[num];
		}

		public Render Renderer
		{
			get
			{
				return renderer;
			}
		}

		#endregion

		/// <summary>
		/// Method to remove all models from the list.
		/// </summary>
		public void ClearModels()
		{
			for (int i = 0; i < mesh.Count; i++)
			{
				// Dispose the model.
				if (!mesh[i].Mesh.Disposed)
					mesh[i].Mesh.Dispose();
			}

			// Clear the list and add 2 empty models.
			mesh.Clear();
			mesh.Add(new Model(device, effect));
			mesh.Add(new Model(device, effect));

			// Reset the lights.
			for(int i = 0; i < lights.Count; i++)
				lights[i].resetLight();

			// Disable the 2nd and 3rd lights.
			lights[1].Enabled = false;
			lights[2].Enabled = false;
		}

		/// <summary>
		/// Method to add a new model.
		/// </summary>
		/// <param name="newModel">New model to add.</param>
		public void addNewModel(Model newModel)
		{
			mesh.Add(newModel);
		}
		
		/// <summary>
		/// Adjust the aspect ratio of the camera.
		/// </summary>
		/// <param name="width">New width of the panel.</param>
		/// <param name="height">New height of the panel.</param>
		public void adjustCamera(int width, int height)
		{	
			presentParams.BackBufferWidth = width;
			presentParams.BackBufferHeight = height;

			camera.SetCamera((float)width, (float)height);
		}

		/// <summary>
		/// Render the scene.
		/// </summary>
		/// <param name="background"></param>
		public void RenderScene(Color background)
		{
			renderer.RenderScene(background, lights, camera);
		}

		/// <summary>
		/// Take a screenshot of the current scene.
		/// </summary>
		/// <param name="path">Path to save the file to.</param>
		/// <param name="format">Texture format to save the screenshot as.</param>
		/// <param name="background">Background color.</param>
		public void screenshot(string path, ImageFileFormat format, Color background)
		{
			// Create the surface to render to.
			Surface screenshot = device.CreateRenderTarget(presentParams.BackBufferWidth, presentParams.BackBufferHeight,
																		  Format.A8R8G8B8, MultiSampleType.None, 0, true);
			// Get the current render target.
			Surface defaultSurface = device.GetRenderTarget(0);

			// Set the screenshot surface as the render target.
			device.SetRenderTarget(0, screenshot);

			RenderScene(background);
			
			// Save the screenshot.
			SurfaceLoader.Save(path, format, screenshot);

			// Restore the render target.
			device.SetRenderTarget(0, defaultSurface);
		}

		/// <summary>
		/// Cleans up local variables to the class.
		/// </summary>
		public void Dispose()
		{
			try
			{
				for (int i = 0; i < mesh.Count; i++)
				{
					if (!mesh[i].Path.Equals(""))
						mesh[i].Dispose();
				}

				effect.Dispose();
				device.Dispose();
			}
			catch (NullReferenceException)
			{ }
		}
	}
}
