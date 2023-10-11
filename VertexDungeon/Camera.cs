using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;

namespace LearnOpenTK.Common
{
    public class Camera
    {
        public Vector3 _position;
        public Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;
        private float _pitch;
        private float _yaw = -MathHelper.PiOver2;
        private float _fov = MathHelper.PiOver2;
        //private float _movementSpeed = 1.5f;
        private float _mouseSensitivity = 2f;
        private Vector3 _target;

        private Vector2 _lastMousePos;
        private bool _isPanning;
        private Vector3 _orbitTarget;
        private bool _isOrbiting;

        public Camera(Vector3 position, Vector3 target, float aspectRatio)
        {
            Position = position;
            _target = target;
            AspectRatio = aspectRatio;
        }

        public float AspectRatio { private get; set; }

        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                UpdateVectors();
            }
        }

        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;

        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }

        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, _target, _up);
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
        }

        private void UpdateTarget()
        {
            _target = Position + _front;
        }

        public void ProcessInput(KeyboardState keyboardState, MouseState mouseState, float deltaTime)
        {
            if (mouseState.IsButtonDown(MouseButton.Middle))
            {
                // Pan the camera
                if (!_isPanning)
                {
                    _isPanning = true;
                    _lastMousePos = mouseState.Position;
                }

                var sensitivity = _mouseSensitivity * deltaTime;
                var mouseDelta = mouseState.Position - _lastMousePos;

                Position -= _right * mouseDelta.X * sensitivity;
                Position += _up * mouseDelta.Y * sensitivity;

                UpdateVectors();
                UpdateTarget(); // Update the camera's target after panning
            }
            else
            {
                _isPanning = false;
            }

            // Check for Alt key to enable camera orbit
            if (keyboardState.IsKeyDown(Keys.LeftAlt) && mouseState.IsButtonDown(MouseButton.Left))
            {
                if (!_isOrbiting)
                {
                    _isOrbiting = true;
                    _lastMousePos = mouseState.Position;
                }


                var sensitivity = _mouseSensitivity * deltaTime;
                var mouseDelta = mouseState.Position - _lastMousePos;

                _yaw += mouseDelta.X * sensitivity;
                _pitch -= mouseDelta.Y * sensitivity;

                // Limit the pitch angle to avoid flipping the camera
                _pitch = MathHelper.Clamp(_pitch, -89f, 89f);

                UpdateOrbitVectors();
                UpdateTarget(); // Update the camera's target after orbiting
            }
            else
            {
                _isOrbiting = false;
            }

            _lastMousePos = mouseState.Position;
        }

        public void ProcessScrollWheel(float delta)
        {
            float zoomSpeed = 0.5f; // Adjust the sensitivity for zooming

            Vector3 cameraDirection = Vector3.Normalize(_front);
            _position += cameraDirection * delta * zoomSpeed;
        }

        private void UpdateOrbitVectors()
        {
            _front = Vector3.Normalize(_target - _position);
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
            _front = Vector3.Normalize(_front);
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        private void UpdateVectors()
        {
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);
            _front = Vector3.Normalize(_front);
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        public void Orbit(float deltaX, float deltaY, float sensitivity)
        {
            _yaw += deltaX * sensitivity;
            _pitch -= deltaY * sensitivity;

            // Limit the pitch to avoid flipping
            _pitch = MathHelper.Clamp(_pitch, -89f, 89f);

            UpdateVectors();
        }

    }
}
