using System;
using LearnOpenTK.Common;
using OpenTK.Graphics;
using OpenTK.Mathematics;

namespace ObjRenderer
{
	public class Material
	{
		//[Flags]
		//public enum Reflection
		//{
		//	Fresnel = 1 << 0,
		//	RayTrace = 1 << 1,
		//}

		//[Flags]
		//public enum Transparency
		//{
		//	Glass = 1 << 0,
		//	Refraction = 1 << 1,
		//}

		//public bool colorOn;
		//public bool ambientOn;
		//public bool highlightOn;
		//public bool castShadowsOnInvisible;
		//public Reflection reflection;
		//public Transparency transparency;

		//public Color4 ambient;
		//public Color4 diffuse;
		//public Color4 specular;
		//public float specularExponent;
		//public float dissolution;

		//public string ambientTexture;
		//public string diffuseTexture;
		//public string specularColorTexture;
		//public string specularHighlight;
		//public string bumpMap;
		//public string displacementMap;
		//public string decalTexture;
		public bool DiffuseTex = false;
		public bool SpecularTex = false;
		public int startIndex;
		public float Shininess;
		public Vector3 AmbientColor;
		public Vector3 DiffuseColor;
		public Vector3 SpecularColor;
		//public Texture DiffuseMap;
		//public Texture SpecularMap;
		public string DiffuseMap;
		public string SpecularMap;

        public readonly string name;

		public Material(string name)
		{
			this.name = name;
		}

		public void MaterialFill()
		{
			if (Shininess == null)
			{
				Shininess = 255;
			}
            if (AmbientColor == null)
            {
                AmbientColor = new Vector3(0.5f, 0.5f, 0.5f);
            }
            if (DiffuseColor == null)
            {
                DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            }
            if (DiffuseMap == null)
            {
                //DiffuseMap = Texture.LoadFromFile("Resources/container2.png");
                DiffuseMap = "";
            }
			if (SpecularMap == null)
            {
                //SpecularMap = Texture.LoadFromFile("Resources/container2_specular.png");
                SpecularMap = "";
            }
        }
	}
}
