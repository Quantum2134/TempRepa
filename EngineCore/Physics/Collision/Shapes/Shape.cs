using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics.Collision.Shapes
{
    internal abstract class Shape
    {
        public Vector2[] vertices {  get; set; }

        public float radius {  get; set; }

        public ShapeType ShapeType {  get; set; }
    }
}
