using System;

namespace AssaultCubeExternal
{
    public struct Vec2 { public float X, Y; }
    public struct Vec3 { public float X, Y, Z; }

    // Column-major 4x4 view matrix (as used in AC / OpenGL style)
    public struct ViewMatrix
    {
        public float m11, m12, m13, m14;
        public float m21, m22, m23, m24;
        public float m31, m32, m33, m34;
        public float m41, m42, m43, m44;

        // Project world position to screen coords via view matrix
        // Returns false if behind camera
        public bool WorldToScreen(Vec3 world, int screenW, int screenH, out Vec2 screen)
        {
            float sX = m11*world.X + m21*world.Y + m31*world.Z + m41;
            float sY = m12*world.X + m22*world.Y + m32*world.Z + m42;
            float sW = m14*world.X + m24*world.Y + m34*world.Z + m44;

            if (sW <= 0.001f) { screen = default; return false; }

            float camX = screenW / 2f;
            float camY = screenH / 2f;

            screen.X = camX + camX * sX / sW;
            screen.Y = camY - camY * sY / sW;
            return true;
        }
    }

    public static class MathHelper
    {
        public static float Dist3D(Vec3 a, Vec3 b)
        {
            float dx = a.X-b.X, dy = a.Y-b.Y, dz = a.Z-b.Z;
            return MathF.Sqrt(dx*dx + dy*dy + dz*dz);
        }

        public static float Dist2D(float ax, float ay, float bx, float by)
        {
            float dx = ax-bx, dy = ay-by;
            return MathF.Sqrt(dx*dx + dy*dy);
        }

        // Calc yaw/pitch in DEGREES to aim from src at dst (for memory write)
        public static void CalcAngle(Vec3 src, Vec3 dst, out float yaw, out float pitch)
        {
            float dx  = dst.X - src.X;
            float dy  = dst.Y - src.Y;
            float dz  = dst.Z - src.Z;
            float hyp = MathF.Sqrt(dx*dx + dy*dy);
            yaw   = MathF.Atan2(dy, dx) * (180f / MathF.PI) + 90f; // +90 AC offset
            pitch = MathF.Atan2(dz, hyp) * (180f / MathF.PI);
        }
    }
}
