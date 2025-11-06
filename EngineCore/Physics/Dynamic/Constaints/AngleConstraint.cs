using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace EngineCore.Physics.Dynamic.Constaints
{
    /// <summary>
    /// Constaints the angle between the line connecting the bodies and the horizontal line.
    /// </summary>
    class AngleConstraint : Constraint
    {
        private float angle;

        public float Angle => angle;
       
        public AngleConstraint(Body bodyA, Body bodyB, float angle)
        {
            this.bodyA = bodyA;
            this.bodyB = bodyB;
            this.angle = MathHelper.DegreesToRadians(angle);

            Alpha = 0.8f;
            Beta = 1.0f;
        }

        public void SetAngle(float angle)
        {
            this.angle = MathHelper.DegreesToRadians(angle);
        }

        public override void InitConstraint()
        {
            
        }

        public override void SolveConstraint(float timeStep)
        {
            
        }
    }
}
