using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Design;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
namespace ModelViewer
{
	/// <summary>
	/// Class to calculate tangents.
	/// Adapted from: http://www.terathon.com/code/tangent.php to work in c#
	/// </summary>
	class TangentBuilder
	{
		/// <summary>
		/// Static method to calculate tangents along with the handedness of the tangents.
		/// </summary>
		/// <param name="model">Model to calculate the tangents on.</param>
		public static void CalculateTangents(Mesh model)
		{
			// Get a copy of the buffers.
			GraphicsStream ib = model.LockIndexBuffer(LockFlags.None);
			GraphicsStream vb = model.LockVertexBuffer(LockFlags.None);

			// List of the final vertex list.
			List<Vertex> final = new List<Vertex>();

			// Temperary lists to store vectors in.
			List<Vector3> tan1 = new List<Vector3>(model.NumberVertices);
			List<Vector3> tan2 = new List<Vector3>(model.NumberVertices);

			// Loop through and copy the vertex list from the vertex buffer
			// and to also add empty values to tan1 and tan2.
			for (int i = 0; i < model.NumberVertices; i++)
			{
				final.Add((Vertex)vb.Read(typeof(Vertex)));
				tan1.Add(new Vector3());
				tan2.Add(new Vector3());
			}

			// Various variables used in the calculation.
			int i1, i2, i3;
			Vector3 v1, v2, v3;
			Vector2 w1, w2, w3;

			float x1, x2, y1, y2, z1, z2;
			float s1, s2, t1, t2, r;

			// Loop through and calculate the tangent information.
			for (int i = 0; i < model.NumberFaces; i++)
			{
				i1 = (int)ib.Read(typeof(int));
				i2 = (int)ib.Read(typeof(int));
				i3 = (int)ib.Read(typeof(int));

				// Get the vertex values for the 3 vertices of a face.
				Vertex vertex1 = final[i1];
				Vertex vertex2 = final[i2];
				Vertex vertex3 = final[i3];

				// Get the positions.
				v1 = vertex1.Position;
				v2 = vertex2.Position;
				v3 = vertex3.Position;

				// Get the texture coordinates.
				w1 = vertex1.TexCoord;
				w2 = vertex2.TexCoord;
				w3 = vertex3.TexCoord;

				x1 = v2.X - v1.X;
				x2 = v3.X - v1.X;
				y1 = v2.Y - v1.Y;
				y2 = v3.Y - v1.Y;
				z1 = v2.Z - v1.Z;
				z2 = v3.Z - v1.Z;

				s1 = w2.X - w1.X;
				s2 = w3.X - w1.X;
				t1 = w2.Y - w1.Y;
				t2 = w3.Y - w1.Y;

				r = 1.0F / (s1 * t2 - s2 * t1);

				// Calculate the direction of the vector
				Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, 
													(t2 * y1 - t1 * y2) * r,
													(t2 * z1 - t1 * z2) * r);
				// Calculate the direction of the uv
				Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, 
													(s1 * y2 - s2 * y1) * r, 
													(s1 * z2 - s2 * z1) * r);

				Vector3 temp1 = tan1[i1];
				Vector3 temp2 = tan1[i2];
				Vector3 temp3 = tan1[i3];
				Vector3 temp4 = tan2[i1];
				Vector3 temp5 = tan2[i2];
				Vector3 temp6 = tan2[i3];

				tan1[i1] = temp1 + sdir;
				tan1[i2] = temp2 + sdir;
				tan1[i3] = temp3 + sdir;

				tan2[i1] = temp4 + tdir;
				tan2[i2] = temp5 + tdir;
				tan2[i3] = temp6 + tdir;
			}

			for (int i = 0; i < model.NumberVertices; i++)
			{
				Vertex tempVertex = final[i];

				Vector3 n = tempVertex.Normal;
				Vector3 t = tan1[i];

				Vector3 temp = (t - n * Vector3.Dot(n, t));
				temp.Normalize();

				// Gram-Schmidt orthogonalize
				tempVertex.Tangent = new Vector4(temp.X, temp.Y, temp.Z, 1.0f);
				
				// Calculate the handedness
				tempVertex.Tangent.W = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0F) ? -1.0F : 1.0F;

				final[i] = tempVertex;
			}

			ib.Close();
			vb.Close();

			model.SetVertexBufferData(final.ToArray(), LockFlags.None);
		}
	}
}
