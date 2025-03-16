using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTK.Mathematics.Colors
{
    public struct LabColor : IColor3<LabColor>, IEquatable<LabColor>
    {
        // D65
        private const float Xn = 95.0489f;
        private const float Yn = 100.0f;
        private const float Zn = 108.8840f;

        public float L;
        public float a;
        public float b;

        public LabColor(float l, float a, float b)
        {
            L = l;
            this.a = a;
            this.b = b;
        }

        public override string ToString()
        {
            return $"(L: {L}, a: {a}, b: {b})";
        }

        public LChabColor ToLCh()
        {
            float C = float.Sqrt((a * a) + (b * b));
            float h = float.Atan2(b, a);
            return new LChabColor(L, C, h);
        }

        public static LabColor Create(float c1, float c2, float c3)
        {
            return new LabColor(c1, c2, c3);
        }

        public static LabColor Lerp(LabColor a, LabColor b, float t)
        {
            // FIXME: A lot of casts...
            return (LabColor)Vector3.Lerp((Vector3)a, (Vector3)b, t);
        }

        public static explicit operator Vector3(LabColor color)
        {
            return new Vector3(color.L, color.a, color.b);
        }

        public static explicit operator LabColor(Vector3 color)
        {
            return new LabColor(color.X, color.Y, color.Z);
        }

        public static bool operator ==(LabColor left, LabColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LabColor left, LabColor right)
        {
            return !(left == right);
        }

        private static float finv(float t)
        {
            const float d = 6.0f / 29.0f;
            const float d2 = d * d;
            if (t > d)
            {
                return t * t * t;
            }
            else
            {
                return 3 * d2 * (t - (4.0f / 29.0f));
            }
        }

        public XyzColor ToXyz()
        {
            float x = Xn * finv(((L + 16) / 166.0f) + (a / 500.0f));
            float y = Yn * finv((L + 16) / 166.0f);
            float z = Xn * finv(((L + 16) / 166.0f) - (b / 500.0f));

            return new XyzColor(x, y, z);
        }

        private static float f(float t)
        {
            const float d = 6.0f / 29.0f;
            const float d2inv = 1.0f / (d * d);
            const float d3 = d * d * d;
            if (t > d3)
            {
                return float.Cbrt(t);
            }
            else
            {
                return (((1.0f / 3.0f) * t) * d2inv) + (4.0f / 29.0f);
            }
        }

        public static LabColor FromXyz(XyzColor xyz)
        {
            float fy = f(xyz.Y / Yn);

            float L = (116 * fy) - 16;
            float a = 500 * (f(xyz.X / Xn) - fy);
            float b = 500 * (fy - f(xyz.Z / Zn));

            return new LabColor(L, a, b);
        }

        public override bool Equals(object obj)
        {
            return obj is LabColor color && Equals(color);
        }

        public bool Equals(LabColor other)
        {
            return L == other.L &&
                   a == other.a &&
                   b == other.b;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(L, a, b);
        }
    }

    public struct LChabColor : IColor3<LChabColor>, IEquatable<LChabColor>
    {
        public float L;
        public float C;
        // FIXME: Is this in degrees or radians??
        public float h;

        public LChabColor(float l, float c, float h)
        {
            L = l;
            C = c;
            this.h = h;
        }

        public override bool Equals(object obj)
        {
            return obj is LChabColor color && Equals(color);
        }

        public bool Equals(LChabColor other)
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
            return $"(Lab L: {L}, C: {C}, h: {h})";
        }

        public LabColor ToLab()
        {
            float a = C * float.Cos(h);
            float b = C * float.Sin(h);
            return new LabColor(L, a, b);
        }

        public static bool operator ==(LChabColor left, LChabColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LChabColor left, LChabColor right)
        {
            return !(left == right);
        }

        public static LChabColor Create(float c1, float c2, float c3)
        {
            return new LChabColor(c1, c2, c3);
        }

        public static LChabColor Lerp(LChabColor a, LChabColor b, float t)
        {
            // FIXME: A lot of casts...
            return (LChabColor)Vector3.Lerp((Vector3)a, (Vector3)b, t);
        }

        public static explicit operator Vector3(LChabColor color)
        {
            return new Vector3(color.L, color.C, color.h);
        }

        public static explicit operator LChabColor(Vector3 color)
        {
            return new LChabColor(color.X, color.Y, color.Z);
        }

        public XyzColor ToXyz()
        {
            return ToLab().ToXyz();
        }

        public static LChabColor FromXyz(XyzColor xyz)
        {
            return LabColor.FromXyz(xyz).ToLCh();
        }
    }
}
