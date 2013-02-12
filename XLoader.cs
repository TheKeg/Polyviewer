using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace ModelViewer
{
	/// <summary>
	/// Class to handle loading in a directx mesh.
	/// </summary>
	public class XLoader
	{
		private Device device;	// Copy of the rendering device.

		/// <summary>
		/// Constructor Class
		/// </summary>
		/// <param name="device">Copy of the rendering device.</param>
		public XLoader(Device device)
		{
			this.device = device;
		}

		/// <summary>
		/// Method to load in a directx mesh.
		/// </summary>
		/// <param name="filename">Path of the model to load.</param>
		/// <returns></returns>
		public Mesh LoadModel(string filename)
		{
			Mesh mesh;

			VertexElement[] vElements = new VertexElement[]
			{
			   new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
			   new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				new VertexElement(0, 20, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
			   new VertexElement(0, 32, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Tangent, 0),
			   VertexElement.VertexDeclarationEnd
			};

			// Load the mesh from file.
			mesh = Mesh.FromFile(filename, MeshFlags.Managed | MeshFlags.Use32Bit, device);
			
			// Clone the mesh.
			mesh = mesh.Clone(MeshFlags.Managed | MeshFlags.Use32Bit, vElements, device);

			// Generates a list of adjacent faces used for generating normals.
			int[] adjacency = new int[mesh.NumberFaces * 3];
			mesh.GenerateAdjacency(0, adjacency);

			//mesh.ComputeTangentFrame(TangentOptions.GenerateInPlace | TangentOptions.CalculateNormals);
			mesh.ComputeNormals(adjacency);

			// Create the variables needed to convert the mesh to right handed coordinates.
			GraphicsStream gStream = mesh.LockVertexBuffer(LockFlags.None);
			List<Vertex> vertices = new List<Vertex>();
			Vertex vertex;

			// Convert the mesh to right handed coordinates.
			for (int i = 0; i < mesh.NumberVertices; i++)
			{
				vertex = (Vertex)gStream.Read(typeof(Vertex));

				vertex.Position.Z *= -1.0f;
				vertex.Normal = Vector3.TransformNormal(vertex.Normal, Matrix.Scaling(1.0f, 1.0f, -1.0f));
				
				vertices.Add(vertex);
			}

			// Set the vertex buffer
			mesh.SetVertexBufferData(vertices.ToArray(), LockFlags.None);

			// Calculate the tangents
			TangentBuilder.CalculateTangents(mesh);

			return mesh;
		}
	}
}
