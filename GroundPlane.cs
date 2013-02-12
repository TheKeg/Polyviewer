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
	[DefaultPropertyAttribute("Ground Plane")]
	public class GroundPlane : Model
	{
		private float uvScale;
		private float size;
		private float minValue;

		public GroundPlane(Device device, Effect effect)
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
			uvScale = 1.0f;
			minValue = 0.0f;
			size = 10.0f;

			// Initialize the texture classes and set the shader variables.
			ambientTexture = new TextureClass(device, effect, "ambientMap", "useAmbientMap");
			diffuseTexture = new TextureClass(device, effect, "diffuseMap", "useDiffuseMap");
			emissiveTexture = new EmissiveClass(device, effect);
			specularTexture = new SpecularClass(device, effect, "specularMap", "useSpecularMap");
			normalTexture = new NormalClass(device, effect, "normalMap", "useNormalMap");

			CreateGroundPlane(minValue, size, uvScale);
		}

		[CategoryAttribute("Details"), DescriptionAttribute("Number of repeats for the UV")]
		public float UVScale
		{
			get
			{
				return uvScale;
			}
			set
			{
				uvScale = value;
				CreateGroundPlane(minValue, size, value);
			}
		}

		[Browsable(false)]
		public float MinValue
		{
			set
			{
				minValue = value;
			}
		}

		[Browsable(false)]
		public float Size
		{
			set
			{
				size = value;
			}
		}

		public void CreateGroundPlane(float minValue, float size, float uvScale)
		{
			VertexElement[] vElements = new VertexElement[]
			{
			   new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
			   new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
				new VertexElement(0, 20, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
			   new VertexElement(0, 32, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Tangent, 0),
			   VertexElement.VertexDeclarationEnd
			};

			model = new Mesh(2, 4, MeshFlags.Managed | MeshFlags.Use32Bit, vElements, device);

			Vertex[] vertexList = new Vertex[4];

			// Initialize the values for the 4 vertexes.
			vertexList[0].Position = new Vector3(-size, minValue, -size);
			vertexList[0].Normal = new Vector3(0, 1.0f, 0);
			vertexList[0].TexCoord = new Vector2(0, 0);

			vertexList[1].Position = new Vector3(-size, minValue, size);
			vertexList[1].Normal = new Vector3(0, 1.0f, 0);
			vertexList[1].TexCoord = new Vector2(0, uvScale);

			vertexList[2].Position = new Vector3(size, minValue, -size);
			vertexList[2].Normal = new Vector3(0, 1.0f, 0);
			vertexList[2].TexCoord = new Vector2(uvScale, 0);

			vertexList[3].Position = new Vector3(size, minValue, size);
			vertexList[3].Normal = new Vector3(0, 1.0f, 0);
			vertexList[3].TexCoord = new Vector2(uvScale, uvScale);

			int[] indexList = { 0, 3, 2, 1, 3, 0 };

			model.SetIndexBufferData(indexList, LockFlags.None);
			model.SetVertexBufferData(vertexList, LockFlags.None);

			TangentBuilder.CalculateTangents(model);
		}
	}
}
