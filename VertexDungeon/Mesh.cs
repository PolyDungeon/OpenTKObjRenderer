using LearnOpenTK.Common;
using ObjRenderer;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace LearnOpenTK
{
    public class Mesh
    {
        public int Vao { get; set; }
        public int DiffuseMap { get; set; }
        public int SpecularMap { get; set; }
        public int NumVertices { get; set; }

        public Vector3 position;

        public Dictionary<string, Material> materials = new Dictionary<string, Material>();
        public List<Vector3> Vertices { get; set; }
        public List<Vector3> Normals { get; set; }
        public List<Vector2> TextureCoordinates { get; set; }
        public List<int> Indices { get; set; }
        public Mesh(int vao, int numVerts, Dictionary<string, Material> mats)
        {
            // Initialize lists for vertices, normals, texture coordinates, and indices
            Vertices = new List<Vector3>();
            Normals = new List<Vector3>();
            TextureCoordinates = new List<Vector2>();
            Indices = new List<int>();
            materials = mats;
            Vao = vao;
            NumVertices = numVerts;
            materials = mats;
            position = new(0, 0, 0);
        }

        private void RenderMesh(Shader shader, Camera camera)
        {
            GL.BindVertexArray(Vao);

            var material = materials.Values.ToList();
            Texture _diffuseMap = material[0].DiffuseMap;
            Texture _specularMap = Texture.LoadFromFile("Resources/container2_specular.png");
            _diffuseMap.Use(TextureUnit.Texture0);
            _specularMap.Use(TextureUnit.Texture1);
            shader.Use();

            shader.SetMatrix4("view", camera.GetViewMatrix());
            shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            shader.SetVector3("viewPos", camera.Position);

            shader.SetInt("material.diffuse", 0);
            shader.SetInt("material.specular", 1);
            shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            shader.SetFloat("material.shininess", 32.0f);

            // Set other shader uniforms as needed

            Matrix4 model = Matrix4.CreateTranslation(mesh.position);
            float angle = 0;
            model = model * Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), angle);
            shader.SetMatrix4("model", model);

            GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.NumVertices);
            for (int i = 0; i < mesh.Vertices.Count; i += 3)
            {
                GL.DrawArrays(PrimitiveType.Triangles, i, 3);
            }
        }
    }
}
