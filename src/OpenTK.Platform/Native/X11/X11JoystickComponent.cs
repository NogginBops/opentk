using System;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Core.Utility;
using static OpenTK.Platform.Native.X11.Libudev;
using static OpenTK.Platform.Native.X11.Libevdev;
using System.ComponentModel;
using System.Threading;
using System.Collections.Generic;

namespace OpenTK.Platform.Native.X11
{
    public class X11JoystickComponent : IJoystickComponent
    {
        /// <inheritdoc/>
        public string Name => nameof(X11JoystickComponent);

        /// <inheritdoc/>
        public PalComponents Provides => PalComponents.Joystick;

        /// <inheritdoc/>
        public ILogger? Logger { get; set; }

        internal static UdevPtr Udev;
        internal static UdevMonitorPtr Monitor;

        internal static List<XJoystickHandle> JoystickHandles = new List<XJoystickHandle>();

        /// <inheritdoc/>
        public void Initialize(ToolkitOptions options)
        {
            // FIXME: check if udev is available...?
            Udev = udev_new();

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
                    string? devnode = udev_device_get_devnode(device);
                    string? devtype = udev_device_get_devtype(device);
                    if (devnode == null)
                    {
                        udev_device_unref(device);
                        continue;
                    }

                    if (IsJSJoystick(devnode))
                    {
                        udev_device_unref(device);
                        continue;
                    }

                    // FIXME: This seems like a decent way of filtering out joysticks??
                    var val = udev_device_get_property_value(device, "ID_INPUT_JOYSTICK"u8);
                    if (val.SequenceEqual("1"u8))
                    {
                        Logger?.LogInfo("ID_INPUT_JOYSTICK");
                    }
                    else
                    {
                        udev_device_unref(device);
                        continue;
                    }

                    ushort model = 0;
                    val = udev_device_get_property_value(device, "ID_MODEL_ID"u8);
                    if (val.Length > 0)
                    {
                        // FIXME: Hex parse
                        ushort.TryParse(val, System.Globalization.NumberStyles.HexNumber, null, out model);
                    }

                    IntPtr devnodePtr = Marshal.StringToCoTaskMemUTF8(devnode);
                    int fd = Linux.open(devnodePtr, Linux.file_flags.O_RDWR | Linux.file_flags.O_CLOEXEC, 0);
                    Marshal.FreeCoTaskMem(devnodePtr);
                    if (fd < 0)
                    {
                        // FIXME: Do we need to unref the device?
                        Logger?.LogWarning($"Failed to open file: '{devnode}'");
                        continue;
                    }

                    var dev = libevdev_new();

                    libevdev_set_device_log_function(dev, test_log_func, libevdev_log_priority.Debug, 0);

                    int status = libevdev_set_fd(dev, fd);
                    string? statusStr = Libc.strerror(-status);

                    string evdev_name = libevdev_get_name(dev);
                    string? evdev_phys = libevdev_get_phys(dev);
                    string? evdev_uniq = libevdev_get_uniq(dev);
                    int evdev_product = libevdev_get_id_product(dev);
                    int evdev_vendor = libevdev_get_id_vendor(dev);
                    BusType evdev_bustype = libevdev_get_id_bustype(dev);
                    int evdev_version = libevdev_get_id_version(dev);
                    int evdev_driver_version = libevdev_get_driver_version(dev);

                    Logger?.LogDebug($"Added input device '{evdev_name}' {evdev_phys} {evdev_uniq} model: 0x{model:X4}, prod: 0x{evdev_product}, vendor: 0x{evdev_vendor:X4}, version: 0x{evdev_version:X4}, bustype: {evdev_bustype}, driver version: 0x{evdev_driver_version:X4}");

                    XJoystickHandle handle = new XJoystickHandle(dev);

                    JoystickHandles.Add(handle);

                    udev_device_unref(device);
                }
            }
            udevEnum = udev_enumerate_unref(udevEnum);
        }

        private static void test_log_func(LibEvdevPtr dev, libevdev_log_priority priority, IntPtr data, IntPtr file, int line, IntPtr func, IntPtr format /*, va_list */)
        {
            Toolkit.Joystick.Logger?.LogDebug("Callback!");
        }

        /// <inheritdoc/>
        public void Uninitialize()
        {
            for (int i = 0; i < JoystickHandles.Count; i++)
            {
                libevdev_free(JoystickHandles[i].Device);
            }

            Udev = udev_unref(Udev);
        }

        private static bool IsJSJoystick(ReadOnlySpan<char> path)
        {
            int index = path.LastIndexOf('/');
            if (index != -1)
                path = path.Slice(index + 1);
            return path.StartsWith("js") && int.TryParse(path.Slice(2), out _);
        }

        internal static void PollForUpdates(ILogger? logger)
        {
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
                    string? devnode = udev_device_get_devnode(device);
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

                    ushort model = 0;
                    var val = udev_device_get_property_value(device, "ID_MODEL_ID"u8);
                    if (val.Length > 0)
                    {
                        // FIXME: Hex parse
                        ushort.TryParse(val, System.Globalization.NumberStyles.HexNumber, null, out model);
                    }

                    IntPtr devnodePtr = Marshal.StringToCoTaskMemUTF8(devnode);
                    int fd = Linux.open(devnodePtr, Linux.file_flags.O_RDWR | Linux.file_flags.O_CLOEXEC, 0);
                    Marshal.FreeCoTaskMem(devnodePtr);
                    if (fd < 0)
                    {
                        // FIXME: Do we need to unref the device?
                        logger?.LogWarning($"Failed to open file: '{devnode}'");
                        continue;
                    }

                    var dev = libevdev_new();

                    libevdev_set_device_log_function(dev, test_log_func, libevdev_log_priority.Debug, 0);

                    int status = libevdev_set_fd(dev, fd);
                    string? statusStr = Libc.strerror(-status);

                    string evdev_name = libevdev_get_name(dev);
                    string? evdev_phys = libevdev_get_phys(dev);
                    string? evdev_uniq = libevdev_get_uniq(dev);
                    int evdev_product = libevdev_get_id_product(dev);
                    int evdev_vendor = libevdev_get_id_vendor(dev);
                    BusType evdev_bustype = libevdev_get_id_bustype(dev);
                    int evdev_version = libevdev_get_id_version(dev);
                    int evdev_driver_version = libevdev_get_driver_version(dev);

                    logger?.LogDebug($"Added input device '{evdev_name}' {evdev_phys} {evdev_uniq} model: 0x{model:X4}, prod: 0x{evdev_product}, vendor: 0x{evdev_vendor:X4}, version: 0x{evdev_version:X4}, bustype: {evdev_bustype}, driver version: 0x{evdev_driver_version:X4}");

                    libevdev_free(dev);

                    Linux.close(fd);
                }
                else if (action.SequenceEqual("remove"u8))
                {
                    logger?.LogInfo("Removed input device.");
                }
            }

            for (int i = 0; i < JoystickHandles.Count; i++)
            {
                XJoystickHandle xjoystick = JoystickHandles[i];

                input_event ev = default;
                while (libevdev_has_event_pending(xjoystick.Device) == 1)
                {
                    libevdev_read_status ev_status = libevdev_next_event(xjoystick.Device, libevdev_read_flag.Normal, ref ev);
                    if (ev_status == libevdev_read_status.Sync)
                    {
                        string name = libevdev_get_name(xjoystick.Device);
                        logger?.LogDebug($"libevdev dropped '{name}'!");

                        while (ev_status == libevdev_read_status.Sync)
                        {
                            if ((EventType)ev.type == EventType.Absolute)
                            {
                                logger?.LogDebug($"{(EventType)ev.type} {(AbsoluteAxis)ev.code} {ev.value} {ev_status}");
                            }
                            else if ((EventType)ev.type == EventType.Key)
                            {
                                logger?.LogDebug($"{(EventType)ev.type} {(Button)ev.code} {ev.value} {ev_status}");
                            }
                            else
                            {
                                logger?.LogDebug($"{(EventType)ev.type} {ev.code} {ev.value} {ev_status}");
                            }
                            ev_status = libevdev_next_event(xjoystick.Device, libevdev_read_flag.Sync, ref ev);
                        }

                        logger?.LogDebug($"libevdev re-synced '{name}'!");
                    }
                    else if (ev_status == libevdev_read_status.Success)
                    {
                        if ((EventType)ev.type == EventType.Absolute)
                        {
                            logger?.LogDebug($"{(EventType)ev.type} {(AbsoluteAxis)ev.code} {ev.value} {ev_status}");
                        }
                        else if ((EventType)ev.type == EventType.Key)
                        {
                            logger?.LogDebug($"{(EventType)ev.type} {(Button)ev.code} {ev.value} {ev_status}");
                        }
                        else
                        {
                            logger?.LogDebug($"{(EventType)ev.type} {ev.code} {ev.value} {ev_status}");
                        }
                    }
                    else if ((int)ev_status != -11 /* EAGAIN */)
                    {
                        logger?.LogDebug($"libevdev_next_event returned error: {Libc.strerror(-(int)ev_status)}");
                    }
                }
            }
        }

        private float LeftDeadzoneValue = 7849 / 32767.0f;
        private float RightDeadzoneValue = 8689 / 32767.0f;
        private float TriggerDeadzoneValue = 0 / 32767.0f;

        /// <inheritdoc/>
        public float LeftDeadzone => LeftDeadzoneValue;

        /// <inheritdoc/>
        public float RightDeadzone => RightDeadzoneValue;

        /// <inheritdoc/>
        public float TriggerThreshold => TriggerDeadzoneValue;

        /// <inheritdoc/>
        public bool IsConnected(int index)
        {
            return index >= 0 && index < JoystickHandles.Count;
        }

        /// <inheritdoc/>
        public JoystickHandle Open(int index)
        {
            return JoystickHandles[index];
        }

        /// <inheritdoc/>
        public void Close(JoystickHandle handle)
        {
            XJoystickHandle xjoystick = handle.As<XJoystickHandle>(this);

            // FIXME:
        }

        /// <inheritdoc/>
        public Guid GetGuid(JoystickHandle handle)
        {
            XJoystickHandle xjoystick = handle.As<XJoystickHandle>(this);

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string GetName(JoystickHandle handle)
        {
            XJoystickHandle xjoystick = handle.As<XJoystickHandle>(this);

            string name = libevdev_get_name(xjoystick.Device);
            return name;
        }

        /// <inheritdoc/>
        public unsafe float GetAxis(JoystickHandle handle, JoystickAxis axis)
        {
            XJoystickHandle xjoystick = handle.As<XJoystickHandle>(this);

            AbsoluteAxis absAxis;
            switch (axis)
            {
                case JoystickAxis.LeftXAxis:
                    absAxis = AbsoluteAxis.X;
                    break;
                case JoystickAxis.LeftYAxis:
                    absAxis = AbsoluteAxis.Y;
                    break;
                case JoystickAxis.RightXAxis:
                    absAxis = AbsoluteAxis.RX;
                    break;
                case JoystickAxis.RightYAxis:
                    absAxis = AbsoluteAxis.RY;
                    break;
                case JoystickAxis.LeftTrigger:
                    absAxis = AbsoluteAxis.Z;
                    break;
                case JoystickAxis.RightTrigger:
                    absAxis = AbsoluteAxis.RZ;
                    break;
                // FIXME:
                default:
                    return 0;
            }

            input_absinfo* abs_info = libevdev_get_abs_info(xjoystick.Device, absAxis);

            float value;
            if (abs_info->value >= 0)
            {
                value = abs_info->value / (float)abs_info->maximum;
            }
            else
            {
                value = -abs_info->value / (float)abs_info->minimum;
            }
            return value;
        }

        /// <inheritdoc/>
        public unsafe bool GetButton(JoystickHandle handle, JoystickButton button)
        {
            XJoystickHandle xjoystick = handle.As<XJoystickHandle>(this);

            Button btn;
            switch (button)
            {
                case JoystickButton.A:
                    btn = Button.BTN_A;
                    break;
                case JoystickButton.B:
                    btn = Button.BTN_B;
                    break;
                case JoystickButton.X:
                    btn = Button.BTN_X;
                    break;
                case JoystickButton.Y:
                    btn = Button.BTN_Y;
                    break;
                case JoystickButton.Start:
                    btn = Button.BTN_START;
                    break;
                case JoystickButton.Back:
                    btn = Button.BTN_SELECT;
                    break;
                case JoystickButton.LeftThumb:
                    btn = Button.BTN_THUMBL;
                    break;
                case JoystickButton.RightThumb:
                    btn = Button.BTN_THUMBR;
                    break;
                case JoystickButton.LeftShoulder:
                    btn = Button.BTN_TL;
                    break;
                case JoystickButton.RightShoulder:
                    btn = Button.BTN_TR;
                    break;
                case JoystickButton.DPadUp:
                    {
                        input_absinfo* absInfo = libevdev_get_abs_info(xjoystick.Device, AbsoluteAxis.HAT0Y);
                        return absInfo->value < 0;
                    }
                case JoystickButton.DPadDown:
                    {
                        input_absinfo* absInfo = libevdev_get_abs_info(xjoystick.Device, AbsoluteAxis.HAT0Y);
                        return absInfo->value > 0;
                    }
                case JoystickButton.DPadLeft:
                    {
                        input_absinfo* absInfo = libevdev_get_abs_info(xjoystick.Device, AbsoluteAxis.HAT0X);
                        return absInfo->value < 0;
                    }
                case JoystickButton.DPadRight:
                    {
                        input_absinfo* absInfo = libevdev_get_abs_info(xjoystick.Device, AbsoluteAxis.HAT0X);
                        return absInfo->value > 0;
                    }
                    // FIXME: These are reported as absolute axis, but we think of them like buttons?
                    return false;

                default:
                    return false;
            }

            int test = libevdev_get_event_value(xjoystick.Device, EventType.Key, (uint)btn);
            return test != 0;
        }

        /// <inheritdoc/>
        public unsafe bool SetVibration(JoystickHandle handle, float lowFreqIntensity, float highFreqIntensity)
        {
            XJoystickHandle xjoystick = handle.As<XJoystickHandle>(this);

            int fd = libevdev_get_fd(xjoystick.Device);

            Span<Libc.ff_effect> effect = stackalloc Libc.ff_effect[1];
            effect[0].type = Libc.FFType.Rumble;
            effect[0].id = xjoystick.FFEffectID;
            effect[0].direction = 0;
            effect[0].trigger = default;
            effect[0].replay = default;
            effect[0].rumble.strong_magnitude = (ushort)float.Clamp(lowFreqIntensity * ushort.MaxValue, 0, ushort.MaxValue);
            effect[0].rumble.weak_magnitude = (ushort)float.Clamp(highFreqIntensity * ushort.MaxValue, 0, ushort.MaxValue);

            Libc.ioctl(fd, Libc.EVIOCSFF, effect);

            xjoystick.FFEffectID = effect[0].id;

            input_event play = default;
            play.type = (ushort)EventType.ForceFeedback;
            play.code = (ushort)effect[0].id;
            play.value = 1;

            nint ret = Linux.write(fd, &play, (nuint)sizeof(input_event));

            xjoystick.LowFreqIntensity = lowFreqIntensity;
            xjoystick.HighFreqIntensity = highFreqIntensity;

            return ret != -1;
        }

        /// <inheritdoc/>
        public bool TryGetBatteryInfo(JoystickHandle handle, out GamepadBatteryInfo batteryInfo)
        {
            XJoystickHandle xjoystick = handle.As<XJoystickHandle>(this);

            // FIXME: Find some way to coneect this /dev/input/event* device to
            // some /sys/class/power_supply/*, not sure how to do this.
            // - Noggin_bops 2025-07-14

            batteryInfo = default;
            return false;
        }
    }
}
