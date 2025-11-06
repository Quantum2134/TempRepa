using EngineCore.Physics.Dynamic;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics.Collision
{
    internal struct CollisionData
    {
        public Body bodyA;
        public Body bodyB;

        public float depth;
        public Vector2 normal;

        public Vector2 contact1;
        public Vector2 contact2;
    }
}
