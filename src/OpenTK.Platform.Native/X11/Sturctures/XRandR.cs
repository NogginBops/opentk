using System;
using System.Diagnostics;

namespace OpenTK.Platform.Native.X11
{
    /// <summary>
    /// Opaque structure to the XRandR screen configuration structure.
    /// </summary>
    public struct XRRScreenConfiguration
    {
    }

    public struct XRRScreenSize
    {
        public int Width;
        public int Height;
        public int MWidth;
        public int MHeight;
    }

    /// <summary>
    /// XRandR Output handle.
    /// </summary>
    [DebuggerDisplay("XID={(System.IntPtr)Id}")]
    public struct RROutput
    {
        public ulong Id { get; }

        public static readonly RROutput None = new RROutput(0);

        public RROutput(ulong id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// XRandR Crtc handle.
    /// </summary>
    [DebuggerDisplay("XID={(System.IntPtr)Id}")]
    public struct RRCrtc
    {
        public ulong Id { get; }

        public static readonly RRCrtc None = new RRCrtc(0);

        public RRCrtc(ulong id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// XRandR mode handle.
    /// </summary>
    [DebuggerDisplay("XID={(System.IntPtr)Id}")]
    public struct RRMode
    {
        public ulong Id { get; }

        public static readonly RRMode None = new RRMode(0);

        public RRMode(ulong id)
        {
            Id = id;
        }
    }

    /// <summary>
    /// XRandR Output handle.
    /// </summary>
    [DebuggerDisplay("XID={(System.IntPtr)Id}")]
    public struct RRProvider
    {
        public ulong Id { get; }

        public static readonly RRProvider None = new RRProvider(0);

        public RRProvider(ulong id)
        {
            Id = id;
        }
    }

    public struct XRRModeInfo
    {
        public RRMode ModeId;
        public uint Width;
        public uint Height;
        public ulong DotClock;
        public uint HSyncStart;
        public uint HSyncEnd;
        public uint HTotal;
        public uint HSkew;
        public uint VSyncStart;
        public uint VSyncEnd;
        public uint VTotal;
        public IntPtr Name;
        public uint NameLength;
        public XRRModeFlags ModeFlags;
    }

    public unsafe struct XRRScreenResources
    {
        public XTime Timestamp;
        public XTime ConfigurationTimestamp;
        public int NumberOfCrtcs;
        public RRCrtc* Crtcs;
        public int NumberOfOutputs;
        public RROutput* Outputs;
        public int NumberOfModes;
        public XRRModeInfo* Modes;
    }
}