using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK.Graphics.OpenGL4;

namespace EngineCore.Graphics.GraphicsManagers.States
{
    public enum BlendMode
    {
        None,
        Alpha,
        Additive,
        Multiply,
        Screen,
        Substract
    }
    public struct BlendState
    {
        public bool Enabled;
        public BlendingFactor SrcFactor;
        public BlendingFactor DstFactor;

        public bool Equals(BlendState other)
        {
            return Enabled == other.Enabled && SrcFactor == other.SrcFactor && DstFactor == other.DstFactor;
        }
    }
}
