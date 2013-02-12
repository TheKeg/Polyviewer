using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace ModelViewer 
{
	/// <summary>
	/// Internal class to handle getting the path of an obj file to load.
	/// </summary>
	internal class ObjectFileNameEditor : UITypeEditor
	{
		private OpenFileDialog ofd = new OpenFileDialog();		

		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
		{
			ofd.FileName = value.ToString();
			ofd.Filter = "3D files (*.obj, *.x)|*.obj;*.x";
			ofd.Filter += "|Obj Files (*.obj)|*.obj";
			ofd.Filter += "|DirectX (*.x)|*.x";
			if (ofd.ShowDialog() == DialogResult.OK)
			{
				return ofd.FileName;
			}
			return base.EditValue(context, provider, value);
		}
	}

	/// <summary>
	/// Defines the Vertex Structure to be used.
	/// Tangent is a 4d vector to store the handedness of the vector.
	/// </summary>
	public struct Vertex
	{
		public Vector3 Position;
		public Vector2 TexCoord;
		public Vector3 Normal;
		public Vector4 Tangent;
	}

	/// <summary>
	/// Class Model which stores information pertaining to the vertex and index 
	/// buffers for a mesh. Also stored is the min and max values for the size
	/// of the mesh, which is used for calculating zoom extents.
	/// </summary>
	[DefaultPropertyAttribute("Model")]
	public class Model
	{
		protected Device device;						// A copy of the rendering device.
		protected Mesh model;							// DirectX mesh to store the model data.
		protected TextureClass diffuseTexture;	// Class for the diffuse texture.
		protected SpecularClass specularTexture;	// Class for the specular texture.
		protected NormalClass normalTexture;		// Class for the normal texture.
		protected TextureClass ambientTexture;	// Class for the ambientOcc texture.
		protected EmissiveClass emissiveTexture;	// Class for the emissive texture.
		protected ObjLoader objLoader;				// Class for loading an Obj file.
		protected XLoader xLoader;					// Class for loading a DirectX file.
		protected bool enabled;						// Boolean for whether the model is to be rendered.
		protected float min, max;						// Minimum and Maximum y value.
		protected string pathname;					// Path from where the model was loaded from.

		public Model()
		{ }

		/// <summary>
		/// Initializes the varibles and sets default values.
		/// </summary>
		/// <param name="device">A copy of the rendering device.</param>
		/// <param name="effect">A copy of the effect class.</param>
		public Model(Device device, Effect effect)
		{
			VertexElement[] vElements = new VertexElement[]
			{
			   new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
			   new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				new VertexElement(0, 20, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
			   new VertexElement(0, 32, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Tangent, 0),
			   VertexElement.VertexDeclarationEnd
			};

			this.device = device;
			objLoader = new ObjLoader(device);
			xLoader = new XLoader(device);
			model = new Mesh(1, 1, MeshFlags.Managed | MeshFlags.Use32Bit, vElements, device);
			min = max = 0;
			enabled = false;
			pathname = "";

			// Initialize the texture classes and set the shader variables.
			ambientTexture = new TextureClass(device, effect, "ambientMap", "useAmbientMap");
			diffuseTexture = new TextureClass(device, effect, "diffuseMap", "useDiffuseMap");
			emissiveTexture = new EmissiveClass(device, effect);
			specularTexture = new SpecularClass(device, effect, "specularMap", "useSpecularMap");
			normalTexture = new NormalClass(device, effect, "normalMap", "useNormalMap");
		}

		/// <summary>
		/// Loads in an obj/x file.
		/// </summary>
		/// <param name="filename">Full path of the 3d model file.</param>
		public void LoadModel(string filename)
		{
			VertexElement[] vElements = new VertexElement[]
			{
			   new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
			   new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				new VertexElement(0, 20, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
			   new VertexElement(0, 32, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Tangent, 0),
			   VertexElement.VertexDeclarationEnd
			};

			model = new Mesh(1, 1, MeshFlags.Managed, vElements, device);

			if (filename.EndsWith(".obj"))
				model = objLoader.LoadModel(filename);
			else if (filename.EndsWith(".x"))
				model = xLoader.LoadModel(filename);

			// Calculate the min/max values.
			calcSize();

			enabled = true;
		}

		#region Set/Get Methods.

		/// <summary>
		/// Set/Get method for enabling/disabling if this model should be rendered.
		/// </summary>
		[BrowsableAttribute(false)]
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

				LoadModel(pathname);
								
				ambientTexture.Device = value;
				diffuseTexture.Device = value;
				specularTexture.Device = value;
				normalTexture.Device = value;
				emissiveTexture.Device = value;
			}
		}

		[Browsable(false)]
		public Effect Effect
		{
			set
			{
				ambientTexture.Effect = value;
				diffuseTexture.Effect = value;
				specularTexture.Effect = value;
				normalTexture.Effect = value;
				emissiveTexture.Effect = value;
			}
		}

		/// <summary>
		/// Get method so that the engine class can render the model.
		/// </summary>
		[BrowsableAttribute(false)]
		public Mesh Mesh
		{
			get
			{
				return model;
			}
			set
			{
				model = value;
			}
		}

		/// <summary>
		/// Set/Get method for the triangle count used in rendering the model.
		/// </summary>
		[CategoryAttribute("Info"), DescriptionAttribute("Number of triangles")]
		public int TriangleCount
		{
			get
			{
				if (model.NumberFaces > 1)
					return model.NumberFaces;
				else
					return 0;
			}
		}

		/// <summary>
		/// Set/Get method for the vertex count used in rendering the model. not one to one with the actual mesh.
		/// </summary>
		[CategoryAttribute("Info"), DescriptionAttribute("Number of vertices")]
		public int FaceCount
		{
			get
			{
				if (model.NumberVertices > 1)
					return model.NumberVertices;
				else
					return 0;
			}
		}

		/// <summary>
		/// Set/Get method for the path of the obj loaded.
		/// </summary>
		[Editor(typeof(ObjectFileNameEditor), typeof(UITypeEditor)),
		 CategoryAttribute("Path"), DescriptionAttribute("Path of the model.")]
		public string Path
		{
			get
			{
				return pathname;
			}
			set
			{
				pathname = value;

				if (!pathname.Equals(""))
					LoadModel(pathname);
			}
		}

		/// <summary>
		/// Set/Get method for the ambient occlusion map
		/// </summary>
		[Browsable(false)]
		public TextureClass Ambient
		{
			get
			{
				return ambientTexture;
			}
			set
			{
				ambientTexture = value;
			}
		}

		/// <summary>
		/// Set/Get method for the diffuse map
		/// </summary>
		[Browsable(false)]
		public TextureClass Diffuse
		{
			get
			{
				return diffuseTexture;
			}
			set
			{
				diffuseTexture = value;
			}
		}

		/// <summary>
		/// Set/Get method for the specular map
		/// </summary>
		[Browsable(false)]
		public SpecularClass Specular
		{
			get
			{
				return specularTexture;
			}
			set
			{
				specularTexture = value;
			}
		}

		/// <summary>
		/// Set/Get method for the emissive map
		/// </summary>
		[Browsable(false)]
		public EmissiveClass Emissive
		{
			get
			{
				return emissiveTexture;
			}
			set
			{
				emissiveTexture = value;
			}
		}

		/// <summary>
		/// Set/Get method for the normal map
		/// </summary>
		[Browsable(false)]
		public NormalClass Normal
		{
			get
			{
				return normalTexture;
			}
			set
			{
				normalTexture = value;
			}
		}

		/// <summary>
		/// Get method for the minimum value.
		/// </summary>
		[BrowsableAttribute(false)]
		public float Min
		{
			get
			{
				return min;
			}
		}

		/// <summary>
		/// Get method for the maximum value.
		/// </summary>
		[BrowsableAttribute(false)]
		public float Max
		{
			get
			{
				return max;
			}
		}

		#endregion

		/// <summary>
		/// Calculates the largest and smallest values.
		/// </summary>
		public void calcSize()
		{
			min = max = 0;
			Vertex vertex;
			GraphicsStream gStream = model.LockVertexBuffer(LockFlags.None);
			
			// Iterates through the vertex list to find the minimum and maximum position values.
			for (int i = 0; i < model.NumberVertices; i++)
			{
				vertex = (Vertex)gStream.Read(typeof(Vertex));

				if (vertex.Position.Y > max)
					max = vertex.Position.Y;

				if (vertex.Position.Y < min)
					min = vertex.Position.Y;
			}
		}

		/// <summary>
		/// Calculates the middle point.
		/// </summary>
		/// <returns>The middle height of the mesh.</returns>
		public float calcMidPoint()
		{
			return (max + min) / 2;
		}

		/// <summary>
		/// Method to rebuild the normals and tangents of the loaded model.
		/// </summary>
		public void rebuildNormals()
		{
			// Generates a list of adjacent faces used for generating normals.
			int[] adjacency = new int[model.NumberFaces * 3];
			model.GenerateAdjacency(0, adjacency);

			model.ComputeNormals(adjacency);

			//model.ComputeTangentFrame(TangentOptions.GenerateInPlace | TangentOptions.CalculateNormals);

			TangentBuilder.CalculateTangents(model);
		}

		/// <summary>
		/// Returns the pathname.
		/// </summary>
		/// <returns>The path of the loaded model.</returns>
		public override string ToString()
		{
			return pathname;
		}

		/// <summary>
		/// Method to handle disposal of all objects.
		/// </summary>
		public void Dispose()
		{
			try
			{
				model.Dispose();

				if(!ambientTexture.Path.Equals(""))
					ambientTexture.Dispose();

				if (!diffuseTexture.Path.Equals(""))
					diffuseTexture.Dispose();

				if (!normalTexture.Path.Equals(""))
					normalTexture.Dispose();

				if (!specularTexture.Path.Equals(""))
					specularTexture.Dispose();

				if (!emissiveTexture.Path.Equals(""))
					emissiveTexture.Dispose();
			}
			catch (NullReferenceException)
			{ }
		}
	}
}
