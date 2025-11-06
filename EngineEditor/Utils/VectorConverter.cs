using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor.Utils
{
    internal static class VectorConverter
    {
        public static System.Numerics.Vector2 ToNumerics(this OpenTK.Mathematics.Vector2 v)
        {
            return new System.Numerics.Vector2(v.X, v.Y);
        }

        public static OpenTK.Mathematics.Vector2 ToOpenTK(this System.Numerics.Vector2 v)
        {
            return new OpenTK.Mathematics.Vector2(v.X, v.Y);
        }

        public static System.Numerics.Vector3 ToNumerics(this OpenTK.Mathematics.Vector3 v)
        {
            return new System.Numerics.Vector3(v.X, v.Y, v.Z);
        }

        public static OpenTK.Mathematics.Vector3 ToOpenTK(this System.Numerics.Vector3 v)
        {
            return new OpenTK.Mathematics.Vector3(v.X, v.Y, v.Z);
        }
    }
}
