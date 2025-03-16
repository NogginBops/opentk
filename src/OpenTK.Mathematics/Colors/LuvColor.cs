using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK.Mathematics.Colors
{
    public struct LuvColor : IColor3<LuvColor>, IEquatable<LuvColor>
    {
        // FIXME: Make the illuminant changable?
        // D65
        private const float Xn = 95.0489f;
        private const float Yn = 100.0f;
        private const float Zn = 108.8840f;

        public float L;
        public float u;
        public float v;

        public LuvColor(float l, float u, float v)
        {
            L = l;
            this.u = u;
            this.v = v;
        }

        public override bool Equals(object obj)
        {
            return obj is LuvColor color && Equals(color);
        }

        public bool Equals(LuvColor other)
        {
            return L == other.L &&
                   u == other.u &&
                   v == other.v;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(L, u, v);
        }

        public override string ToString()
        {
            return $"(L: {L}, u: {u}, v: {v})";
        }

        public LChuvColor ToLChuv()
        {
            float C = float.Sqrt((u * u) + (v * v));
            float h = float.Atan2(v, u);
            return new LChuvColor(L, C, h);
        }

        public static bool operator ==(LuvColor left, LuvColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LuvColor left, LuvColor right)
        {
            return !(left == right);
        }

        public static LuvColor Create(float c1, float c2, float c3)
        {
            return new LuvColor(c1, c2, c3);
        }

        public static LuvColor Lerp(LuvColor a, LuvColor b, float t)
        {
            // FIXME: A lot of casts...
            return (LuvColor)Vector3.Lerp((Vector3)a, (Vector3)b, t);
        }

        public static explicit operator Vector3(LuvColor color)
        {
            return new Vector3(color.L, color.u, color.v);
        }

        public static explicit operator LuvColor(Vector3 color)
        {
            return new LuvColor(color.X, color.Y, color.Z);
        }

        public XyzColor ToXyz()
        {
            static float u(XyzColor xyz)
            {
                return (4 * xyz.X) / (xyz.X + (15 * xyz.Y) + (3 * xyz.Z));
            }

            static float v(XyzColor xyz)
            {
                return (9 * xyz.X) / (xyz.X + (15 * xyz.Y) + (3 * xyz.Z));
            }

            float uprim = ((1.0f / 13.0f) * (this.u / L)) + u(new XyzColor(Xn, Yn, Zn));
            float vprim = ((1.0f / 13.0f) * (this.v / L)) + v(new XyzColor(Xn, Yn, Zn));

            const float ThreeTwentyninethPow3 = (3.0f / 29.0f) * (3.0f / 29.0f) * (3.0f / 29.0f);

            float Y;
            if (L <= 8)
            {
                Y = ThreeTwentyninethPow3 * L * Yn;
            }
            else
            {
                float t = (1.0f / 116.0f) * (L + 16);
                Y = t * t * t * Yn;
            }

            float X = ((9.0f * uprim) / (4 * vprim)) * Y;
            float Z = ((12 - (3 * uprim) - (20 * vprim)) / (4 * vprim)) * Y;

            return new XyzColor(X, Y, Z);
        }

        public static LuvColor FromXyz(XyzColor xyz)
        {
            static float u(XyzColor xyz)
            {
                return (4 * xyz.X) / (xyz.X + (15 * xyz.Y) + (3 * xyz.Z));
            }

            static float v(XyzColor xyz)
            {
                return (9 * xyz.X) / (xyz.X + (15 * xyz.Y) + (3 * xyz.Z));
            }

            const float LThresh = (6.0f / 29.0f) * (6.0f / 29.0f) * (6.0f / 29.0f);

            float yyn = xyz.Y / Yn;
            float L;
            if (yyn <= LThresh)
            {
                L = LThresh * yyn;
            }
            else
            {
                L = (116 * float.Cbrt(yyn)) - 16;
            }

            // FIXME: We can precalculate un and vn?
            float ustar = 13 * L * (u(xyz) - u(new XyzColor(Xn, Yn, Zn)));
            float vstar = 13 * L * (v(xyz) - v(new XyzColor(Xn, Yn, Zn)));

            return new LuvColor(L, ustar, vstar);
        }
    }

    public struct LChuvColor : IColor3<LChuvColor>, IEquatable<LChuvColor>
    {
        public float L;
        public float C;
        public float h;

        public LChuvColor(float l, float c, float h)
        {
            L = l;
            C = c;
            this.h = h;
        }

        public override bool Equals(object obj)
        {
            return obj is LChuvColor color && Equals(color);
        }

        public bool Equals(LChuvColor other)
        {
            return L == other.L &&
                   C == other.C &&
                   h == other.h;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(L, C, h);
        }

        public override string ToString()
        {
            return $"(Luv L: {L}, C: {C}, h: {h})";
        }

        public LuvColor ToLuv()
        {
            float u = C * float.Cos(h);
            float v = C * float.Sin(h);
            return new LuvColor(L, u, v);
        }

        public static bool operator ==(LChuvColor left, LChuvColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LChuvColor left, LChuvColor right)
        {
            return !(left == right);
        }

        public static LChuvColor Create(float c1, float c2, float c3)
        {
            return new LChuvColor(c1, c2, c3);
        }

        public static LChuvColor Lerp(LChuvColor a, LChuvColor b, float t)
        {
            // FIXME: A lot of casts...
            return (LChuvColor)Vector3.Lerp((Vector3)a, (Vector3)b, t);
        }

        public static explicit operator Vector3(LChuvColor color)
        {
            return new Vector3(color.L, color.C, color.h);
        }

        public static explicit operator LChuvColor(Vector3 color)
        {
            return new LChuvColor(color.X, color.Y, color.Z);
        }

        public XyzColor ToXyz()
        {
            return ToLuv().ToXyz();
        }

        public static LChuvColor FromXyz(XyzColor xyz)
        {
            return LuvColor.FromXyz(xyz).ToLChuv();
        }
    }
}
