using EngineCore.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.ECS.Components
{
    public abstract class ScriptComponent : Component
    {
        public string name;

        public Application Application;

        public abstract void Start();


        public abstract void Update(float dt);
    }
}
