using System;
using OpenTK.Platform;
using System.Diagnostics;
using OpenTK.Core.Utility;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using static OpenTK.Platform.Native.X11.LibX11;

namespace OpenTK.Platform.Native.X11
{
    /// <summary>
    /// Static X11 values.
    /// </summary>
    internal static class X11
    {
        // FIXME: Maybe merge this with LibX11?
        
        public static XDisplayPtr Display { get; set; }

        public static int DefaultScreen { get; set; }

        public static XWindow DefaultRootWindow { get; set; }

        public static XAtomDictionary Atoms { get; set; }

        public static HashSet<string> Extensions { get; set; }

        private static XTime _lastTime;

        /// <summary>The latest time reported by the X11 server.</summary>
        public static XTime LastTime => _lastTime;

        public static unsafe void SetLastTime(XWindow window, XTime time) {
            // FIXME: Support _NET_WM_USER_TIME_WINDOW..
            _lastTime = time;
            //Debug.WriteLine($"Updating latest time: {time.Value}");
            //XChangeProperty(Display, window, Atoms[KnownAtoms._NET_WM_USER_TIME], Atoms[KnownAtoms.CARDINAL], 32, XPropertyMode.Replace, (IntPtr)(&time), 1);
        }


        public static int XRandREventBase { get; set; }

        public static int XRandRErrorBase { get; set; }

        public static bool XFixesAvailable { get; set; }

        public static int XFixesEventBase { get; set; }

        public static int XFixesErrorBase { get; set; }

        public static bool XI2Available { get; set; }

        public static int XI2EventBase { get; set; }

        public static int XI2ErrorBase { get; set; }


        /// <summary>
        /// remove/unset property
        /// </summary>
        public const long _NET_WM_STATE_REMOVE = 0;

        /// <summary>
        /// add/set property
        /// </summary>
        public const long _NET_WM_STATE_ADD = 1;

        /// <summary>
        /// toggle property
        /// </summary>
        public const long _NET_WM_STATE_TOGGLE = 2;

        internal static unsafe bool Poll(Libc.pollfd* fds, int count, int timeout)
        {
            while (true)
            {
                int result = Libc.poll(fds, (uint)count, timeout);
                int errno = Marshal.GetLastSystemError();

                const int EINTR = 4;
                const int EAGAIN = 11;

                if (result > 0)
                {
                    return true;
                }
                else if (result < 0 && errno != EINTR && errno != EAGAIN)
                {
                    return false;
                }
                else // (result == 0)
                {
                    return false;
                }
            }
        }

        // FIXME: Better name!
        internal static unsafe bool FileDescriptorHasReadData(int fd)
        {
            // FIXME: Add POLLPRI?
            Libc.pollfd pollfd = new Libc.pollfd(){
                fd = fd,
                events = Libc.poll_event.POLLIN,
            };

            return Poll(&pollfd, 1, 0);
        }

        internal static unsafe bool WaitForXEvents()
        {
            Libc.pollfd fd = new Libc.pollfd(){
                fd = LibX11.XConnectionNumber(X11.Display),
                events = Libc.poll_event.POLLIN,
            };

            while (LibX11.XPending(X11.Display) == 0)
            {
                // poll with no timeout.
                if (Poll(&fd, 1, -1) == false)
                    return false;
            }

            return true;
        }

        internal static unsafe bool IsSelectionPropertyNewValueNotify(XDisplayPtr display, ref XEvent @event, IntPtr pointer)
        {
            XEvent* notification = (XEvent*)pointer;
            return @event.Type == XEventType.PropertyNotify &&
                @event.Property.state == PropertyState.PropertyNewValue &&
                @event.Property.window == notification->Selection.requestor &&
                @event.Property.atom == notification->Selection.property;
        }
    }
}
