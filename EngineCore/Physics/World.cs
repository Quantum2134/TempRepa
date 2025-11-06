using EngineCore.Physics.Dynamic;
using EngineCore.Physics.Dynamic.Constaints;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics;

namespace EngineCore.Physics
{   
    internal class World
    {
        private List<Body> bodies;
        private List<Constraint> constraints;

        public List<Body> Bodies => bodies;
        public List<Constraint> Contraints => constraints;
        public World()
        {
            bodies = new List<Body>();
            constraints = new List<Constraint>();
        }

        public void AddBody(Body body)
        {
            bodies.Add(body);
        }
        public bool RemoveBody(Body body)
        {
            return bodies.Remove(body);
        }

        public void AddConstraint(Constraint constraint)
        {
            constraints.Add(constraint);
        }
        public bool RemoveConstraint(Constraint constraint)
        {
            return constraints.Remove(constraint);
        }

        public void Update(float timeStep, int subSteps)
        {
            float subDt = timeStep / subSteps;

            for(int i = 0; i < subSteps; i++)
            {
                //Integrate body velocity
                foreach(Body body in bodies)
                {
                    //body.Velocity += body.acceleration * subDt;
                }

                //Solve velocity constraints
                foreach(Constraint constraint in constraints)
                {
                    constraint.SolveConstraint(subDt);
                }

                //Integrate body positions
                foreach (Body body in bodies)
                {
                    //body.position += body.velocity * subDt;
                }

                //Solve position constraints
                foreach (Constraint constraint in constraints)
                {

                }
            }
        }
    }
}
