using Assimp;
using Assimp.Unmanaged;
using LearnOpenTK.Common;
using ObjRenderer;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Diagnostics;
using static OpenTK.Graphics.OpenGL.GL;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;

namespace LearnOpenTK
{
    public class Mesh
    {
        public int Vao { get; set; }
        public int VBO { get; set; }
        public Shader shader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
        public Texture DiffuseMap { get; set; }
        public Texture SpecularMap { get; set; }
        public int NumVertices { get; set; }

        public Vector3 position;
        List<string> matNames;
        //public List<ObjRenderer.Material> materialsList;
        public ObjRenderer.Material material;
        public List<Vector3> Vertices { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<Vector2> TextureCoordinates { get; set; }
        public List<int> Indices { get; set; }
        public Mesh(int vao, int vbo,int numVerts, ObjRenderer.Material mat, List<string> materialNames, List<int> Indice)
        {
            matNames = materialNames;
            // Initialize lists for vertices, normals, texture coordinates, and indices
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            TextureCoordinates = new List<Vector2>();
            Indices = Indice;
            material = mat;
            //materialsList = mats;
            //DiffuseMap = materialsList[0].DiffuseMap;
            //DiffuseMap = materialsList[0].SpecularMap;
            Vao = vao;
            VBO = vbo;
            NumVertices = numVerts;
            position = new(0, 0, 0);
        }

        public Mesh()
        {
            matNames = new List<string>();
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            TextureCoordinates = new List<Vector2>();
            position = new(0, 0, 0);
            Indices = new List<int>();
        }
        
        
        
    }
}
