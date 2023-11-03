using System;
using LearnOpenTK.Common;
using OpenTK.Graphics;
using OpenTK.Mathematics;

namespace ObjRenderer
{
	public class Material
	{
		public bool DiffuseTex = false;
		public bool SpecularTex = false;
		public int startIndex;
		public float Shininess;
		public Vector3 AmbientColor;
		public Vector3 DiffuseColor;
		public Vector3 SpecularColor;
		public Texture DiffuseTexture;
		public Texture SpecularTexture;
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
