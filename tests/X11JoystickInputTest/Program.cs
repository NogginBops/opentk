using System;
using OpenTK.Core.Utility;
using OpenTK.Graphics;
using OpenTK.Platform;

namespace X11JoystickInputTest;
class Program
{
    
    static WindowHandle _window;
    static OpenGLContextHandle _glContext;
    static bool _isRunning = true;
    public static void Main(string[] args)
    {

        ToolkitOptions options = new ToolkitOptions()
        {
            ApplicationName = "X11 Joystick Input Test",
            Logger = new ConsoleLogger()
        };

        Toolkit.Init(options);

        OpenGLGraphicsApiHints contextSettings = new OpenGLGraphicsApiHints()
        {
            Version = new Version(4, 6),
            Profile = OpenGLProfile.Core,
            DepthBits = ContextDepthBits.Depth24,
            StencilBits = ContextStencilBits.Stencil8
        };

        _window = Toolkit.Window.Create(contextSettings);
        _glContext = Toolkit.OpenGL.CreateFromWindow(_window);

        Toolkit.OpenGL.SetCurrentContext(_glContext);
        GLLoader.LoadBindings(Toolkit.OpenGL.GetBindingsContext(_glContext));

        Toolkit.Window.SetTitle(_window, "Hello Joystick");
        Toolkit.Window.SetSize(_window, (900, 800));
        Toolkit.Window.SetMode(_window, WindowMode.Normal);

        EventQueue.EventRaised += OnEventRaised;

        // Joystick init
        Toolkit.Joystick.Initialize(options);

        if (Toolkit.Joystick.IsConnected(0))
        {

            JoystickHandle handle = Toolkit.Joystick.Open(0);
            Console.WriteLine($"The joystick {Toolkit.Joystick.GetName(handle)} has been connected.");

        } else
        {

            Console.WriteLine("No joystick connected at index 0");

        }

        while (_isRunning)
        {

            Toolkit.Window.ProcessEvents(false);

            // Drawing
            // Console.WriteLine(Toolkit.Joystick.IsConnected(0));

            Toolkit.OpenGL.SwapBuffers(_glContext);

        }

        Toolkit.Window.Destroy(_window);

    }

    static void OnEventRaised(PalHandle? handle, PlatformEventType type, EventArgs args)
    {

        if (args is CloseEventArgs)
        {

            _isRunning = false;

        }

    }

}

