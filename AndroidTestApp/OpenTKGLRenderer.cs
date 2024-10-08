using Android.Graphics;
using Android.Opengl;
using Android.Runtime;
using Android.Views;
using OpenTK.Graphics.OpenGLES2;
using Javax.Microedition.Khronos.Opengles;
using OpenTK.Graphics.Egl;
using OpenTK;
using OpenTK.Graphics;

namespace AndroidTestApp
{
    internal class AndroidBindingsContext : IBindingsContext
    {
        public nint GetProcAddress(string procName)
        {
            return Egl.GetProcAddress(procName);
        }
    }
}
