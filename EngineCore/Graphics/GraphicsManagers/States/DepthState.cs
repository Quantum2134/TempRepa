using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;


namespace EngineCore.Graphics.GraphicsManagers.States
{
    public enum DepthMode
    {
        Less,
        LessOrEqual,
        Equal,
        Greater,
        GreaterOrEqual,
        Always,
        Never
    }
    public struct DepthState
    {
        public bool Enabled;
        public DepthFunction Func;        

        public bool Equals(DepthState other)
        {          
            return Enabled == other.Enabled && Func == other.Func;
        }
    }
}
