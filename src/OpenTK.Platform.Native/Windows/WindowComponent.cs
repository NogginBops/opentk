﻿using OpenTK.Core.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace OpenTK.Platform.Native.Windows
{
    public partial class WindowComponent : IWindowComponent
    {
        /// <summary>
        /// Class name used to create windows.
        /// </summary>
        public const string CLASS_NAME = "OpenTK Window";

        /// <summary>
        /// The applications HInstance.
        /// </summary>
        public static readonly IntPtr HInstance;

        /// <summary>
        /// The helper window used to load wgl extensions.
        /// </summary>
        public static IntPtr HelperHWnd { get; private set; }

        private static Win32.WNDPROC WindowProc = Win32WindowProc;

        internal static readonly Dictionary<IntPtr, HWND> HWndDict = new Dictionary<IntPtr, HWND>();

        static WindowComponent()
        {
            // Get the hInstance of the current exe
            HInstance = Win32.GetModuleHandle(null);
        }

        /// <inheritdoc/>
        public string Name => "Win32Window";

        /// <inheritdoc/>
        public PalComponents Provides => PalComponents.Window;

        /// <inheritdoc/>
        public void Initialize(PalComponents which)
        {
            if (which != PalComponents.Window)
            {
                throw new Exception("WindowComponent can only initialize the Window component.");
            }

            // Here we register the window class that we will use for all created windows.
            Win32.WNDCLASSEX wndClass = Win32.WNDCLASSEX.Create();
            wndClass.lpfnWndProc = WindowProc;
            wndClass.hInstance = HInstance;
            wndClass.lpszClassName = CLASS_NAME;
            wndClass.style = ClassStyles.OwnDC;

            short wndClassAtom = Win32.RegisterClassEx(in wndClass);

            if (wndClassAtom == 0)
            {
                throw new Win32Exception("RegisterClassEx failed!");
            }

            HelperHWnd = Win32.CreateWindowEx(
                0,
                WindowComponent.CLASS_NAME,
                "OpenTK Helper Window",
                WindowStyles.WS_CLIPSIBLINGS | WindowStyles.WS_CLIPCHILDREN,
                Win32.CW_USEDEFAULT,
                Win32.CW_USEDEFAULT,
                Win32.CW_USEDEFAULT,
                Win32.CW_USEDEFAULT,
                IntPtr.Zero,
                IntPtr.Zero,
                HInstance,
                IntPtr.Zero);

            if (HelperHWnd == IntPtr.Zero)
            {
                throw new Win32Exception("Failed to create helper window");
            }

            // Eat all messages so that the WM_CREATE messages get processed etc.
            while (Win32.PeekMessage(out Win32.MSG lpMsg, HelperHWnd, 0, 0, PM.REMOVE) != 0)
            {
                Win32.TranslateMessage(in lpMsg);
                Win32.DispatchMessage(in lpMsg);
            }
        }

        /// <inheritdoc/>
        public bool CanSetIcon => true;

        /// <inheritdoc/>
        public bool CanGetDisplay => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool CanSetCursor => true;

        public IReadOnlyList<PlatformEventType> SupportedEvents => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<WindowStyle> SupportedStyles => _SupportedStyles;

        /// <inheritdoc/>
        public IReadOnlyList<WindowMode> SupportedModes => _SupportedModes;

        // FIXME: HACK!!!!!!
        private static bool quit = false;

        public void Loop(WindowHandle handle, Func<bool> callback)
        {
            HWND hwnd = handle.As<HWND>(this);

            while (true)
            {
                while (Win32.PeekMessage(out Win32.MSG lpMsg, IntPtr.Zero, 0, 0, PM.REMOVE) != 0)
                {
                    Win32.TranslateMessage(in lpMsg);
                    Win32.DispatchMessage(in lpMsg);
                }

                if (quit == true)
                {
                    break;
                }

                if (callback() == false)
                {
                    break;
                }
            }
        }

        public void SwapBuffers(WindowHandle handle)
        {
            HWND hwnd = handle.As<HWND>(this);

            IntPtr hDC = Win32.GetDC(hwnd.HWnd);

            bool success = Win32.SwapBuffers(hDC);

            if (success == false)
            {
                throw new Win32Exception();
                throw new Win32Exception("SwapBuffers failed");
            }
        }

        private static IntPtr Win32WindowProc(IntPtr hWnd, WM uMsg, UIntPtr wParam, IntPtr lParam)
        {
            //Console.WriteLine("WinProc " + message + " " + hWnd);
            switch (uMsg)
            {
                case WM.KEYDOWN:
                    {
                        // FIXME: Ability to "eat" key down presses?
                        HWND h = HWndDict[hWnd];

                        ulong vk = wParam.ToUInt64();
                        long l = lParam.ToInt64();
                        bool wasDown = (l & (1 << 30)) != 0;
                        bool extended = (l & (1 << 24)) != 0;

                        EventQueue.Raise(h, PlatformEventType.KeyDown, new KeyDownEventArgs(vk, wasDown, extended));

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.CHAR:
                    {
                        HWND h = HWndDict[hWnd];
                        if (Win32.IsWindowUnicode(hWnd))
                        {
                            // UTF-16
                            string str;

                            ulong chars = wParam.ToUInt64();
                            char hi = (char)(chars >> 16);
                            char lo = (char)(chars & 0xFFFF);
                            if (char.IsSurrogatePair(hi, lo))
                            {
                                Span<char> text = stackalloc char[2];
                                text[0] = hi;
                                text[1] = lo;
                                str = new string(text);
                            }
                            else
                            {
                                str = new string(lo, 1);
                            }

                            Console.WriteLine($"wParam: 0x{wParam.ToUInt64():X16}, lo: {(ushort)lo:X4}, hi: 0x{(ushort)hi:X4}");

                            if (char.IsSurrogate(lo))
                            {
                                ;
                            }

                            EventQueue.Raise(h, PlatformEventType.TextInput, new TextInputEventArgs(str));
                        }
                        else
                        {
                            // ANSI
                            EventQueue.Raise(h, PlatformEventType.TextInput, new TextInputEventArgs(new string((char)(wParam.ToUInt64()), 1)));
                        }

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.SETFOCUS:
                    {
                        HWND h = HWndDict[hWnd];
                        //EventQueue.Raise(h, WindowEventType.GotFocus, null);
                        Console.WriteLine("Got focus");
                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.KILLFOCUS:
                    {
                        // This message can be sent after WM_CLOSE which means that the specificed window might not exist any more.
                        if (HWndDict.TryGetValue(hWnd, out HWND? h))
                        {
                            //EventQueue.Raise(h, WindowEventType.LostFocus, null);
                            Console.WriteLine("Lost focus");
                        }

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.SETCURSOR:
                    {
                        int ht = ((int)lParam) & Win32.LoWordMask;
                        if (ht == Win32.HTCLIENT)
                        {
                            HWND h = HWndDict[hWnd];
                            // FIXME: Figure out what to do if h.HCursor is null
                            Win32.SetCursor(h.HCursor?.Cursor ?? IntPtr.Zero);
                            return new IntPtr(1);
                        }
                        else
                        {
                            return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                        }
                    }
                case WM.MOUSEMOVE:
                    {
                        int x = Win32.GET_X_LPARAM(lParam);
                        int y = Win32.GET_Y_LPARAM(lParam);

                        HWND h = HWndDict[hWnd];

                        if (h.TrackingMouse == false)
                        {
                            Win32.TRACKMOUSEEVENT tme = default;
                            tme.cbSize = (uint)Marshal.SizeOf<Win32.TRACKMOUSEEVENT>();
                            tme.dwFlags = TME.Leave;
                            tme.hwndTrack = hWnd;
                            Win32.TrackMouseEvent(ref tme);

                            h.TrackingMouse = true;

                            EventQueue.Raise(h, PlatformEventType.MouseEnter, new MouseEnterEventArgs(true));
                        }

                        EventQueue.Raise(h, PlatformEventType.MouseMove, new MouseMoveEventArgs(x, y));

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.MOUSELEAVE:
                    {
                        HWND h = HWndDict[hWnd];
                        h.TrackingMouse = false;

                        EventQueue.Raise(h, PlatformEventType.MouseEnter, new MouseEnterEventArgs(false));

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.LBUTTONDOWN:
                case WM.MBUTTONDOWN:
                case WM.RBUTTONDOWN:
                case WM.XBUTTONDOWN:
                    {
                        MouseButton? button;
                        switch (uMsg)
                        {
                            case WM.LBUTTONDOWN:
                                button = MouseButton.Button1;
                                break;
                            case WM.MBUTTONDOWN:
                                button = MouseButton.Button3;
                                break;
                            case WM.RBUTTONDOWN:
                                button = MouseButton.Button2;
                                break;
                            case WM.XBUTTONDOWN:
                                {
                                    const int XBUTTON1 = 0x0001;
                                    const int XBUTTON2 = 0x0002;
                                    int hiWord = ((int)wParam.ToUInt32() & Win32.HiWordMask) >> 16;

                                    if (hiWord == XBUTTON1)
                                    {
                                        button = MouseButton.Button4;
                                    }
                                    else if (hiWord == XBUTTON2)
                                    {
                                        button = MouseButton.Button5;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Unknown xbutton: {(uint)hiWord}");
                                        button = null;
                                    }

                                    break;
                                }
                            default:
                                throw new InvalidEnumArgumentException(nameof(uMsg), (int)uMsg, typeof(WM));
                        }

                        if (button != null)
                        {
                            HWND h = HWndDict[hWnd];
                            EventQueue.Raise(h, PlatformEventType.MouseDown, new MouseButtonDownEventArgs(button.Value));
                        }

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.LBUTTONUP:
                case WM.MBUTTONUP:
                case WM.RBUTTONUP:
                case WM.XBUTTONUP:
                    {
                        MouseButton? button;
                        switch (uMsg)
                        {
                            case WM.LBUTTONUP:
                                button = MouseButton.Button1;
                                break;
                            case WM.MBUTTONUP:
                                button = MouseButton.Button3;
                                break;
                            case WM.RBUTTONUP:
                                button = MouseButton.Button2;
                                break;
                            case WM.XBUTTONUP:
                                {
                                    const int XBUTTON1 = 0x0001;
                                    const int XBUTTON2 = 0x0002;
                                    int hiWord = ((int)wParam.ToUInt32() & Win32.HiWordMask) >> 16;

                                    if (hiWord == XBUTTON1)
                                    {
                                        button = MouseButton.Button4;
                                    }
                                    else if (hiWord == XBUTTON2)
                                    {
                                        button = MouseButton.Button5;
                                    }
                                    else
                                    {
                                        Console.WriteLine($"Unknown xbutton: {(uint)hiWord}");
                                        button = null;
                                    }

                                    break;
                                }
                            default:
                                throw new InvalidEnumArgumentException(nameof(uMsg), (int)uMsg, typeof(WM));
                        }

                        if (button != null)
                        {
                            HWND h = HWndDict[hWnd];
                            EventQueue.Raise(h, PlatformEventType.MouseUp, new MouseButtonUpEventArgs(button.Value));
                        }

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.SIZE:
                    {
                        SIZE size = (SIZE)wParam.ToUInt32();

                        HWND h = HWndDict[hWnd];

                        switch (size)
                        {
                            case SIZE.Maximized:
                                h.WindowState = WindowState.Maximized;
                                Console.WriteLine($"Move: {size}");
                                //EventQueue.Raise(h, WindowEventType.Maximized, null);
                                break;
                            case SIZE.Minimized:
                                h.WindowState = WindowState.Minimized;
                                Console.WriteLine($"Move: {size}");
                                //EventQueue.Raise(h, WindowEventType.Minimized, null);
                                break;
                            case SIZE.Restored:
                                if (h.WindowState != WindowState.Restored)
                                {
                                    h.WindowState = WindowState.Restored;
                                    Console.WriteLine($"Move: {size}");
                                    //EventQueue.Raise(h, WindowEventType.Restored, null);
                                }
                                break;
                            case SIZE.MaxShow:
                            case SIZE.MaxHide:
                            default:
                                // Igonre these for now!
                                break;
                        }

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.CLOSE:
                    {
                        // FIXME: Get the result back from the user here!
                        // Or delay the closing of the window until the user has handled the message.
                        // This could hang the window if the user code is stuck.
                        HWND h = HWndDict[hWnd];
                        EventQueue.Raise(h, PlatformEventType.Close, new CloseEventArgs(h));

                        // FIXME: HACK! This is not the greatest way to get the WindowComponent...
                        h.WindowComponent.Destroy(h);

                        return IntPtr.Zero;
                    }
                case WM.DESTROY:
                    {
                        if (HWndDict.Count == 0)
                        {
                            Win32.PostQuitMessage(0);
                            quit = true;
                        }
                        return IntPtr.Zero;
                    }
                case WM.DEVICECHANGE:
                    {
                        Console.WriteLine($"{uMsg} {(DBT)wParam}");
                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.DISPLAYCHANGE:
                    {
                        Console.WriteLine($"{uMsg} Bit depth: {wParam.ToUInt64()}, ResX: {(lParam.ToInt64() & Win32.HiWordMask) >> 16}, ResY: {lParam.ToInt64() & Win32.LoWordMask}");

                        // FIXME: Some other way of notifying the DisplayComponent that things have changed.
                        DisplayComponent.UpdateMonitors();
                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.DPICHANGED:
                    {
                        Console.WriteLine($"DPI Changed! dpiY: {(wParam.ToUInt32() & Win32.HiWordMask) >> 16}, dpiX: {wParam.ToUInt32() & Win32.LoWordMask}");

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                case WM.INPUT:
                    {
                        //Console.WriteLine("WM_INPUT");

                        uint size = 0;
                        unsafe
                        {
                            uint ret;
                            ret = Win32.GetRawInputData(lParam, RID.Header, null, ref size, (uint)sizeof(Win32.RAWINPUTHEADER));
                            if (ret != 0)
                            {
                                throw new Win32Exception();
                            }

                            byte[] bytes = new byte[size];
                            uint oldSize = size;
                            ret = Win32.GetRawInputData(lParam, RID.Header, bytes, ref size, (uint)sizeof(Win32.RAWINPUTHEADER));
                            if (ret == unchecked((uint)-1))
                            {
                                throw new Win32Exception();
                            }

                            ref Win32.RAWINPUTHEADER raw = ref MemoryMarshal.Cast<byte, Win32.RAWINPUTHEADER>(new Span<byte>(bytes))[0];

                            // FIXME!! We can't call GetDeviceNameFromHandle for some reason.
                            // The handle is not valid when disconnecting? But it is valid enough above??
                            // To workaround this we just look the device up in the dictionary and get the cached name.
                            string hidName = JoystickComponent.DeviceDict[raw.hDevice].PublicName;

                            size = 0;
                            ret = Win32.GetRawInputDeviceInfo(raw.hDevice, RIDI.PreParsedData, (Span<byte>)null, ref size);
                            if (ret == unchecked((uint)-1))
                            {
                                throw new Win32Exception();
                            }

                            byte[] preparsedData = new byte[size];

                            ret = Win32.GetRawInputDeviceInfo(raw.hDevice, RIDI.PreParsedData, (Span<byte>)preparsedData, ref size);
                            if (ret == unchecked((uint)-1))
                            {
                                throw new Win32Exception();
                            }

                            HIDPStatus status = Win32.HidP_GetCaps(preparsedData, out Win32.HIDP_CAPS caps);
                            if (status != HIDPStatus.HIDP_STATUS_SUCCESS)
                            {
                                throw new PlatformException($"HidP_GetCaps failed with: {status}");
                            }

                            Console.WriteLine($"{hidName} Header: RIM:{raw.dwType}, size:{raw.dwSize}, hDevice: {raw.hDevice}, wParam: {raw.wParam}");

                            Console.WriteLine($"Input report: {caps.InputReportByteLength}, Output report: {caps.OutputReportByteLength}, Feature report: {caps.FeatureReportByteLength} Link Collection nodes: {caps.NumberLinkCollectionNodes}");
                            Console.WriteLine($"Input Buttons: {caps.NumberInputButtonCaps}, Value Caps: {caps.NumberInputValueCaps}, Indices: {caps.NumberInputDataIndices}");
                            Console.WriteLine($"Output Buttons: {caps.NumberOutputButtonCaps}, Value Caps: {caps.NumberOutputValueCaps}, Indices: {caps.NumberOutputDataIndices}");
                            Console.WriteLine($"Feature Buttons: {caps.NumberFeatureButtonCaps}, Value Caps: {caps.NumberFeatureValueCaps}, Indices: {caps.NumberFeatureDataIndices}");

                            Win32.HIDP_VALUE_CAPS[] valueCaps = new Win32.HIDP_VALUE_CAPS[caps.NumberInputValueCaps];

                            status = Win32.HidP_GetValueCaps(HIDPReportType.Input, valueCaps, ref caps.NumberInputValueCaps, preparsedData);
                            if (status != HIDPStatus.HIDP_STATUS_SUCCESS)
                            {
                                throw new PlatformException($"HidP_GetValueCaps failed with: {status}");
                            }

                            for (int i = 0; i < caps.NumberInputValueCaps; i++)
                            {
                                Console.WriteLine($"Value cap {i}:");
                                Console.WriteLine(ValueCapsString(valueCaps[i]));
                                Console.WriteLine();

                                static string ValueCapsString(Win32.HIDP_VALUE_CAPS caps)
                                {
                                    StringBuilder sb = new StringBuilder();

                                    sb.AppendLine($"ReportID: {caps.ReportID}, IsAlias: {caps.IsAlias}, BitField: {caps.BitField}");
                                    sb.AppendLine($"LinkCollection: {caps.LinkCollection}, LinkUsage: {caps.LinkUsage}, LinkUsagePage: {caps.LinkUsagePage}");
                                    sb.AppendLine($"IsRange: {caps.IsRange}, IsStringRange: {caps.IsStringRange}, IsDesignatorRange: {caps.IsDesignatorRange}, IsAbsolute: {caps.IsAbsolute}");
                                    sb.AppendLine($"HasNull: {caps.HasNull}, BitSize: {caps.BitSize}, ReportCount: {caps.ReportCount}");
                                    sb.AppendLine($"UnitsExp: {caps.UnitsExp}, Units: {caps.Units}");
                                    sb.AppendLine($"LogicalMin: {caps.LogicalMin}, LogicalMin: {caps.LogicalMax}, PhysicalMin: {caps.PhysicalMin}, PhysicalMax: {caps.LogicalMax}");

                                    return sb.ToString();
                                }
                            }

                            ref Win32.RAWHID hid = ref MemoryMarshal.Cast<byte, Win32.RAWHID>(new Span<byte>(bytes))[0];

                            fixed (byte* rawDataPtr = hid.bRawData)
                            {
                                Span<byte> hidData = new Span<byte>(rawDataPtr, (int)(hid.dwSizeHid * hid.dwCount));

                                /*foreach (var b in hidData)
                                {
                                    Console.Write($"{b:X2}");
                                }
                                Console.WriteLine();*/
                            }
                        }

                        // MSDN told us to call this function.
                        // https://docs.microsoft.com/en-us/windows/win32/inputdev/wm-input
                        IntPtr result = Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                        return IntPtr.Zero;
                    }
                case WM.INPUT_DEVICE_CHANGE:
                    {
                        GIDC gidc = (GIDC)wParam.ToUInt64();

                        Console.WriteLine("WM_INPUT_DEVICE_CHANGE");

                        bool connected = gidc switch
                        {
                            GIDC.Arrival => true,
                            GIDC.Removal => false,
                            _ => throw new Exception($"Unknown GIDC value '{gidc}'"),
                        };

                        JoystickComponent.JoysticksChanged(connected, lParam);

                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
                default:
                    {
                        //Console.WriteLine(uMsg);
                        return Win32.DefWindowProc(hWnd, uMsg, wParam, lParam);
                    }
            }
        }

        public WindowHandle Create(GraphicsApiHints hints)
        {
            IntPtr hWnd = Win32.CreateWindowEx(
                0,
                CLASS_NAME,
                "OpenTK Window",
                WindowStyles.WS_OVERLAPPEDWINDOW,
                Win32.CW_USEDEFAULT,
                Win32.CW_USEDEFAULT,
                Win32.CW_USEDEFAULT,
                Win32.CW_USEDEFAULT,
                IntPtr.Zero,
                IntPtr.Zero,
                HInstance,
                IntPtr.Zero);

            if (hWnd == IntPtr.Zero)
            {
                throw new Win32Exception("CreateWindowEx failed!");
            }

            HWND hwnd = new HWND(hWnd, this, hints);

            HWndDict.Add(hwnd.HWnd, hwnd);

            return hwnd;
        }

        /// <inheritdoc/>
        public void Destroy(WindowHandle handle)
        {
            HWND hwnd = handle.As<HWND>(this);

            HWndDict.Remove(hwnd.HWnd);

            bool success = Win32.DestroyWindow(hwnd.HWnd);

            // FIXME: Do we add back the hglrc to HGLRCDict?
            if (success == false)
            {
                throw new Win32Exception("DestroyWindow failed!");
            }
        }

        /// <inheritdoc/>
        public string GetTitle(WindowHandle handle)
        {
            HWND hwnd = handle.As<HWND>(this);

            int textLength = Win32.GetWindowTextLength(hwnd.HWnd);

            int error = Marshal.GetLastWin32Error();
            if (textLength == 0 && error != 0)
            {
                throw new Win32Exception(error, "GetWindowTextLength: Could not get length of window text");
            }

            StringBuilder title = new StringBuilder(textLength + 1);

            Win32.GetWindowText(hwnd.HWnd, title, title.Capacity);

            return title.ToString();
        }

        /// <inheritdoc/>
        public void SetTitle(WindowHandle handle, string title)
        {
            HWND hwnd = handle.As<HWND>(this);

            bool success = Win32.SetWindowText(hwnd.HWnd, title);

            if (success == false)
            {
                throw new Win32Exception("Could not set window title");
            }
        }

        /// <inheritdoc/>
        public IconHandle GetIcon(WindowHandle handle)
        {
            HWND hwnd = handle.As<HWND>(this);

            // FIXME: If the user has changed the icon outside of our API this will not return the correct results
            // We could make a custom response to WM.SETICON that only returns the icons set with our API.
            // But ideally we would avoid this.
            if (hwnd.HIcon != null)
            {
                return hwnd.HIcon;
            }
            else
            {
                // FIXME: Return a (new?) iconHandle for the window default icon.
                throw new NotImplementedException();
            }
        }

        /// <inheritdoc/>
        public void SetIcon(WindowHandle handle, IconHandle icon)
        {
            HWND hwnd = handle.As<HWND>(this);
            HIcon hicon = icon.As<HIcon>(this);

            hwnd.HIcon = hicon;

            // Send messages to set the icon.
            // DefWinProc returns the last sent lParam so we don't need to handle WM.SETICON as it already does what we want.
            Win32.SendMessage(hwnd.HWnd, WM.SETICON, new UIntPtr(Win32.ICON_SMALL), hicon.Icon);
            Win32.SendMessage(hwnd.HWnd, WM.SETICON, new UIntPtr(Win32.ICON_BIG), hicon.Icon);
        }

        /// <inheritdoc/>
        public void GetPosition(WindowHandle handle, out int x, out int y)
        {
            HWND hwnd = handle.As<HWND>(this);

            bool success = Win32.GetWindowRect(hwnd.HWnd, out Win32.RECT lpRect);

            if (success == false)
            {
                throw new Win32Exception("GetWindowRect failed");
            }

            x = lpRect.left;
            y = lpRect.top;
        }

        /// <inheritdoc/>
        public void SetPosition(WindowHandle handle, int x, int y)
        {
            HWND hwnd = handle.As<HWND>(this);

            // FIXME: What do we want to do here with SetWindowPosFlags.NoActivate??
            // FIXME: Constant for this SetWindowPosFlags combo?
            bool success = Win32.SetWindowPos(hwnd.HWnd, IntPtr.Zero, x, y, 0, 0, SetWindowPosFlags.NoSize | SetWindowPosFlags.NoActivate | SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoOwnerZOrder);

            if (success == false)
            {
                throw new Win32Exception("Could not set window position");
            }
        }

        /// <inheritdoc/>
        public void GetSize(WindowHandle handle, out int width, out int height)
        {
            HWND hwnd = handle.As<HWND>(this);

            bool success = Win32.GetWindowRect(hwnd.HWnd, out Win32.RECT lpRect);

            if (success == false)
            {
                throw new Win32Exception("GetWindowRect failed");
            }

            width = lpRect.right - lpRect.left;
            height = lpRect.bottom - lpRect.top;
        }

        /// <inheritdoc/>
        public void SetSize(WindowHandle handle, int width, int height)
        {
            HWND hwnd = handle.As<HWND>(this);

            // FIXME: What do we want to do here with SetWindowPosFlags.NoActivate??
            // FIXME: Constant for this SetWindowPosFlags combo?
            bool success = Win32.SetWindowPos(hwnd.HWnd, IntPtr.Zero, 0, 0, width, height, SetWindowPosFlags.NoMove | SetWindowPosFlags.NoActivate | SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoOwnerZOrder);

            if (success == false)
            {
                throw new Win32Exception("Could not set window size");
            }
        }

        /// <inheritdoc/>
        public void GetClientPosition(WindowHandle handle, out int x, out int y)
        {
            HWND hwnd = handle.As<HWND>(this);

            Win32.POINT point = new Win32.POINT(0, 0);

            bool success = Win32.ClientToScreen(hwnd.HWnd, ref point);

            if (success == false)
            {
                throw new Win32Exception("ClientToScreen failed");
            }

            x = point.X;
            y = point.Y;
        }

        /// <inheritdoc/>
        public void SetClientPosition(WindowHandle handle, int x, int y)
        {
            HWND hwnd = handle.As<HWND>(this);

            Win32.RECT rect = new Win32.RECT(x, y, 0, 0);
            WindowStyles currentStyle = (WindowStyles)Win32.GetWindowLongPtr(hwnd.HWnd, GetGWLPIndex.Style).ToInt64();
            // This doesn't consider potential scroll bars, so if that is ever a thing we would need to add code for that here!
            // FIXME: Menu
            bool success = Win32.AdjustWindowRect(ref rect, currentStyle, false);

            if (success == false)
            {
                throw new Win32Exception("AdjustWindowRect failed");
            }

            // FIXME: What do we want to do here with SetWindowPosFlags.NoActivate??
            // FIXME: Constant for this SetWindowPosFlags combo?
            success = Win32.SetWindowPos(hwnd.HWnd, IntPtr.Zero, rect.left, rect.top, 0, 0, SetWindowPosFlags.NoSize | SetWindowPosFlags.NoActivate | SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoOwnerZOrder);

            if (success == false)
            {
                throw new Win32Exception("Could not set window position");
            }
        }

        /// <inheritdoc/>
        public void GetClientSize(WindowHandle handle, out int width, out int height)
        {
            HWND hwnd = handle.As<HWND>(this);

            bool success = Win32.GetClientRect(hwnd.HWnd, out Win32.RECT lpRect);

            if (success == false)
            {
                throw new Win32Exception("GetClientRect failed");
            }

            width = lpRect.right;
            height = lpRect.bottom;
        }

        /// <inheritdoc/>
        public void SetClientSize(WindowHandle handle, int width, int height)
        {
            HWND hwnd = handle.As<HWND>(this);

            Win32.RECT rect = new Win32.RECT(0, 0, width, height);

            WindowStyles currentStyle = (WindowStyles)Win32.GetWindowLongPtr(hwnd.HWnd, GetGWLPIndex.Style).ToInt64();
            // This doesn't consider potential scroll bars, so if that is ever a thing we would need to add code for that here!
            // FIXME: Menu
            bool success = Win32.AdjustWindowRect(ref rect, currentStyle, false);

            if (success == false)
            {
                throw new Win32Exception("AdjustWindowRect failed");
            }

            // FIXME: What do we want to do here with SetWindowPosFlags.NoActivate??
            // FIXME: Constant for this SetWindowPosFlags combo?
            success = Win32.SetWindowPos(hwnd.HWnd, IntPtr.Zero, 0, 0, rect.Width, rect.Height, SetWindowPosFlags.NoMove | SetWindowPosFlags.NoActivate | SetWindowPosFlags.NoZOrder | SetWindowPosFlags.NoOwnerZOrder);

            if (success == false)
            {
                throw new Win32Exception("SetWindowPos failed");
            }
        }

        /// <inheritdoc/>
        public DisplayHandle GetDisplay(WindowHandle handle)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public WindowMode GetMode(WindowHandle handle)
        {
            HWND hwnd = handle.As<HWND>(this);

            // We need ToInt64 here as Style is a uint which means that 0x00000000_ffffffff could be returned,
            // and ToInt32 is going to throw in that case
            WindowStyles style = (WindowStyles)Win32.GetWindowLongPtr(hwnd.HWnd, GetGWLPIndex.Style).ToInt64();

            if (style.HasFlag(WindowStyles.WS_VISIBLE) == false)
            {
                return WindowMode.Hidden;
            }
            else
            {
                if (style.HasFlag(WindowStyles.WS_MINIMIZE))
                {
                    return WindowMode.Minimized;
                }
                else if (style.HasFlag(WindowStyles.WS_MAXIMIZE))
                {
                    return WindowMode.Maximized;
                }
                else
                {
                    // FIXME: Figure out how to return the remaining window modes!
                    return WindowMode.Normal;
                }
            }
        }

        private static readonly WindowMode[] _SupportedModes = new[]
        {
            WindowMode.Hidden,
            WindowMode.Minimized,
            WindowMode.Normal,
            WindowMode.Maximized
        };

        /// <inheritdoc/>
        public void SetMode(WindowHandle handle, WindowMode mode)
        {
            HWND hwnd = handle.As<HWND>(this);

            // FIXME: Handle ShowWindowCommands.ShowMinimized?
            switch (mode)
            {
                case WindowMode.Hidden:
                    Win32.ShowWindow(hwnd.HWnd, ShowWindowCommands.Hide);
                    break;
                case WindowMode.Minimized:
                    Win32.ShowWindow(hwnd.HWnd, ShowWindowCommands.Minimize);
                    break;
                case WindowMode.Normal:
                    // FIXME: Use ShowWindowCommands.Show?
                    Win32.ShowWindow(hwnd.HWnd, ShowWindowCommands.Normal);
                    break;
                case WindowMode.Maximized:
                    Win32.ShowWindow(hwnd.HWnd, ShowWindowCommands.Maximize);
                    break;
                case WindowMode.WindowedFullscreen:
                    throw new NotImplementedException("Windowed fullscreen is not implemented yet");
                case WindowMode.ExclusiveFullscreen:
                    throw new NotImplementedException("Exclusive fullscreen is not implemented yet");
                default:
                    throw new InvalidEnumArgumentException(nameof(mode), (int)mode, mode.GetType());
            }
        }

        /// <inheritdoc/>
        public WindowStyle GetBorderStyle(WindowHandle handle)
        {
            HWND hwnd = handle.As<HWND>(this);

            // We need ToInt64 here as Style is a uint which means that 0x00000000_ffffffff could be returned,
            // and ToInt32 is going to throw in that case
            WindowStyles style = (WindowStyles)Win32.GetWindowLongPtr(hwnd.HWnd, GetGWLPIndex.Style).ToInt64();

            // FIXME: Better way of checking these!!
            if (style.HasFlag(WindowStyles.WS_OVERLAPPEDWINDOW & ~WindowStyles.WS_THICKFRAME) &&
                style.HasFlag(WindowStyles.WS_THICKFRAME) == false)
            {
                return WindowStyle.FixedBorder;
            }
            else if (style.HasFlag(WindowStyles.WS_OVERLAPPEDWINDOW & ~(WindowStyles.WS_CAPTION | WindowStyles.WS_BORDER | WindowStyles.WS_THICKFRAME)) &&
                     style.HasFlag(WindowStyles.WS_CAPTION) == false &&
                     style.HasFlag(WindowStyles.WS_BORDER) == false &&
                     style.HasFlag(WindowStyles.WS_THICKFRAME) == false)
            {
                return WindowStyle.Borderless;
            }
            else if (style.HasFlag(WindowStyles.WS_OVERLAPPEDWINDOW))
            {
                return WindowStyle.ResizableBorder;
            }
            else
            {
                throw new NotImplementedException($"Unknown window style combination! (or toolbox?) (Style: {style})");
            }
        }

        private static readonly WindowStyle[] _SupportedStyles = new[]
        {
            WindowStyle.Borderless,
            WindowStyle.FixedBorder,
            WindowStyle.ResizableBorder,
        };

        /// <inheritdoc/>
        public unsafe void SetBorderStyle(WindowHandle handle, WindowStyle style)
        {
            HWND hwnd = handle.As<HWND>(this);

            // We need ToInt64 here as Style is a uint which means that 0x00000000_ffffffff could be returned,
            // and ToInt32 is going to throw in that case
            WindowStyles windowStyle = (WindowStyles)Win32.GetWindowLongPtr(hwnd.HWnd, GetGWLPIndex.Style).ToInt64();

            // FIXME: If we are going from borderless to the other styles we need to set these flags again
            // This feels like it needs quite a lot of precision and is fragile, maybe there is another way?
            // Having every window style have a specific flag combination?
            switch (style)
            {
                case WindowStyle.Borderless:
                    // FIXME: Get rid of the wait cursor.
                    windowStyle &= ~WindowStyles.WS_CAPTION;
                    windowStyle &= ~WindowStyles.WS_BORDER;
                    windowStyle &= ~WindowStyles.WS_THICKFRAME;
                    Win32.SetWindowLongPtr(hwnd.HWnd, SetGWLPIndex.Style, new IntPtr((uint)windowStyle));
                    break;
                case WindowStyle.FixedBorder:
                    windowStyle |= WindowStyles.WS_OVERLAPPEDWINDOW;
                    windowStyle &= ~WindowStyles.WS_THICKFRAME;
                    Win32.SetWindowLongPtr(hwnd.HWnd, SetGWLPIndex.Style, new IntPtr((uint)windowStyle));
                    break;
                case WindowStyle.ResizableBorder:
                    windowStyle |= WindowStyles.WS_OVERLAPPEDWINDOW;
                    Win32.SetWindowLongPtr(hwnd.HWnd, SetGWLPIndex.Style, new IntPtr((uint)windowStyle));
                    break;
                case WindowStyle.ToolBox:
                    throw new NotImplementedException();
                default:
                    break;
            }

            Win32.SetWindowPos(hwnd.HWnd, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.NoMove | SetWindowPosFlags.NoSize | SetWindowPosFlags.NoZOrder | SetWindowPosFlags.FrameChanged);
        }

        /// <inheritdoc/>
        public void SetCursor(WindowHandle handle, CursorHandle cursor)
        {
            HWND hwnd = handle.As<HWND>(this);
            HCursor? hcursor = cursor?.As<HCursor>(this);

            hwnd.HCursor = hcursor;

            // A hCursor = null means a hidden cursor, as we want.
            Win32.SetCursor(hcursor?.Cursor ?? IntPtr.Zero);
        }

        /// <inheritdoc/>
        public void ScreenToClient(WindowHandle handle, int x, int y, out int clientX, out int clientY)
        {
            HWND hwnd = handle.As<HWND>(this);

            Win32.POINT point;
            point.X = x;
            point.Y = y;
            bool success = Win32.ScreenToClient(hwnd.HWnd, ref point);
            if (success == false)
            {
                throw new Win32Exception("ScreenToClient failed.");
            }

            clientX = point.X;
            clientY = point.Y;
        }

        /// <inheritdoc/>
        public void ClientToScreen(WindowHandle handle, int clientX, int clientY, out int x, out int y)
        {
            HWND hwnd = handle.As<HWND>(this);

            Win32.POINT point;
            point.X = clientX;
            point.Y = clientY;
            bool success = Win32.ClientToScreen(hwnd.HWnd, ref point);
            if (success == false)
            {
                throw new Win32Exception("ClientToScreen failed.");
            }

            x = point.X;
            y = point.Y;
        }

        /// <inheritdoc/>
        public IEventQueue<PlatformEventType, WindowEventArgs> GetEventQueue(WindowHandle handle)
        {
            HWND hwnd = handle.As<HWND>(this);
            return null;
        }
    }
}
