using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
        List<string> indices = new List<string>();
        List<int> vertexIndices = new List<int>();
        List<int> normalIndices = new List<int>();
        List<int> texCoordIndices = new List<int>();
        int vbo = 0;
        List<string> materialNames = new List<string>();
        List<Mesh> meshes = new List<Mesh>();
        Mesh currentMesh = null;
        string currentMaterialName = null;
        Shader _lightingShader = new Shader("C:\\Users\\xxmon\\source\\repos\\old\\VertexDungeon\\VertexDungeon\\bin\\Debug\\net6.0\\Shaders\\shader.vert", "C:\\Users\\xxmon\\source\\repos\\old\\VertexDungeon\\VertexDungeon\\bin\\Debug\\net6.0\\Shaders\\lighting.frag");
        List<int> materialStartIndex = new List<int>();
        int faceCount = 0;
        Dictionary<string, Material> materials = new Dictionary<string, Material>();
        mtlFilePath = ChangeObjToMtl(objFilePath);
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

                case "usemtl":
                    //if (currentMesh != null)
                    //{
                    //    meshes.Add(currentMesh);
                    //}
                    ////?
                    //currentMesh = new Mesh();

                    materialStartIndex.Add(faceCount);
                    //Debug.Print("material starts at: " + faceCount);
                    if (parts.Length >= 2)
                    {
                        currentMaterialName = parts[1];
                    }

                    break;

                case "f":
                    faceCount += 12;
                    if (parts.Length < 4)
                        throw new Exception("Invalid face definition in OBJ file.");
                    for (int i = 1; i < 4; i++)
                    {
                        string[] indice = parts[i].Split('/');
                        if (indice.Length < 1 || indice.Length > 3)
                            throw new Exception("Invalid face definition in OBJ file.");

                        int vertexIndex = int.Parse(indice[0]) - 1;
                        vertexIndices.Add(vertexIndex);

                        if (indice.Length > 1 && !string.IsNullOrWhiteSpace(indice[1]))
                        {
                            int texCoordIndex = int.Parse(indice[1]) - 1;
                            texCoordIndices.Add(texCoordIndex);
                        }
                        else
                        {
                            texCoordIndices.Add(-1); // No texture coordinate
                        }

                        if (indice.Length > 2)
                        {
                            int normalIndex = int.Parse(indice[2]) - 1;
                            normalIndices.Add(normalIndex);
                        }
                        else
                        {
                            normalIndices.Add(-1); // No normal
                        }

                        // Assign the current material to this face
                        materialNames.Add(currentMaterialName);
                        materialNames.Add(currentMaterialName);
                        materialNames.Add(currentMaterialName);
                        materialNames.Add(currentMaterialName);




                    }/*
                    if (currentMesh == null)
                    {
                        // If 'f' is encountered before 'usemtl', create a new mesh
                        currentMesh = new Mesh();
                    }
                    for (int i = 1; i < parts.Length; i++)
                    {
                        string [] indice = parts[i].Split('/').ToArray();
                        //indices = indice.ToArray();
                        currentMesh.Indices.Add(int.Parse(indice[0]) - 1); // Vertex indices
                        currentMesh.TextureCoordinates.Add(texCoords[int.Parse(indice[1]) - 1]); // Texture coordinates
                        currentMesh.Normals.Add(normals[int.Parse(indice[2]) - 1]); // Normals
                    }
                    */
                    break;

                case "mtllib":
                    if (mtlFilePath != null)
                    {
                        //string mtlFile = Path.Combine(Path.GetDirectoryName(objFilePath), parts[1]);
                        Debug.Print("mtl file: " + mtlFilePath);
                        materials = MtlLoader.LoadMtlFile(mtlFilePath);
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

        

        vbo = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, verticesData.Count * sizeof(float), verticesData.ToArray(), BufferUsageHint.StaticDraw);


        int vao = GL.GenVertexArray();
        GL.BindVertexArray(vao);

        var positionLocation = _lightingShader.GetAttribLocation("aPos");
        GL.EnableVertexAttribArray(positionLocation);
        GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 0, 0);

        var normalLocation = _lightingShader.GetAttribLocation("aNormal");
        GL.EnableVertexAttribArray(normalLocation);
        GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 0, 3 * sizeof(float));

        var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoords");
        GL.EnableVertexAttribArray(texCoordLocation);
        GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 0, 6 * sizeof(float));


        int numVertices = vertexIndices.Count;

        

        List<Material> materialList = materials.Values.ToList();
        int j = 0;
        foreach (Material mat in materialList)
        {
            //Debug.Print(mat.name + "starts at " + materialStartIndex[j]);
            mat.startIndex = materialStartIndex[j];
            j++;
        }
        //Debug.Print("matNames Count: " + materialNames.Count);

        return new Mesh(vao, vbo, numVertices, materialList, materialNames, vertexIndices);
    }

    public static string ChangeObjToMtl(string objFilePath)
    {
        if (string.IsNullOrEmpty(objFilePath) || !File.Exists(objFilePath))
        {
            // Handle invalid file paths or non-existing files as needed.
            return null;
        }

        // Get the directory and file name without extension from the OBJ file path.
        string directory = Path.GetDirectoryName(objFilePath);
        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(objFilePath);

        // Append ".mtl" to the file name and create the new MTL file path.
        string mtlFilePath = Path.Combine(directory, fileNameWithoutExt + ".mtl");

        return mtlFilePath;
    }
}