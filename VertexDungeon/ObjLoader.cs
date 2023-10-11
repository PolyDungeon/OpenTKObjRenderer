using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LearnOpenTK;
using LearnOpenTK.Common;
using ObjRenderer;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class ObjLoader
{
    public static Mesh LoadObjFile(string objFilePath, string mtlFilePath = null)
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> texCoords = new List<Vector2>();
        List<int> vertexIndices = new List<int>();
        List<int> normalIndices = new List<int>();
        List<int> texCoordIndices = new List<int>();
        Dictionary<string, Material> materials = new Dictionary<string, Material>();

        StreamReader objReader = new StreamReader(objFilePath);

        string line;
        while ((line = objReader.ReadLine()) != null)
        {
            line = line.Trim();
            string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                continue;

            switch (parts[0])
            {
                case "v":
                    if (parts.Length < 4)
                        throw new Exception("Invalid vertex definition in OBJ file.");
                    float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                    float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                    vertices.Add(new Vector3(x, y, z));
                    break;

                case "vn":
                    if (parts.Length < 4)
                        throw new Exception("Invalid normal definition in OBJ file.");
                    float nx = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    float ny = float.Parse(parts[2], CultureInfo.InvariantCulture);
                    float nz = float.Parse(parts[3], CultureInfo.InvariantCulture);
                    normals.Add(new Vector3(nx, ny, nz));
                    break;

                case "vt":
                    if (parts.Length < 3)
                        throw new Exception("Invalid texture coordinate definition in OBJ file.");
                    float u = float.Parse(parts[1], CultureInfo.InvariantCulture);
                    float v = float.Parse(parts[2], CultureInfo.InvariantCulture);
                    texCoords.Add(new Vector2(u, v));
                    break;

                case "f":
                    if (parts.Length < 4)
                        throw new Exception("Invalid face definition in OBJ file.");
                    for (int i = 1; i < 4; i++)
                    {
                        string[] indices = parts[i].Split('/');
                        if (indices.Length < 1 || indices.Length > 3)
                            throw new Exception("Invalid face definition in OBJ file.");

                        int vertexIndex = int.Parse(indices[0]) - 1;
                        vertexIndices.Add(vertexIndex);

                        if (indices.Length > 1 && !string.IsNullOrWhiteSpace(indices[1]))
                        {
                            int texCoordIndex = int.Parse(indices[1]) - 1;
                            texCoordIndices.Add(texCoordIndex);
                        }
                        else
                        {
                            texCoordIndices.Add(-1); // No texture coordinate
                        }

                        if (indices.Length > 2)
                        {
                            int normalIndex = int.Parse(indices[2]) - 1;
                            normalIndices.Add(normalIndex);
                        }
                        else
                        {
                            normalIndices.Add(-1); // No normal
                        }
                    }
                    break;

                case "mtllib":
                    if (mtlFilePath != null)
                    {
                        string mtlFile = Path.Combine(Path.GetDirectoryName(objFilePath), parts[1]);
                        materials = MtlLoader.LoadMtlFile(mtlFile);
                    }
                    break;
            }
        }

        objReader.Close();

        List<float> verticesData = new List<float>();
        foreach (int vertexIndex in vertexIndices)
        {
            Vector3 vertex = vertices[vertexIndex];
            verticesData.AddRange(new float[] { vertex.X, vertex.Y, vertex.Z });
        }

        List<float> normalsData = new List<float>();
        foreach (int normalIndex in normalIndices)
        {
            Vector3 normal = normals[normalIndex];
            normalsData.AddRange(new float[] { normal.X, normal.Y, normal.Z });
        }

        List<float> texCoordsData = new List<float>();
        foreach (int texCoordIndex in texCoordIndices)
        {
            if (texCoordIndex >= 0)
            {
                Vector2 texCoord = texCoords[texCoordIndex];
                texCoordsData.AddRange(new float[] { texCoord.X, texCoord.Y });
            }
            else
            {
                texCoordsData.AddRange(new float[] { 0.0f, 0.0f });
            }
        }

        int vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        int vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, verticesData.Count * sizeof(float), verticesData.ToArray(), BufferUsageHint.StaticDraw);

        int normalVbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, normalVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, normalsData.Count * sizeof(float), normalsData.ToArray(), BufferUsageHint.StaticDraw);

        int texCoordVbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordVbo);
        GL.BufferData(BufferTarget.ArrayBuffer, texCoordsData.Count * sizeof(float), texCoordsData.ToArray(), BufferUsageHint.StaticDraw);

        int numVertices = vertexIndices.Count;

        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(0);

        GL.BindBuffer(BufferTarget.ArrayBuffer, normalVbo);
        GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(1);

        GL.BindBuffer(BufferTarget.ArrayBuffer, texCoordVbo);
        GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, 0, 0);
        GL.EnableVertexAttribArray(2);

        Texture diffuseMap;
        Texture specularMap;

        /*
        if (materials.ContainsKey("default"))
        {
            Material defaultMaterial = materials["default"];
            diffuseMap = defaultMaterial.DiffuseMap;
            specularMap = defaultMaterial.SpecularMap;
        }
        */
        return new Mesh(vao, numVertices, materials);
    }
}
