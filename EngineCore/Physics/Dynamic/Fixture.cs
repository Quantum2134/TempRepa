using EngineCore.Physics.Collision.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics.Dynamic
{
    internal class Fixture
    {
        public Body Body { get; set; }
        public Shape Shape { get; set; }
        public PhysicsMaterial Material { get; set; }
    }
}
