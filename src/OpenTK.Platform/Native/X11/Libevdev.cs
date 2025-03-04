

using System.Data;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;

namespace OpenTK.Platform.Native.X11
{

    internal unsafe static class Libevdev
    {

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

        struct input_id 
        {
            ushort bustype;
            ushort vendor;
            ushort product;
            ushort version;
        }

        // for axes
        struct input_absinfo
        {
            int value;
            int minimum;
            int maximum;
            int fuzz;
            int flat;
            int resolution;
        }

        struct input_keymap_entry
        {
            byte flags;
            byte len;
            ushort index;
            uint keycode;
            byte[] scancode = new byte[32];

            public input_keymap_entry(){}
        }

        struct input_mask
        {
            uint type;
            uint codes_size;
            ulong codes_ptr;
        }

        internal static ulong EVIOCGVERSION => _IOR('E', 0x01, sizeof(int)); // driver version
        internal static ulong EVIOCGID => _IOR('E', 0x02, Marshal.SizeOf<input_id>()); // device ID
        internal static ulong EVIOCGREP => _IOR('E', 0x03, sizeof(uint) * 2); // get repeat settings
        internal static ulong EVIOCSREP => _IOW('E', 0x03, sizeof(uint) * 2); // set repeat settings
        internal static ulong EVIOCGKEYCODE => _IOR('E', 0x04, sizeof(uint) * 2); // get keycode TODO: Do we really need this?
        internal static ulong EVIOGKEYCODE_V2 => _IOR('E', 0x04, Marshal.SizeOf<input_keymap_entry>()); // TODO: do we really need this?
        internal static ulong EVIOCSKEYCODE => _IOW('E', 0x04, sizeof(uint) * 2); // set keycode TODO: Do we really need this?
        internal static ulong EVIOCSKEYCODE_V2 => _IOW('E', 0x04, Marshal.SizeOf<input_keymap_entry>()); // TODO: do we really need this?
        internal static ulong EVIOCGNAME(int len) => _IOC(_IOC_READ, 'E', 0x06, len); // get device name
        internal static ulong EVIOCGPHYS(int len) => _IOC(_IOC_READ, 'E', 0x07, len); // get physical location
        internal static ulong EVIOCGUNIQ(int len) => _IOC(_IOC_READ, 'E', 0x08, len); // get uuid
        internal static ulong EVICGPROP(int len) => _IOC(_IOC_READ, 'E', 0x09, len); // get device properties

        internal static ulong EVIOCGMTSLOTS(int len) => _IOC(_IOC_READ, 'E', 0x0a, len);

        internal static ulong EVIOCGKEY(int len) => _IOC(_IOC_READ, 'E', 0x18, len); // get global key state
        internal static ulong EVIOCGLED(int len) => _IOC(_IOC_READ, 'E', 0x19, len); // get all LEDs
        internal static ulong EVIOCGSND(int len) => _IOC(_IOC_READ, 'E', 0x1a, len); // get all sounds status
        internal static ulong EVIOCGSW(int len) => _IOC(_IOC_READ, 'E', 0x1b, len); // get all switch states

        internal static ulong EVIOGCBIT(int ev, int len) => _IOC(_IOC_READ, 'E', 0x20 + ev, len); // get event bits
        internal static ulong EVIOCGABS(int abs) => _IOR('E', 0x40 + abs, Marshal.SizeOf<input_absinfo>()); // get abs value/limit
        internal static ulong EVIOCSABS(int abs) => _IOW('E', 0xc0 + abs, Marshal.SizeOf<input_absinfo>()); // set abs value/limit

        internal static ulong EVIOCSFF => _IOW('E', 0x80, Marshal.SizeOf<ff_effect>()); // send a force effect to a force feedback device
        internal static ulong EVIOCRMFF => _IOW('E', 0x81, sizeof(int)); // erase a force effect
        internal static ulong EVOCGEFFECTS => _IOR('E', 0x84, sizeof(int)); // report number of effects playable at the same time

        internal static ulong EVIOGRAB => _IOW('E', 0x90, sizeof(int)); // grab/release device
        internal static ulong EVIOCREMOVE => _IOW('E', 0x91, sizeof(int)); // revoke device access
        internal static ulong EVIOCGMASK => _IOR('E', 0x92, Marshal.SizeOf<input_mask>()); // get event masks
        internal static ulong EVIOCSMASK => _IOW('E', 0x93, Marshal.SizeOf<input_mask>()); // set event masks
        internal static ulong EVIOCSCLOCKID => _IOW('E', 0xa0, sizeof(int)); // set clockid for timestamps

        const int ID_BUS            = 0;
        const int ID_VENDOR         = 1;
        const int ID_PRODUCT        = 2;
        const int ID_VERSION        = 3;

        const int BUS_PCI           = 0x01;
        const int BUS_ISAPNP        = 0x02;
        const int BUS_USB           = 0x03;
        const int BUS_HIL           = 0x04;
        const int BUS_BLUETOOTH     = 0x05;
        const int BUS_VIRTUAL       = 0x06;

        const int BUS_ISA			= 0x10;
        const int BUS_I8042		    = 0x11;
        const int BUS_XTKBD		    = 0x12;
        const int BUS_RS232		    = 0x13;
        const int BUS_GAMEPOR       = 0x14;
        const int BUS_PARPORT		= 0x15;
        const int BUS_AMIGA		    = 0x16;
        const int BUS_ADB			= 0x17;
        const int BUS_I2C			= 0x18;
        const int BUS_HOST		    = 0x19;
        const int BUS_GSC			= 0x1A;
        const int BUS_ATARI		    = 0x1B;
        const int BUS_SPI			= 0x1C;
        const int BUS_RMI			= 0x1D;
        const int BUS_CEC			= 0x1E;
        const int BUS_INTEL_ISHTP	= 0x1F;
        const int BUS_AMD_SFH		= 0x20;

        // MT_TOOL types
        const int MT_TOOL_FINGER    = 0x00;
        const int MT_TOOL_PEN		= 0x01;
        const int MT_TOOL_PALM		= 0x02;
        const int MT_TOOL_DIAL		= 0x0a;
        const int MT_TOOL_MAX		= 0x0f;

        // Describing the status of a force-feedback effect
        const int FF_STATUS_STOPPED	= 0x00;
        const int FF_STATUS_PLAYING	= 0x01;
        const int FF_STATUS_MAX		= 0x01;

        struct ff_replay
        {
            ushort length;
            ushort delay;
        }

        struct ff_trigger
        {
            ushort button;
            ushort interval;
        }

        struct ff_envelope
        {
            ushort attack_length;
            ushort attack_level;
            ushort fade_length;
            ushort fade_level;
        }

        struct ff_constant_effect
        {
            short level;
            ff_envelope envelope;
        }

        struct ff_ramp_effect
        {
            short start_level;
            short end_level;
            ff_envelope envelope;
        }

        struct ff_condition_effect
        {
            ushort right_saturation;
            ushort left_saturation;

            short right_coeff;
            short left_coeff;

            ushort deadband;
            short center;
        }

        struct ff_periodic_effect
        {
            ushort waveform;
            ushort period;
            short magnitude;
            short offset;
            ushort phase;
            ff_envelope envelope;
            uint custom_len;
            short* custom_data;
        }

        struct ff_rumble_effect
        {
            ushort strong_magnitude;
            ushort weak_magnitude;
        }

        struct ff_effect
        {
            ushort type;
            short id;
            ushort direction;
            ff_trigger trigger;
            ff_replay replay;

            ff_condition_effect constant;
            ff_ramp_effect ramp;
            ff_periodic_effect periodic;
            ff_condition_effect[] condition = new ff_condition_effect[2];
            ff_rumble_effect rumble;

            public ff_effect(){}
        }

    }

}