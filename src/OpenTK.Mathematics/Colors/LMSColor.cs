using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK.Mathematics.Colors
{
    public struct LMSColor : IColor3<LMSColor>, IEquatable<LMSColor>
    {
        private static readonly Matrix3 T = new Matrix3((1.94735469f, 0.68990272f, 0.0f), (-1.41445123f, 0.34832189f, 0.0f), (0.36476327f, 0.0f, 1.93485343f));

        private static readonly Matrix3 TInv = T.Inverted();

        public float L;
        public float M;
        public float S;

        public LMSColor(float l, float m, float s)
        {
            L = l;
            M = m;
            S = s;
        }

        public override bool Equals(object obj)
        {
            return obj is LMSColor color && Equals(color);
        }

        public bool Equals(LMSColor other)
        {
            return L == other.L &&
                   M == other.M &&
                   S == other.S;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(L, M, S);
        }

        public override string ToString()
        {
            return $"(L: {L}, M: {M}, S: {S})";
        }

        public static bool operator ==(LMSColor left, LMSColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(LMSColor left, LMSColor right)
        {
            return !(left == right);
        }

        public static LMSColor Create(float c1, float c2, float c3)
        {
            return new LMSColor(c1, c2, c3);
        }

        public static LMSColor Lerp(LMSColor a, LMSColor b, float t)
        {
            // FIXME: A lot of casts...
            return (LMSColor)Vector3.Lerp((Vector3)a, (Vector3)b, t);
        }

        public static explicit operator Vector3(LMSColor color)
        {
            return new Vector3(color.L, color.M, color.S);
        }

        public static explicit operator LMSColor(Vector3 color)
        {
            return new LMSColor(color.X, color.Y, color.Z);
        }

        // FIXME: LMS to XYZ is undefined unless XYZ really means XFYFZF.
        public XyzColor ToXyz()
        {
            return (XyzColor)(((Vector3)this) * T);
        }

        public static LMSColor FromXyz(XyzColor xyz)
        {
            return (LMSColor)(((Vector3)xyz) * TInv);
        }
    }
}
