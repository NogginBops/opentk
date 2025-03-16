using System;

namespace OpenTK.Mathematics
{
    public struct XyzColor : IColor3<XyzColor>, IEquatable<XyzColor>
    {
        public float X;
        public float Y;
        public float Z;

        public XyzColor(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public override bool Equals(object obj)
        {
            return obj is XyzColor color && Equals(color);
        }

        public bool Equals(XyzColor other)
        {
            return X == other.X &&
                   Y == other.Y &&
                   Z == other.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Z);
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        public static bool operator ==(XyzColor left, XyzColor right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(XyzColor left, XyzColor right)
        {
            return !(left == right);
        }

        public static XyzColor Create(float c1, float c2, float c3)
        {
            return new XyzColor(c1, c2, c3);
        }

        public static XyzColor Lerp(XyzColor a, XyzColor b, float t)
        {
            // FIXME: A lot of casts...
            return (XyzColor)Vector3.Lerp((Vector3)a, (Vector3)b, t);
        }

        public static explicit operator Vector3(XyzColor color)
        {
            return new Vector3(color.X, color.Y, color.Z);
        }

        public static explicit operator XyzColor(Vector3 color)
        {
            return new XyzColor(color.X, color.Y, color.Z);
        }

        public XyzColor ToXyz()
        {
            return this;
        }

        public static XyzColor FromXyz(XyzColor xyz)
        {
            return xyz;
        }
    }
}
