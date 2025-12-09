using EngineCore.Platform.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using System.Collections;
using EngineCore.ECS.Components;
using System.Drawing;


namespace EngineCore.Graphics
{
    public class Renderer
    {
        struct DefaultVertex
        {
            public Vector3 position;
            public Color4 Color;
        }
        struct TextureVertex
        {
            public Vector3 position;
            public Color4 Color;
            public Vector2 uv;
        }

        DefaultVertex[] defaultVertices;
        VertexArray defaultVao;
        VertexBuffer defaultVbo;
        IndexBuffer defaultIbo;

        TextureVertex[] textureVertices;
        VertexArray textureVao;
        VertexBuffer textureVbo;
        IndexBuffer textureIbo;


        UniformBuffer ubo;

        ShaderProgram textureShader;
        ShaderProgram defaultShader;


        public Renderer(GraphicsContext graphicsContext)
        {

            //TEXTURE
            textureVao = new VertexArray(0);
            textureVao.Bind();



            
            textureVao.AddVertexAttrib(VertexType.Vector3, VertexUsage.Position);
            textureVao.AddVertexAttrib(VertexType.Vector4, VertexUsage.Color);
            textureVao.AddVertexAttrib(VertexType.Vector2, VertexUsage.TextureCoord);

            textureVbo = new VertexBuffer(4 * Unsafe.SizeOf<TextureVertex>());
            textureVbo.AttachToVertexArray(textureVao);


            textureIbo = new IndexBuffer(new int[] { 0, 1, 2, 0, 2, 3 });
            textureIbo.Bind();

            textureVertices = new TextureVertex[4];

            textureShader = graphicsContext.ShaderManager.GetShader("TextureShader");
            //textureShader = new ShaderProgram("D:\\Projects\\GameEngine\\EngineEditor\\Resources\\TextureVertex.vert",
            //                            "D:\\Projects\\GameEngine\\EngineEditor\\Resources\\TextureFragment.frag");


            //DEFAULT
            defaultVao = new VertexArray(2);
            defaultVao.Bind();
           
            defaultVao.AddVertexAttrib(VertexType.Vector3, VertexUsage.Position);
            defaultVao.AddVertexAttrib(VertexType.Vector4, VertexUsage.Color);

            defaultVbo = new VertexBuffer(30 * Unsafe.SizeOf<DefaultVertex>());
            defaultVbo.AttachToVertexArray(defaultVao);

            defaultIbo = new IndexBuffer(84 * sizeof(int)); //3(n-2) 
            defaultIbo.Bind();

            defaultVertices = new DefaultVertex[30];

            defaultShader = graphicsContext.ShaderManager.GetShader("DefaultShader");
            //defaultShader = new ShaderProgram("D:\\Projects\\GameEngine\\EngineEditor\\Resources\\DefaultVertex.vert",
            //                                        "D:\\Projects\\GameEngine\\EngineEditor\\Resources\\DefaultFragment.frag");

           

            //RENDERER
            ubo = new UniformBuffer(2 * 4 * 4 * sizeof(float));       
        }


        public void Begin(Camera camera)
        {
            camera.UpdateMatrices();

            ubo.Bind(0);
            ubo.AddData(camera.Projection, 4 * 4 * sizeof(float), 0);
            ubo.AddData(camera.View, 4 * 4 * sizeof(float), 4 * 4 * sizeof(float));
        }      

        public void DrawTexture(Texture texture, Color4 color, Vector2 position, Vector2 scale, Vector2[] UV, float z)
        {
            textureShader.Bind();
            textureVao.Bind();
            textureVbo.Bind();
            textureIbo.Bind();
            texture.Bind();

            textureVertices[0].position = new Vector3(position.X, position.Y, z) - new Vector3(scale.X / 2, scale.Y / 2, 0);
            textureVertices[0].Color = color;
            textureVertices[0].uv = UV[0];

            textureVertices[1].position = new Vector3(position.X, position.Y, z) + new Vector3(-scale.X / 2, scale.Y / 2, 0);
            textureVertices[1].Color = color;
            textureVertices[1].uv = UV[1];

            textureVertices[2].position = new Vector3(position.X, position.Y, z) + new Vector3(scale.X / 2, scale.Y / 2, 0);
            textureVertices[2].Color = color;
            textureVertices[2].uv = UV[2];

            textureVertices[3].position = new Vector3(position.X, position.Y, z) + new Vector3(scale.X / 2, -scale.Y / 2, 0);
            textureVertices[3].Color = color;
            textureVertices[3].uv = UV[3];

            textureVbo.SetData(textureVertices, 4 * Unsafe.SizeOf<TextureVertex>());

            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        public void DrawWidthLine(Vector2 start, Vector2 end, float width, Color4 color, float z)
        {
            defaultShader.Bind();
            defaultVao.Bind();
            defaultVbo.Bind();
            textureIbo.Bind();
          
            Vector2 nperp = (end - start).PerpendicularRight.Normalized() * width;

            defaultVertices[0].position = new Vector3(start.X + nperp.X, start.Y + nperp.Y, z);
            defaultVertices[0].Color = color;

            defaultVertices[1].position = new Vector3(start.X - nperp.X, start.Y - nperp.Y, z);
            defaultVertices[1].Color = color;

            defaultVertices[2].position = new Vector3(end.X - nperp.X, end.Y - nperp.Y, z);
            defaultVertices[2].Color = color;

            defaultVertices[3].position = new Vector3(end.X + nperp.X, end.Y + nperp.Y, z);
            defaultVertices[3].Color = color;


            defaultVbo.SetData(defaultVertices, 4 * Unsafe.SizeOf<DefaultVertex>());

            GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);
        }

        public void DrawPolygon(Vector2[] vertices, Transform transform, Color4 color, float z)
        {
            if (vertices.Length > 30) throw new ArgumentOutOfRangeException("vertices > 10");

            defaultShader.Bind();
            defaultVao.Bind();
            defaultVbo.Bind();
            defaultIbo.Bind();

            int[] indices = polygonIndices(vertices.Length, 0);
            defaultIbo.SetData(indices);

            for(int i = 0; i < vertices.Length; i++)
            {
                Vector4 vec = new Vector4(vertices[i]);
                vec = vec * transform.Matrix;
                vertices[i] = new Vector2(vec.X, vec.Y);
            }

            for(int i = 0; i < vertices.Length; i++)
            {
                defaultVertices[i].position = new Vector3(vertices[i].X, vertices[i].Y, z);
                defaultVertices[i].Color = color;
            }

            defaultVbo.SetData(defaultVertices, vertices.Length * Unsafe.SizeOf<DefaultVertex>());

            GL.DrawElements(BeginMode.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void DrawWidthLines(Vector2[] vertices, float width, Color4 color)
        {

        }

        public void DrawWidthArrow(Vector2 start, Vector2 dir, float length, float width, Color4 color, float z)
        {
            var direction = Vector2.Normalize(dir);
            var end = start + direction * length;

            // Основная линия
            DrawWidthLine(start, end, width, color, z);

            // Параметры стрелки
            const float headRatio = 0.2f;  // 20% от длины стрелки
            const float headAngle = 25f;   // Угол стрелки в градусах

            var headLength = 20;
            var headStart = end - direction * headLength; // Точка, где начинается стрелка

            // Угол поворота для стрелки
            var angleRad = MathHelper.DegreesToRadians(headAngle);
            var cos = MathF.Cos(angleRad);
            var sin = MathF.Sin(angleRad);

            // Левая стрелка (по часовой стрелке)
            var leftDir = new Vector2(
                direction.X * cos - direction.Y * sin,
                direction.X * sin + direction.Y * cos
            );

            // Правая стрелка (против часовой стрелки)
            var rightDir = new Vector2(
                direction.X * cos - direction.Y * (-sin),  // sin с минусом для противоположного поворота
                direction.X * (-sin) + direction.Y * cos
            );

            var leftEnd = leftDir * headLength;
            var rightEnd = rightDir * headLength;

            // Рисуем стрелку
            DrawWidthLine(end, leftEnd, width, color, z);
            DrawWidthLine(end, rightEnd, width, color, z);
        }

        public void DrawWidthArrow(Vector2 start, Vector2 direction, float width, Color4 color, float z)
        {
            var length = direction.Length;
            if (length < 0.001f) return; // Защита от нулевого вектора

            var dir = Vector2.Normalize(direction);
            var end = start + direction; // direction уже содержит и длину, и направление

            // Основная линия
            DrawWidthLine(start, end, width, color, z);

            // Параметры стрелки
            const float headRatio = 0.2f;  // 20% от длины стрелки
            const float headAngle = 160f;   // Угол стрелки в градусах

            var headLength = length;
            var headStart = end - dir * headLength; // Точка, где начинается стрелка

            // Угол поворота для стрелки
            var angleRad = MathHelper.DegreesToRadians(headAngle);
            var cos = MathF.Cos(angleRad);
            var sin = MathF.Sin(angleRad);

            // Левая стрелка (по часовой стрелке)
            var leftDir = new Vector2(
                dir.X * cos - dir.Y * sin,
                dir.X * sin + dir.Y * cos
            );

            // Правая стрелка (против часовой стрелки)
            var rightDir = new Vector2(
                dir.X * cos - dir.Y * (-sin),  // sin с минусом для противоположного поворота
                dir.X * (-sin) + dir.Y * cos
            );

            var leftEnd = headStart + leftDir * headLength;
            var rightEnd = headStart + rightDir * headLength;

            // Рисуем стрелку
            DrawWidthLine(end, leftEnd, width, color, z);
            DrawWidthLine(end, rightEnd, width, color, z);
        }

        public void DrawLine(Vector2 start, Vector2 end, Color4 color, float z)
        {
            defaultShader.Bind();
            defaultVao.Bind();
            defaultVbo.Bind();

            //textureIbo.Bind();


            defaultVertices[0].position = new Vector3(start.X, start.Y, z);
            defaultVertices[0].Color = color;

            defaultVertices[1].position = new Vector3(end.X, end.Y, z);
            defaultVertices[1].Color = color;          

            defaultVbo.SetData(defaultVertices, 2 * Unsafe.SizeOf<DefaultVertex>());

            //GL.DrawElements(BeginMode.Triangles, 6, DrawElementsType.UnsignedInt, 0);

            GL.DrawArrays(PrimitiveType.Lines, 0, 2);
        }

        public void DrawCircle(Vector2 center, float radius, Color4 color, float z, int num = 30)
        {
            float t = 0;
            float dt = MathHelper.TwoPi / num;

            Vector2[] points = new Vector2[num];

            for(int i = 0; i < num; i++)
            {
                float x = center.X + radius * MathF.Cos(t);
                float y = center.Y + radius * MathF.Sin(t);

                points[i] = new Vector2(x, y);

                t += dt;
            }

            for(int i = 0; i < num;  i++)
            {
                DrawLine(points[i], points[(i + 1) % num], color, z);
            }
        }
        public void DrawArrow(Vector2 start, Vector2 direction, Color4 color, float z)
        {
            var length = direction.Length;
            if (length < 0.001f) return; // Защита от нулевого вектора

            var dir = Vector2.Normalize(direction);
            var end = start + direction; // direction уже содержит и длину, и направление

            // Основная линия
            DrawLine(start, end, color, z);

            // Параметры стрелки
            const float headRatio = 0.2f;  // 20% от длины стрелки
            const float headAngle = 160f;   // Угол стрелки в градусах

            var headLength = length * headRatio;
            var headStart = end - dir * headLength; // Точка, где начинается стрелка

            // Угол поворота для стрелки
            var angleRad = MathHelper.DegreesToRadians(headAngle);
            var cos = MathF.Cos(angleRad);
            var sin = MathF.Sin(angleRad);

            // Левая стрелка (по часовой стрелке)
            var leftDir = new Vector2(
                dir.X * cos - dir.Y * sin,
                dir.X * sin + dir.Y * cos
            );

            // Правая стрелка (против часовой стрелки)
            var rightDir = new Vector2(
                dir.X * cos - dir.Y * (-sin),  // sin с минусом для противоположного поворота
                dir.X * (-sin) + dir.Y * cos
            );

            var leftEnd = end + leftDir * MathHelper.Clamp(headLength, 0, 8);
            var rightEnd = end + rightDir * MathHelper.Clamp(headLength, 0, 8);

            // Рисуем стрелку
            DrawLine(end, leftEnd, color, z);
            DrawLine(end, rightEnd, color, z);
        }

        private int[] polygonIndices(int vertices, int offset)
        {
            int[] output = new int[3 * (vertices - 2)];
            int c = 0;
            for (int i = 0; i < vertices - 2; i++)
            {
                output[c] = offset;
                output[c + 1] = 1 + offset + i;
                output[c + 2] = 2 + offset + i;
                c += 3;
            }
            return output;
        }
    }
}
