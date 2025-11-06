using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics.Dynamic.Constaints
{
    internal abstract class Constraint
    {
        protected Body bodyA;
        protected Body bodyB;

        public Body BodyA => bodyA;
        public Body BodyB => bodyB;


        public float Alpha { get; set; }
        public float Beta { get; set; }

        public abstract void InitConstraint();
        public abstract void SolveConstraint(float timeStep);
    }
}
