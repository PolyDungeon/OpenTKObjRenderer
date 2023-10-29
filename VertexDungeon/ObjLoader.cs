using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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


using Mesh = LearnOpenTK.Mesh;
using Material = ObjRenderer.Material;

public class ObjLoader
{
    static int _vertexArrayObject;
    static int _vertexBufferObject;
    static int _elementBufferObject;
    static private Shader _shader;

    static private Texture _texture;

    static private Texture _texture2;
    public static List<Mesh> LoadObjFile(string filePath, string mtlFilePath = null)
    {

        
        var meshes = new List<Mesh>();
        Mesh currentMesh = null;
        Material currentMaterial = null;
        Dictionary<string, Material> materials = new Dictionary<string, Material>();

        List<float> vertFloats = new();
        List<Vector3> vertices = new List<Vector3>();
        List<Vector3> normals = new List<Vector3>();
        List<Vector2> textures = new List<Vector2>();

        List<Vector3> faceVerts = new List<Vector3>();
        List<Vector3> faceNormals = new List<Vector3>();
        List<Vector2> faceTextures = new List<Vector2>();
        List<int> faceIndices = new List<int>();

        List<Material> materialList = new();
        int materialIndex = 0;
        List<int> indices = new List<int>();
        bool firstMat = true;

        StreamReader objReader = new StreamReader(filePath);
        mtlFilePath = ChangeObjToMtl(filePath);


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
                    // Normal
                    normals.Add(new Vector3(
                        float.Parse(parts[1]),
                        float.Parse(parts[2]),
                        float.Parse(parts[3])
                    ));
                    break;

                case "vt":
                    // Texture coordinate
                    textures.Add(new Vector2(
                        float.Parse(parts[1]),
                        float.Parse(parts[2])
                    ));
                    break;

                case "f":
                    // Face
                    for (int i = 1; i < 4; i++)
                    {
                        string[] vertexData = parts[i].Split('/');
                        int vertexIndex = int.Parse(vertexData[0]) - 1;
                        int textureIndex = vertexData.Length > 1 ? int.Parse(vertexData[1]) - 1 : 0;
                        int normalIndex = vertexData.Length > 2 ? int.Parse(vertexData[2]) - 1 : 0;

                        faceVerts.Add(vertices[vertexIndex]);
                        faceTextures.Add(textures[textureIndex]);
                        faceNormals.Add(normals[normalIndex]);
                        faceIndices.Add(vertices.Count - 1);
                    }
                    break;

                case "usemtl":
                    // Material
                    if (currentMesh != null)
                    {
                        currentMesh.material = materialList[materialIndex];
                        currentMesh.Vertices.AddRange(faceVerts);
                        currentMesh.Normals.AddRange(faceNormals);
                        currentMesh.TextureCoordinates.AddRange(faceTextures);
                        currentMesh.Indices.AddRange(faceIndices);
                        currentMesh.NumVertices = currentMesh.Vertices.Count;
                        faceVerts.Clear();
                        faceNormals.Clear();
                        faceTextures.Clear();
                        faceIndices.Clear();
                        meshes.Add(currentMesh);
                        materialIndex++;
                    }
                    //Debug.Print("Mesh Mat: " + currentMesh.material.name);

                    currentMesh = new Mesh();

                    break;

                case "mtllib":
                    if (mtlFilePath != null)
                    {
                        //string mtlFile = Path.Combine(Path.GetDirectoryName(objFilePath), parts[1]);
                        Debug.Print(mtlFilePath);
                        materials = MtlLoader.LoadMtlFile(mtlFilePath);
                        materialList = materials.Values.ToList();
                    }
                    break;

                default:
                    break;
            }
        }

        if (currentMesh != null)
        {
            currentMesh.Vertices.AddRange(faceVerts);
            currentMesh.Normals.AddRange(faceNormals);
            currentMesh.TextureCoordinates.AddRange(faceTextures);
            currentMesh.Indices.AddRange(faceIndices);
            currentMesh.material = materialList[materialIndex];
            currentMesh.NumVertices = currentMesh.Vertices.Count;
            meshes.Add(currentMesh);
        }



        /*
        //fix then add mesh here




        _vertexArrayObject = GL.GenVertexArray();
        GL.BindVertexArray(_vertexArrayObject);
        foreach (var mesh in meshes) 
        {

            //not sure this is right. 
            vertFloats = vector3ToFloat(mesh.Vertices);
            Debug.Print("how many");
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertFloats.Count * sizeof(float), vertFloats.ToArray(), BufferUsageHint.StaticDraw);

            _elementBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _elementBufferObject);
            GL.BufferData(BufferTarget.ElementArrayBuffer, mesh.Indices.Count* sizeof(uint), mesh.Indices.ToArray(), BufferUsageHint.StaticDraw);

            // shader.frag has been modified yet again, take a look at it as well.
            _shader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
            _shader.Use();

            var vertexLocation = _shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            var texCoordLocation = _shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            if (mesh.material.DiffuseTex)
            {
                _texture = Texture.LoadFromFile(mesh.material.DiffuseMap);
                _texture.Use(TextureUnit.Texture0);
            }

            if (mesh.material.SpecularTex)
            {
                _texture2 = Texture.LoadFromFile(mesh.material.SpecularMap);
                _texture2.Use(TextureUnit.Texture1);
            }
            // Next, we must setup the samplers in the shaders to use the right textures.
            // The int we send to the uniform indicates which texture unit the sampler should use.
            //_shader.SetInt("texture0", 0);
            //_shader.SetInt("texture1", 1);
        }
        */

        return meshes;
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

    protected static List<float> vector3ToFloat(List<Vector3> vectors)
    {
        List<float> floatList = new(); 
        foreach (Vector3 vector in vectors)
        {
            floatList.Add(vector.X);
            floatList.Add(vector.Y);
            floatList.Add(vector.Z);
        }
        return floatList;
        // Now, floatList contains all the components of the Vector3 objects.
        //This code iterates through the Vector3 objects, extracting their X, Y, and Z components and adding them to the floatList, effectively flattening the 3D vectors into a list of floats.
    }
}
