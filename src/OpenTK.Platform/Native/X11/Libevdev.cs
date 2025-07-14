using System;
using System.Runtime.InteropServices;

namespace OpenTK.Platform.Native.X11
{
    internal struct LibEvdevPtr {
        public IntPtr Value;
    }

    internal enum libevdev_grab_mode
    {
        // FIXME: Values
        Grab = 3,
        Ungrab = 4,
    }

    internal enum libevdev_log_priority
    {
        Error = 10,
        Info = 20,
        Debug = 30,
    }

    internal struct timeval
    {
        public ulong tv_sec;
        public ulong tv_usec;
    }

    // FIXME: This struct has many static if-defs
    // that could potentially change the layout of the type...
    // - Noggin_bops 2025-07-13
    internal struct input_event
    {
        public timeval time;
        public ushort type;
        public ushort code;
        public int value;
    }

    [Flags]
    internal enum libevdev_read_flag : uint
    {
        // FIXME:
        Sync = 1,
        Normal = 2,
        ForceSync = 4,
        Blocking = 8,
    }

    internal enum libevdev_read_status : int
    {
        Success = 0,
        Sync = 1,
    }

    internal struct input_absinfo
    {
        public int value;
        public int minimum;
        public int maximum;
        public int fuzz;
        public int flat;
        public int resolution;
    }

    internal enum BusType : int
    {
        PCI = 0x01,
        ISAPNP = 0x02,
        USB = 0x03,
        HIL = 0x04,
        BLUETOOTH = 0x05,
        VIRTUAL = 0x06,
        ISA = 0x10,
        I8042 = 0x11,
        XTKBD = 0x12,
        RS232 = 0x13,
        GAMEPORT = 0x14,
        PARPORT = 0x15,
        AMIGA = 0x16,
        ADB = 0x17,
        I2C = 0x18,
        HOST = 0x19,
        GSC = 0x1A,
        ATARI = 0x1B,
        SPI = 0x1C,
    }

    internal enum AbsoluteAxis : int
    {
        X = 0x00,
        Y = 0x01,
        Z = 0x02,
        RX = 0x03,
        RY = 0x04,
        RZ = 0x05,
        THROTTLE = 0x06,
        RUDDER = 0x07,
        WHEEL = 0x08,
        GAS = 0x09,
        BRAKE = 0x0a,
        HAT0X = 0x10,
        HAT0Y = 0x11,
        HAT1X = 0x12,
        HAT1Y = 0x13,
        HAT2X = 0x14,
        HAT2Y = 0x15,
        HAT3X = 0x16,
        HAT3Y = 0x17,
        PRESSURE = 0x18,
        DISTANCE = 0x19,
        TILT_X = 0x1a,
        TILT_Y = 0x1b,
        TOOL_WIDTH = 0x1c,
        VOLUME = 0x20,
        MISC = 0x28,
        /* MT slot being modified */
        MT_SLOT = 0x2f,
        /* Major axis of touching ellipse */
        MT_TOUCH_MAJOR = 0x30,
        /* Minor axis (omit if circular) */
        MT_TOUCH_MINOR = 0x31,
        /* Major axis of approaching ellipse */
        MT_WIDTH_MAJOR = 0x32,
        /* Minor axis (omit if circular) */
        MT_WIDTH_MINOR = 0x33,
        /* Ellipse orientation */
        MT_ORIENTATION = 0x34,
        /* Center X touch position */
        MT_POSITION_X = 0x35,
        /* Center Y touch position */
        MT_POSITION_Y = 0x36,
        /* Type of touching device */
        MT_TOOL_TYPE = 0x37,
        /* Group a set of packets as a blob */
        MT_BLOB_ID = 0x38,
        /* Unique ID of initiated contact */
        MT_TRACKING_ID = 0x39,
        /* Pressure on contact area */
        MT_PRESSURE = 0x3a,
        /* Contact hover distance */
        MT_DISTANCE = 0x3b,
        /* Center X tool position */
        MT_TOOL_X = 0x3c,
        /* Center Y tool position */
        MT_TOOL_Y = 0x3d,
    }

    internal enum EventType : uint
    {
        /* EV_SYN */
        Synchronization = 0x00,
        /* EV_KEY */
        Key = 0x01,
        /* EV_REL */
        Relative = 0x02,
        /* EV_ABS */
        Absolute = 0x03,
        /* EV_MSC */
        Misc = 0x04,
        /* EV_SW */
        Switch = 0x05,
        /* EV_LED */
        LED = 0x11,
        /* EV_SND */
        Sound = 0x12,
        /* EV_REP */
        Repeat = 0x14,
        /* EV_FF */
        ForceFeedback = 0x15,
        /* EV_PWR */
        Power = 0x16,
        /* EV_FF_STATUS */
        ForceFeedbackStatus = 0x17,
    }

    internal enum Button : uint
    {
        BTN_MISC = 0x100,
        BTN_0 = 0x100,
        BTN_1 = 0x101,
        BTN_2 = 0x102,
        BTN_3 = 0x103,
        BTN_4 = 0x104,
        BTN_5 = 0x105,
        BTN_6 = 0x106,
        BTN_7 = 0x107,
        BTN_8 = 0x108,
        BTN_9 = 0x109,
        BTN_MOUSE = 0x110,
        BTN_LEFT = 0x110,
        BTN_RIGHT = 0x111,
        BTN_MIDDLE = 0x112,
        BTN_SIDE = 0x113,
        BTN_EXTRA = 0x114,
        BTN_FORWARD = 0x115,
        BTN_BACK = 0x116,
        BTN_TASK = 0x117,
        BTN_JOYSTICK = 0x120,
        BTN_TRIGGER = 0x120,
        BTN_THUMB = 0x121,
        BTN_THUMB2 = 0x122,
        BTN_TOP = 0x123,
        BTN_TOP2 = 0x124,
        BTN_PINKIE = 0x125,
        BTN_BASE = 0x126,
        BTN_BASE2 = 0x127,
        BTN_BASE3 = 0x128,
        BTN_BASE4 = 0x129,
        BTN_BASE5 = 0x12a,
        BTN_BASE6 = 0x12b,
        BTN_DEAD = 0x12f,
        BTN_GAMEPAD = 0x130,
        BTN_SOUTH = 0x130,
        BTN_A = BTN_SOUTH,
        BTN_EAST = 0x131,
        BTN_B = BTN_EAST,
        BTN_C = 0x132,
        BTN_NORTH = 0x133,
        BTN_X = BTN_NORTH,
        BTN_WEST = 0x134,
        BTN_Y = BTN_WEST,
        BTN_Z = 0x135,
        BTN_TL = 0x136,
        BTN_TR = 0x137,
        BTN_TL2 = 0x138,
        BTN_TR2 = 0x139,
        BTN_SELECT = 0x13a,
        BTN_START = 0x13b,
        BTN_MODE = 0x13c,
        BTN_THUMBL = 0x13d,
        BTN_THUMBR = 0x13e,
        BTN_DIGI = 0x140,
        BTN_TOOL_PEN = 0x140,
        BTN_TOOL_RUBBER = 0x141,
        BTN_TOOL_BRUSH = 0x142,
        BTN_TOOL_PENCIL = 0x143,
        BTN_TOOL_AIRBRUSH = 0x144,
        BTN_TOOL_FINGER = 0x145,
        BTN_TOOL_MOUSE = 0x146,
        BTN_TOOL_LENS = 0x147,
        /* Five fingers on trackpad */
        BTN_TOOL_QUINTTAP = 0x148,
        BTN_STYLUS3 = 0x149,
        BTN_TOUCH = 0x14a,
        BTN_STYLUS = 0x14b,
        BTN_STYLUS2 = 0x14c,
        BTN_TOOL_DOUBLETAP = 0x14d,
        BTN_TOOL_TRIPLETAP = 0x14e,
        /* Four fingers on trackpad */
        BTN_TOOL_QUADTAP = 0x14f,
        BTN_WHEEL = 0x150,
        BTN_GEAR_DOWN = 0x150,
        BTN_GEAR_UP = 0x151,
    }

    internal enum InputProp : uint
    {
        /* needs a pointer */
        Pointer = 0x00,
        /* direct input devices */
        Direct = 0x01,
        /* has button(s) under pad */
        ButtonPad = 0x02,
        /* touch rectangle only */
        SemiMT = 0x03,
    }

    internal static class Libevdev
    {
        private const string EVDEV = "evdev";

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void libevdev_log_func(libevdev_log_priority priority, IntPtr data, IntPtr file, int line, IntPtr func, IntPtr format /*, va_list*/);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void libevdev_device_log_func(LibEvdevPtr dev, libevdev_log_priority priority, IntPtr data, IntPtr file, int line, IntPtr func, IntPtr format /*, va_list*/);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern LibEvdevPtr libevdev_new();


        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_new_from_fd(int fd, out LibEvdevPtr dev);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void libevdev_free(LibEvdevPtr dev);


        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_grab(LibEvdevPtr dev, libevdev_grab_mode grab);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_set_fd(LibEvdevPtr dev, int fd);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_change_fd(LibEvdevPtr dev, int fd);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_fd(LibEvdevPtr dev);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void libevdev_set_log_function(libevdev_log_func logfunc, IntPtr data);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void libevdev_set_log_priority(libevdev_log_priority priority);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern libevdev_log_priority libevdev_get_log_priority();

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern void libevdev_set_device_log_function(LibEvdevPtr dev, libevdev_device_log_func logfunc, libevdev_log_priority priority, IntPtr data);

        internal static string libevdev_get_name(LibEvdevPtr dev)
        {
            IntPtr ptr = libevdev_get_name(dev);
            string str = Marshal.PtrToStringUTF8(ptr)!;
            return str;

            [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
            static extern IntPtr libevdev_get_name(LibEvdevPtr dev);
        }

        internal static string? libevdev_get_phys(LibEvdevPtr dev)
        {
            IntPtr ptr = libevdev_get_phys(dev);
            string? str = Marshal.PtrToStringUTF8(ptr);
            return str;

            [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
            static extern IntPtr libevdev_get_phys(LibEvdevPtr dev);
        }

        internal static string? libevdev_get_uniq(LibEvdevPtr dev)
        {
            IntPtr ptr = libevdev_get_uniq(dev);
            string? str = Marshal.PtrToStringUTF8(ptr);
            return str;

            [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
            static extern IntPtr libevdev_get_uniq(LibEvdevPtr dev);
        }

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_id_product(LibEvdevPtr dev);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_id_vendor(LibEvdevPtr dev);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern BusType libevdev_get_id_bustype(LibEvdevPtr dev);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_id_version(LibEvdevPtr dev);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_driver_version(LibEvdevPtr dev);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_has_property(LibEvdevPtr dev, InputProp prop);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_has_event_type(LibEvdevPtr dev, EventType type);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_has_event_code(LibEvdevPtr dev, EventType type, uint code);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_abs_minimum(LibEvdevPtr dev, AbsoluteAxis code);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_abs_maximum(LibEvdevPtr dev, AbsoluteAxis code);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_abs_fuzz(LibEvdevPtr dev, AbsoluteAxis code);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_abs_flat(LibEvdevPtr dev, AbsoluteAxis code);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_abs_resolution(LibEvdevPtr dev, AbsoluteAxis code);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern input_absinfo* libevdev_get_abs_info(LibEvdevPtr dev, AbsoluteAxis code);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_event_value(LibEvdevPtr dev, EventType type, uint code);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_fetch_event_value(LibEvdevPtr dev, EventType type, uint code, out int value);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_get_repeat(LibEvdevPtr dev, out int delay, out int period);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern libevdev_read_status libevdev_next_event(LibEvdevPtr dev, libevdev_read_flag flags, ref input_event ev);

        [DllImport(EVDEV, CallingConvention = CallingConvention.Cdecl)]
        internal static extern int libevdev_has_event_pending(LibEvdevPtr dev);
    }
    
}

