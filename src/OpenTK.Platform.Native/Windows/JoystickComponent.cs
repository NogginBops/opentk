using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using OpenTK.Core.Platform;

namespace OpenTK.Platform.Native.Windows
{
    public class JoystickComponent : IJoystickComponent
    {
        public string Name => "Win32JoystickComponent";

        public PalComponents Provides => PalComponents.JoystickInput;

        private static Dictionary<IntPtr, HDevice> DeviceDict = new Dictionary<IntPtr, HDevice>();

        private static StringBuilder deviceName = new StringBuilder();
        private static StringBuilder hidNameBuilder = new StringBuilder(4093);
        public static string GetDeviceNameFromHandle(IntPtr hDevice)
        {
            uint result;
            uint size = 0;

            result = Win32.GetRawInputDeviceInfo(hDevice, RIDI.DeviceName, null, ref size);
            if (result == unchecked((uint)-1))
            {
                throw new Win32Exception();
            }

            deviceName.EnsureCapacity((int)size);

            result = Win32.GetRawInputDeviceInfo(hDevice, RIDI.DeviceName, deviceName, ref size);
            if (result == unchecked((uint)-1))
            {
                throw new Win32Exception();
            }

            return deviceName.ToString();
        }
        public static bool TryGetHidNameFromDeviceName(string deviceName, [NotNullWhen(true)] out string? hidName)
        {
            IntPtr handle = Win32.CreateFile(deviceName, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, FileAttributes.Normal, IntPtr.Zero);
            if (handle == new IntPtr(-1))
            {
                Win32.CloseHandle(handle);
                hidName = null;
                return false;
            }

            bool gotName = Win32.HidD_GetProductString(handle, hidNameBuilder, (ulong)hidNameBuilder.Capacity);
            if (gotName == false)
            {
                Win32.CloseHandle(handle);
                hidName = null;
                return false;
            }

            Win32.CloseHandle(handle);

            if (gotName)
            {
                hidName = hidNameBuilder.ToString();
                return true;
            }
            else
            {
                hidName = null;
                return false;
            }
        }

        internal static void JoysticksChanged(bool connected, IntPtr hDevice)
        {
            if (connected)
            {
                Debug.Assert(DeviceDict.ContainsKey(hDevice) == false);

                Win32.RID_DEVICE_INFO info = default;
                unsafe
                {
                    uint size = (uint)sizeof(Win32.RID_DEVICE_INFO);
                    uint result = Win32.GetRawInputDeviceInfo(hDevice, RIDI.DeviceInfo, out info, ref size);
                    if (result == unchecked((uint)-1))
                    {
                        throw new Win32Exception();
                    }
                }

                if (info.dwType == RIM.TypeHID &&
                    info.hid.usUsagePage == HIDUsagePage.Generic &&
                    ((HIDUsageGeneric)info.hid.usUsage == HIDUsageGeneric.Gamepad ||
                     (HIDUsageGeneric)info.hid.usUsage == HIDUsageGeneric.Joystick))
                {
                    string deviceName = GetDeviceNameFromHandle(hDevice);
                    if (TryGetHidNameFromDeviceName(deviceName, out string? hidName) == false)
                    {
                        throw new Exception($"Could not get product name for '{deviceName}' (HidD_GetProductString).");
                    }

                    HDevice device = new HDevice(hDevice, deviceName, hidName);

                    DeviceDict.Add(hDevice, device);

                    EventQueue.Raise(device, PlatformEventType.JoystickConnected, new JoystickConnectedEventArgs(true, device));
                }
            }
            else
            {
                // We only care about devices that we have in our dictionary.
                if (DeviceDict.TryGetValue(hDevice, out HDevice? device))
                {
                    // FIXME: Here the lParam handle is no longer valid, so we cannot query anything about it.

                    // This device is no longer connected.
                    DeviceDict.Remove(hDevice);

                    EventQueue.Raise(device, PlatformEventType.JoystickConnected, new JoystickConnectedEventArgs(false, device));
                }
            }
        }

        public unsafe void Initialize(PalComponents which)
        {
            if (which != PalComponents.JoystickInput)
            {
                throw new Exception("JoystickComponent can only initialize the JoystickInput component.");
            }

            uint deviceCount = 0;
            uint result;
            result = Win32.GetRawInputDeviceList(null, ref deviceCount, (uint)sizeof(Win32.RAWINPUTDEVICELIST));
            if (result == unchecked((uint)-1))
            {
                throw new Win32Exception();
            }

            Win32.RAWINPUTDEVICELIST[] devices = new Win32.RAWINPUTDEVICELIST[deviceCount];
            
            result = Win32.GetRawInputDeviceList(devices, ref deviceCount, (uint)sizeof(Win32.RAWINPUTDEVICELIST));
            if (result == unchecked((uint)-1))
            {
                throw new Win32Exception();
            }

            Console.WriteLine();
            Console.WriteLine("Controllers:");

            for (int i = 0; i < devices.Length; i++)
            {
                string deviceName = GetDeviceNameFromHandle(devices[i].hDevice);
                TryGetHidNameFromDeviceName(deviceName, out string? hidName);

                Win32.RID_DEVICE_INFO info = default;
                uint size = (uint)sizeof(Win32.RID_DEVICE_INFO);
                result = Win32.GetRawInputDeviceInfo(devices[i].hDevice, RIDI.DeviceInfo, out info, ref size);
                if (result == unchecked((uint)-1))
                {
                    throw new Win32Exception();
                }

                switch (info.dwType)
                {
                    case RIM.TypeMouse:
                        Win32.RID_DEVICE_INFO_MOUSE mouse = info.mouse;
                        //Console.WriteLine($"    Id: {mouse.dwId}, Buttons: {mouse.dwNumberOfButtons}, Sample Rate: {mouse.dwSampleRate}, Horizontal Wheel: {mouse.fHasHorizontalWheel == 1}");
                        break;
                    case RIM.TypeKeyboard:
                        Win32.RID_DEVICE_INFO_KEYBOARD keyboard = info.keyboard;
                        //Console.WriteLine($"    Type: {keyboard.dwType}, SubType: {keyboard.dwSubType}, Mode: {keyboard.dwKeyboardMode}, No Function Keys: {keyboard.dwNumberOfFunctionKeys}, Indicators: {keyboard.dwNumberOfIndicators}, Keys: {keyboard.dwNumberOfKeysTotal}");
                        break;
                    case RIM.TypeHID:
                        Win32.RID_DEVICE_INFO_HID hid = info.hid;

                        if (hid.usUsagePage == HIDUsagePage.Generic && (HIDUsageGeneric)hid.usUsage == HIDUsageGeneric.Gamepad)
                        {
                            Console.WriteLine($"  {hidName ?? "unknown"} ({deviceName})");
                            //Console.WriteLine($"    VendorID: {hid.dwVendorId}, ProductID: {hid.dwProductId}, Version: {hid.dwVersionNumber}, UsagePage: {hid.usUsagePage:X}, Usage: {(hid.usUsagePage == HIDUsagePage.Generic ? ((HIDUsageGeneric)hid.usUsage).ToString() : hid.usUsage.ToString("X"))}");
                        }
                        break;
                }

                
            }

            Console.WriteLine();

            // FIXME: Do we want to use WindowComponent.HelperHWnd here?
            Span<Win32.RAWINPUTDEVICE> rid = stackalloc Win32.RAWINPUTDEVICE[]
            {
                new Win32.RAWINPUTDEVICE()
                {
                    usUsagePage = HIDUsagePage.Generic,
                    usUsage = (ushort)HIDUsageGeneric.Gamepad,
                    dwFlags = RIDEV.DevNotify,
                    hwndTarget = WindowComponent.HelperHWnd,
                },
                new Win32.RAWINPUTDEVICE()
                {
                    usUsagePage = HIDUsagePage.Generic,
                    usUsage = (ushort)HIDUsageGeneric.Joystick,
                    dwFlags = RIDEV.DevNotify,
                    hwndTarget = WindowComponent.HelperHWnd,
                },
                new Win32.RAWINPUTDEVICE()
                {
                    usUsagePage = HIDUsagePage.Generic,
                    usUsage = (ushort)HIDUsageGeneric.Mouse,
                    dwFlags = RIDEV.DevNotify,
                    hwndTarget = WindowComponent.HelperHWnd,
                },
                new Win32.RAWINPUTDEVICE()
                {
                    usUsagePage = HIDUsagePage.Generic,
                    usUsage = (ushort)HIDUsageGeneric.Keyboard,
                    dwFlags = RIDEV.DevNotify,
                    hwndTarget = WindowComponent.HelperHWnd,
                }
            };

            bool success;

            success = Win32.RegisterRawInputDevices(rid, (uint)rid.Length, (uint)sizeof(Win32.RAWINPUTDEVICE));
            if (success == false)
            {
                throw new Win32Exception();
            }
        }

        public int MaxSupportedJoysticks => 4;

        public int GetActiveJoysticks(Span<JoystickHandle> handle)
        {
            throw new NotImplementedException();
        }

        public string GetJoystickName(JoystickHandle handle)
        {
            HDevice device = handle.As<HDevice>(this);

            if (DeviceDict.ContainsKey(device.Device) == false)
            {
                throw new PalException(this, "Invalid JoystickHandle, are you trying to query a disconnected joystick?");
            }

            // FIXME: Verify that this is still a valid handle?

            string deviceName = GetDeviceNameFromHandle(device.Device);
            TryGetHidNameFromDeviceName(deviceName, out string? hidName);

            // FIXME: What if the string is null??
            // Is there another name we can use here?
            return hidName ?? "";
        }
    }
}
