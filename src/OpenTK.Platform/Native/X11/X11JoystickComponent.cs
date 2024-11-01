using System;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Core.Utility;
using static OpenTK.Platform.Native.X11.Libudev;

namespace OpenTK.Platform.Native.X11
{
    public class X11JoystickComponent : IJoystickComponent
    {
        public string Name => nameof(X11JoystickComponent);

        public PalComponents Provides => PalComponents.Joystick;

        public ILogger? Logger { get; set; }

        internal static UdevPtr Udev;
        internal static UdevMonitorPtr Monitor;

        public void Initialize(ToolkitOptions options)
        {
            // FIXME: check if udev is available...?
            Udev  = udev_new();

            Monitor = udev_monitor_new_from_netlink(Udev, "udev"u8);
            udev_monitor_filter_add_match_subsystem_devtype(Monitor, "input"u8, null);
            udev_monitor_enable_receiving(Monitor);

            UdevEnumeratePtr udevEnum = udev_enumerate_new(Udev);

            udev_enumerate_add_match_subsystem(udevEnum, "input"u8);

            // FIXME: check result..
            int result = udev_enumerate_scan_devices(udevEnum);
            UdevListEntryPtr list = udev_enumerate_get_list_entry(udevEnum);
            for (UdevListEntryPtr entry = list; entry.Value != 0; entry = udev_list_entry_get_next(entry))
            {
                string? name = udev_list_entry_get_name(entry);
                string? value = udev_list_entry_get_value(entry);

                Logger?.LogInfo($"{name} {value}");
                if (name == null)
                    continue;
                
                UdevDevicePtr device = udev_device_new_from_syspath(Udev, name);
                if (device.Value != 0)
                {
                    IntPtr devnode = udev_device_get_devnode(device);
                    if (devnode == IntPtr.Zero)
                    {
                        udev_device_unref(device);
                        continue;
                    }

                    // Figure out if this is a joystick?
                    var val = udev_device_get_property_value(device, "ID_INPUT_JOYSTICK"u8);
                    if (val.SequenceEqual("1"u8)) {
                        Logger?.LogInfo("ID_INPUT_JOYSTICK");

                        // FIXME: We only really care about these devices..
                    }

                    val = udev_device_get_property_value(device, "ID_INPUT_MOUSE"u8);
                    if (val.SequenceEqual("1"u8)) {
                        Logger?.LogInfo("ID_INPUT_MOUSE");
                    }

                    val = udev_device_get_property_value(device, "ID_CLASS"u8);
                    if (val.Length > 0)
                    {
                        string cls = Encoding.UTF8.GetString(val);
                        Logger?.LogInfo($"Class: {cls}");
                    }

                    udev_device_unref(device);
                }
            }
            udevEnum = udev_enumerate_unref(udevEnum);
        }

        internal void Uninitialize()
        {
            Udev = udev_unref(Udev);
        }

        internal static void PollForUpdates(ILogger? logger)
        {
            Span<byte> joystickName = stackalloc byte[128];

            int monitorfd = udev_monitor_get_fd(Monitor);
            while (X11.FileDescriptorHasReadData(monitorfd))
            {
                UdevDevicePtr device = udev_monitor_receive_device(Monitor);
                if (device.Value == 0)
                    break;

                ReadOnlySpan<byte> action = udev_device_get_action(device);
                if (action.SequenceEqual("add"u8))
                {
                    // figure out if this is a js joystick or a newer event joystick.
                    IntPtr devnodePtr = udev_device_get_devnode(device);
                    string? devnode = Marshal.PtrToStringUTF8(devnodePtr);
                    // FIXME: Do we need to unref the device?
                    if (devnode == null)
                    {
                        continue;
                    }
                    
                    // FIXME: Do we need to unref the device?
                    if (IsJSJoystick(devnode))
                    {
                        continue;
                    }

                    ushort vendor = 0;
                    var val = udev_device_get_property_value(device, "ID_VENDOR_ID"u8);
                    if (val.Length > 0)
                    {
                        // FIXME: Hex parse
                        ushort.TryParse(val, System.Globalization.NumberStyles.HexNumber, null, out vendor);
                    }

                    ushort model = 0;
                    val = udev_device_get_property_value(device, "ID_MODEL_ID"u8);
                    if (val.Length > 0)
                    {
                        // FIXME: Hex parse
                        ushort.TryParse(val, System.Globalization.NumberStyles.HexNumber, null, out model);
                    }

                    ushort version = 0;
                    val = udev_device_get_property_value(device, "ID_REVISION"u8);
                    if (val.Length > 0)
                    {
                        // FIXME: Hex parse
                        ushort.TryParse(val, System.Globalization.NumberStyles.HexNumber, null, out version);
                    }

                    int fd = Linux.open(devnodePtr, Linux.file_flags.O_RDONLY | Linux.file_flags.O_CLOEXEC, 0);
                    if (fd < 0)
                    {
                        // FIXME: Do we need to unref the device?
                        logger?.LogWarning($"Failed to open file: '{devnode}'");
                        continue;
                    }

                    if (Linux.fstat(fd, out Linux.stat_t st) == -1)
                    {
                        Linux.close(fd);
                        continue;
                    }

                    // FIXME: EVIOCGNAME only seems to work the second time a joystick is added to the system.
                    // The first time this call fails.
                    // JSIOCGNAME doesn't seem to work at all...?
                    // - Noggin_bops 2024-10-30
                    if (Linux.ioctl(fd, Linux.EVIOCGNAME(joystickName.Length), joystickName) <= 0)
                    {
                        Linux.close(fd);
                        continue;
                    }

                    string name = $"{Encoding.UTF8.GetString(joystickName.SliceAtFirstNull())} v:{vendor:X4} m:{model:X4} v:{version:X4}";

                    Linux.close(fd);

                    logger?.LogInfo($"Added input device '{name}'. devnode: {devnode}, vendor: 0x{vendor:X4}, model: 0x{model:X4}, version: 0x{version:X4}");

                    static bool IsJSJoystick(ReadOnlySpan<char> path)
                    {
                        int index = path.LastIndexOf('/');
                        if (index != -1)
                            path = path.Slice(index + 1);
                        return path.StartsWith("js") && int.TryParse(path.Slice(2), out _);
                    }
                }
                else if(action.SequenceEqual("remove"u8))
                {
                    logger?.LogInfo("Removed input device.");
                }
            }
        }

        public float LeftDeadzone => 0;

        public float RightDeadzone => 0;

        public float TriggerThreshold => 0;

        public bool IsConnected(int index)
        {
            throw new NotImplementedException();
        }

        public JoystickHandle Open(int index)
        {
            throw new NotImplementedException();
        }

        public void Close(JoystickHandle handle)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(JoystickHandle handle)
        {
            throw new NotImplementedException();
        }

        public string GetName(JoystickHandle handle)
        {
            throw new NotImplementedException();
        }

        public float GetAxis(JoystickHandle handle, JoystickAxis axis)
        {
            throw new NotImplementedException();
        }

        public bool GetButton(JoystickHandle handle, JoystickButton button)
        {
            throw new NotImplementedException();
        }

        public bool SetVibration(JoystickHandle handle, float lowFreqIntensity, float highFreqIntensity)
        {
            throw new NotImplementedException();
        }

        public bool TryGetBatteryInfo(JoystickHandle handle, out GamepadBatteryInfo batteryInfo)
        {
            throw new NotImplementedException();
        }
    }
}
