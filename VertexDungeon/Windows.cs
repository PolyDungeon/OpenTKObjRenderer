//using System.Globalization;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using LearnOpenTK.Common;
//using System.IO;
using Camera = LearnOpenTK.Common.Camera;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using Mesh = LearnOpenTK.Mesh;
using System.Diagnostics;
using System;
using ObjRenderer;
using Material = ObjRenderer.Material;
using System.Resources;
using Assimp;
using System.Xml.Linq;
using Assimp.Unmanaged;
//using System.Diagnostics;

namespace LearnOpenTK
{
    // In this tutorial we focus on how to set up a scene with multiple lights, both of different types but also
    // with several point lights
    public class Window : GameWindow
    {
        static Vector3 newPosition = new Vector3(0, 0, 0); // Replace x, y, z with your desired coordinates
        static Matrix4 translationMatrix = Matrix4.CreateTranslation(newPosition);
        //static Mesh importObject = ObjLoader.Load("C:\\Users\\xxmon\\source\\repos\\old\\VertexDungeon\\VertexDungeon\\LongCube.obj");
        List<Mesh> cubeMesh; //= ObjLoader.LoadObjFile("C:\\Users\\xxmon\\source\\repos\\old\\VertexDungeon\\VertexDungeon\\LongCube.obj"); // Load a cube object
        Mesh sphereMesh; //= ObjLoader.LoadObjFile("C:\\Users\\xxmon\\source\\repos\\old\\VertexDungeon\\VertexDungeon\\PolyKnight.obj"); // Load a sphere object
        static int _vertexArrayObject;
        static int _vertexBufferObject;
        static int _elementBufferObject;



        // We need the point lights' positions to draw the lamps and to get light the materials properly
        private readonly Vector3[] _pointLightPositions =
        {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3(0.0f, 0.0f, -3.0f)
        };
        private Vector3 _loadedModelPosition = Vector3.Zero;


        private int _vaoModel;
        private int posBuf;
        private int normBuf;
        private int texBuf;
        private int _vaoLamp;

        private Shader _lightingShader;
        private Shader _lampShader;

        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;
        // The texture containing information for the diffuse map, this would more commonly
        // just be called the color/texture of the object.
        private Texture _diffuseMap;

        // The specular map is a black/white representation of how specular each part of the texture is.
        private Texture _specularMap;
        public Window(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        private readonly float[] _vertices =
        {
            // Positions          Normals              Texture coords
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f,  0.0f, -1.0f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f,  0.0f,  1.0f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f, -1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f,  0.0f,  0.0f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, -1.0f,  0.0f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f,  1.0f,  0.0f,  0.0f, 1.0f
        };

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            _vertexBufferObject = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            //cubeMesh.position = new(2, 0, 0);

            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            {
                _vaoLamp = GL.GenVertexArray();
                GL.BindVertexArray(_vaoLamp);

                var positionLocation = _lampShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            }

            _camera = new Camera(new Vector3(0, 0, 5), Vector3.UnitY, Size.X / (float)Size.Y);
            cubeMesh = ObjLoader.LoadObjFile("C:\\Users\\xxmon\\source\\repos\\old\\VertexDungeon\\VertexDungeon\\Object.obj"); // Load a cube object

            //fix then add mesh here
            _vaoModel = GL.GenVertexArray();
            GL.BindVertexArray(_vaoModel);
            foreach (var mesh in cubeMesh)
            {
                mesh.Vao = GL.GenVertexArray();
                GL.BindVertexArray(mesh.Vao);

                // Get position and normal data from OBJ loader

                // Position VBO
                int vboPositions = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboPositions);
                GL.BufferData(BufferTarget.ArrayBuffer, mesh.Vertices.Count * Vector3.SizeInBytes, mesh.Vertices.ToArray(), BufferUsageHint.StaticDraw);

                // Set position attribute pointer
                var positionLocation = _lightingShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);

                // Normal VBO 
                int vboNormals = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboNormals);
                GL.BufferData(BufferTarget.ArrayBuffer, mesh.Normals.Count * Vector3.SizeInBytes, mesh.Normals.ToArray(), BufferUsageHint.StaticDraw);

                // Set normal attribute pointer
                var normalLocation = _lightingShader.GetAttribLocation("aNormal");
                GL.EnableVertexAttribArray(normalLocation);
                GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
                
                //texture VBO 
                int vboTexCoords = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, vboTexCoords);
                GL.BufferData(BufferTarget.ArrayBuffer, mesh.TextureCoordinates.Count * Vector2.SizeInBytes, mesh.TextureCoordinates.ToArray(), BufferUsageHint.StaticDraw);

                //Set texture attribute pointer
                var texCoordLocation = _lightingShader.GetAttribLocation("aTexCoords");
                GL.EnableVertexAttribArray(texCoordLocation);
                GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 0, 0);

                Material material = mesh.material; // Assuming materials are stored per mesh
                //_diffuseMap = Texture.LoadFromFile(cubeMesh[0].material.DiffuseMap);
                if (material.DiffuseTex)
                {
                    // GL.ActiveTexture(TextureUnit.Texture0);
                    mesh.material.DiffuseTexture = Texture.LoadFromFile(material.DiffuseMap);                   
                }

                if (material.SpecularTex)
                {
                    //GL.ActiveTexture(TextureUnit.Texture1);
                    mesh.material.DiffuseTexture = Texture.LoadFromFile(material.SpecularMap);
                    mesh.shader.SetInt("material.specular", 1);
                    //_specularMap.Use(TextureUnit.Texture1);
                } 

            }

            CursorState = CursorState.Grabbed;
        }
        
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            

            // Render cube
            RenderMesh(cubeMesh);

            SwapBuffers();
        }

       


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            if (!IsFocused)
            {
                return;
            }

            var input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }


            const float cameraSpeed = 1.5f;
            const float sensitivity = 0.2f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
            }
            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
            }



            var keyboardState = KeyboardState;
            var mouseState = MouseState;

            //_camera.ProcessKeyboardInput(keyboardState, (float)e.Time);

            _camera.ProcessInput(keyboardState, mouseState, (float)e.Time);

            if (mouseState.ScrollDelta != new Vector2(0, 0))
            {
                _camera.ProcessScrollWheel(mouseState.ScrollDelta.Y * 0.1f);
            }

        }
        bool once = true;
        private void RenderMesh(List<Mesh> meshes)
        {
            foreach (var mesh in meshes)
            {
                GL.BindVertexArray(mesh.Vao);
                
                // Bind textures and set material-specific uniforms
                //Material material = mesh.material; // Assuming materials are stored per mesh
                mesh.shader.Use();

                mesh.shader.SetMatrix4("view", _camera.GetViewMatrix());
                mesh.shader.SetMatrix4("projection", _camera.GetProjectionMatrix());
                mesh.shader.SetVector3("viewPos", _camera.Position);
                // Here we specify to the shaders what textures they should refer to when we want to get the positions.
                mesh.shader.SetInt("material.diffuse", 0);
                mesh.shader.SetInt("material.specular", 1);
                mesh.shader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
                mesh.shader.SetFloat("material.shininess", 32.0f);

                mesh.shader.SetVector3("light.position", new Vector3(0,0,0));
                mesh.shader.SetVector3("light.ambient", new Vector3(1.0f));
                mesh.shader.SetVector3("light.diffuse", new Vector3(1.0f));
                mesh.shader.SetVector3("light.specular", new Vector3(1.0f));
                if (mesh.material.DiffuseTex)
                {
                    //DiffuseTexture = Texture.LoadFromFile(material.DiffuseMap);
                    mesh.material.DiffuseTexture.Use(TextureUnit.Texture0);
                }

                if (mesh.material.SpecularTex)
                {
                    //DiffuseTexture = Texture.LoadFromFile(material.SpecularMap);
                    mesh.material.SpecularTexture.Use(TextureUnit.Texture1);
                }

                Matrix4 model = Matrix4.CreateTranslation(mesh.position);
                float angle = 0f;
                model = model * Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), angle);
                mesh.shader.SetMatrix4("model", model);

                // Draw the elements using DrawArrays
                GL.DrawArrays(PrimitiveType.Triangles, 0, mesh.NumVertices);

            }
        }


        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);

            _camera.Fov -= e.OffsetY;
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);
            _camera.AspectRatio = Size.X / (float)Size.Y;
        }

        

    }  

}