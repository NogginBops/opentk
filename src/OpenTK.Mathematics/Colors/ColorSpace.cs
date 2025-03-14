using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK.Mathematics
{
    public struct RGBColorSpace
    {
#pragma warning disable SA1307 // Accessible fields should begin with upper-case letter
#pragma warning disable SA1311 // Static readonly fields should begin with upper-case letter
        /// <summary>
        /// The sRGB color space.
        /// See: <see href="https://en.wikipedia.org/wiki/SRGB"/>.
        /// </summary>
        public static RGBColorSpace sRGB => new RGBColorSpace((0.6400f, 0.3300f), (0.3000f, 0.6000f), (0.1500f, 0.0600f), (0.31272f, 0.32903f));
#pragma warning restore SA1311 // Static readonly fields should begin with upper-case letter
#pragma warning restore SA1307 // Accessible fields should begin with upper-case letter

        /// <summary>
        /// The Rec.2020 (BT.2020) color space.
        /// See: <see href="https://en.wikipedia.org/wiki/Rec._2020"/>.
        /// </summary>
        public static RGBColorSpace Rec2020 => new RGBColorSpace((0.708f, 0.292f), (0.170f, 0.797f), (0.131f, 0.046f), (0.3127f, 0.3290f));

        // FIXME: Should we be able to change these after we've created the color space?
        public Vector2 PrimaryR;
        public Vector2 PrimaryG;
        public Vector2 PrimaryB;
        public Vector2 Whitepoint;

        public Matrix3 XYZ_to_RGB { get; private set; }

        public Matrix3 RGB_to_XYZ { get; private set; }

        public RGBColorSpace(Vector2 primaryR, Vector2 primaryG, Vector2 primaryB, Vector2 whitepoint)
        {
            PrimaryR = primaryR;
            PrimaryG = primaryG;
            PrimaryB = primaryB;
            Whitepoint = whitepoint;

            // http://www.brucelindbloom.com/index.html?Eqn_RGB_XYZ_Matrix.html
            Vector3 r2xyz = xyToXYZ(primaryR);
            Vector3 g2xyz = xyToXYZ(primaryG);
            Vector3 b2xyz = xyToXYZ(primaryB);
            Vector3 w = xyToXYZ(whitepoint);

            Matrix3 m = new Matrix3(r2xyz, g2xyz, b2xyz);
            Vector3 scale = w * m.Inverted();
            m.Row0 *= scale.X;
            m.Row1 *= scale.Y;
            m.Row2 *= scale.Z;

            RGB_to_XYZ = m;
            XYZ_to_RGB = m.Inverted();
        }

        /// <summary>
        /// Converts an RGB color to an XYZ color.
        /// </summary>
        /// <param name="rgb">The RGB color to convert to XYZ.</param>
        /// <returns>The converted XYZ color.</returns>
        public readonly Color3<Xyz> ToXyz(Color3<Rgb> rgb)
        {
            return new Color3<Xyz>(((Vector3)rgb) * RGB_to_XYZ);
        }

        /// <summary>
        /// Converts an XYZ color to an RGB color.
        /// This function does not clamp the resulting RGB color to the range [0, 1], and as such can result in negative values.
        /// </summary>
        /// <param name="xyz">The XYZ color to convert to RGB.</param>
        /// <returns>The converted RGB color.</returns>
        public readonly Color3<Rgb> ToRgb(Color3<Xyz> xyz)
        {
            return new Color3<Rgb>(((Vector3)xyz) * XYZ_to_RGB);
        }

        public static Vector3 xyToXYZ(Vector2 xy)
        {
            if (xy.Y == 0)
            {
                return new Vector3(0);
            }
            else
            {
                return new Vector3(xy.X / xy.Y, 1, (1 - xy.X - xy.Y) / xy.Y);
            }
        }

        /// <summary>
        /// Converts a correlated color temperature <paramref name="T"/> to the xy chromaticity for a CIE D-illuminant with the specified correlated color temperature.
        /// </summary>
        /// <param name="T">The correleated color temperature, clamped to the range [4000, 25000] kelvin.</param>
        /// <returns>The xy chromaticity of a D-illuminant with the specified correlated color temperature.</returns>
        public static Vector2 DIlluminantTemperatureToxy(float T)
        {
            // See: http://www.brucelindbloom.com/index.html?Eqn_XYZ_to_T.html
            T = float.Clamp(T, 4000, 25000);
            float T2 = T * T;
            float T3 = T * T2;
            float xD;
            if (T <= 7000)
            {
                xD = (-4.6070e9f / T3) + (2.9678e6f / T2) + (0.09911e3f / T) + 0.244063f;
            }
            else
            {
                xD = (-2.0064e9f / T3) + (1.9018e6f / T2) + (0.24778e3f / T) + 0.237040f;
            }
            float yD = (-3.0f * (xD * xD)) + (2.870f * xD) - 0.275f;
            return (xD, yD);
        }

        /// <summary>
        /// Converts a correlated color temperature <paramref name="T"/> to the xy chromaticity for a CIE D-illuminant with the specified correlated color temperature.
        /// </summary>
        /// <param name="T">The correleated color temperature, in the range [1667, 25000] kelvin.</param>
        /// <returns>The xy chromaticity of a D-illuminant with the specified correlated color temperature.</returns>
        public static Vector2 TemperatureToxy(float T)
        {
            // See: https://en.wikipedia.org/wiki/Planckian_locus#Approximation
            T = float.Clamp(T, 1667, 25000);
            float T2 = T * T;
            float T3 = T * T2;
            float xc;
            if (T <= 4000)
            {
                xc = (-0.2661239e9f / T3) + (-0.2343589e6f / T2) + (0.8776956e3f / T) + 0.179910f;
            }
            else
            {
                xc = (-3.0258469e9f / T3) + (2.1070379e6f / T2) + (0.2226347e3f / T) + 0.240390f;
            }

            float xc2 = xc * xc;
            float xc3 = xc * xc2;

            float yc;
            if (T <= 2222)
            {
                yc = (-1.1063814f * xc3) - (1.34811020f * xc2) + (2.18555832f * xc) - 0.20219683f;
            }
            else if (T <= 4000)
            {
                yc = (-0.9549476f * xc3) - (1.37418593f * xc2) + (2.09137015f * xc) - 0.16748867f;
            }
            else
            {
                yc = (3.0817580f * xc3) - (5.87338670f * xc2) + (3.75112997f * xc) - 0.37001483f;
            }

            return (xc, yc);
        }

        // FIXME: Better name!
        public static Color3<Rgb> TosRGB(Color3<Rgb> linear)
        {
            static float TosRGB(float r)
            {
                if (r <= 0.0031308f)
                {
                    return r * 12.92f;
                }
                else
                {
                    return (1.055f * float.Pow(r, 1 / 2.4f)) - 0.055f;
                }
            }

            return new Color3<Rgb>(TosRGB(linear.X), TosRGB(linear.Y), TosRGB(linear.Z));
        }

        // FIXME: Better name!
        public static Color3<Rgb> ToLinear(Color3<Rgb> srgb)
        {
            static float ToLinear(float r)
            {
                if (r < 0.04045f)
                {
                    return r / 12.92f;
                }
                else
                {
                    return float.Pow((r + 0.055f) / 1.055f, 2.4f);
                }
            }

            return new Color3<Rgb>(ToLinear(srgb.X), ToLinear(srgb.Y), ToLinear(srgb.Z));
        }

        public static Color3<T> Clip<T>(Color3<T> c) where T : IColorSpace3
        {
            return new Color3<T>(float.Clamp(c.X, 0, 1), float.Clamp(c.Y, 0, 1), float.Clamp(c.Z, 0, 1));
        }

        public static Color3<Xyz> WavelengthToXYZ(float lambda)
        {
            if (lambda >= 390 && lambda <= 830)
            {
                float f = (lambda - 390) / 5.0f;
                int i = (int)float.Truncate(f);
                float t = f - i;
                if (i == CIEXYZ_2006_5nm.Length - 1)
                {
                    i = CIEXYZ_2006_5nm.Length - 2;
                    t = 1.0f;
                }

                Vector3 xyz = Vector3.Lerp(CIEXYZ_2006_5nm[i + 0], CIEXYZ_2006_5nm[i + 1], t);
                return new Color3<Xyz>(xyz);
            }
            else
            {
                // FIXME: Interpolate out to 0?
                return new Color3<Xyz>(0, 0, 0);
            }
        }

        /// <summary>
        /// CIE XYZ 2006 color matching fucntions. 390 to 830 (inclusive) nm, 5nm step.
        /// </summary>
        internal static readonly Vector3[] CIEXYZ_2006_5nm = new Vector3[((830 - 390) / 5) + 1]
        {
            (3.769647E-03f, 4.146161E-04f, 1.847260E-02f),
            (9.382967E-03f, 1.059646E-03f, 4.609784E-02f),
            (2.214302E-02f, 2.452194E-03f, 1.096090E-01f),
            (4.742986E-02f, 4.971717E-03f, 2.369246E-01f),
            (8.953803E-02f, 9.079860E-03f, 4.508369E-01f),
            (1.446214E-01f, 1.429377E-02f, 7.378822E-01f),
            (2.035729E-01f, 2.027369E-02f, 1.051821E+00f),
            (2.488523E-01f, 2.612106E-02f, 1.305008E+00f),
            (2.918246E-01f, 3.319038E-02f, 1.552826E+00f),
            (3.227087E-01f, 4.157940E-02f, 1.748280E+00f),
            (3.482554E-01f, 5.033657E-02f, 1.917479E+00f),
            (3.418483E-01f, 5.743393E-02f, 1.918437E+00f),
            (3.224637E-01f, 6.472352E-02f, 1.848545E+00f),
            (2.826646E-01f, 7.238339E-02f, 1.664439E+00f),
            (2.485254E-01f, 8.514816E-02f, 1.522157E+00f),
            (2.219781E-01f, 1.060145E-01f, 1.428440E+00f),
            (1.806905E-01f, 1.298957E-01f, 1.250610E+00f),
            (1.291920E-01f, 1.535066E-01f, 9.991789E-01f),
            (8.182895E-02f, 1.788048E-01f, 7.552379E-01f),
            (4.600865E-02f, 2.064828E-01f, 5.617313E-01f),
            (2.083981E-02f, 2.379160E-01f, 4.099313E-01f),
            (7.097731E-03f, 2.850680E-01f, 3.105939E-01f),
            (2.461588E-03f, 3.483536E-01f, 2.376753E-01f),
            (3.649178E-03f, 4.277595E-01f, 1.720018E-01f),
            (1.556989E-02f, 5.204972E-01f, 1.176796E-01f),
            (4.315171E-02f, 6.206256E-01f, 8.283548E-02f),
            (7.962917E-02f, 7.180890E-01f, 5.650407E-02f),
            (1.268468E-01f, 7.946448E-01f, 3.751912E-02f),
            (1.818026E-01f, 8.575799E-01f, 2.438164E-02f),
            (2.405015E-01f, 9.071347E-01f, 1.566174E-02f),
            (3.098117E-01f, 9.544675E-01f, 9.846470E-03f),
            (3.804244E-01f, 9.814106E-01f, 6.131421E-03f),
            (4.494206E-01f, 9.890228E-01f, 3.790291E-03f),
            (5.280233E-01f, 9.994608E-01f, 2.327186E-03f),
            (6.133784E-01f, 9.967737E-01f, 1.432128E-03f),
            (7.016774E-01f, 9.902549E-01f, 8.822531E-04f),
            (7.967750E-01f, 9.732611E-01f, 5.452416E-04f),
            (8.853376E-01f, 9.424569E-01f, 3.386739E-04f),
            (9.638388E-01f, 8.963613E-01f, 2.117772E-04f),
            (1.051011E+00f, 8.587203E-01f, 1.335031E-04f),
            (1.109767E+00f, 8.115868E-01f, 8.494468E-05f),
            (1.143620E+00f, 7.544785E-01f, 5.460706E-05f),
            (1.151033E+00f, 6.918553E-01f, 3.549661E-05f),
            (1.134757E+00f, 6.270066E-01f, 2.334738E-05f),
            (1.083928E+00f, 5.583746E-01f, 1.554631E-05f),
            (1.007344E+00f, 4.895950E-01f, 1.048387E-05f),
            (9.142877E-01f, 4.229897E-01f, 0.000000E+00f),
            (8.135565E-01f, 3.609245E-01f, 0.000000E+00f),
            (6.924717E-01f, 2.980865E-01f, 0.000000E+00f),
            (5.755410E-01f, 2.416902E-01f, 0.000000E+00f),
            (4.731224E-01f, 1.943124E-01f, 0.000000E+00f),
            (3.844986E-01f, 1.547397E-01f, 0.000000E+00f),
            (2.997374E-01f, 1.193120E-01f, 0.000000E+00f),
            (2.277792E-01f, 8.979594E-02f, 0.000000E+00f),
            (1.707914E-01f, 6.671045E-02f, 0.000000E+00f),
            (1.263808E-01f, 4.899699E-02f, 0.000000E+00f),
            (9.224597E-02f, 3.559982E-02f, 0.000000E+00f),
            (6.639960E-02f, 2.554223E-02f, 0.000000E+00f),
            (4.710606E-02f, 1.807939E-02f, 0.000000E+00f),
            (3.292138E-02f, 1.261573E-02f, 0.000000E+00f),
            (2.262306E-02f, 8.661284E-03f, 0.000000E+00f),
            (1.575417E-02f, 6.027677E-03f, 0.000000E+00f),
            (1.096778E-02f, 4.195941E-03f, 0.000000E+00f),
            (7.608750E-03f, 2.910864E-03f, 0.000000E+00f),
            (5.214608E-03f, 1.995557E-03f, 0.000000E+00f),
            (3.569452E-03f, 1.367022E-03f, 0.000000E+00f),
            (2.464821E-03f, 9.447269E-04f, 0.000000E+00f),
            (1.703876E-03f, 6.537050E-04f, 0.000000E+00f),
            (1.186238E-03f, 4.555970E-04f, 0.000000E+00f),
            (8.269535E-04f, 3.179738E-04f, 0.000000E+00f),
            (5.758303E-04f, 2.217445E-04f, 0.000000E+00f),
            (4.058303E-04f, 1.565566E-04f, 0.000000E+00f),
            (2.856577E-04f, 1.103928E-04f, 0.000000E+00f),
            (2.021853E-04f, 7.827442E-05f, 0.000000E+00f),
            (1.438270E-04f, 5.578862E-05f, 0.000000E+00f),
            (1.024685E-04f, 3.981884E-05f, 0.000000E+00f),
            (7.347551E-05f, 2.860175E-05f, 0.000000E+00f),
            (5.259870E-05f, 2.051259E-05f, 0.000000E+00f),
            (3.806114E-05f, 1.487243E-05f, 0.000000E+00f),
            (2.758222E-05f, 1.080001E-05f, 0.000000E+00f),
            (2.004122E-05f, 7.863920E-06f, 0.000000E+00f),
            (1.458792E-05f, 5.736935E-06f, 0.000000E+00f),
            (1.068141E-05f, 4.211597E-06f, 0.000000E+00f),
            (7.857521E-06f, 3.106561E-06f, 0.000000E+00f),
            (5.768284E-06f, 2.286786E-06f, 0.000000E+00f),
            (4.259166E-06f, 1.693147E-06f, 0.000000E+00f),
            (3.167765E-06f, 1.262556E-06f, 0.000000E+00f),
            (2.358723E-06f, 9.422514E-07f, 0.000000E+00f),
            (1.762465E-06f, 7.053860E-07f, 0.000000E+00f),
        };
    }
}
