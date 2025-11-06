using EngineCore.Physics.Collision.Shapes;
using EngineCore.Utils;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics.Dynamic
{
    internal class Body
    {
        private float mass;
        private float inertiaMoment;

        public Vector2 Position {  get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Acceleration { get; set; }
        public Vector2 Force { get; set; }
        public float Mass
        {
            get => mass;
            set
            {
                mass = value;
                
                if(mass <= 0f)
                {
                    mass = 1f;
                }

                InvMass = 1f / mass;
            }
        }
        public float InvMass { get; private set; }


        public float Rotation { get; set; }
        public float AngularVelocity { get; set; }
        public float AngularAcceleration { get; set; }
        public float Torque { get; set; }
        public float InertiaMoment
        {
            get => inertiaMoment;
            set
            {
                inertiaMoment = value;

                if(inertiaMoment <= 0f)
                {
                    inertiaMoment = 1f;
                }

                InvInertiaMoment = 1f / inertiaMoment;
            }
        }
        public float InvInertiaMoment { get; private set;}


        public BodyType BodyType;

        public World World { get; set; }

        public Shape Shape { get; set; }

        
        public Body(World world)
        {
            Position = new Vector2(0, 0);
            Velocity = new Vector2(0, 0);
            Acceleration = new Vector2(0, 0);
            Force = new Vector2(0, 0);
            Mass = 1;
            InvMass = 1;

            Rotation = 0;
            AngularVelocity = 0;
            AngularAcceleration = 0;
            Torque = 0;
            InertiaMoment = 1;
            InvInertiaMoment = 1;

            BodyType = BodyType.Dynamic;

            World = world;
        }

        public void AddImpulse(Vector2 impulse, Vector2 point)
        {
            Velocity += impulse * InvMass;
            AngularVelocity += MathUtils.Cross(point, impulse) * InvInertiaMoment;
        }      
    }
}
