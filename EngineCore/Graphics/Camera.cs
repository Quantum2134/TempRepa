using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;


namespace EngineCore.Graphics
{
    public class Camera
    {
        private Vector3 _position;

        private Matrix4 _projection;
        private Matrix4 _view;
           
        public Vector2 Position { get; set; }
        public float Size { get; set; }
        public float AspectRatio {  get; set; }
        public Matrix4 Projection => _projection;
        public Matrix4 View => _view;

        public Camera()
        {
            _position = Vector3.Zero;
            Position = Vector2.Zero;
            Size = 0;
            AspectRatio = 0;

            _projection = Matrix4.Identity;
            _view = Matrix4.Identity;
        }

        public void UpdateMatrices()
        {
            _position.Xy = Position;

            _projection = Matrix4.CreateOrthographic(AspectRatio * Size, Size, 0f, 10000f);
            _view = Matrix4.LookAt(_position, _position - Vector3.UnitZ, Vector3.UnitY);
        }
    }
}
