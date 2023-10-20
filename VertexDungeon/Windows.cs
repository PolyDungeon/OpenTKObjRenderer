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
        Mesh cubeMesh; //= ObjLoader.LoadObjFile("C:\\Users\\xxmon\\source\\repos\\old\\VertexDungeon\\VertexDungeon\\LongCube.obj"); // Load a cube object
        Mesh sphereMesh; //= ObjLoader.LoadObjFile("C:\\Users\\xxmon\\source\\repos\\old\\VertexDungeon\\VertexDungeon\\PolyKnight.obj"); // Load a sphere object




        // We need the point lights' positions to draw the lamps and to get light the materials properly
        private readonly Vector3[] _pointLightPositions =
        {
            new Vector3(0.7f, 0.2f, 2.0f),
            new Vector3(2.3f, -3.3f, -4.0f),
            new Vector3(-4.0f, 2.0f, -12.0f),
            new Vector3(0.0f, 0.0f, -3.0f)
        };
        private Vector3 _loadedModelPosition = Vector3.Zero;

        private int _vertexBufferObject;

        private int _vaoModel;
        private int posBuf;
        private int normBuf;
        private int texBuf;
        private int _vaoLamp;

        private Shader _lampShader;

        private Shader _lightingShader;

        private Camera _camera;

        private bool _firstMove = true;

        private Vector2 _lastPos;
        private Texture DiffuseTexture;
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

            //_vertexBufferObject = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            //GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);
            
            cubeMesh = ObjLoader.LoadObjFile("C:\\Users\\xxmon\\source\\repos\\old\\VertexDungeon\\VertexDungeon\\multiCube.obj"); // Load a cube object

            cubeMesh.position = new(2, 0, 0);

            _lightingShader = new Shader("Shaders/shader.vert", "Shaders/lighting.frag");
            _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");

            {
                _vaoLamp = GL.GenVertexArray();
                GL.BindVertexArray(_vaoLamp);

                var positionLocation = _lampShader.GetAttribLocation("aPos");
                GL.EnableVertexAttribArray(positionLocation);
                GL.VertexAttribPointer(positionLocation, 3, VertexAttribPointerType.Float, false, 8 * sizeof(float), 0);
            }

            //DiffuseTexture = Texture.LoadFromFile("Resources\\wood.jpg");
            //_specularMap = Texture.LoadFromFile("Resources\\container2_specular.png");

            _camera = new Camera(new Vector3(0, 0, 5), Vector3.UnitY, Size.X / (float)Size.Y);

            CursorState = CursorState.Grabbed;
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // Render cube
            RenderMesh(cubeMesh);

            _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());
           
            _lightingShader.SetVector3("viewPos", _camera.Position);

            _lightingShader.SetInt("material.diffuse", 1);
            _lightingShader.SetInt("material.specular", 1);
            _lightingShader.SetVector3("material.specular", new Vector3(0.5f, 0.5f, 0.5f));
            _lightingShader.SetFloat("material.shininess", 32.0f);


            // Render sphere

            //RenderMesh(sphereMesh, _lightingShader);

            

            /*
            Here we set all the uniforms for the 5/6 types of lights we have. We have to set them manually and index
            the proper PointLight struct in the array to set each uniform variable. This can be done more code-friendly
            by defining light types as classes and set their values in there, or by using a more efficient uniform approach
            by using 'Uniform buffer objects', but that is something we'll discuss in the 'Advanced GLSL' tutorial.
            */
            // Directional light
            _lightingShader.SetVector3("dirLight.direction", new Vector3(-0.2f, -1.0f, -0.3f));
            _lightingShader.SetVector3("dirLight.ambient", new Vector3(0.05f, 0.05f, 0.05f));
            _lightingShader.SetVector3("dirLight.diffuse", new Vector3(0.4f, 0.4f, 0.4f));
            _lightingShader.SetVector3("dirLight.specular", new Vector3(0.5f, 0.5f, 0.5f));

            GL.BindVertexArray(cubeMesh.Vao);
            _lampShader.Use();
            _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            for (int j = 0; j < _pointLightPositions.Length; j++)
            {

            
                // We use a loop to draw all the lights at the proper position
                for (int i = 0; i < cubeMesh.materialsList.Count; i++)
                {
                    Material material = cubeMesh.materialsList[i];
                    material.MaterialFill();
                    int startIndex = material.startIndex;

                    int indexCount = 0;
                    if (i < cubeMesh.materialsList.Count - 1)
                    {
                        indexCount = cubeMesh.materialsList[i + 1].startIndex - startIndex;
                    }
                    else if (i == cubeMesh.materialsList.Count - 1)
                    {
                        indexCount = (cubeMesh.NumVertices * 4) - startIndex;
                    }
                    else
                    {
                        Debug.Print("Uh oh");
                    }


                    Matrix4 lampMatrix = Matrix4.CreateScale(0.2f);
                    lampMatrix = lampMatrix * Matrix4.CreateTranslation(_pointLightPositions[j]);

                    _lampShader.SetMatrix4("model", lampMatrix);

                    GL.DrawArrays(PrimitiveType.Triangles, startIndex, indexCount);
                }
            }

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
        private void RenderMesh(Mesh mesh)
        {
            GL.BindVertexArray(mesh.Vao);
            // Iterate through materials and draw for each
            for (int i = 0; i < mesh.materialsList.Count; i++)
            {
                Material material = mesh.materialsList[i];
                //material.MaterialFill();
                
                // Bind textures and set material-specific uniforms
                if (material.DiffuseTex)
                {
                    if (once)
                    {
                        Debug.Print("mat name: " + material.name);
                        Debug.Print(material.DiffuseMap);
                        once = false;
                    }
                    DiffuseTexture = Texture.LoadFromFile(material.DiffuseMap);
                    DiffuseTexture.Use(TextureUnit.Texture0);
                }

                if (material.SpecularTex)
                {
                    Texture SpecularTex = Texture.LoadFromFile(material.DiffuseMap);
                    SpecularTex.Use(TextureUnit.Texture1);
                }
                _lightingShader.Use();

                // Determine the start index and index count for this material
                int startIndex = material.startIndex;
                
                int indexCount = 0;
                if (i < mesh.materialsList.Count - 1)
                {
                    indexCount = mesh.materialsList[i + 1].startIndex - startIndex;
                }
                else if (i == mesh.materialsList.Count - 1)
                {
                    indexCount = (mesh.NumVertices * 4) - startIndex;
                }
                else
                {
                    Debug.Print("Uh oh");
                }

                Matrix4 model = Matrix4.CreateTranslation(mesh.position);
                float angle = 0f;
                model = model * Matrix4.CreateFromAxisAngle(new Vector3(1.0f, 0.3f, 0.5f), angle);
                _lightingShader.SetMatrix4("model", model);

               // Debug.Print(material.name + "Start: " + startIndex + " Count: " + indexCount);

                // Draw the elements using separate draw calls for each material
                GL.DrawArrays(PrimitiveType.Triangles, startIndex, indexCount);
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