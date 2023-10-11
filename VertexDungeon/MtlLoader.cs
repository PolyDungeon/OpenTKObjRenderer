using LearnOpenTK.Common;
using ObjRenderer;
using OpenTK.Mathematics;
using System.Globalization;

public class MtlLoader
{
    public static Dictionary<string, Material> LoadMtlFile(string mtlFilePath)
    {
        Dictionary<string, Material> materials = new Dictionary<string, Material>();
        Material currentMaterial = null;

        StreamReader mtlReader = new StreamReader(mtlFilePath);

        string line;
        while ((line = mtlReader.ReadLine()) != null)
        {
            line = line.Trim();
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                continue;

            switch (parts[0])
            {
                case "newmtl":
                    if (currentMaterial != null)
                    {
                        materials.Add(currentMaterial.name, currentMaterial);
                    }
                    currentMaterial = new Material(parts[1]);
                    break;

                case "Ka":
                    if (currentMaterial != null && parts.Length >= 4)
                    {
                        float r = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float g = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float b = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        currentMaterial.AmbientColor = new Vector3(r, g, b);
                    }
                    break;

                case "Kd":
                    if (currentMaterial != null && parts.Length >= 4)
                    {
                        float r = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float g = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float b = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        currentMaterial.DiffuseColor = new Vector3(r, g, b);
                    }
                    break;

                case "Ks":
                    if (currentMaterial != null && parts.Length >= 4)
                    {
                        float r = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float g = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float b = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        currentMaterial.SpecularColor = new Vector3(r, g, b);
                    }
                    break;

                case "Ns":
                    if (currentMaterial != null && parts.Length >= 2)
                    {
                        float shininess = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        currentMaterial.Shininess = shininess;
                    }
                    break;

                case "map_Kd":
                    if (currentMaterial != null && parts.Length >= 2)
                    {
                        string textureFile = parts[1];
                        Texture textureId = Texture.LoadFromFile(textureFile);
                        currentMaterial.DiffuseMap = textureId;
                    }
                    break;

                case "map_Ks":
                    if (currentMaterial != null && parts.Length >= 2)
                    {
                        string textureFile = parts[1];
                        Texture textureId = Texture.LoadFromFile(textureFile);
                        currentMaterial.SpecularMap = textureId;
                    }
                    break;
            }
        }

        if (currentMaterial != null)
        {
            materials.Add(currentMaterial.name, currentMaterial);
        }

        mtlReader.Close();

        return materials;
    }

}
