using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

namespace EngineCore.Graphics.GraphicsManagers.States
{
    public struct ScissorState
    {
        public bool Enabled;
        public int X, Y, Width, Height;

        public bool Equals(ScissorState other)
        {
            return Enabled == other.Enabled && X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
        }
    }
}
