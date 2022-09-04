using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using OpenTK.Core.Platform;

namespace OpenTK.Platform.Native.Windows
{
    public class JoystickComponent : IJoystickComponent
    {
        public string Name => "Win32JoystickComponent";

        public PalComponents Provides => PalComponents.JoystickInput;

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

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < devices.Length; i++)
            {
                Console.WriteLine($"{i+1} Type: {devices[i].dwType}");

                uint size = 0;

                result = Win32.GetRawInputDeviceInfo(devices[i].hDevice, RIDI.DeviceName, null, ref size);
                if (result == unchecked((uint)-1))
                {
                    throw new Win32Exception();
                }

                sb.EnsureCapacity((int)size);

                result = Win32.GetRawInputDeviceInfo(devices[i].hDevice, RIDI.DeviceName, sb, ref size);
                if (result == unchecked((uint)-1))
                {
                    throw new Win32Exception();
                }

                //Console.WriteLine($"  Name: {sb}");

                Win32.RID_DEVICE_INFO info = default;
                size = (uint)sizeof(Win32.RID_DEVICE_INFO);
                result = Win32.GetRawInputDeviceInfo(devices[i].hDevice, RIDI.DeviceInfo, out info, ref size);
                if (result == unchecked((uint)-1))
                {
                    throw new Win32Exception();
                }

                switch (info.dwType)
                {
                    case RIM.TypeMouse:
                        Win32.RID_DEVICE_INFO_MOUSE mouse = info.mouse;
                        Console.WriteLine($"    Id: {mouse.dwId}, Buttons: {mouse.dwNumberOfButtons}, Sample Rate: {mouse.dwSampleRate}, Horizontal Wheel: {mouse.fHasHorizontalWheel == 1}");
                        break;
                    case RIM.TypeKeyboard:
                        Win32.RID_DEVICE_INFO_KEYBOARD keyboard = info.keyboard;
                        Console.WriteLine($"    Type: {keyboard.dwType}, SubType: {keyboard.dwSubType}, Mode: {keyboard.dwKeyboardMode}, No Function Keys: {keyboard.dwNumberOfFunctionKeys}, Indicators: {keyboard.dwNumberOfIndicators}, Keys: {keyboard.dwNumberOfKeysTotal}");
                        break;
                    case RIM.TypeHID:
                        Win32.RID_DEVICE_INFO_HID hid = info.hid;
                        Console.WriteLine($"    VendorID: {hid.dwVendorId}, ProductID: {hid.dwProductId}, Version: {hid.dwVersionNumber}, UsagePage: {hid.usUsagePage:X}, Usage: {(hid.usUsagePage == HIDUsagePage.Generic ? ((HIDUsageGeneric)hid.usUsage).ToString() : hid.usUsage.ToString("X"))}");
                        break;
                }
            }

            Span<Win32.RAWINPUTDEVICE> rid = stackalloc Win32.RAWINPUTDEVICE[]
            {
                new Win32.RAWINPUTDEVICE()
                {
                    usUsagePage = HIDUsagePage.Generic,
                    usUsage = (ushort)HIDUsageGeneric.Gamepad,
                    dwFlags = RIDEV.DevNotify,
                    hwndTarget = IntPtr.Zero,
                },
                new Win32.RAWINPUTDEVICE()
                {
                    usUsagePage = HIDUsagePage.Generic,
                    usUsage = (ushort)HIDUsageGeneric.Joystick,
                    dwFlags = RIDEV.DevNotify,
                    hwndTarget = IntPtr.Zero,
                },
                new Win32.RAWINPUTDEVICE()
                {
                    usUsagePage = HIDUsagePage.Generic,
                    usUsage = (ushort)HIDUsageGeneric.Mouse,
                    dwFlags = RIDEV.DevNotify,
                    hwndTarget = IntPtr.Zero,
                },
                new Win32.RAWINPUTDEVICE()
                {
                    usUsagePage = HIDUsagePage.Generic,
                    usUsage = (ushort)HIDUsageGeneric.Keyboard,
                    dwFlags = RIDEV.DevNotify,
                    hwndTarget = IntPtr.Zero,
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
            throw new NotImplementedException();
        }
    }
}
