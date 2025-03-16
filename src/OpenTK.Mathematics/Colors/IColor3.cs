namespace OpenTK.Mathematics
{
    // We ideally would like to be alpha agnostic in most of our stuff.
    // Like we can add an alpha value to all types of colors...

    // Should XYZ <-> RGB conversions be simple matrices you get or should they be functions?

    public interface IColor3<T> where T : IColor3<T>
    {
        public static abstract T Create(float c1, float c2, float c3);

        public static abstract T Lerp(T a, T b, float t);

        static abstract explicit operator Vector3(T color);

        static abstract explicit operator T(Vector3 color);

        public XyzColor ToXyz();

        public static abstract T FromXyz(XyzColor xyz);
    }
}
