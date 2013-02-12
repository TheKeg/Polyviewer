using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace ModelViewer
{
	[DefaultPropertyAttribute("Camera")]
	/// <summary>
	/// Class to handle movement of a Camera.
	/// </summary>
	public class Camera
	{
		private Vector3 position;		// Where the camera is placed in the world.
		private Vector3 offset;			// How much to move the camera and target away from the center point (0,0,0).
		private Vector3 target;			// Where the camera is looking.
		private Vector3 upVector;		// Which way is up on the camera.
		private Vector3 rotationAxis;	// Axis to rotate the camera up/down.
		private Matrix rotMatrix;		// Temperary matrix used for rotating the camera.
		private Device device;			// Copy of the rendering device.
		private float zoom;				// The distance the Camera is from the origin.
		private float width;				// The width of the current rendering panel.
		private float height;			// The height of the current rendering panel.
		private Matrix projection;
		private Matrix view;

		/// <summary>
		/// Constructor which sets the default camera position.
		/// </summary>
		/// <param name="device">The device being used for rendering.</param>
		public Camera(ref Device device)
		{
			this.device = device;

			position = new Vector3(0, 0f, 1f);
			offset = new Vector3(0, 0, 0);
			target = new Vector3(0, 0, 0);
			upVector = new Vector3(0, 1, 0);
			rotationAxis = new Vector3(1, 0, 0);
						
			zoom = 1.0f;
		}

		#region Set/Get Methods

		/// <summary>
		/// Set/Get method for obtaining the current zoom level.
		/// </summary>
		[CategoryAttribute("Zoom"), DescriptionAttribute("Level of Zoom")]
		public float Zoom
		{
			get
			{
				return zoom;
			}
			set
			{
				zoom = value;
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

		/// <summary>
		/// Set/Get method where the camera is.
		/// </summary>
		[CategoryAttribute("General"), DescriptionAttribute("Camera Position")]
		public Vector3 Position
		{
			get
			{
				return position;
			}
			set
			{
				position = value;
				RotateCamera(0f, 0f);
			}
		}

		/// <summary>
		/// Set/Get method for where the camera is looking.
		/// </summary>
		[CategoryAttribute("General"), DescriptionAttribute("Camera Target")]
		public Vector3 Target
		{
			get
			{
				return target;
			}
			set
			{
				target = value;
				RotateCamera(0f, 0f);
			}
		}

		/// <summary>
		/// Set/Get method for the camera's up vector.
		/// </summary>
		[CategoryAttribute("General"), DescriptionAttribute("Camera Up Vector")]
		public Vector3 UpVector
		{
			get
			{
				return upVector;
			}
			set
			{
				upVector = value;
				RotateCamera(0f, 0f);
			}
		}

		/// <summary>
		/// Set/Get method for the offset from 0,0,0
		/// </summary>
		[CategoryAttribute("General"), DescriptionAttribute("Offset from the origin")]
		public Vector3 Offset
		{
			get
			{
				return offset;
			}
			set
			{
				offset = value;
				RotateCamera(0f, 0f);
			}
		}

		public Matrix Projection
		{
			get
			{
				return projection;
			}
		}

		public Matrix View
		{
			get
			{
				return view;
			}
		}

		#endregion

		#region Adjusting the Camera

		/// <summary>
		/// Resets the camera to the starting position.
		/// </summary>
		public void ResetCamera()
		{
			position = new Vector3(0, 0f, 1f);
			offset = new Vector3(0, 0, 0);
			target = new Vector3(0, 0, 0);
			rotationAxis = new Vector3(1, 0, 0);
			
			zoom = 1.0f;

			position = Vector3.Multiply(position, zoom);
			
			// Updates the camera position.
			device.SetTransform(TransformType.View, Matrix.LookAtRH(position, target, upVector));
		}

		/// <summary>
		/// Sets up the camera's perspective and view.
		/// </summary>
		/// <param name="width">The width of the panel being used.</param>
		/// <param name="height">The height of the panel being used.</param>
		public void SetCamera(float width, float height)
		{
			this.width = width;
			this.height = height;

			projection = Matrix.PerspectiveFovRH((float)(Math.PI / 4), width / height, 2.0f, 100000000.0f);
			view = Matrix.LookAtRH(position, target, upVector);

			device.SetTransform(TransformType.Projection, projection);
			device.SetTransform(TransformType.View, view);
		}

		#endregion

		#region Camera Controls

		/// <summary>
		/// Method that handles moving the camera.
		/// </summary>
		/// <param name="x">How much to move the camera left/right.</param>
		/// <param name="y">How much to move the camera up/down.</param>
		public void MoveCamera(float x, float y)
		{
			// Sets the movement to be based relative to the zoom level.
			x *= zoom * 0.01f;
			y *= zoom * 0.01f;

			// Create a new vector that is perpendicular to the camera and up vector.
			Vector3 strafe = Vector3.Normalize(Vector3.Cross(target - position, upVector));

			// Adjust the strafe vector by the passed in values.
			strafe.Y += y;
			strafe.X *= x;
			strafe.Z *= x;

			// Add the strafe vector to the camera position, target and offset vectors.
			position += strafe;
			target += strafe;
			offset += strafe;

			// Update the camera position information.
			view = Matrix.LookAtRH(position, target, upVector);

			device.SetTransform(TransformType.View, view);
		}

		/// <summary>
		/// Rotates the camera around the target.
		/// </summary>
		/// <param name="yAngle">Amount to rotate the camera left or right.</param>
		/// <param name="zAngle">Amount to rotate the camera up or down.</param>
		public void RotateCamera(float yAngle, float zAngle)
		{
			// Move the camera back so it's relative to the origin
			position -= offset;
			
			// Rotate the camera left or right
			rotMatrix = Matrix.RotationY(yAngle);
			position = Vector3.TransformCoordinate(position, rotMatrix);
			rotationAxis = Vector3.TransformCoordinate(rotationAxis, rotMatrix);

			// Rotate the camera up or down.
			rotMatrix = Matrix.RotationAxis(rotationAxis, zAngle);
			position = Vector3.TransformCoordinate(position, rotMatrix);

			// Move the camera to where it is meant to be in the scene.
			position += offset;

			view = Matrix.LookAtRH(position, target, upVector);

			device.SetTransform(TransformType.View, view);
		}

		/// <summary>
		/// Zooms the camera in or out.
		/// </summary>
		/// <param name="zoomIn">Whether to zoom the camera in or out.</param>
		public void ZoomCamera(bool zoomIn)
		{
			if (zoomIn)
			{
				// Keeps from zooming in too far.
				if(zoom > 0.2f)
					zoom -= zoom * 0.05f;	// decreses the zoom by 10%
			}
			else
			{
				zoom += zoom * 0.05f;	// increases the zoom by 10%
			}

			// Sets the camera position to the origin point, then zooms in/out and offsets the camera.
			position -= offset;
			position.Normalize();
			position = position = Vector3.Multiply(position, zoom);
			position += offset;

			projection = Matrix.PerspectiveFovRH((float)(Math.PI / 4), width / height, zoom / 100.0f, float.MaxValue);
			view = Matrix.LookAtRH(position, target, upVector);

			// Updates the camera's position.
			device.SetTransform(TransformType.View, view);
			device.SetTransform(TransformType.Projection, projection);
		}

		/// <summary>
		/// Zooms the camera to a set value.
		/// </summary>
		/// <param name="zoomValue"></param>
		public void ZoomCamera(float zoomValue)
		{
			zoom = zoomValue;

			// Sets the camera position to the camAngle(normalized) then zooms in/out and offsets the camera and target.
			position.Normalize();
			position = Vector3.Multiply(position, zoom);
			position += offset;
			target += offset;

			projection = Matrix.PerspectiveFovRH((float)(Math.PI / 4), width / height, zoom / 100.0f, float.MaxValue);
			view = Matrix.LookAtRH(position, target, upVector);

			// Updates the camera's position.
			device.SetTransform(TransformType.View, view);
			device.SetTransform(TransformType.Projection, projection);
		}
		
		#endregion

		#region Input/Output from file

		/// <summary>
		/// Parses the passed in string and sets the values for the camera.
		/// </summary>
		/// <param name="input">The string loaded from the settings file pertaining to the camera information.</param>
		public void FromString(string input)
		{
			if (input.Length == 0)
				return;

			string[] values = input.Split(',');

			// Parse the values and sets the new values.
			Position = new Vector3(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]));
			Target = Offset = new Vector3(float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
			rotationAxis = new Vector3(float.Parse(values[6]), float.Parse(values[7]), float.Parse(values[8]));
			Zoom = float.Parse(values[9]);
		}

		/// <summary>
		/// Creates a comma seperated value string for output to file.
		/// </summary>
		/// <returns>A CSV based string containing the current values of the string.</returns>
		public override string ToString()
		{
			StringBuilder fileOutString = new StringBuilder();

			// Append the position information to the string.
			fileOutString.Append(position.X);
			fileOutString.Append(",");
			fileOutString.Append(position.Y);
			fileOutString.Append(",");
			fileOutString.Append(position.Z);
			fileOutString.Append(",");

			// Append the target information to the string.
			fileOutString.Append(target.X);
			fileOutString.Append(",");
			fileOutString.Append(target.Y);
			fileOutString.Append(",");
			fileOutString.Append(target.Z);
			fileOutString.Append(",");

			// Append the rotation axis information to the string.
			fileOutString.Append(rotationAxis.X);
			fileOutString.Append(",");
			fileOutString.Append(rotationAxis.Y);
			fileOutString.Append(",");
			fileOutString.Append(rotationAxis.Z);
			fileOutString.Append(",");

			// Append the zoom level to the string.
			fileOutString.Append(zoom);

			return fileOutString.ToString();
		}

		#endregion
	}
}
