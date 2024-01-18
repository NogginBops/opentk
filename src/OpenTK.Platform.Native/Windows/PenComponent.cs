using OpenTK.Core.Platform;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenTK.Platform.Native.Windows
{
    internal class PenComponent
    {
        public void Initialize(PalComponents which)
        {
        }

        public static void GetPenState(uint pointerID, out PenState state)
        {
            Win32.GetPointerType(pointerID, out POINTER_INPUT_TYPE type);
            Debug.Assert(type == POINTER_INPUT_TYPE.PEN);
            Win32.GetPointerPenInfo(pointerID, out Win32.POINTER_PEN_INFO penInfo);

            // FIXME: Higher precision?
            state.Position = (penInfo.pointerInfo.ptPixelLocationRaw.X, penInfo.pointerInfo.ptPixelLocationRaw.Y);

            if (penInfo.penMask.HasFlag(PEN_MASK.PRESSURE))
                state.Pressure = penInfo.pressure / 1024.0f;
            else
                // FIXME: What should this be?
                state.Pressure = -1;

            if (penInfo.penMask.HasFlag(PEN_MASK.ROTATION))
                state.Angle = penInfo.rotation;
            else
                state.Angle = 0;

            if (penInfo.penMask.HasFlag(PEN_MASK.TILT_X))
                state.Tilt.X = penInfo.tiltX;
            else
                state.Tilt.X = 0;

            if (penInfo.penMask.HasFlag(PEN_MASK.TILT_Y))
                state.Tilt.Y = penInfo.tiltY;
            else
                state.Tilt.Y = 0;

            // FIXME!! Apply the changes from the last button state....?
            state.PressedButtons = 0;
        }

        // FIXME: Make non-static...
        public static IntPtr HandlePointerEvent(IntPtr hWnd, WM uMsg, UIntPtr wParam, IntPtr lParam)
        {
            if (hWnd == WindowComponent.HelperHWnd)
            {
                switch (uMsg)
                {
                    case WM.POINTERDEVICECHANGE:
                        {
                            Console.WriteLine("config change..");

                            return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                        }
                    case WM.POINTERDEVICEINRANGE:
                        {
                            EventQueue.Raise(null, PlatformEventType.PointerRange, new PointerRangeEventArgs(true));

                            return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                        }
                    case WM.POINTERDEVICEOUTOFRANGE:
                        {
                            EventQueue.Raise(null, PlatformEventType.PointerRange, new PointerRangeEventArgs(false));

                            return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                        }
                    default:
                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                }
            }
            
            switch (uMsg)
            {
                case WM.POINTERACTIVATE:
                    {
                        Console.WriteLine("pointer ativate!");
                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.POINTERENTER:
                case WM.POINTERLEAVE:
                    {
                        HWND hwnd = WindowComponent.HWndDict[hWnd];

                        // FIXME: Check what type of input device this is?

                        bool enter = uMsg == WM.POINTERENTER;

                        // FIXME: Position data? other data?
                        EventQueue.Raise(hwnd, 0, new PointerEnterEventArgs(hwnd, enter));

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.POINTERDOWN:
                case WM.POINTERUP:
                    {
                        HWND hwnd = WindowComponent.HWndDict[hWnd];

                        uint id = Win32.GET_POINTERID_WPARAM(wParam);

                        GetPenState(id, out PenState state);

                        bool down = uMsg == WM.POINTERDOWN;

                        EventQueue.Raise(hwnd, 0, new PointerDownEventArgs(hwnd, down, state));

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.POINTERUPDATE:
                    {
                        HWND hwnd = WindowComponent.HWndDict[hWnd];

                        uint id = Win32.GET_POINTERID_WPARAM(wParam);
                        Win32.GetPointerType(id, out POINTER_INPUT_TYPE type);

                        if (type == POINTER_INPUT_TYPE.PEN)
                        {
                            GetPenState(id, out PenState state);

                            EventQueue.Raise(hwnd, 0, new PointerUpdateEventArgs(hwnd, state));
                        }
                        else if (type == POINTER_INPUT_TYPE.TOUCH)
                        {
                            // FIXME: Touch input events
                        }
                        else if (type == POINTER_INPUT_TYPE.TOUCHPAD)
                        {
                            // FIXME: Touchpad input events?
                        }

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                default:
                    {
                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
            }
        }

    }
}
