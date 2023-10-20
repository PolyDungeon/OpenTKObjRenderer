using LearnOpenTK.Common;
using ObjRenderer;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System.Diagnostics;
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
                        //Debug.Print("rgb: " + r + ", " + g + ", " + b + "\n");
                        currentMaterial.AmbientColor = new Vector3(r, g, b);
                    }
                    break;

                case "Kd":
                    if (currentMaterial != null && parts.Length >= 4)
                    {
                        float r = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float g = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float b = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        //Debug.Print("rgb: " + r + ", " + g + ", " + b + "\n");
                        currentMaterial.DiffuseColor = new Vector3(r, g, b);
                    }
                    break;

                case "Ks":
                    if (currentMaterial != null && parts.Length >= 4)
                    {
                        float r = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        float g = float.Parse(parts[2], CultureInfo.InvariantCulture);
                        float b = float.Parse(parts[3], CultureInfo.InvariantCulture);
                        //Debug.Print("rgb: " + r + ", " + g + ", " + b + "\n");
                        currentMaterial.SpecularColor = new Vector3(r, g, b);
                    }
                    break;

                case "Ns":
                    if (currentMaterial != null && parts.Length >= 2)
                    {
                        float shininess = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        //Debug.Print("shininess: " + shininess + "\n");
                        currentMaterial.Shininess = shininess;
                    }
                    break;

                case "map_Kd":
                    if (currentMaterial != null && parts.Length >= 2)
                    {
                        string textureFile = parts[1];
                        //Debug.Print("DiffuseMap: " + textureFile + "\n");
                        //Texture textureId = Texture.LoadFromFile(unixPath);
                        //currentMaterial.DiffuseMap = textureId;
                        currentMaterial.DiffuseTex = true;
                        currentMaterial.DiffuseMap = textureFile;
                        
                    }
                    break;

                case "map_Ks":
                    if (currentMaterial != null && parts.Length >= 2)
                    {
                        string textureFile = parts[1];
                        //string unixPath = ConvertToUnixPath(textureFile);
                        //Debug.Print("SpecularMap: " + textureFile + "\n");
                        //Texture textureId = Texture.LoadFromFile(unixPath);
                        //currentMaterial.SpecularMap = textureId;
                        currentMaterial.SpecularTex = true;
                        currentMaterial.SpecularMap = textureFile;
                    }
                    break;
            }
        }

        if (currentMaterial != null)
        {
            materials.Add(currentMaterial.name, currentMaterial);
        }

        mtlReader.Close();

        List<string> objectNames = GetDictionaryKeys(materials);

        foreach (var name in objectNames)
        {
            //Debug.Print(name);
        }
        return materials; //why is material list size 18 when it should only be 3. 
    }

    public static List<string> GetDictionaryKeys<TValue>(Dictionary<string, TValue> dictionary)
    {
        List<string> keys = new List<string>(dictionary.Keys);
        return keys;
    }

    static string ConvertToUnixPath(string windowsPath)
    {
        // Replace backslashes with forward slashes and then eliminate consecutive slashes
        return windowsPath.Replace('\\', '/');
    }

}
