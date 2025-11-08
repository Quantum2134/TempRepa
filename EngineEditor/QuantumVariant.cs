using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace EngineEditor
{
    internal static class QuantumVariant
    {
        //C point
        public static Vector2 Cpoint(float t)
        {
            float x = MathF.Sqrt(2) * 0.5f * MathF.Sin(MathF.PI / 4 * t) - 0.5f;
            float y = 0.5f;

            return new Vector2(x, y); //100 pixels = 1m
        }
        public static Vector2 Cvel(float t)
        {
            return new Vector2(0.125f * MathF.Sqrt(2) * MathF.PI * MathF.Cos(MathF.PI / 4 * t), 0);
        }
        public static Vector2 Cacc(float t)
        {
            return new Vector2(-0.03125f * MathF.Sqrt(2) * MathF.PI * MathF.PI * MathF.Sin(MathF.PI / 4 * t), 0);
        }


        //A point
        public static Vector2 Apoint(float t)
        {
            float x = 0.5f * MathF.Cos(-0.5f * MathF.PI * t + MathF.PI / 2) - 0.5f;
            float y = 0.5f * MathF.Sin(-0.5f * MathF.PI * t + MathF.PI / 2) - 0.5f;

            return new Vector2(x, y); //100 pixels = 1m
        }
        public static Vector2 Avel(float t)
        {
            Vector2 FAperpNorm = (Apoint(t) - new Vector2(-0.5f, -0.5f)).PerpendicularRight.Normalized();
            return FAperpNorm * MathHelper.PiOver4;
        }
        public static Vector2 Aacc(float t)
        {
            Vector2 AFNorm = -(Apoint(t) - new Vector2(-0.5f, -0.5f)).Normalized();
            return AFNorm * MathF.PI * MathF.PI / 8f;
        }



        //B point
        public static Vector2 Bpoint(float t)
        {
            Vector2 c = Cpoint(t);
            Vector2 a = Apoint(t);

            Vector2 d = a - c;
            float dlength = d.Length;

            Vector2 center = (a + c) / 2;

            Vector2 ex = d / dlength;
            Vector2 ey = new Vector2(-ex.Y, ex.X);

            float hsqr = 1f - (dlength * dlength * 0.25f);
            float h = MathF.Sqrt(MathF.Max(0, hsqr));



            return center + h * ey;
        }

        public static Vector2 Bvel(float t, float dt)
        {
            Vector2 b = Bpoint(t);
            Vector2 b2 = Bpoint(t + dt);
            return (b2 - b) / dt;
        }

        public static Vector2 Bacc(float t, float dt)
        {
            Vector2 b = Bvel(t, dt);
            Vector2 b2 = Bvel(t + dt, dt);
            return (b2 - b) / dt;
        }

        //mcs
        public static bool Mcs(Vector2 pos1, Vector2 vel1, Vector2 pos2, Vector2 vel2, out Vector2 mcs)
        {
            const float eps = 1e-6f;
            mcs = Vector2.Zero;

            // Проверка на нулевые скорости
            bool v1Zero = vel1.LengthSquared < eps;
            bool v2Zero = vel2.LengthSquared < eps;

            if (v1Zero && v2Zero)
            {
                // Тело неподвижно — МЦС можно выбрать как любую точку (например, pos1)
                mcs = pos1;
                return true;
            }

            if (v1Zero)
            {
                // Тогда МЦС — это pos1
                mcs = pos1;
                return true;
            }

            if (v2Zero)
            {
                mcs = pos2;
                return true;
            }

            // Перпендикуляры к скоростям (направления линий, на которых лежит МЦС)
            Vector2 perp1 = new Vector2(-vel1.Y, vel1.X);
            Vector2 perp2 = new Vector2(-vel2.Y, vel2.X);

            // Проверка на параллельность линий (т.е. коллинеарность перпендикуляров = коллинеарность скоростей)
            float cross = perp1.X * perp2.Y - perp1.Y * perp2.X; // perp1 × perp2

            if (Math.Abs(cross) < eps)
            {
                // Скорости коллинеарны → чистый перенос или вращение вокруг точки на прямой AB
                // Проверим: равны ли скорости?
                if ((vel1 - vel2).LengthSquared < eps)
                {
                    // Чистый перенос — МЦС в бесконечности
                    return false;
                }

                // Иначе — МЦС лежит на прямой AB.
                // Используем факт: v = ω × r → |v| = ω * d → d = |v| / ω
                // Но проще: найдём точку P = pos1 + λ*(pos2 - pos1), такую что (P - pos1) ⟂ vel1
                // На самом деле: в этом случае МЦС лежит на пересечении прямой AB и перпендикуляра к скорости в одной из точек.

                Vector2 ab = pos2 - pos1;
                // Проекция: решаем (pos1 + λ*ab - pos1) · vel1 = 0? Нет.
                // Правильнее: вектор от МЦС к точке перпендикулярен скорости:
                // (pos1 - P) · vel1 = 0 и P = pos1 + λ*ab → -λ*ab · vel1 = 0 → либо ab ⟂ vel1, либо λ=0.
                // Это не общий метод.

                // Вместо этого: используем отношение скоростей
                // |v1| / |v2| = |P - pos1| / |P - pos2|
                // Но без направления сложно.

                // Проще: в этом вырожденном случае можно вернуть false или использовать альтернативный подход.
                // Для простоты пока считаем, что не можем определить однозначно.
                return false;
            }

            // Общий случай: решаем систему
            // pos1 + t * perp1 = pos2 + s * perp2
            // => t * perp1 - s * perp2 = pos2 - pos1
            Vector2 rhs = pos2 - pos1;

            // Матрица системы: [perp1.X   -perp2.X] [t]   = [rhs.X]
            //                  [perp1.Y   -perp2.Y] [s]     [rhs.Y]

            float det = -cross; // = -(perp1.X * perp2.Y - perp1.Y * perp2.X)
            if (Math.Abs(det) < eps)
                return false;

            float t = (-perp2.Y * rhs.X + perp2.X * rhs.Y) / det;
            // float s = ( -perp1.Y * rhs.X + perp1.X * rhs.Y ) / det;

            mcs = pos1 + t * perp1;
            return true;
        }

        public static bool Mcu(Vector2 pos1, Vector2 acc1, Vector2 pos2, Vector2 acc2, out Vector2 mcu)
        {
            const float eps = 1e-6f;
            mcu = Vector2.Zero;

            Vector2 r = pos2 - pos1;
            float r2 = r.LengthSquared;

            if (r2 < eps)
            {
                // Точки совпадают — невозможно определить
                return false;
            }

            Vector2 da = acc2 - acc1;

            // Вычисляем ω² и α
            float omega2 = (-r.X * da.X - r.Y * da.Y) / r2;
            float alpha = (-r.Y * da.X + r.X * da.Y) / r2;

            // Проверка физичности: ω² не может быть отрицательной (с точностью до погрешности)
            if (omega2 < -eps)
            {
                // Невозможное ускорение для твёрдого тела
                return false;
            }

            // Исправляем возможные отрицательные малые значения
            omega2 = Math.Max(omega2, 0.0f);

            // Теперь решаем: найти вектор r0 = pos1 - mca такой, что:
            // acc1 = omega2 * r0 - alpha * R90(r0)
            // где R90(x, y) = (-y, x)
            // =>
            // acc1.x = omega2 * r0.x + alpha * r0.y
            // acc1.y = omega2 * r0.y - alpha * r0.x

            // Система:
            // [omega2    alpha ] [r0.x]   = [acc1.x]
            // [-alpha   omega2 ] [r0.y]     [acc1.y]

            float det = omega2 * omega2 + alpha * alpha;

            if (det < eps)
            {
                // Поступательное движение (ω = 0, α = 0)
                if (acc1.LengthSquared < eps)
                {
                    // Всё тело неподвижно → МЦУ везде
                    mcu = pos1;
                    return true;
                }
                else
                {
                    // Ускорение одинаково и ≠ 0 → МЦУ не существует
                    return false;
                }
            }

            // Решение системы
            float r0x = (omega2 * acc1.X - alpha * acc1.Y) / det;
            float r0y = (alpha * acc1.X + omega2 * acc1.Y) / det;

            // r0 = pos1 - mca  →  mca = pos1 - r0
            mcu = pos1 - new Vector2(r0x, r0y);
            return true;
        }
    }
}
