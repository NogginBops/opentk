using OpenTK.Core.Native;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics.Vulkan;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;

namespace LocalTest
{
    class Window : GameWindow
    {
        static void Main(string[] args)
        {
            Vector2 a = (1E-45f, -5.9167414f);
            Vector2 b = (1E-45f, 13.882292f);
            Vector2 c = Vector2.Slerp(a, b, 0);
            Vector2 d = Vector2.Slerp(a, b, 1);

            var res = Vector3.Elerp((1e-45f, 1, 1), (1, 1, 4), 0.3f);
            
            GameWindowSettings gwSettings = new GameWindowSettings()
            {
                UpdateFrequency = 250,
            };

            NativeWindowSettings nwSettings = new NativeWindowSettings()
            {
                API = ContextAPI.OpenGL,
                APIVersion = new Version(4, 5),
                AutoLoadBindings = true,
                Flags = ContextFlags.ForwardCompatible,
                IsEventDriven = false,
                Profile = ContextProfile.Core,
                ClientSize = (800, 600),
                StartFocused = true,
                StartVisible = true,
                Title = "OpenTK Test",
                WindowBorder = WindowBorder.Resizable,
                WindowState = WindowState.Normal,
                SrgbCapable = true,
            };

            using (Window window = new Window(gwSettings, nwSettings))
            {
                window.Run();
            }
        }

        public Window(GameWindowSettings gwSettings, NativeWindowSettings nwSettings) : base(gwSettings, nwSettings)
        {
        }

        public const string FullscreenTriangleVertexSource = @"#version 450 core

out vec2 uv;
 
void main()
{
    float x = -1.0 + float((gl_VertexID & 1) << 2);
    float y = -1.0 + float((gl_VertexID & 2) << 1);
    uv.x = (x+1.0)*0.5;
    uv.y = (y+1.0)*0.5;
    gl_Position = vec4(x, y, 0.0, 1.0);
}
";

        int tex;
        int prog;
        protected unsafe override void OnLoad()
        {
            base.OnLoad();

            string ver = GLFW.GetVersionString();
            Console.WriteLine($"GLFW version: {ver}");

            Color4<Rgba>[] colors = new Color4<Rgba>[500 * 500];
            for (int y = 0; y < 500; y++)
            {
                for (int x = 0; x < 500; x++)
                {
                    float t = x / (float)(500 - 1);

                    Color4<Rgba> color;
                    if (y < 250)
                    {
                        Color3<Xyz> rainbow = RGBColorSpace.WavelengthToXYZ(MathHelper.Lerp(390, 830, t));
                        rainbow.X *= 0.8f;
                        rainbow.Y *= 0.8f;
                        rainbow.Z *= 0.8f;
                        color = RGBColorSpace.Clip(RGBColorSpace.sRGB.ToRgb(rainbow)).ToRgba(1);
                    }
                    else
                    {
                        Vector2 cctxy = RGBColorSpace.TemperatureToxy(MathHelper.Lerp(1667, 25000, t));
                        Vector3 cctXYZ = RGBColorSpace.xyToXYZ(cctxy);
                        color = RGBColorSpace.Clip((RGBColorSpace.sRGB.ToRgb(new Color3<Xyz>(cctXYZ * 0.8f)))).ToRgba(1);
                    }

                    colors[y * 500 + x] = color;
                }
            }

            tex = GL.CreateTexture(TextureTarget.Texture2d);
            GL.TextureStorage2D(tex, 1, SizedInternalFormat.Rgba32f, 500, 500);
            GL.TextureSubImage2D(tex, 0, 0, 0, 500, 500, PixelFormat.Rgba, PixelType.Float, colors);

            int vao = GL.CreateVertexArray();
            GL.BindVertexArray(vao);


            int vert = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vert, FullscreenTriangleVertexSource);
            GL.CompileShader(vert);
            GL.GetShaderInfoLog(vert, out string vertLog);
            Console.WriteLine(vertLog);

            int frag = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(frag,
                """
                #version 450 core

                in vec2 uv;

                out vec4 color;

                uniform sampler2D tex;

                void main()
                {
                    color = vec4(texture(tex, uv).rgb, 1);
                }
                """);
            GL.CompileShader(frag);
            GL.GetShaderInfoLog(frag, out string fragLog);
            Console.WriteLine(fragLog);

            prog = GL.CreateProgram();
            GL.AttachShader(prog, vert);
            GL.AttachShader(prog, frag);
            GL.LinkProgram(prog);
            GL.GetProgramInfoLog(prog, out string progLog);
            Console.WriteLine(progLog);

            GL.UseProgram(prog);

            GL.ProgramUniform1i(prog, GL.GetUniformLocation(prog, "tex"), 0);
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2d, tex);

            GL.Enable(EnableCap.FramebufferSrgb);
        }

        protected unsafe override void OnUnload()
        {
            base.OnUnload();
        }

        protected unsafe override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        const float CycleTime = 12.0f;
        float Time = 0;

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Time += (float)args.Time;
            if (Time > CycleTime) Time = 0;

            float x = float.Clamp(MouseState.X / FramebufferSize.X, 0, 1);

            Color4<Rgba> color = new Color4<Hsva>(Time / CycleTime, 1, 1, 1).ToRgba();

            //float t = Time / CycleTime;
            //t = t - float.Truncate(t);
            //Vector2 cctxy = RGBColorSpace.TemperatureToxy(MathHelper.Lerp(1667, 25000, x));
            //Vector3 cctXYZ = RGBColorSpace.xyToXYZ(cctxy);
            //color = RGBColorSpace.TosRGB(RGBColorSpace.sRGB.ToRgb(new Color3<Xyz>(cctXYZ))).ToRgba(1);
            //
            //Color3<Xyz> rainbow = RGBColorSpace.WavelengthToXYZ(MathHelper.Lerp(390, 830, x));
            //color = RGBColorSpace.TosRGB(RGBColorSpace.sRGB.ToRgb(rainbow)).ToRgba(1);
            //color = RGBColorSpace.Clip(RGBColorSpace.sRGB.ToRgb(rainbow)).ToRgba(1);
            
            GL.ClearColor(color);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.UseProgram(prog);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 3);

            SwapBuffers();
        }

        protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        {
            base.OnFramebufferResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
        }

        protected override void OnMove(WindowPositionEventArgs e)
        {
            base.OnMove(e);
        }
    }
}
