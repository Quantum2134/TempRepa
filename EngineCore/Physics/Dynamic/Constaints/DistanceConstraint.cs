using OpenTK.Graphics.ES20;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics.Dynamic.Constaints
{
    /// <summary>
    /// Constraints the distance between the bodies.
    /// </summary>
    internal class DistanceConstraint : Constraint
    {
        private float distance;

        public float Distance => distance;
        public DistanceConstraint(Body bodyA, Body bodyB, float distance)
        {
            this.bodyA = bodyA;
            this.bodyB = bodyB;
            this.distance = distance;

            Alpha = 0.8f;
            Beta = 1.0f;
        }
        public override void InitConstraint()
        {
            
        }

        public override void SolveConstraint(float timeStep)
        {
            
        }
    }
}
