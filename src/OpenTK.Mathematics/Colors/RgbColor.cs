using System;
using System.Drawing;

namespace OpenTK.Mathematics
{
    public interface IColorSpace<T> where T : IColor3<T>
    {
        static abstract XyzColor ToXyz(T color);

        static abstract T FromXyz(XyzColor color);
    }

    public interface IRgbColorSpace<T> : IColorSpace<RgbColor<T>> where T : IRgbColorSpace<T>
    {
        static abstract XyzColor ToXyz(RgbColor<T> rgb);

        static abstract RgbColor<T> FromXyz(XyzColor xyz);
    }

    public sealed class sRGB : IRgbColorSpace<sRGB>
    {
        private sRGB()
        {
        }

        public static XyzColor ToXyz(RgbColor<sRGB> rgb)
        {
            return new XyzColor(rgb.R, rgb.G, rgb.B);
        }

        public static RgbColor<sRGB> FromXyz(XyzColor xyz)
        {
            return new RgbColor<sRGB>(xyz.X, xyz.Y, xyz.Z);
        }

        public static void Test()
        {
            RgbColor<sRGB> a = new RgbColor<sRGB>(0.1f, 0.2f, 0.6f);
            var hsv = HsvColor<sRGB>.FromXyz(a.ToXyz());
            RgbColor<sRGB>.Lerp(a, a, 0.1f);
        }
    }

    // FIXME: We might want to be able to have RgbColorspace instances...?
    // How do we deal with EOTFs?
    public struct RgbColor<T> : IColor3<RgbColor<T>>, IEquatable<RgbColor<T>> where T : IRgbColorSpace<T>
    {
        public float R;
        public float G;
        public float B;

        public RgbColor(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public override bool Equals(object obj)
        {
            return obj is RgbColor<T> color && Equals(color);
        }

        public bool Equals(RgbColor<T> other)
        {
            return R == other.R &&
                   G == other.G &&
                   B == other.B;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(R, G, B);
        }

        public override string ToString()
        {
            return $"(R: {R}, G: {G}, B: {B})";
        }

        public HslColor<T> ToHsl()
        {
            float r = R;
            float g = G;
            float b = B;

            float max = MathF.Max(r, MathF.Max(g, b));
            float min = MathF.Min(r, MathF.Min(g, b));
            float diff = max - min;

            float h = 0.0f;
            if (diff == 0)
            {
                h = 0.0f;
            }
            else if (max == r)
            {
                h = ((g - b) / diff) % 6;
                if (h < 0)
                {
                    h += 6;
                }
            }
            else if (max == g)
            {
                h = ((b - r) / diff) + 2.0f;
            }
            else if (max == b)
            {
                h = ((r - g) / diff) + 4.0f;
            }

            float hue = h / 6.0f;
            if (hue < 0.0f)
            {
                hue += 1.0f;
            }

            float lightness = (max + min) / 2.0f;

            float saturation = 0.0f;
            if ((1.0f - Math.Abs((2.0f * lightness) - 1.0f)) != 0)
            {
                saturation = diff / (1.0f - Math.Abs((2.0f * lightness) - 1.0f));
            }

            return new HslColor<T>(hue * 360, saturation, lightness);
        }

        public HsvColor<T> ToHsv()
        {
            float r = R;
            float g = G;
            float b = B;

            float max = Math.Max(r, Math.Max(g, b));
            float min = Math.Min(r, Math.Min(g, b));
            float diff = max - min;

            float h = 0.0f;
            if (diff == 0)
            {
                h = 0.0f;
            }
            else if (max == r)
            {
                h = ((g - b) / diff) % 6.0f;
                if (h < 0)
                {
                    h += 6f;
                }
            }
            else if (max == g)
            {
                h = ((b - r) / diff) + 2.0f;
            }
            else if (max == b)
            {
                h = ((r - g) / diff) + 4.0f;
            }

            float hue = h * 60.0f / 360.0f;

            float saturation = 0.0f;
            if (max != 0.0f)
            {
                saturation = diff / max;
            }

            return new HsvColor<T>(hue * 360, saturation, max);
        }

        public static bool operator ==(RgbColor<T> left, RgbColor<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(RgbColor<T> left, RgbColor<T> right)
        {
            return !(left == right);
        }

        public static RgbColor<T> Create(float c1, float c2, float c3)
        {
            return new RgbColor<T>(c1, c2, c3);
        }

        public static explicit operator Vector3(RgbColor<T> color)
        {
            return new Vector3(color.R, color.G, color.B);
        }

        public static explicit operator RgbColor<T>(Vector3 color)
        {
            return new RgbColor<T>(color.X, color.Y, color.Z);
        }

        public static RgbColor<T> FromXyz(XyzColor xyz)
        {
            return T.FromXyz(xyz);
        }

        public XyzColor ToXyz()
        {
            return T.ToXyz(this);
        }

        public static RgbColor<T> Lerp(RgbColor<T> a, RgbColor<T> b, float t)
        {
            // FIXME: A lot of casts...
            return (RgbColor<T>)Vector3.Lerp((Vector3)a, (Vector3)b, t);
        }
    }

    public struct HslColor<T> : IColor3<HslColor<T>>, IEquatable<HslColor<T>> where T : IRgbColorSpace<T>
    {
        /// <summary>
        /// Hue [0, 360).
        /// </summary>
        public float H;
        public float S;
        public float L;

        public HslColor(float h, float s, float l)
        {
            H = h;
            S = s;
            L = l;
        }

        public override bool Equals(object obj)
        {
            return obj is HslColor<T> color && Equals(color);
        }

        public bool Equals(HslColor<T> other)
        {
            return H == other.H &&
                   S == other.S &&
                   L == other.L;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(H, S, L);
        }

        public override string ToString()
        {
            return $"(H: {H}, S: {S}, L: {L})";
        }

        public RgbColor<T> ToRgb()
        {
            float hue = H;
            float saturation = S;
            float lightness = L;

            float c = (1.0f - MathF.Abs((2.0f * lightness) - 1.0f)) * saturation;

            float h = hue / 60.0f;
            float x = c * (1.0f - MathF.Abs((h % 2.0f) - 1.0f));

            float r, g, b;
            if (h >= 0.0f && h < 1.0f)
            {
                r = c;
                g = x;
                b = 0.0f;
            }
            else if (h >= 1.0f && h < 2.0f)
            {
                r = x;
                g = c;
                b = 0.0f;
            }
            else if (h >= 2.0f && h < 3.0f)
            {
                r = 0.0f;
                g = c;
                b = x;
            }
            else if (h >= 3.0f && h < 4.0f)
            {
                r = 0.0f;
                g = x;
                b = c;
            }
            else if (h >= 4.0f && h < 5.0f)
            {
                r = x;
                g = 0.0f;
                b = c;
            }
            else if (h >= 5.0f && h < 6.0f)
            {
                r = c;
                g = 0.0f;
                b = x;
            }
            else
            {
                r = 0.0f;
                g = 0.0f;
                b = 0.0f;
            }

            var m = lightness - (c / 2.0f);
            if (m < 0)
            {
                m = 0;
            }

            return new RgbColor<T>(r + m, g + m, b + m);
        }

        public static bool operator ==(HslColor<T> left, HslColor<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HslColor<T> left, HslColor<T> right)
        {
            return !(left == right);
        }

        public float Comp1 => H;

        public float Comp2 => S;

        public float Comp3 => L;

        public static HslColor<T> Create(float c1, float c2, float c3)
        {
            return new HslColor<T>(c1, c2, c3);
        }

        public static explicit operator Vector3(HslColor<T> color)
        {
            return new Vector3(color.H, color.S, color.L);
        }

        public static explicit operator HslColor<T>(Vector3 color)
        {
            return new HslColor<T>(color.X, color.Y, color.Z);
        }

        public static HslColor<T> FromXyz(XyzColor xyz)
        {
            return T.FromXyz(xyz).ToHsl();
        }

        public XyzColor ToXyz()
        {
            return T.ToXyz(this.ToRgb());
        }

        public static HslColor<T> Lerp(HslColor<T> a, HslColor<T> b, float t)
        {
            // FIXME: A lot of casts...
            return (HslColor<T>)Vector3.Lerp((Vector3)a, (Vector3)b, t);
        }
    }

    public struct HsvColor<T> : IColor3<HsvColor<T>>, IEquatable<HsvColor<T>> where T : IRgbColorSpace<T>
    {
        public float H;
        public float S;
        public float V;

        public HsvColor(float h, float s, float v)
        {
            H = h;
            S = s;
            V = v;
        }

        public override bool Equals(object obj)
        {
            return obj is HsvColor<T> color && Equals(color);
        }

        public bool Equals(HsvColor<T> other)
        {
            return H == other.H &&
                   S == other.S &&
                   V == other.V;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(H, S, V);
        }

        public override string ToString()
        {
            return $"(H: {H}, S: {S}, V: {V})";
        }

        public RgbColor<T> ToRgb()
        {
            float hue = H;
            float saturation = S;
            float value = V;

            float c = value * saturation;

            float h = hue / 60.0f;
            float x = c * (1.0f - Math.Abs((h % 2.0f) - 1.0f));

            float r, g, b;
            if (h >= 0.0f && h < 1.0f)
            {
                r = c;
                g = x;
                b = 0.0f;
            }
            else if (h >= 1.0f && h < 2.0f)
            {
                r = x;
                g = c;
                b = 0.0f;
            }
            else if (h >= 2.0f && h < 3.0f)
            {
                r = 0.0f;
                g = c;
                b = x;
            }
            else if (h >= 3.0f && h < 4.0f)
            {
                r = 0.0f;
                g = x;
                b = c;
            }
            else if (h >= 4.0f && h < 5.0f)
            {
                r = x;
                g = 0.0f;
                b = c;
            }
            else if (h >= 5.0f && h < 6.0f)
            {
                r = c;
                g = 0.0f;
                b = x;
            }
            else
            {
                r = 0.0f;
                g = 0.0f;
                b = 0.0f;
            }

            float m = value - c;

            return new RgbColor<T>(r + m, g + m, b + m);
        }

        public static bool operator ==(HsvColor<T> left, HsvColor<T> right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HsvColor<T> left, HsvColor<T> right)
        {
            return !(left == right);
        }

        public float Comp1 => H;

        public float Comp2 => S;

        public float Comp3 => V;

        public static HsvColor<T> Create(float c1, float c2, float c3)
        {
            return new HsvColor<T>(c1, c2, c3);
        }

        public static explicit operator Vector3(HsvColor<T> color)
        {
            return new Vector3(color.Comp1, color.Comp2, color.Comp3);
        }

        public static explicit operator HsvColor<T>(Vector3 color)
        {
            return new HsvColor<T>(color.X, color.Y, color.Z);
        }

        public static HsvColor<T> FromXyz(XyzColor xyz)
        {
            return T.FromXyz(xyz).ToHsv();
        }

        public XyzColor ToXyz()
        {
            return T.ToXyz(this.ToRgb());
        }

        public static HsvColor<T> Lerp(HsvColor<T> a, HsvColor<T> b, float t)
        {
            // FIXME: A lot of casts...
            return (HsvColor<T>)Vector3.Lerp((Vector3)a, (Vector3)b, t);
        }
    }
}
