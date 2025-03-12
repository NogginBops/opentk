using System;
using System.Collections.Generic;
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

        private static List<XJoystickHandle> _connectedJoystickHandles = new();
        private static List<XJoystickHandle> _openedJoystickHandles = new();

        /// <inheritdoc/>
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
            // TODO: Fixed?
            if (result < 0) throw new Exception("Scanning devices failed.");

            UdevListEntryPtr list = udev_enumerate_get_list_entry(udevEnum);

            Span<byte> potentialJoystickName = stackalloc byte[128];

            for (UdevListEntryPtr entry = list; entry.Value != 0; entry = udev_list_entry_get_next(entry))
            {
                string? name = udev_list_entry_get_name(entry);
                string? value = udev_list_entry_get_value(entry);

                Logger?.LogInfo($"{name}, {value}");
                if (name == null)
                    continue;
                
                // Gets a device from the system path. Is not always a joystick.
                UdevDevicePtr device = udev_device_new_from_syspath(Udev, name);
                if (device.Value != 0)
                {
                    IntPtr devnodePtr = udev_device_get_devnode(device);
                    string? devnodeString = Marshal.PtrToStringAnsi(devnodePtr);
                    if (devnodePtr == IntPtr.Zero)
                    {
                        udev_device_unref(device);
                        continue;
                    }

                    if (devnodeString == null) continue;

                    // Filter out joydev inputs. We only need evdev inputs.
                    if (IsJSJoystick(Marshal.PtrToStringAnsi(devnodePtr))) continue;

                    // Figure out if this is a joystick?
                    var val = udev_device_get_property_value(device, "ID_INPUT_JOYSTICK"u8);
                    if (val.SequenceEqual("1"u8)) {
                        Logger?.LogInfo("ID_INPUT_JOYSTICK");

                        // FIXME: We only really care about these devices..

                        int fd = Linux.open(devnodePtr, Linux.file_flags.O_RDONLY | Linux.file_flags.O_CLOEXEC, 0);
                        if (fd < 0)
                        {
                            // FIXME: Do we need to unref the device?
                            Logger?.LogWarning($"Failed to open file: '{devnodePtr}'");
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
                        if (Linux.ioctl(fd, Linux.EVIOCGNAME(potentialJoystickName.Length), potentialJoystickName) <= 0)
                        {
                            Linux.close(fd);
                            // continue;
                        }

                        // while (Linux.ioctl(fd, Linux.EVIOCGNAME(potentialJoystickName.Length), potentialJoystickName) <= 0);

                        Logger?.LogInfo(name);

                        XJoystickHandle handle = new XJoystickHandle() { JoystickPtr = device, JoystickName = Encoding.UTF8.GetString(potentialJoystickName.SliceAtFirstNull()) };

                        if (_connectedJoystickHandles.Count == 0) _connectedJoystickHandles.Add(handle);

                        foreach (XJoystickHandle h in _connectedJoystickHandles)
                        {

                            string? hName = Marshal.PtrToStringAnsi(udev_device_get_devnode(h.JoystickPtr));
                            string? currName = Marshal.PtrToStringAnsi(udev_device_get_devnode(handle.JoystickPtr));

                            Console.WriteLine($"{hName}, {currName}");

                            if (hName != currName)
                            {
                                _connectedJoystickHandles.Add(handle);
                                continue;
                            }

                        }
                        // if (!_connectedJoystickHandles.Contains(handle)) _connectedJoystickHandles.Add(handle);

                    } else
                    {

                        udev_device_unref(device);
                        continue;

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

                    // udev_device_unref(device);
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

                    var isJoystick = udev_device_get_property_value(device, "ID_INPUT_JOYSTICK"u8);
                    if (isJoystick.SequenceEqual("1"u8))
                    {

                        // This is a joystick.
                        logger?.LogInfo($"A joystick {devnode} has been added");
                        _connectedJoystickHandles.Add( new XJoystickHandle() { JoystickPtr = device } );

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
                    int err = Linux.ioctl(fd, Linux.EVIOCGNAME(joystickName.Length), joystickName);
                    if (err <= 0)
                    {
                        //Console.WriteLine($"EVIOCGNAME errored with int {err}");
                        Linux.close(fd);
                        continue;
                    }

                    string name = $"{Encoding.UTF8.GetString(joystickName.SliceAtFirstNull())} v:{vendor:X4} m:{model:X4} v:{version:X4}";

                    Linux.close(fd);

                    logger?.LogInfo($"Added input device '{name}'. devnode: {devnode}, vendor: 0x{vendor:X4}, model: 0x{model:X4}, version: 0x{version:X4}");

                }
                else if(action.SequenceEqual("remove"u8))
                {
                    
                    var potentialJoystick = udev_device_get_property_value(device, "ID_INPUT_JOYSTICK"u8);

                    if (potentialJoystick.SequenceEqual("1"u8))
                    {

                        foreach (XJoystickHandle handle in _connectedJoystickHandles)
                        {

                            string? handleDevNode = Marshal.PtrToStringAnsi(udev_device_get_devnode(handle.JoystickPtr));
                            string? deviceDevNode = Marshal.PtrToStringAnsi(udev_device_get_devnode(device));

                            if (handleDevNode == deviceDevNode)
                            {

                                logger?.LogInfo($"A joystick {handle.JoystickName} has been removed.");
                                udev_device_unref(handle.JoystickPtr);
                                _connectedJoystickHandles.Remove(handle);
                                break;

                            }

                        }

                    }
                    
                }
            }
        }

        private static bool IsJSJoystick(ReadOnlySpan<char> path)
        {
            int index = path.LastIndexOf('/');
            if (index != -1)
                path = path.Slice(index + 1);
            return path.StartsWith("js") && int.TryParse(path.Slice(2), out _);
        }

        // FIXME: These are copied over from Windows' JoystickComponent.
        // Should they be different?

        /// <inheritdoc/>
        public float LeftDeadzone => 7849 / 32767.0f;

        /// <inheritdoc/>
        public float RightDeadzone => 8689 / 32767.0f;

        /// <inheritdoc/>
        public float TriggerThreshold => 30 / 255.0f;

        /// <inheritdoc/>
        public bool IsConnected(int index)
        {

            foreach (XJoystickHandle handle in _connectedJoystickHandles)
            {

                // Console.WriteLine(Marshal.PtrToStringAnsi(udev_device_get_devnode(handle.JoystickPtr)));

            }

            // Console.WriteLine(_connectedJoystickHandles.Count);

            return _connectedJoystickHandles.Count > index;

        }

        /// <inheritdoc/>
        public JoystickHandle Open(int index)
        {

            _connectedJoystickHandles[index].FD = Linux.open(udev_device_get_devnode(_connectedJoystickHandles[index].JoystickPtr), 
                                                             Linux.file_flags.O_RDONLY | Linux.file_flags.O_CLOEXEC, 0);
            _openedJoystickHandles.Add(_connectedJoystickHandles[index]);
            return _connectedJoystickHandles[index];

        }

        /// <inheritdoc/>
        public void Close(JoystickHandle handle)
        {

            // TODO: What should this do? The handle is still inside _connectedJoystickHandles.
            Linux.close(((XJoystickHandle)handle).FD);
            _openedJoystickHandles.Remove((XJoystickHandle)handle);

        }

        /// <inheritdoc/>
        public Guid GetGuid(JoystickHandle handle)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public string GetName(JoystickHandle handle)
        {
            return ((XJoystickHandle)handle).JoystickName;
        }

        /// <inheritdoc/>
        public float GetAxis(JoystickHandle handle, JoystickAxis axis)
        {

            // int fd = Linux.open(udev_device_get_devnode(((XJoystickHandle)handle).JoystickPtr), Linux.file_flags.O_RDONLY | Linux.file_flags.O_CLOEXEC, 0);

            // read
            

            // Linux.close(fd);

            // return axis info

            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool GetButton(JoystickHandle handle, JoystickButton button)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool SetVibration(JoystickHandle handle, float lowFreqIntensity, float highFreqIntensity)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public bool TryGetBatteryInfo(JoystickHandle handle, out GamepadBatteryInfo batteryInfo)
        {
            throw new NotImplementedException();
        }
    }
}
