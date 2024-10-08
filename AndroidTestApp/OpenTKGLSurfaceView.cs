using Android.Content;
using Android.Opengl;
using Javax.Microedition.Khronos.Egl;
using OpenTK.Graphics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.Egl;
using Android.Views;
using Android.Runtime;
using Android.Graphics;
using OpenTK.Graphics.OpenGLES2;
using OpenTK.Mathematics;
using System.Diagnostics;
using OpenTK.Platform;

namespace AndroidTestApp
{
    internal class OpenTKGLSurfaceView : SurfaceView, ISurfaceHolderCallback, View.IOnTouchListener
    {
        internal string[] Extensions;
        
        internal IntPtr eglDisplay;
        internal Version eglVersion;

        internal IntPtr aNativeWindow;

        internal nint eglConfig;
        internal nint eglSurface;
        internal nint eglContext;

        internal Thread glThread;
        internal bool shouldStop = false;
        internal ColorTriangle triangle;

        [DllImport("libandroid", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern IntPtr ANativeWindow_fromSurface(IntPtr jniEnv, IntPtr /* jobject */ surface);

        [DllImport("libandroid", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern void ANativeWindow_release(IntPtr /* ANativeWindow* */ window);

        [DllImport("libandroid", CallingConvention = CallingConvention.Cdecl)]
        static unsafe extern int ANativeWindow_setBuffersGeometry(IntPtr /* ANativeWindow* */ window, int width, int height, int format);

        public OpenTKGLSurfaceView(Context? context) : base(context)
        {
            Focusable = true;
            RequestFocus();
            SetOnTouchListener(this);

            const IntPtr EGL_NO_DISPLAY = 0;
            IntPtr extensionsPtr = Egl.QueryString(EGL_NO_DISPLAY, Egl.EXTENSIONS);
            string extensionsStr = Marshal.PtrToStringAnsi(extensionsPtr)!;
            Extensions = extensionsStr.Split(" ", StringSplitOptions.RemoveEmptyEntries);
            int[] attribs = new int[] { Egl.RENDERABLE_TYPE, Egl.OPENGL_ES3_BIT, Egl.NONE };
            const int EGL_PLATFORM_ANDROID_KHR = 0x3141;
            eglDisplay = Egl.GetPlatformDisplay(EGL_PLATFORM_ANDROID_KHR, Egl.DEFAULT_DISPLAY, attribs);
            
            bool success = Egl.Initialize(eglDisplay, out int major, out int minor);
            if (success == false)
            {
                OpenTK.Graphics.Egl.ErrorCode error = Egl.GetError();
                throw new Exception($"EGL couldn't initialize successfully. {error}");
            }
            
            eglVersion = new Version(major, minor);
            
            // FIXME
            Egl.BindAPI(RenderApi.ES);
            CheckEGLError("BindAPI");

            Egl.GetConfigs(eglDisplay, null, 0, out int numConfigs);
            nint[] configs = new nint[numConfigs];
            Egl.GetConfigs(eglDisplay, configs, numConfigs, out numConfigs);
            CheckEGLError("GetConfigs");
            for (int i = 0; i < numConfigs; i++)
            {
                IntPtr config = configs[i];
            
                Egl.GetConfigAttrib(eglDisplay, config, Egl.SURFACE_TYPE, out int configSupportedSurfaceTypes);
                if ((configSupportedSurfaceTypes & Egl.WINDOW_BIT) == 0)
                    continue;
            
                Egl.GetConfigAttrib(eglDisplay, config, Egl.RENDERABLE_TYPE, out int renderableType);
                if ((renderableType & Egl.OPENGL_ES_BIT) == 0 &&
                    (renderableType & Egl.OPENGL_ES2_BIT) == 0 &&
                    (renderableType & Egl.OPENGL_ES3_BIT) == 0)
                {
                    continue;
                }
            
                Egl.GetConfigAttrib(eglDisplay, config, Egl.RED_SIZE, out int configRedBits);
                Egl.GetConfigAttrib(eglDisplay, config, Egl.GREEN_SIZE, out int configGreenBits);
                Egl.GetConfigAttrib(eglDisplay, config, Egl.BLUE_SIZE, out int configBlueBits);
                Egl.GetConfigAttrib(eglDisplay, config, Egl.ALPHA_SIZE, out int configAlphaBits);
                Egl.GetConfigAttrib(eglDisplay, config, Egl.DEPTH_SIZE, out int configDepthBits);
                Egl.GetConfigAttrib(eglDisplay, config, Egl.STENCIL_SIZE, out int configStencilBits);
                Egl.GetConfigAttrib(eglDisplay, config, Egl.SAMPLES, out int configSamples);
            
                Console.WriteLine($"{i}: R:{configRedBits}, G:{configGreenBits}, B:{configBlueBits}, A: {configAlphaBits}, D: {configDepthBits}, S:{configStencilBits}, MSAA:{configSamples}");
            }
            
            eglConfig = configs[0];

            this.Holder!.AddCallback(this);

            GLLoader.LoadBindings(new AndroidBindingsContext());

            EventQueue.EventRaised += EventQueue_EventRaised;

            glThread = new Thread(new ThreadStart(() => {
                try
                {
                    Android.OS.Process.SetThreadPriority(Android.OS.ThreadPriority.Display);
                }
                catch
                {
                    Console.WriteLine("Failed to set thread priority.");
                }
                

                Egl.MakeCurrent(eglDisplay, eglSurface, eglSurface, eglContext);
                CheckEGLError("MakeCurrent");

                triangle = new ColorTriangle();
                triangle.Initialize();

                while (shouldStop == false)
                {
                    GL.ClearColor(Color4.Coral);
                    GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

                    triangle.Render();

                    bool success = Egl.SwapBuffers(eglDisplay, eglSurface);
                    if (success == false)
                    {
                        CheckEGLError("SwapBuffers");
                    }
                }

                triangle.Deinitialize();
            }));
        }

        private void EventQueue_EventRaised(PalHandle? handle, PlatformEventType type, EventArgs args)
        {
            if (args is TouchEvent touch)
            {
                triangle.HandleEvent(touch);
            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            lock (MainActivity.ActivityLock)
            {
                Surface? surf = holder.Surface;
                if (surf == null)
                {
                    return;
                }

                aNativeWindow = ANativeWindow_fromSurface(JNIEnv.Handle, JNIEnv.ToJniHandle(surf));
                if (aNativeWindow == IntPtr.Zero)
                {
                    return;
                }

                Egl.GetConfigAttrib(eglDisplay, eglConfig, Egl.NATIVE_VISUAL_ID, out int nativeFormat);
                ANativeWindow_setBuffersGeometry(aNativeWindow, 0, 0, nativeFormat);

                List<int> surface_attribs = new List<int>();
                surface_attribs.Add(Egl.RENDER_BUFFER);
                surface_attribs.Add(Egl.BACK_BUFFER);
                surface_attribs.Add(Egl.NONE);
                eglSurface = Egl.CreateWindowSurface(eglDisplay, eglConfig, aNativeWindow, surface_attribs.ToArray());
                CheckEGLError("CreatePlatformWindowSurface");
                if (eglSurface == Egl.NO_SURFACE)
                {
                    return;
                }

                List<int> context_attribs = new List<int>() { Egl.CONTEXT_MAJOR_VERSION, 3, Egl.CONTEXT_MINOR_VERSION, 2 };
                context_attribs.Add(Egl.NONE);

                eglContext = Egl.CreateContext(eglDisplay, eglConfig, IntPtr.Zero, context_attribs.ToArray());
                CheckEGLError("CreateContext");

                glThread.Start();
            }
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            lock(MainActivity.ActivityLock)
            {

            }
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            lock (MainActivity.ActivityLock)
            {
                // FIXME: Make the update thread stop!
                shouldStop = true;

                if (eglSurface != Egl.NO_SURFACE)
                {
                    Egl.DestroySurface(eglDisplay, eglSurface);
                    eglSurface = Egl.NO_SURFACE;
                }

                if (aNativeWindow != 0)
                {
                    ANativeWindow_release(aNativeWindow);
                    aNativeWindow = 0;
                }
            }
        }

        static void CheckEGLError(string desc)
        {
            OpenTK.Graphics.Egl.ErrorCode error = Egl.GetError();
            if (error != OpenTK.Graphics.Egl.ErrorCode.SUCCESS)
            {
                Console.WriteLine($"{desc}: {error}");
            }
        }

        public bool OnTouch(View? v, MotionEvent? e)
        {
            float x = e!.GetX(0);
            float y = e.GetY(0);

            float width = this.Holder?.SurfaceFrame?.Width() ?? 0;
            float height = this.Holder?.SurfaceFrame?.Height() ?? 0;

            EventQueue.Raise(null, 0, new TouchEvent((x, y), (width, height)));

            return true;
        }

        internal class TouchEvent : EventArgs
        {
            public Vector2 FirstTouch;

            public Vector2 ClientSize;

            public TouchEvent(Vector2 first, Vector2 clientSize) : base()
            {
                FirstTouch = first;
                ClientSize = clientSize;
            }
        }
    }
}
