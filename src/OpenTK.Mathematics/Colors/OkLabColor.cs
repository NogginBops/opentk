using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OpenTK.Mathematics.Colors
{
    public struct OkLabColor : IColor3<OkLabColor>, IEquatable<OkLabColor>
    {
        // FIXME: Check that these matrices are correct!
        public static readonly Matrix3 M1 = new Matrix3(new Vector3(+0.8189330101f, +0.0329845436f, +0.0482003018f), new Vector3(+0.3618667424f, +0.9293118715f, +0.2643662691f), new Vector3(-0.1288597137f, +0.0361456387f, +0.6338517070f));
        public static readonly Matrix3 M2 = new Matrix3(new Vector3(+0.2104542553f, +1.9779984951f, +0.0259040371f), new Vector3(+0.7936177850f, -2.4285922050f, +0.7827717662f), new Vector3(-0.0040720468f, +0.4505937099f, -0.8086757660f));

        public static readonly Matrix3 M1Inv = M1.Inverted();
        public static readonly Matrix3 M2Inv = M2.Inverted();

        public float L;
        public float a;
        public float b;

        public OkLabColor(float l, float a, float b)
        {
            L = l;
            this.a = a;
            this.b = b;
        }

        public override string ToString()
        {
            return $"(Ok L: {L}, a: {a}, b: {b})";
        }

        public OkLChColor ToOkLCh()
        {
            float C = float.Sqrt((a * a) + (b * b));
            float h = float.Atan2(b, a);
            return new OkLChColor(L, C, h);
        }

        public static OkLabColor Create(float c1, float c2, float c3)
        {
            return new OkLabColor(c1, c2, c3);
        }

        public static OkLabColor Lerp(OkLabColor a, OkLabColor b, float t)
        {
            // FIXME: A lot of casts...
            return (OkLabColor)Vector3.Lerp((Vector3)a, (Vector3)b, t);
        }

        public static explicit operator Vector3(OkLabColor color)
        {
            return new Vector3(color.L, color.a, color.b);
        }

        public static explicit operator OkLabColor(Vector3 color)
        {
            return new OkLabColor(color.X, color.Y, color.Z);
        }

        public static bool operator ==(OkLabColor left, OkLabColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OkLabColor left, OkLabColor right)
        {
            return !(left == right);
        }

        public XyzColor ToXyz()
        {
            Vector3 lmsprime = ((Vector3)this) * M1Inv;
            Vector3 lms = lmsprime * lmsprime * lmsprime;
            Vector3 xyz = lms * M2Inv;
            return new XyzColor(xyz.X, xyz.Y, xyz.Z);
        }

        public static OkLabColor FromXyz(XyzColor xyz)
        {
            Vector3 lms = ((Vector3)xyz) * M1;
            Vector3 lmsprime = new Vector3(float.Cbrt(lms.X), float.Cbrt(lms.Y), float.Cbrt(lms.Z));
            Vector3 Lab = lmsprime * M2;
            return new OkLabColor(Lab.X, Lab.Y, Lab.Z);
        }

        public override bool Equals(object obj)
        {
            return obj is OkLabColor color && Equals(color);
        }

        public bool Equals(OkLabColor other)
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

    public struct OkLChColor : IColor3<OkLChColor>, IEquatable<OkLChColor>
    {
        public float L;
        public float C;
        public float h;

        public OkLChColor(float l, float c, float h)
        {
            L = l;
            C = c;
            this.h = h;
        }

        public override string ToString()
        {
            return $"(Ok L: {L}, C: {C}, h: {h})";
        }

        public static OkLChColor Create(float c1, float c2, float c3)
        {
            return new OkLChColor(c1, c2, c3);
        }

        public static OkLChColor Lerp(OkLChColor a, OkLChColor b, float t)
        {
            // FIXME: A lot of casts...
            return (OkLChColor)Vector3.Lerp((Vector3)a, (Vector3)b, t);
        }

        public static explicit operator Vector3(OkLChColor color)
        {
            return new Vector3(color.L, color.C, color.h);
        }

        public static explicit operator OkLChColor(Vector3 color)
        {
            return new OkLChColor(color.X, color.Y, color.Z);
        }

        public static bool operator ==(OkLChColor left, OkLChColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(OkLChColor left, OkLChColor right)
        {
            return !(left == right);
        }

        public OkLabColor ToOkLab()
        {
            float a = C * float.Cos(h);
            float b = C * float.Sin(h);
            return new OkLabColor(L, a, b);
        }

        public XyzColor ToXyz()
        {
            return this.ToOkLab().ToXyz();
        }

        public static OkLChColor FromXyz(XyzColor xyz)
        {
            return OkLabColor.FromXyz(xyz).ToOkLCh();
        }

        public override bool Equals(object obj)
        {
            return obj is OkLChColor color && Equals(color);
        }

        public bool Equals(OkLChColor other)
        {
            return L == other.L &&
                   C == other.C &&
                   h == other.h;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(L, C, h);
        }
    }
}
