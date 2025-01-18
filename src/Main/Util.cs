using System;

namespace DuckGame.C44P;

public static class Util
{
    public static void DrawCircle(Vec2 pos, float radius, Color col, float width = 1f, Depth depth = default, int iterations = 32, int upTo = 32)
    {
        Vec2 vec = Vec2.Zero;
        for (int i = 0; i < iterations; i++)
        {
            float num = Maths.DegToRad(360f / (iterations - 1) * i);
            Vec2 vec2 = new Vec2((float)Math.Cos(num), 0f - (float)Math.Sin(num)) * radius;
            if (i > 0)
                Graphics.DrawLine(pos + vec2, pos + vec, col, width, depth);

            vec = vec2;

            if (i >= upTo) return;
        }
    }
}
