using System;
using System.Runtime.InteropServices;

namespace OpenTK.Platform.Native.X11
{
    public static class Libc
    {
        [Flags]
        internal enum poll_event : short
        {
            /* These are specified by iBCS2 */
            POLLIN = 0x0001,
            POLLPRI = 0x0002,
            POLLOUT = 0x0004,
            POLLERR = 0x0008,
            POLLHUP = 0x0010,
            POLLNVAL = 0x0020,
            /* The rest seem to be more-or-less nonstandard. Check them! */
            POLLRDNORM = 0x0040,
            POLLRDBAND = 0x0080,
            POLLWRNORM = 0x0100,
            POLLWRBAND = 0x0200,
            POLLMSG = 0x0400,
            POLLREMOVE = 0x1000,
            POLLRDHUP = 0x2000,
        }

        internal struct pollfd
        {
            public int fd;
            public poll_event events;
            public poll_event revents;
        }

        [DllImport("libc", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        internal static unsafe extern int poll(pollfd* fds, uint nfds, int timeout);

        internal enum LC : int
        {
            LC_CTYPE = 0,
            LC_NUMERIC = 1,
            LC_TIME = 2,
            LC_COLLATE = 3,
            LC_MONETARY = 4,
            LC_MESSAGES = 5,
            LC_ALL = 6,
            LC_PAPER = 7,
            LC_NAME = 8,
            LC_ADDRESS = 9,
            LC_TELEPHONE = 10,
            LC_MEASUREMENT = 11,
            LC_IDENTIFICATION = 12,
        }

        internal static unsafe string? setlocale(LC category, string? locale)
        {
            byte* localePtr = (byte*)Marshal.StringToCoTaskMemUTF8(locale);
            byte* retPtr = setlocale(category, localePtr);
            Marshal.ZeroFreeCoTaskMemUTF8((IntPtr)localePtr);
            return Marshal.PtrToStringUTF8((nint)retPtr);

            [DllImport("libc", EntryPoint = "setlocale")]
            static unsafe extern byte* setlocale(LC category, /* const */ byte* locale);
        }


        internal static unsafe string? strerror(int errnum)
        {
            byte* res = strerror(errnum);
            string? str = Marshal.PtrToStringUTF8((IntPtr)res);
            return str;

            [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
            static unsafe extern byte* strerror(int errnum);
        }

        // FIXME: These values could potentially be different on different platforms...
        // - Noggin_bops 2024-10-30
        const int _IOC_NRBITS = 8;
        const int _IOC_TYPEBITS = 8;
        const int _IOC_SIZEBITS = 14;
        const int _IOC_DIRBITS = 2;
        const int _IOC_NRMASK = (1 << _IOC_NRBITS) - 1;
        const int _IOC_TYPEMASK = (1 << _IOC_TYPEBITS) - 1;
        const int _IOC_SIZEMASK = (1 << _IOC_SIZEBITS) - 1;
        const int _IOC_DIRMASK = (1 << _IOC_DIRBITS) - 1;
        const int _IOC_NRSHIFT = 0;
        const int _IOC_TYPESHIFT = _IOC_NRSHIFT + _IOC_NRBITS;
        const int _IOC_SIZESHIFT = _IOC_TYPESHIFT + _IOC_TYPEBITS;
        const int _IOC_DIRSHIFT = _IOC_SIZESHIFT + _IOC_SIZEBITS;

        const int _IOC_NONE = 0;
        const int _IOC_WRITE = 1;
        const int _IOC_READ = 2;

        internal static ulong _IOC(int dir, int type, int nr, int size)
        {
            return (((ulong)dir) << _IOC_DIRSHIFT) | (((ulong)type) << _IOC_TYPESHIFT) | (((ulong)nr) << _IOC_NRSHIFT) | (((ulong)size) << _IOC_SIZESHIFT);
        }

        internal static ulong _IOR(int type, int nr, int size) => _IOC(_IOC_READ, type, nr, size);
        internal static ulong _IOW(int type, int nr, int size) => _IOC(_IOC_WRITE, type, nr, size);

        internal enum FFType : ushort
        {
            Constant = 0x52,
            Periodic = 0x51,
            Ramp = 0x57,
            Spring = 0x53,
            Friction = 0x54,
            Damper = 0x55,
            Rumble = 0x50,
            Inertia = 0x56,
            Custom = 0x5d,
        }

        /**
        * struct ff_effect - defines force feedback effect
        * @type: type of the effect (FF_CONSTANT, FF_PERIODIC, FF_RAMP, FF_SPRING,
        *        FF_FRICTION, FF_DAMPER, FF_RUMBLE, FF_INERTIA, or FF_CUSTOM)
        * @id: an unique id assigned to an effect
        * @direction: direction of the effect
        * @trigger: trigger conditions (struct ff_trigger)
        * @replay: scheduling of the effect (struct ff_replay)
        * @u: effect-specific structure (one of ff_constant_effect, ff_ramp_effect,
        *        ff_periodic_effect, ff_condition_effect, ff_rumble_effect) further
        *        defining effect parameters
        *
        * This structure is sent through ioctl from the application to the driver.
        * To create a new effect application should set its @id to -1; the kernel
        * will return assigned @id which can later be used to update or delete
        * this effect.
        *
        * Direction of the effect is encoded as follows:
        *        0 deg -> 0x0000 (down)
        *        90 deg -> 0x4000 (left)
        *        180 deg -> 0x8000 (up)
        *        270 deg -> 0xC000 (right)
        */
        [StructLayout(LayoutKind.Explicit)]
        internal struct ff_effect
        {
            [FieldOffset(0)] public FFType type;
            [FieldOffset(2)] public short id;
            [FieldOffset(4)] public ushort direction;
            [FieldOffset(6)] public ff_trigger trigger;
            [FieldOffset(10)] public ff_replay replay;

            [FieldOffset(16)] public ff_constant_effect constant;
            [FieldOffset(16)] public ff_ramp_effect ramp;
            [FieldOffset(16)] public ff_periodic_effect periodic;
            [FieldOffset(16)] public ff_condition_effect condition0; /* One for each axis */
            [FieldOffset(28)] public ff_condition_effect condition1; /* One for each axis */
            [FieldOffset(16)] public ff_rumble_effect rumble;
        }
        
        
        /**
        * struct ff_replay - defines scheduling of the force-feedback effect
        * @length: duration of the effect
        * @delay: delay before effect should start playing
        */
        internal struct ff_replay {
            public ushort length;
            public ushort delay;
        }
        
        /**
        * struct ff_trigger - defines what triggers the force-feedback effect
        * @button: number of the button triggering the effect
        * @interval: controls how soon the effect can be re-triggered
        */
        internal struct ff_trigger {
            public ushort button;
            public ushort interval;
        }
        
        /**
        * struct ff_envelope - generic force-feedback effect envelope
        * @attack_length: duration of the attack (ms)
        * @attack_level: level at the beginning of the attack
        * @fade_length: duration of fade (ms)
        * @fade_level: level at the end of fade
        *
        * The @attack_level and @fade_level are absolute values; when applying
        * envelope force-feedback core will convert to positive/negative
        * value based on polarity of the default level of the effect.
        * Valid range for the attack and fade levels is 0x0000 - 0x7fff
        */
        internal struct ff_envelope {
            public ushort attack_length;
            public ushort attack_level;
            public ushort fade_length;
            public ushort fade_level;
        }

        
        /**
        * struct ff_constant_effect - defines parameters of a constant force-feedback effect
        * @level: strength of the effect; may be negative
        * @envelope: envelope data
        */
        internal struct ff_constant_effect {
            public short level;
            public ff_envelope envelope;
        }

        /**
        * struct ff_ramp_effect - defines parameters of a ramp force-feedback effect
        * @start_level: beginning strength of the effect; may be negative
        * @end_level: final strength of the effect; may be negative
        * @envelope: envelope data
        */
        internal struct ff_ramp_effect {
            public short start_level;
            public short end_level;
            public ff_envelope envelope;
        }

        /**
        * struct ff_condition_effect - defines a spring or friction force-feedback effect
        * @right_saturation: maximum level when joystick moved all way to the right
        * @left_saturation: same for the left side
        * @right_coeff: controls how fast the force grows when the joystick moves
        *        to the right
        * @left_coeff: same for the left side
        * @deadband: size of the dead zone, where no force is produced
        * @center: position of the dead zone
        */
        internal struct ff_condition_effect {
            public ushort right_saturation;
            public ushort left_saturation;

            public short right_coeff;
            public short left_coeff;

            public ushort deadband;
            public short center;
        }

        /**
        * struct ff_periodic_effect - defines parameters of a periodic force-feedback effect
        * @waveform: kind of the effect (wave)
        * @period: period of the wave (ms)
        * @magnitude: peak value
        * @offset: mean value of the wave (roughly)
        * @phase: 'horizontal' shift
        * @envelope: envelope data
        * @custom_len: number of samples (FF_CUSTOM only)
        * @custom_data: buffer of samples (FF_CUSTOM only)
        *
        * Known waveforms - FF_SQUARE, FF_TRIANGLE, FF_SINE, FF_SAW_UP,
        * FF_SAW_DOWN, FF_CUSTOM. The exact syntax FF_CUSTOM is undefined
        * for the time being as no driver supports it yet.
        *
        * Note: the data pointed by custom_data is copied by the driver.
        * You can therefore dispose of the memory after the upload/update.
        */
        internal unsafe struct ff_periodic_effect {
            public ushort waveform;
            public ushort period;
            public short magnitude;
            public short offset;
            public ushort phase;

            public ff_envelope envelope;

            public uint custom_len;
            public short* custom_data;
        }

        /**
        * struct ff_rumble_effect - defines parameters of a periodic force-feedback effect
        * @strong_magnitude: magnitude of the heavy motor
        * @weak_magnitude: magnitude of the light one
        *
        * Some rumble pads have two motors of different weight. Strong_magnitude
        * represents the magnitude of the vibration generated by the heavy one.
        */
        internal struct ff_rumble_effect {
            public ushort strong_magnitude;
            public ushort weak_magnitude;
        }


        /* get event bits */
        internal static ulong EVIOCGBIT(int ev, int len) => _IOC(_IOC_READ, 'E', 0x20 + ev, len);
        /* send a force effect to a force feedback device */
        internal static readonly unsafe ulong EVIOCSFF = _IOC(_IOC_WRITE, 'E', 0x80, sizeof(ff_effect));
        /* Erase a force effect */
        internal static readonly ulong EVIOCRMFF = _IOW('E', 0x81, sizeof(int));
        /* Report number of effects playable at the same time */
        internal static readonly ulong EVIOCGEFFECTS = _IOR('E', 0x84, sizeof(int));



        [DllImport("libc", CallingConvention = CallingConvention.Cdecl)]
        internal static unsafe extern int ioctl(int fd, ulong op, byte* data);

        internal static unsafe int ioctl<T>(int fd, ulong op, Span<T> data) where T : unmanaged
        {
            fixed(T* dataPtr = data)
            {
                return ioctl(fd, op, (byte*)dataPtr);
            }
        }
    }
}