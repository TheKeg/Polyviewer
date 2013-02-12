using System;
using System.Drawing;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace ModelViewer
{
	/// <summary>
	/// Class to bring up the color chooser dialog within the property grid panel
	/// </summary>
	internal class ColorChooserEditor: UITypeEditor
	{
		private ColorDialog cd = new ColorDialog();

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.DropDown;
		}

		/// <summary>
		/// Bring up the colour chooser dialog and return a value otherwise use the default parent method.
		/// </summary>
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			cd.Color = (Color)value;
			cd.AllowFullOpen = true;
			cd.FullOpen = true;
		   

		   if (cd.ShowDialog() == DialogResult.OK)
		   {
				return cd.Color;
		   }

		   return base.EditValue(context, provider, value);
		}
	}

	
	/// <summary>
	/// Class to handle basic information pertaining to a light. Handles rotations of the light.
	/// </summary>
	[DefaultPropertyAttribute("Light")]
	public class LightObj
	{
		private Vector3 direction;		// Used to set the direction of the light.
		private Matrix tempMatrix;		// Used for the matrix transforms used in rotating the light.
		private Vector3 rotationAxis;	// Axis to rotate the light up/down along.
		private Device device;			// A copy of the rendering device.
		private Effect effect;			// A copy of the effect class.
		private Color ambient;
		private Color diffuse;
		private Color specular;
		private float distance;			// Distance the light is from 0,0,0.
		private float lightIntensity;	// The strength of the light.
		private bool enabled;
		private bool enableShadows;

		/// <summary>
		/// Constructor for the light class.
		/// </summary>
		/// <param name="device">The directx device to add the light to.</param>
		/// <param name="effect">Copy of the effect.</param>
		/// <param name="num">The index of the light.</param>
		public LightObj(Device device, Effect effect, bool enabled)
		{
			this.device = device;
			this.effect = effect;
			this.enabled = enabled;
			enableShadows = false;

			direction = new Vector3(0, 1, 1);		// Set the default direction of the light.
			rotationAxis = new Vector3(1, 0, 0);	// Set the default rotation axis for up/down movement.

			ambient = Color.Black;
			diffuse = Color.White;
			specular = Color.DarkGray;
			
			distance = 5.0f; // Default distance;
			lightIntensity = 1.0f;

			direction.Normalize();
			direction = Vector3.Multiply(direction, distance);
		}

		/// <summary>
		/// Restores the light to the default starting direction.
		/// </summary>
		public void resetLight()
		{
			direction = new Vector3(0, 1, 1);
			direction.Normalize();
			direction = Vector3.Multiply(direction, distance);

			rotationAxis = new Vector3(1, 0, 0);
			Intensity = 1.0f;
		}

		#region Set/Get Methods

		[Browsable(false)]
		public bool Enabled
		{
			get
			{
				return enabled;
			}
			set
			{
				enabled = value;
			}
		}

		[Browsable(false)]
		public Device Device
		{
			set
			{
				device = value;
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
		/// Set/Get methods for the ambient light color.
		/// </summary>
		[CategoryAttribute("Color"), DescriptionAttribute("Ambient Color"), Editor(typeof(ColorChooserEditor), typeof(UITypeEditor))]
		public Color Ambient
		{
			get
			{
				return ambient;
			}
			set
			{
				ambient = value;
			}
		}

		/// <summary>
		/// Set/Get methods for the diffuse light color.
		/// </summary>
		[CategoryAttribute("Color"), DescriptionAttribute("Diffuse Color"), Editor(typeof(ColorChooserEditor), typeof(UITypeEditor))]
		public Color Diffuse
		{
			get
			{
				return diffuse;
			}
			set
			{
				diffuse = value;
			}
		}

		/// <summary>
		/// Set/Get methods for the specular light color.
		/// </summary>
		[CategoryAttribute("Color"), DescriptionAttribute("Specular Color"), Editor(typeof(ColorChooserEditor), typeof(UITypeEditor))]
		public Color Specular
		{
			get
			{
				return specular;
			}
			set
			{
				specular = value;
			}
		}

		/// <summary>
		/// Set/Get methods for the distance of a light from the origin.
		/// </summary>
		[CategoryAttribute("General"), DescriptionAttribute("Distance from the origin")]
		public float Distance
		{
			get
			{
				return distance;
			}
			set
			{
				distance = value;
				direction.Normalize();
				direction = Vector3.Multiply(direction, distance);
			}
		}

		/// <summary>
		/// Set/Get methods for managing the direction of the light.
		/// </summary>
		[CategoryAttribute("General"), DescriptionAttribute("Angle of the light")]
		public Vector3 Direction
		{
			get
			{
				return direction;
			}
			set
			{
				direction = value;
			}
		}

		/// <summary>
		/// Set/Get method for the intensity of the light.
		/// </summary>
		[CategoryAttribute("General"), DescriptionAttribute("Intensity of the light")]
		public float Intensity
		{
			get
			{
				return lightIntensity;
			}
			set
			{
				lightIntensity = value;
			}
		}

		/// <summary>
		/// Set/Get method to enable/disable shadows emitted from this light.
		/// </summary>
		[CategoryAttribute("General"), DescriptionAttribute("Enable/Disable Shadows")]
		public bool Shadows
		{
			get
			{
				return enableShadows;
			}
			set
			{
				enableShadows = value;
			}
		}

		#endregion

		#region Light Control

		/// <summary>
		/// Rotates the light around the y axis and z axis.
		/// </summary>
		/// <param name="yAngle">The amount in radians to rotate around the y axis.</param>
		/// <param name="zAngle">The amount in radians to rotate around the z axis.</param>
		public void rotateLight(float yAngle, float zAngle)
		{
			// Rotate the light left or right.
			tempMatrix = Matrix.RotationY(yAngle);
			direction = Vector3.TransformCoordinate(direction, tempMatrix);
			rotationAxis = Vector3.TransformCoordinate(rotationAxis, tempMatrix);

			// Rotate the camera up or down.
			tempMatrix = Matrix.RotationAxis(rotationAxis, zAngle);
			direction = Vector3.TransformCoordinate(direction, tempMatrix);
		}

		#endregion

		#region Input/Output from file

		/// <summary>
		/// Creates a comma seperated value string for output to file.
		/// </summary>
		/// <returns>A CSV based string containing the current values of the string.</returns>
		public override string ToString()
		{
			StringBuilder fileOutString = new StringBuilder();

			// Append the direction of the light to the string.
			fileOutString.Append(direction.X);
			fileOutString.Append(",");
			fileOutString.Append(direction.Y);
			fileOutString.Append(",");
			fileOutString.Append(direction.Z);
			fileOutString.Append(",");

			// Append the rotation axis of the light to the string.
			fileOutString.Append(rotationAxis.X);
			fileOutString.Append(",");
			fileOutString.Append(rotationAxis.Y);
			fileOutString.Append(",");
			fileOutString.Append(rotationAxis.Z);
			fileOutString.Append(",");

			// Append the distance of the light to the string.
			fileOutString.Append(distance);
			fileOutString.Append(",");

			// Append the intensity of the light to the string.
			fileOutString.Append(Intensity);
			fileOutString.Append(",");

			// Append the ambient colour of the light to the string.
			fileOutString.Append(Ambient.ToArgb());
			fileOutString.Append(",");

			// Append the diffuse colour of the light to the string.
			fileOutString.Append(Diffuse.ToArgb());
			fileOutString.Append(",");

			// Append the specular colour of the light to the string.
			fileOutString.Append(Specular.ToArgb());
			fileOutString.Append(",");

			fileOutString.Append(enabled);
						
			return fileOutString.ToString();
		}

		/// <summary>
		/// Parses the passed in string and sets the values for the light.
		/// </summary>
		/// <param name="input">The string loaded from the settings file pertaining to the camera information.</param>
		public void FromString(string input)
		{
			if (input.Length == 0)
				return;

			string[] values = input.Split(',');

			Direction = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
			rotationAxis = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
			Distance = float.Parse(values[6]);
			Intensity = float.Parse(values[7]);

			Ambient = Color.FromArgb(int.Parse(values[8]));
			Diffuse = Color.FromArgb(int.Parse(values[9]));
			Specular = Color.FromArgb(int.Parse(values[10]));

			enabled = bool.Parse(values[11]);
		}

		#endregion
	}
}
