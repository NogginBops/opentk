﻿using OpenTK.Core.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace OpenTK.Platform.Native.Windows
{
    internal enum WindowState
    {
        Restored,
        Maximized,
        Minimized,
    }

#pragma warning disable SA1649 // File name should match first type name
    internal class HWND : WindowHandle
    {
        public IntPtr HWnd { get; private set; }

        /// <summary> The current cursor for this window. </summary>
        public HCursor? HCursor { get; set; }

        public HIcon? HIcon { get; set; }

        public bool TrackingMouse { get; set; }

        // FIXME: Initialize this property properly!
        public WindowState WindowState { get; set; }

        // FIXME: This is kind of a hack so that we can get access to the window component in the WndProc...
        public WindowComponent WindowComponent { get; private set; }

        public HWND(IntPtr hWnd, WindowComponent windowComponent, GraphicsApiHints hints)
        {
            HWnd = hWnd;
            WindowComponent = windowComponent;
            GraphicsApiHints = hints;
        }
    }

    internal class HGLRC : OpenGLContextHandle
    {
        public IntPtr HGlrc { get; private set; }

        // FIXME: How do we want to handle this??
        public IntPtr HDC { get; private set; }

        public HGLRC? SharedContext { get; private set; }

        // FIXME: Is this needed?
        public OpenGLComponent OpenGLComponent { get; private set; }

        public HGLRC(IntPtr hGlrc, IntPtr hdc, HGLRC? sharedContext, OpenGLComponent openglComponent)
        {
            HGlrc = hGlrc;
            HDC = hdc;
            SharedContext = sharedContext;
            OpenGLComponent = openglComponent;
        }
    }

    internal class HCursor : CursorHandle
    {
        public IntPtr Cursor { get; set; }

        public int HotSpotX { get; set; }

        public int HotSpotY { get; set; }

        public CursorMode Mode { get; set; } = CursorMode.Uninitialized;

        public IntPtr ColorBitmap { get; set; }

        public IntPtr MaskBitmap { get; set; }

        internal enum CursorMode
        {
            Uninitialized,
            SystemCursor,
            Icon,
            FileIcon,
        }
    }

    internal class HIcon : IconHandle
    {
        public IconMode Mode { get; set; }

        public IntPtr Icon { get; set; }

        public IntPtr ColorBitmap { get; set; }

        public IntPtr MaskBitmap { get; set; }

        internal enum IconMode
        {
            Uninitialized,
            SystemIcon,
            Icon,
            FileIcon,
        }
    }

    internal class HMonitor : DisplayHandle
    {
        public IntPtr Monitor { get; set; }

        public string Name { get; set; }

        public string PublicName { get; set; }

        public bool IsPrimary { get; set; }

        public Win32.POINTL Position { get; set; }

        public DisplayResolution Resolution { get; set; }

        public Win32.RECT WorkArea { get; set; }

        public int RefreshRate { get; set; }

        public int DpiX { get; set; }

        public int DpiY { get; set; }
    }

    internal class HDevice : JoystickHandle
    {
        public IntPtr Device { get; set; }

        public string DeviceName { get; set; }

        // FIXME: Should we cache these?
        public string PublicName { get; set; }

        // Type of controller? layout?

        public HDevice(IntPtr device, string deviceName, string publicName)
        {
            Device = device;
            DeviceName = deviceName;
            PublicName = publicName;
        }
    }

    internal class Win32EventQueue : IEventQueue<PlatformEventType, WindowEventArgs>
    {
        public event QueueEventHandler<PlatformEventType, WindowEventArgs> EventRaised;

        public void ProcessEvents()
        {
            throw new NotImplementedException();
        }

        public void IgnoreEvents()
        {
            throw new NotImplementedException();
        }

        public void DefaultEventHandler(object sender, PlatformEventType type, WindowEventArgs arguments)
        {
            throw new NotImplementedException();
        }
    }

#pragma warning restore SA1649 // File name should match first type name
}
