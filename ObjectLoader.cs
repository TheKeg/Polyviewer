using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Text;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace ModelViewer
{
	/// <summary>
	/// Class to handle loading of *.obj files.
	/// </summary>
	public class ObjLoader
	{
		private string buffer;					// buffer to hold the text loaded from file.
		private List<Vector3> vertexList;	// list to store the vertex positions.
		private List<Vector3> normalList;	// list to store the normal values.
		private List<Vector2> uvList;			// List to store the uv coordinates
		private List<int> faceList;			// List to hold the index list.
		private List<Vertex> vertexInfo;		// List to hold the vertex information.
		private Hashtable table;				// Hash table used to handle creating the index list.
		private Mesh mesh;						// Mesh to store the loaded data into.
		private Device device;					// Copy of the rendering device.
		private bool hasNormals;				// Boolean to determine if the model loaded has normals stored.
		private char[] enumerator;				// Char array for reading in files and splitting up values.

		/// <summary>
		/// Constructor class. Initalizes all local variables.
		/// </summary>
		/// <param name="device">Reference of the current rendering device.</param>
		public ObjLoader(Device device)
		{
			vertexList = new List<Vector3>();
			normalList = new List<Vector3>();
			uvList = new List<Vector2>();
			faceList = new List<int>();
			vertexInfo = new List<Vertex>();
			table = new Hashtable();
			hasNormals = false;
			enumerator = new char[] { ' ' };
			this.device = device;	
		}

		/// <summary>
		/// Loads an obj file from disk.
		/// </summary>
		/// <param name="fileName">Location of the file to load.</param>
		/// <returns>A copy of the model.</returns>
		public Mesh LoadModel(string fileName)
		{
			try
			{
				//tempFile = new FileStream(fileName, FileMode.Open);

            readObjectFile(fileName); // Read in the file and store the data
				buildMesh();		// Copy over the appropriate data to the model.
			}
			catch (FileLoadException)
			{
				MessageBox.Show("Invalid file, Please select another obj file", "Error Loading Image",
										 MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

			return mesh;
		}

		/// <summary>
		/// Scans through the obj file and stores the data appropriatly.
		/// </summary>
		private void readObjectFile(string fileName)
		{
			hasNormals = false;

         StreamReader sr = new StreamReader(fileName, true);//(tempFile, Encoding.ASCII);
         
			// Clear all the various lists.
			vertexList.Clear();
			vertexInfo.Clear();
			normalList.Clear();
			uvList.Clear();
			faceList.Clear();
			table.Clear();

         try
         {
            // While loop to read each line of the obj file and handle it.
            while (!sr.EndOfStream)
            {
               buffer = sr.ReadLine(); // gets the next line.

               // Handles if the line defines a normal.
               if (buffer.StartsWith("vn"))
               {
                  addNormal();
               }
               // Handles texture coordinates.
               else if (buffer.StartsWith("vt"))
               {
                  addTexCoord();
               }
               // Handles vertex coordinates
               else if (buffer.StartsWith("v"))
               {
                  addVertex();
               }
               // Handles the face information.
               else if (buffer.StartsWith("f"))
               {
                  addFace();
               }
            }
         }
         catch (Exception e)
         {
            MessageBox.Show(e.ToString());
            sr.Close();
         }
			sr.Close(); // Closes the stream.
		}

		/// <summary>
		/// Treats the buffer string as a vertex.
		/// </summary>
		private void addVertex()
		{
			// Splits up the buffer string by spaces.
			string[] values = buffer.Split(enumerator, StringSplitOptions.RemoveEmptyEntries);

			// Adds a new vertex to the list.
			vertexList.Add(new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));
		}

		/// <summary>
		/// Treats the buffer string as a normal.
		/// </summary>
		private void addNormal()
		{
			// Splits up the buffer string by spaces.
			string[] values = buffer.Split(enumerator, StringSplitOptions.RemoveEmptyEntries);

			// Adds a new normal vector to the normal list.
			normalList.Add(new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3])));

			hasNormals = true;
		}
		
		/// <summary>
		/// Treats the buffered string as a texture coordinate.
		/// </summary>
		private void addTexCoord()
		{
			string[] values = buffer.Split(enumerator, StringSplitOptions.RemoveEmptyEntries); // Splits up the buffer string by spaces.

         //Console.WriteLine(values[0] + " " + values[1] + " " + values[2]);

			// Adds the new uv coordinate to the uv list.
			uvList.Add(new Vector2(float.Parse(values[1]), 1.0f - float.Parse(values[2])));
		}

		/// <summary>
		/// Treats the buffered string as face information.
		/// </summary>
		private void addFace()
		{
			string[] face1, face2, face3, face4; // Info for up to 4 faces.
			Vertex[] vertices = new Vertex[4];	// Array of 4 Vertex's
			bool isQuad = false; // bool for handling quad faces.

			string[] values = buffer.Split(enumerator, StringSplitOptions.RemoveEmptyEntries); // Splits up the buffer string by spaces.

			// Splits up the face information even more (puts 1/1/1 into three spots in an array)
			face1 = values[1].Split('/');
			face2 = values[2].Split('/');
			face3 = values[3].Split('/');
			face4 = face3; // so face4 isn't null.

			// Checks if the length is consistant with a quad face.
			if (values.Length == 5)
			{
				// Makes sure the 5th array isn't empty
				if (!values[4].Equals(""))
				{
					// Splits up the data for the 4th value of the quad.
					face4 = values[4].Split('/');

					// sets quad to true.
					isQuad = true;
				}
			}

			// Sets the position vectors for the first three values.
			vertices[0].Position = vertexList[Math.Abs(Int32.Parse(face1[0])) - 1];
			vertices[1].Position = vertexList[Math.Abs(Int32.Parse(face2[0])) - 1];
			vertices[2].Position = vertexList[Math.Abs(Int32.Parse(face3[0])) - 1];

			// Sets the fourth position vector if the face described in the file is a quad.
			if (isQuad)
				vertices[3].Position = vertexList[Math.Abs(Int32.Parse(face4[0])) - 1];

			// Handles if there is texture coordinates.
			if (face1.Length >= 2)
			{
				if (!face1[1].Equals(""))
				{
					// Sets the texture coordinate vectors for the first three values.
					vertices[0].TexCoord = uvList[Math.Abs(Int32.Parse(face1[1])) - 1];
					vertices[1].TexCoord = uvList[Math.Abs(Int32.Parse(face2[1])) - 1];
					vertices[2].TexCoord = uvList[Math.Abs(Int32.Parse(face3[1])) - 1];

					// Sets the fourth texture coordinate vector if the face described in the file is a quad.
					if (isQuad)
						vertices[3].TexCoord = uvList[Math.Abs(Int32.Parse(face4[1])) - 1];
				}
			}

			// Handles if there is normals.
			if (face1.Length == 3)
			{
				if (!face1[2].Equals(""))
				{
					// Sets the normal vectors for the first three values.
					vertices[0].Normal = normalList[Math.Abs(Int32.Parse(face1[2])) - 1];
					vertices[1].Normal = normalList[Math.Abs(Int32.Parse(face2[2])) - 1];
					vertices[2].Normal = normalList[Math.Abs(Int32.Parse(face3[2])) - 1];

					// Sets the fourth normal vector if the face described in the file is a quad.
					if (isQuad)
						vertices[3].Normal = normalList[Math.Abs(Int32.Parse(face4[2])) - 1];
				}
			}

			// Handles adding the face information to the face list.
			if (!isQuad)
			{
				// Calls the addVertex method to get the vertex index to be used for the index buffer.
				faceList.Add(addVertex(Math.Abs(Int32.Parse(face1[0])), vertices[0]));
				faceList.Add(addVertex(Math.Abs(Int32.Parse(face2[0])), vertices[1]));
				faceList.Add(addVertex(Math.Abs(Int32.Parse(face3[0])), vertices[2]));
			}
			// Adds two triangles if the face is a quad.
			else
			{
				// Calls the addVertex method to get the vertex index to be used for the index buffer.
				faceList.Add(addVertex(Math.Abs(Int32.Parse(face1[0])), vertices[0]));
				faceList.Add(addVertex(Math.Abs(Int32.Parse(face2[0])), vertices[1]));
				faceList.Add(addVertex(Math.Abs(Int32.Parse(face4[0])), vertices[3]));

				faceList.Add(addVertex(Math.Abs(Int32.Parse(face2[0])), vertices[1]));
				faceList.Add(addVertex(Math.Abs(Int32.Parse(face3[0])), vertices[2]));
				faceList.Add(addVertex(Math.Abs(Int32.Parse(face4[0])), vertices[3]));
			}
		}

		/// <summary>
		/// Calculates if a vertex value is already in the list of vertices.
		/// </summary>
		/// <param name="hash">The index into the vertex list.</param>
		/// <param name="vertex">The vertex to look for.</param>
		/// <returns>The index into the array of vertices.</returns>
		private int addVertex(int hash, Vertex vertex)
		{
			bool foundInList = false;
			int index = 0;

			// Checks to see if the hash value is part of the table.
			if (table.ContainsKey(hash))
			{
				// Checks if the vertex at the index of the hash value is equal to the
				// value of the vertex passed into the function.
				if (table[hash].Equals(vertex))
				{
					// Sets the index to the hash and that it was found.
					index = hash;
					foundInList = true;
				}
			}

			if (!foundInList)
			{
				index = vertexInfo.Count;	// Sets the index to the last value in the list.
				vertexInfo.Add(vertex);		// Adds the vertex to the list.
				table.Add(index, vertex);	// Adds the vertex to a hash list.
			}

			return (int)index;
		}

		/// <summary>
		/// Copies over the vertex and index buffers and calls the function within
		/// the model class to build a Mesh class.
		/// </summary>
		private void buildMesh()
		{
			VertexElement[] vElements = new VertexElement[]
			{
			   new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
			   new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				new VertexElement(0, 20, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
			   new VertexElement(0, 32, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Tangent, 0),
			   VertexElement.VertexDeclarationEnd
			};

			// Creates a new mesh
			try
			{
				mesh = new Mesh(faceList.Count, vertexInfo.Count, MeshFlags.Managed | MeshFlags.Use32Bit, vElements, device);

				mesh.SetVertexBufferData(vertexInfo.ToArray(), LockFlags.None);
				mesh.SetIndexBufferData(faceList.ToArray(), LockFlags.None);
			}
			catch(DirectXException)
			{
				MessageBox.Show("A problem occured with the model", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				faceList.Clear();
				vertexInfo.Clear();
				vertexList.Clear();
				mesh = new Mesh(1, 1, MeshFlags.Managed, vElements, device);
				return;
			}

			// Try loop to generate normals (if needed), tangents and binormals.
			try
			{
				// Generates a list of adjacent faces used for generating normals.
				int[] adjacency = new int[mesh.NumberFaces * 3];
				mesh.GenerateAdjacency(0, adjacency);

				if(!hasNormals)
					mesh.ComputeNormals(adjacency);

				TangentBuilder.CalculateTangents(mesh);
			}
			catch (DirectXException dxe)
			{
				Console.WriteLine(dxe);
			}
		}

		/// <summary>
		/// Cleans up the private variables.
		/// </summary>
		public void Dispose()
		{
			try
			{
				vertexList.Clear();
				vertexInfo.Clear();
				normalList.Clear();
				uvList.Clear();
				faceList.Clear();
				table.Clear();
			}
			catch (NullReferenceException)
			{ }
		}
	}
}
