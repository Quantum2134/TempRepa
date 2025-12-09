using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;


namespace EngineCore.Graphics
{
    public class Camera
    {     
        private Vector2 _position;
        private float _size;

        private float _aspectRatio;
        private float _nearPlane;
        private float _farPlane;

        private Matrix4 _projection;
        private Matrix4 _view;

        public Vector2 Position
        {
            get => _position;
            set => _position = value;
        }

        public float Size
        {
            get => _size;
            set => _size = Math.Max(value, 0.001f);
        }

        public float AspectRatio
        {
            get => _aspectRatio;
            set => _aspectRatio = MathF.Max(value, 0.001f);
        }

        public float NearPlane
        {
            get => _nearPlane;
            set => _nearPlane = value;
        }

        public float FarPlane
        {
            get => _farPlane;
            set => _farPlane = Math.Max(value, _nearPlane + 0.001f);
        }    

        public Matrix4 Projection => _projection;
        public Matrix4 View => _view;

        public Camera()
        {          
            _position = Vector2.Zero;
            _size = 10f;
            _aspectRatio = 16f / 9f;

            _nearPlane = 0.1f;
            _farPlane = 10000f;
        }     

        public void UpdateMatrices()
        {
            float height = _size;
            float width = _aspectRatio * _size;

            _projection = Matrix4.CreateOrthographic(width, height, _nearPlane, _farPlane);

            Vector3 eye = new Vector3(_position.X, Position.Y, 10);
            Vector3 target = new Vector3(_position.X, _position.Y, -1f);
            Vector3 up = Vector3.UnitY;

            _view = Matrix4.LookAt(eye, target, up);
        }
    }
}
