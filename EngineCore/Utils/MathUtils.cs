using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Utils
{
    internal static class MathUtils
    {
        /// <summary>
        /// Computes magnitude of cross product of two vectors.
        /// </summary>
        public static float Cross(Vector2 left, Vector2 right)
        {
            return left.X * right.Y - left.Y * right.X;
        }

        /// <summary>
        /// Computes area of a convex polygon.
        /// </summary>
        /// <param name="vertices">An array of polygon local points in counterclockwise order.</param>
        public static float PolygonArea(Vector2[] vertices)
        {
            float area = 0;

            for(int i = 0; i < vertices.Length; i++)
            {
                area += Cross(vertices[i], vertices[(i + 1) % vertices.Length]);
            }

            

            area *= 0.5f;

            return area;
        }

        /// <summary>
        /// Computes area of the circle.
        /// </summary>
        public static float CircleArea(float radius)
        {
            return MathF.PI * radius * radius;
        }

        //public static float PolygonInertia(Vector2[] vertices)
        //{

        //}
    }
}
