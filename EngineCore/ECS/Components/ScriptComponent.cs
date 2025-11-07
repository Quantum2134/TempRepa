using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.ECS.Components
{
    internal abstract class ScriptComponent : Component
    {
        public string name;

        public abstract void Start();


        public abstract void Update(float dt);
    }
}
