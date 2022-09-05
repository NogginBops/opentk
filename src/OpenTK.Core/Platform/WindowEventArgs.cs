using System;

namespace OpenTK.Core.Platform
{
    /// <summary>
    /// Base class for OpenTK window event arguments.
    /// </summary>
    public class WindowEventArgs : EventArgs
    {
    }

    public class KeyDownEventArgs : WindowEventArgs
    {
        // FIXME: These properties are for testing with the Win32 backend.

        public ulong VirtualKey { get; private set; }

        public bool WasDown { get; private set; }

        public bool Extended { get; private set; }

        public KeyDownEventArgs(ulong virtualKey, bool wasDown, bool extended)
        {
            VirtualKey = virtualKey;
            WasDown = wasDown;
            Extended = extended;
        }
    }

    public class TextInputEventArgs : WindowEventArgs
    {
        public string Text { get; private set; }

        public TextInputEventArgs(string text)
        {
            Text = text;
        }
    }

    public class MouseEnterEventArgs : WindowEventArgs
    {
        public bool Entered { get; set; }

        public MouseEnterEventArgs(bool entered)
        {
            Entered = entered;
        }
    }

    public class MouseMoveEventArgs : WindowEventArgs
    {
        public int DeltaX { get; private set; }

        public int DeltaY { get; private set; }

        public MouseMoveEventArgs(int deltaX, int deltaY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
        }
    }

    public class MouseButtonDownEventArgs : WindowEventArgs
    {
        public MouseButton Button { get; private set; }

        public MouseButtonDownEventArgs(MouseButton button)
        {
            Button = button;
        }
    }

    public class MouseButtonUpEventArgs : WindowEventArgs
    {
        public MouseButton Button { get; private set; }

        public MouseButtonUpEventArgs(MouseButton button)
        {
            Button = button;
        }
    }

    public class CloseEventArgs : WindowEventArgs
    {
        public WindowHandle Window { get; private set; }

        public CloseEventArgs(WindowHandle window)
        {
            Window = window;
        }
    }

    public class JoystickConnectedEventArgs : WindowEventArgs
    {
        public bool Connected { get; private set; }

        public JoystickHandle Joystick { get; private set; }

        public JoystickConnectedEventArgs(bool connected, JoystickHandle joystick)
        {
            Connected = connected;
            Joystick = joystick;
        }
    }
}
