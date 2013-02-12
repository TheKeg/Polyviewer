using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing.Design;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace ModelViewer
{
	/// <summary>
	/// Class to handle loading images from the property grid.
	/// </summary>
	internal class FilteredFileNameEditor : UITypeEditor
	{
		private OpenFileDialog ofd = new OpenFileDialog();

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			ofd.FileName = value.ToString();
			ofd.Filter = "Image Files (*.BMP;*.DDS;*.JPG;*.PNG;*.TGA)|*.BMP;*.JPG;*.DDS;*.PNG;*.TGA";
			ofd.Filter += "|Bitmap Files (.BMP)|*.BMP";
			ofd.Filter += "|DirectX Files (.DDS)|*.DDS";
			ofd.Filter += "|Jpeg Files (.JPG)|*.JPG";
			ofd.Filter += "|PNG Files (.PNG)|*.PNG";
			ofd.Filter += "|Target Files (.TGA)|*.TGA";
			ofd.Filter += "|All files (*.*)|*.*";

			if (ofd.ShowDialog() == DialogResult.OK)
			{
				return ofd.FileName;
			}
			
			return base.EditValue(context, provider, value);
		}
	}

	/// <summary>
	/// Class to store a texture and various properties stored with it.
	/// </summary>
	[DefaultPropertyAttribute("Texture")]
	public class TextureClass
	{
		protected string pathName;				// Path to where the file is located.
		protected string textureEffectName;	// String used for setting the texture in the shader.
		protected string textureEffectBool; // String used for setting whether the texture is to be used in the shader.
		protected Texture texture;				// The texture.
		protected bool enableTexture;			// Bool for if the texture is being used or not
		protected Effect effect;				// Copy of the effect for handling the shader.
		protected Device device;				// Copy of the rendering device.
		protected DateTime modified;			// DateTime the texture was last modified.
		protected EffectHandle technique;	// Technique being used for the model the texture is assigned to.

		/// <summary>
		/// Default constructor for use with child classes.
		/// </summary>
		public TextureClass()
		{
			enableTexture = false;
			technique = "PerPixel";
		}

		/// <summary>
		/// Primary Constructor.
		/// </summary>
		/// <param name="device">Copy of the device.</param>
		/// <param name="effect">Copy of the effect.</param>
		/// <param name="effectName">Name used for setting the texture in the shader.</param>
		/// <param name="effectBool">Name used for toggling the use of the texture in the shader.</param>
		public TextureClass(Device device, Effect effect, string effectName, string effectBool)
		{
			pathName = "";
			this.device = device;
			this.effect = effect;
			textureEffectName = effectName;
			textureEffectBool = effectBool;
			enableTexture = false;
			technique = "PerPixel";
		}

		#region Set/Get Methods.

		/// <summary>
		/// Get method for the technique assigned to this class.
		/// </summary>
		[Browsable(false)]
		public EffectHandle Technique
		{
			get
			{
				return technique;
			}
		}

		[Browsable(false)]
		public Device Device
		{
			set
			{
				device = value;

				if(!pathName.Equals(""))
					texture = TextureLoader.FromFile(value, pathName, 0, 0, 3, Usage.None, Format.A8B8G8R8, 
																Pool.Managed, Filter.Linear, Filter.Box, 0);
			}
		}

		[Browsable(false)]
		public Effect Effect
		{
			set
			{
				effect = value;
			}
		}

		/// <summary>
		/// Get method for the texture variable name within the shader.
		/// </summary>
		[Browsable(false)]
		public string EffectName
		{
			get
			{
				return textureEffectName;
			}
		}

		/// <summary>
		/// Get method for the texture variable bool within the shader.
		/// </summary>
		[Browsable(false)]
		public string EffectBool
		{
			get
			{
				return textureEffectBool;
			}
		}

		/// <summary>
		/// Set/Get Method for the texture
		/// </summary>
		[Browsable(false)]
		public Texture Texture
		{
			get
			{
				return texture;
			}
			set
			{
				texture = value;
				effect.SetValue(textureEffectName, texture);
				effect.SetValue(textureEffectBool, true);
				enableTexture = true;
				modified = new FileInfo(pathName).LastWriteTime;
			}
		}

		/// <summary>
		/// Set/Get method for the path name. Set method also handles loading in a texture.
		/// </summary>
		[Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor)),
		 CategoryAttribute("Path"), DescriptionAttribute("Path of the texture.")]
		public virtual string Path
		{
			get
			{
				return pathName;
			}
			set
			{
				pathName = value;

				if (!pathName.Equals(""))
				{
					texture = TextureLoader.FromFile(device, value, 0, 0, 3, Usage.None, Format.A8B8G8R8, Pool.Managed, Filter.Linear, Filter.Box, 0);
					effect.SetValue(textureEffectName, texture);
					effect.SetValue(textureEffectBool, true);
					enableTexture = true;
					modified = new FileInfo(pathName).LastWriteTime;
				}
				else
				{
					Enable = false;
					modified = new DateTime();
				}
			}
		}

		/// <summary>
		/// Get method for the last time this texture was modified.
		/// </summary>
		[CategoryAttribute("Path"), DescriptionAttribute("Time the texture was last modified")]
		public DateTime LastModified
		{
			get
			{
				return modified;
			}
		}

		/// <summary>
		/// Set/Get method for whether the texture is enabled or not.
		/// </summary>
		[Browsable(false)]
		public virtual bool Enable
		{
			get
			{
				return enableTexture;
			}
			set
			{
				enableTexture = value;
				effect.SetValue(textureEffectBool, enableTexture);
			}
		}

		#endregion

		/// <summary>
		/// Method to output the texture information to a csv style string.
		/// </summary>
		public override string ToString()
		{
			StringBuilder temp = new StringBuilder();

			temp.Append(pathName);
			temp.Append(",");

			temp.Append(enableTexture);

			return temp.ToString();
		}

		/// <summary>
		/// Method to read in the texture values from a csv style string.
		/// </summary>
		/// <param name="input"></param>
		public virtual void FromString(string input)
		{
			if (input.Length == 0)
				return;

			string[] values = input.Split(',');

			Path = values[0];
			Enable = bool.Parse(values[1]);
		}

		/// <summary>
		/// Method to handle disposing of the texture
		/// </summary>
		public void Dispose()
		{
			try
			{
				texture.Dispose();
			}
			catch (NullReferenceException)
			{ }
		}
	}

	/// <summary>
	/// Child class to include options specific to a specular map.
	/// </summary>
	[DefaultPropertyAttribute("Specular")]
	public class SpecularClass : TextureClass
	{
		private int specPower;	// Strength of the specular decay.
		private bool useAlpha;	// Bool to handle if the alpha channel should be used to control the specPower.

		/// <summary>
		/// Primary Constructor.
		/// </summary>
		/// <param name="device">Copy of the device.</param>
		/// <param name="effect">Copy of the effect.</param>
		/// <param name="effectName">Name used for setting the texture in the shader.</param>
		/// <param name="effectBool">Name used for toggling the use of the texture in the shader.</param>
		public SpecularClass(Device device, Effect effect, string effectName, string effectBool)
		{
			pathName = "";
			this.device = device;
			this.effect = effect;
			textureEffectName = effectName;
			textureEffectBool = effectBool;
			specPower = 100;
			useAlpha = false;
		}

		#region Set/Get Methods.

		/// <summary>
		/// Set/Get method for the specular falloff.
		/// </summary>
		[CategoryAttribute("Specular"), DescriptionAttribute("Change the falloff for the specular channel"), BrowsableAttribute(true)]
		public int SpecularFalloff
		{
			get
			{
				return specPower;
			}
			set
			{
				specPower = value;
				effect.SetValue("shine", specPower);
			}
		}

		/// <summary>
		/// Set/Get method to enable/disable using the alpha for specular decay control.
		/// </summary>
		[CategoryAttribute("Specular"), DescriptionAttribute("Change if you want the alpha channel to control the specular shine."), 
		 BrowsableAttribute(true)]
		public bool UseAlpha
		{
			get
			{
				return useAlpha;
			}
			set
			{
				useAlpha = value;
				effect.SetValue("useSpecularAlpha", useAlpha);
			}
		}

		#endregion

		/// <summary>
		/// Method to output the texture information to a csv style string.
		/// </summary>
		public override string ToString()
		{
			StringBuilder temp = new StringBuilder(base.ToString());

			temp.Append(",");
			temp.Append(specPower);
			temp.Append(",");
			temp.Append(useAlpha);

			return temp.ToString();
		}
		
		/// <summary>
		/// Method to read in the texture values from a csv style string.
		/// </summary>
		public override void FromString(string input)
		{
			if (input.Length == 0)
				return;

			string[] values = input.Split(',');

			Path = values[0];
			Enable = bool.Parse(values[1]);
			SpecularFalloff = int.Parse(values[2]);
			UseAlpha = bool.Parse(values[3]);
		}
	}

	[DefaultPropertyAttribute("Emissive")]
	public class EmissiveClass : TextureClass
	{
		private float glowStrength;	// Strength of the glow.

		/// <summary>
		/// Default Constructor.
		/// </summary>
		/// <param name="device">Copy of the rendering device.</param>
		/// <param name="effect">Copy of the effect class.</param>
		public EmissiveClass(Device device, Effect effect)
		{
			pathName = "";
			this.device = device;
			this.effect = effect;
			glowStrength = 3.0f;
		}

		/// <summary>
		/// Set/Get method for the path name. Set method also handles loading in a texture.
		/// </summary>
		[Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor)),
		 CategoryAttribute("Path"), DescriptionAttribute("Path of the texture.")]
		public override string Path
		{
			get
			{
				return pathName;
			}
			set
			{
				pathName = value;

				if (!pathName.Equals(""))
				{
					texture = TextureLoader.FromFile(device, value, 0, 0, 3, Usage.None, Format.A8B8G8R8, Pool.Managed, Filter.Linear, Filter.Box, 0);
					enableTexture = true;
					modified = new FileInfo(pathName).LastWriteTime;
				}
				else
				{
					Enable = false;
					modified = new DateTime();
				}
			}
		}

		/// <summary>
		/// Set/Get method for whether the texture is enabled or not.
		/// </summary>
		[Browsable(false)]
		public override bool Enable
		{
			get
			{
				return enableTexture;
			}
			set
			{
				enableTexture = value;
			}
		}

		/// <summary>
		/// Strength of the glow.
		/// </summary>
		[CategoryAttribute("Emissive"), DescriptionAttribute("Set the strength of the glow.")]
		public float Strength
		{
			get
			{
				return glowStrength;
			}
			set
			{
				glowStrength = value;
				effect.SetValue("glowStrength", glowStrength);
			}
		}

		/// <summary>
		/// Method to output the texture information to a csv style string.
		/// </summary>
		public override string ToString()
		{
			StringBuilder temp = new StringBuilder(base.ToString());

			temp.Append(",");
			temp.Append(glowStrength);
			
			return temp.ToString();
		}

		/// <summary>
		/// Method to read in the texture values from a csv style string.
		/// </summary>
		public override void FromString(string input)
		{
			if (input.Length == 0)
				return;

			string[] values = input.Split(',');

			Path = values[0];
			Enable = bool.Parse(values[1]);
			glowStrength = float.Parse(values[2]);
		}
	}

	/// <summary>
	/// Child class to handle properties related to a normal map.
	/// </summary>
	[DefaultPropertyAttribute("Normal")]
	public class NormalClass : TextureClass
	{
		private float normalStrength;	// Strength of the normal map.
		private float heightStrength;	// Strength of the parallax map.
		private int minSamples;			// Minimum number of samples to use for the parallax method.
		private int maxSamples;			// Maximum number of samples to use for the parallax method.
		private bool useParallax;		// Bool to enable/disable parallax mapping.
		private Caps devCaps;			// Caps to check card capabilities.

		/// <summary>
		/// Primary Constructor.
		/// </summary>
		/// <param name="device">Copy of the device.</param>
		/// <param name="effect">Copy of the effect.</param>
		/// <param name="effectName">Name used for setting the texture in the shader.</param>
		/// <param name="effectBool">Name used for toggling the use of the texture in the shader.</param>
		public NormalClass(Device device, Effect effect, string effectName, string effectBool)
		{
			pathName = "";
			this.device = device;
			this.effect = effect;
			textureEffectName = effectName;
			textureEffectBool = effectBool;
			normalStrength = 1.0f;
			heightStrength = 0.05f;
			minSamples = 5;
			maxSamples = 50;
			useParallax = false;
			devCaps = Manager.GetDeviceCaps(0, DeviceType.Hardware);
		}

		#region Set/Get Methods.

		/// <summary>
		/// Set/Get method for the path name. Set method also handles loading in a texture.
		/// </summary>
		[Editor(typeof(FilteredFileNameEditor), typeof(UITypeEditor)),
		 CategoryAttribute("Path"), DescriptionAttribute("Path of the texture.")]
		public override string Path
		{
			get
			{
				return pathName;
			}
			set
			{
				pathName = value;

				if (!pathName.Equals(""))
				{
					texture = TextureLoader.FromFile(device, value, 0, 0, 3, Usage.None, Format.A8B8G8R8, Pool.Managed, Filter.Linear, Filter.Box, 0);
					effect.SetValue(textureEffectName, texture);
					effect.SetValue(textureEffectBool, true);
					enableTexture = true;
					modified = new FileInfo(pathName).LastWriteTime;

					// Set the technique.
					if (!useParallax)
					{
						effect.Technique = "NormalMap";
						technique = "NormalMap";
					}
					else
					{
						effect.Technique = "ParallaxMap";
						technique = "ParallaxMap";
					}
				}
				else
				{
					Enable = false;
					modified = new DateTime();
				}
			}
		}

		/// <summary>
		/// Set/Get for enabling the normal/parallax map.
		/// </summary>
		[CategoryAttribute("General"), DescriptionAttribute("Enable/Disable the Texture")]
		//[Browsable(false)]
		public override bool Enable
		{
			get
			{
				return enableTexture;
			}
			set
			{
				enableTexture = value;
				effect.SetValue(textureEffectBool, enableTexture);

				// Set the technique.
				if (!enableTexture)
				{
					effect.Technique = "PerPixel";
					technique = "PerPixel";
				}
				else if (enableTexture && !useParallax)
				{
					effect.Technique = "NormalMap";
					technique = "NormalMap";
				}
				else if (enableTexture && useParallax)
				{
					effect.Technique = "ParallaxMap";
					technique = "ParallaxMap";
				}
			}
		}

		/// <summary>
		/// Set/Get method for the strength of the normal map.
		/// </summary>
		[CategoryAttribute("Normal"), DescriptionAttribute("Change the influence of the normal map"), BrowsableAttribute(true)]
		public float NormalStrength
		{
			get
			{
				return normalStrength;
			}
			set
			{
				normalStrength = value;
				effect.SetValue("normalStrength", normalStrength);
			}
		}

		/// <summary>
		/// Set/Get method for the strength of the parallax map.
		/// </summary>
		[CategoryAttribute("Parallax"), DescriptionAttribute("Change the strength of the parallax map"), BrowsableAttribute(true)]
		//[Browsable(false)]
		public float ParallaxStrength
		{
			get
			{
				return heightStrength;
			}
			set
			{
				heightStrength = value;
				effect.SetValue("heightMapScale", heightStrength);
			}
		}

		/// <summary>
		/// Set/Get method for the minimum number of samples.
		/// </summary>
		[CategoryAttribute("Parallax"), DescriptionAttribute("Change the minimum number of samples used."), BrowsableAttribute(true)]
		//[Browsable(false)]
		public int MinSamples
		{
			get
			{
				return minSamples;
			}
			set
			{
				minSamples = value;
				effect.SetValue("minSamples", minSamples);
			}
		}

		/// <summary>
		/// Set/Get method for the maximum number of samples
		/// </summary>
		[CategoryAttribute("Parallax"), DescriptionAttribute("Change the maximum number of samples used."), BrowsableAttribute(true)]
		//[Browsable(false)]
		public int MaxSamples
		{
			get
			{
				return maxSamples;
			}
			set
			{
				maxSamples = value;
				effect.SetValue("maxSamples", maxSamples);
			}
		}

		/// <summary>
		/// Set/Get method for the enabling/disabling the parallax map.
		/// </summary>
		[CategoryAttribute("Parallax"), DescriptionAttribute("Enable/Disable parallax mapping."), BrowsableAttribute(true)]
		//[Browsable(false)]
		public bool UseParallax
		{
			get
			{
				return useParallax;
			}
			set
			{
				useParallax = value;

				if (useParallax && devCaps.PixelShaderVersion >= new Version(3, 0))
				{
					effect.Technique = "ParallaxMap";
					technique = "ParallaxMap";
				}
				else
				{
					useParallax = false;
					effect.Technique = "NormalMap";
					if (devCaps.PixelShaderVersion < new Version(3, 0))
						MessageBox.Show("Error: Your graphics card does not support pixel shader 3.0", "Error",
											 MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		#endregion

		/// <summary>
		/// Method to output the texture information to a csv style string.
		/// </summary>
		public override string ToString()
		{
			StringBuilder temp = new StringBuilder(base.ToString());

			temp.Append(",");
			temp.Append(normalStrength);
			temp.Append(",");

			temp.Append(useParallax);
			temp.Append(",");
			temp.Append(maxSamples);
			temp.Append(",");
			temp.Append(minSamples);
			temp.Append(",");
			temp.Append(heightStrength);

			return temp.ToString();
		}

		/// <summary>
		/// Method to read in the texture values from a csv style string.
		/// </summary>
		public override void FromString(string input)
		{
			if (input.Length == 0)
				return;

			string[] values = input.Split(',');

			Path = values[0];
			Enable = bool.Parse(values[1]);
			NormalStrength = float.Parse(values[2]);
			UseParallax = bool.Parse(values[3]);
			MaxSamples = int.Parse(values[4]);
			MinSamples = int.Parse(values[5]);
			ParallaxStrength = float.Parse(values[6]);
		}
	}
}
