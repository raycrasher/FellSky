using System;
using Godot;

public static class Utils
{
    public const float TWO_PI = Mathf.Pi * 2;
    public static Vector3 ToVector3(this Vector2 v, float y = 0)
    {
        return new Vector3(v.X, y, v.Y);
    }
    public static Vector2 ToVector2(this Vector3 v)
    {
        return new Vector2(v.X, v.Z);
    }

    public static Vector2 GetRandomVectorInCircle(this Random random, float radius)
    {
        // source: https://stackoverflow.com/a/5838991
        float a = random.NextSingle();
        float b = random.NextSingle();
        if (b < a)
        {
            var t = a;
            a = b;
            b = t;
        }
        return new Vector2(b * radius * Mathf.Cos(TWO_PI * a / b), b * radius * Mathf.Sin(TWO_PI * a / b));
    }
}