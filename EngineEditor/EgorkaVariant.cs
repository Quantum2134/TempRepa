using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineEditor
{
    internal static class EgorkaVariant
    {
        public static Vector2 Cpoint(float t)
        {
            float x = 0.075f * (t * t + 1);
            float y = 0.15f * MathF.Sqrt(3) / 2 * (t * t + 1);



            return new Vector2(x, y); //100 pixels = 1m
        }
        public static Vector2 Cvel(float t)
        {
            return new Vector2(0.15f * t, 0.15f * MathF.Sqrt(3) * t);
        }

        public static Vector2 Apoint(float t)
        {
            float x = 0.05f * t * t * t + 0.25f;
            float y = 0;

            return new Vector2(x, y); //100 pixels = 1m
        }
        public static Vector2 Avel(float t)
        {
            return new Vector2(0.15f * t * t, 0);
        }

        public static Vector2 Bpoint(float t)
        {
            Vector2 c = Cpoint(t);
            Vector2 a = Apoint(t);

            Vector2 d = a - c;
            float dlength = d.Length;

            Vector2 center = (a + c) / 2;

            Vector2 ex = d / dlength;
            Vector2 ey = new Vector2(-ex.Y, ex.X);

            float hsqr = 0.3f * 0.3f - (dlength * dlength * 0.25f);
            float h = MathF.Sqrt(MathF.Max(0, hsqr));



            return center + h * ey;
        }
    }
}
