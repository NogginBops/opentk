﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpenTK.Platform.Native.Windows
{
    // FIXME: Make enum names consistent between all enums.

    /// <summary>
    /// Window Styles.
    /// The following styles can be specified wherever a window style is required. After the control has been created, these styles cannot be modified, except as noted.
    /// </summary>
    [Flags]
    internal enum WindowStyles : uint
    {
        /// <summary>The window has a thin-line border.</summary>
        WS_BORDER = 0x800000,

        /// <summary>The window has a title bar (includes the WS_BORDER style).</summary>
        WS_CAPTION = 0xc00000,

        /// <summary>The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the WS_POPUP style.</summary>
        WS_CHILD = 0x40000000,

        /// <summary>Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.</summary>
        WS_CLIPCHILDREN = 0x2000000,

        /// <summary>
        /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the WS_CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
        /// If WS_CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
        /// </summary>
        WS_CLIPSIBLINGS = 0x4000000,

        /// <summary>The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.</summary>
        WS_DISABLED = 0x8000000,

        /// <summary>The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.</summary>
        WS_DLGFRAME = 0x400000,

        /// <summary>
        /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the WS_GROUP style.
        /// The first control in each group usually has the WS_TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
        /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
        /// </summary>
        WS_GROUP = 0x20000,

        /// <summary>The window has a horizontal scroll bar.</summary>
        WS_HSCROLL = 0x100000,

        /// <summary>The window is initially maximized.</summary>
        WS_MAXIMIZE = 0x1000000,

        /// <summary>The window has a maximize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
        WS_MAXIMIZEBOX = 0x10000,

        /// <summary>The window is initially minimized.</summary>
        WS_MINIMIZE = 0x20000000,

        /// <summary>The window has a minimize button. Cannot be combined with the WS_EX_CONTEXTHELP style. The WS_SYSMENU style must also be specified.</summary>
        WS_MINIMIZEBOX = 0x20000,

        /// <summary>The window is an overlapped window. An overlapped window has a title bar and a border.</summary>
        WS_OVERLAPPED = 0x0,

        /// <summary>The window is an overlapped window.</summary>
        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,

        /// <summary>The window is a pop-up window. This style cannot be used with the WS_CHILD style.</summary>
        WS_POPUP = 0x80000000u,

        /// <summary>The window is a pop-up window. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.</summary>
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,

        /// <summary>The window has a sizing border.</summary>
        WS_THICKFRAME = 0x40000,

        /// <summary>The window has a window menu on its title bar. The WS_CAPTION style must also be specified.</summary>
        WS_SYSMENU = 0x80000,

        /// <summary>
        /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
        /// Pressing the TAB key changes the keyboard focus to the next control with the WS_TABSTOP style.
        /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
        /// For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
        /// </summary>
        WS_TABSTOP = 0x10000,

        /// <summary>The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.</summary>
        WS_VISIBLE = 0x10000000,

        /// <summary>The window has a vertical scroll bar.</summary>
        WS_VSCROLL = 0x200000
    }

    [Flags]
    internal enum WindowStylesEx : uint
    {
        /// <summary>Specifies a window that accepts drag-drop files.</summary>
        WS_EX_ACCEPTFILES = 0x00000010,

        /// <summary>Forces a top-level window onto the taskbar when the window is visible.</summary>
        WS_EX_APPWINDOW = 0x00040000,

        /// <summary>Specifies a window that has a border with a sunken edge.</summary>
        WS_EX_CLIENTEDGE = 0x00000200,

        /// <summary>
        /// Specifies a window that paints all descendants in bottom-to-top painting order using double-buffering.
        /// This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC. This style is not supported in Windows 2000.
        /// </summary>
        /// <remarks>
        /// With WS_EX_COMPOSITED set, all descendants of a window get bottom-to-top painting order using double-buffering.
        /// Bottom-to-top painting order allows a descendent window to have translucency (alpha) and transparency (color-key) effects,
        /// but only if the descendent window also has the WS_EX_TRANSPARENT bit set.
        /// Double-buffering allows the window and its descendents to be painted without flicker.
        /// </remarks>
        WS_EX_COMPOSITED = 0x02000000,

        /// <summary>
        /// Specifies a window that includes a question mark in the title bar. When the user clicks the question mark,
        /// the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message.
        /// The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command.
        /// The Help application displays a pop-up window that typically contains help for the child window.
        /// WS_EX_CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
        /// </summary>
        WS_EX_CONTEXTHELP = 0x00000400,

        /// <summary>
        /// Specifies a window which contains child windows that should take part in dialog box navigation.
        /// If this style is specified, the dialog manager recurses into children of this window when performing navigation operations
        /// such as handling the TAB key, an arrow key, or a keyboard mnemonic.
        /// </summary>
        WS_EX_CONTROLPARENT = 0x00010000,

        /// <summary>Specifies a window that has a double border.</summary>
        WS_EX_DLGMODALFRAME = 0x00000001,

        /// <summary>
        /// Specifies a window that is a layered window.
        /// This cannot be used for child windows or if the window has a class style of either CS_OWNDC or CS_CLASSDC.
        /// </summary>
        WS_EX_LAYERED = 0x00080000,

        /// <summary>
        /// Specifies a window with the horizontal origin on the right edge. Increasing horizontal values advance to the left.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        WS_EX_LAYOUTRTL = 0x00400000,

        /// <summary>Specifies a window that has generic left-aligned properties. This is the default.</summary>
        WS_EX_LEFT = 0x00000000,

        /// <summary>
        /// Specifies a window with the vertical scroll bar (if present) to the left of the client area.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        WS_EX_LEFTSCROLLBAR = 0x00004000,

        /// <summary>
        /// Specifies a window that displays text using left-to-right reading-order properties. This is the default.
        /// </summary>
        WS_EX_LTRREADING = 0x00000000,

        /// <summary>
        /// Specifies a multiple-document interface (MDI) child window.
        /// </summary>
        WS_EX_MDICHILD = 0x00000040,

        /// <summary>
        /// Specifies a top-level window created with this style does not become the foreground window when the user clicks it.
        /// The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
        /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the WS_EX_APPWINDOW style.
        /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
        /// </summary>
        WS_EX_NOACTIVATE = 0x08000000,

        /// <summary>
        /// Specifies a window which does not pass its window layout to its child windows.
        /// </summary>
        WS_EX_NOINHERITLAYOUT = 0x00100000,

        /// <summary>
        /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
        /// </summary>
        WS_EX_NOPARENTNOTIFY = 0x00000004,

        /// <summary>
        /// The window does not render to a redirection surface.
        /// This is for windows that do not have visible content or that use mechanisms other than surfaces to provide their visual.
        /// </summary>
        WS_EX_NOREDIRECTIONBITMAP = 0x00200000,

        /// <summary>Specifies an overlapped window.</summary>
        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,

        /// <summary>Specifies a palette window, which is a modeless dialog box that presents an array of commands.</summary>
        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,

        /// <summary>
        /// Specifies a window that has generic "right-aligned" properties. This depends on the window class.
        /// The shell language must support reading-order alignment for this to take effect.
        /// Using the WS_EX_RIGHT style has the same effect as using the SS_RIGHT (static), ES_RIGHT (edit), and BS_RIGHT/BS_RIGHTBUTTON (button) control styles.
        /// </summary>
        WS_EX_RIGHT = 0x00001000,

        /// <summary>Specifies a window with the vertical scroll bar (if present) to the right of the client area. This is the default.</summary>
        WS_EX_RIGHTSCROLLBAR = 0x00000000,

        /// <summary>
        /// Specifies a window that displays text using right-to-left reading-order properties.
        /// The shell language must support reading-order alignment for this to take effect.
        /// </summary>
        WS_EX_RTLREADING = 0x00002000,

        /// <summary>Specifies a window with a three-dimensional border style intended to be used for items that do not accept user input.</summary>
        WS_EX_STATICEDGE = 0x00020000,

        /// <summary>
        /// Specifies a window that is intended to be used as a floating toolbar.
        /// A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font.
        /// A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB.
        /// If a tool window has a system menu, its icon is not displayed on the title bar.
        /// However, you can display the system menu by right-clicking or by typing ALT+SPACE.
        /// </summary>
        WS_EX_TOOLWINDOW = 0x00000080,

        /// <summary>
        /// Specifies a window that should be placed above all non-topmost windows and should stay above them, even when the window is deactivated.
        /// To add or remove this style, use the SetWindowPos function.
        /// </summary>
        WS_EX_TOPMOST = 0x00000008,

        /// <summary>
        /// Specifies a window that should not be painted until siblings beneath the window (that were created by the same thread) have been painted.
        /// The window appears transparent because the bits of underlying sibling windows have already been painted.
        /// To achieve transparency without these restrictions, use the SetWindowRgn function.
        /// </summary>
        WS_EX_TRANSPARENT = 0x00000020,

        /// <summary>Specifies a window that has a border with a raised edge.</summary>
        WS_EX_WINDOWEDGE = 0x00000100
    }

    [Flags]
    internal enum ClassStyles : uint
    {
        /// <summary>Aligns the window's client area on a byte boundary (in the x direction). This style affects the width of the window and its horizontal placement on the display.</summary>
        ByteAlignClient = 0x1000,

        /// <summary>Aligns the window on a byte boundary (in the x direction). This style affects the width of the window and its horizontal placement on the display.</summary>
        ByteAlignWindow = 0x2000,

        /// <summary>
        /// Allocates one device context to be shared by all windows in the class.
        /// Because window classes are process specific, it is possible for multiple threads of an application to create a window of the same class.
        /// It is also possible for the threads to attempt to use the device context simultaneously. When this happens, the system allows only one thread to successfully finish its drawing operation.
        /// </summary>
        ClassDC = 0x40,

        /// <summary>Sends a double-click message to the window procedure when the user double-clicks the mouse while the cursor is within a window belonging to the class.</summary>
        DoubleClicks = 0x8,

        /// <summary>
        /// Enables the drop shadow effect on a window. The effect is turned on and off through SPI_SETDROPSHADOW.
        /// Typically, this is enabled for small, short-lived windows such as menus to emphasize their Z order relationship to other windows.
        /// </summary>
        DropShadow = 0x20000,

        /// <summary>Indicates that the window class is an application global class. For more information, see the "Application Global Classes" section of About Window Classes.</summary>
        GlobalClass = 0x4000,

        /// <summary>Redraws the entire window if a movement or size adjustment changes the width of the client area.</summary>
        HorizontalRedraw = 0x2,

        /// <summary>Disables Close on the window menu.</summary>
        NoClose = 0x200,

        /// <summary>Allocates a unique device context for each window in the class.</summary>
        OwnDC = 0x20,

        /// <summary>
        /// Sets the clipping rectangle of the child window to that of the parent window so that the child can draw on the parent.
        /// A window with the CS_PARENTDC style bit receives a regular device context from the system's cache of device contexts.
        /// It does not give the child the parent's device context or device context settings. Specifying CS_PARENTDC enhances an application's performance.
        /// </summary>
        ParentDC = 0x80,

        /// <summary>
        /// Saves, as a bitmap, the portion of the screen image obscured by a window of this class.
        /// When the window is removed, the system uses the saved bitmap to restore the screen image, including other windows that were obscured.
        /// Therefore, the system does not send WM_PAINT messages to windows that were obscured if the memory used by the bitmap has not been discarded and if other screen actions have not invalidated the stored image.
        /// This style is useful for small windows (for example, menus or dialog boxes) that are displayed briefly and then removed before other screen activity takes place.
        /// This style increases the time required to display the window, because the system must first allocate memory to store the bitmap.
        /// </summary>
        SaveBits = 0x800,

        /// <summary>Redraws the entire window if a movement or size adjustment changes the height of the client area.</summary>
        VerticalRedraw = 0x1
    }

    internal enum CFS : uint
    {
        /// <summary>
        /// Move the composition window to the default position.
        /// The IME window can display the composition window
        /// outside the client area, such as in a floating window.
        /// </summary>
        CFS_DEFAULT = 0,

        /// <summary>
        /// Display the upper left corner of the composition window
        /// at exactly the position specified by ptCurrentPos.
        /// The coordinates are relative to the upper left corner
        /// of the window containing the composition window
        /// and are not subject to adjustment by the IME.
        /// </summary>
        CFS_FORCE_POSITION = 32,

        /// <summary>
        /// Display the upper left corner of the composition window
        /// at the position specified by ptCurrentPos.
        /// The coordinates are relative to the upper left corner
        /// of the window containing the composition window
        /// and are subject to adjustment by the IME.
        /// </summary>
        CFS_POINT = 2,

        /// <summary>
        /// Display the composition window at the position specified by rcArea.
        /// The coordinates are relative to the upper left
        /// of the window containing the composition window.
        /// </summary>
        CFS_RECT = 1,
    }

    internal enum ShowWindowCommands
    {
        /// <summary>
        /// Hides the window and activates another window.
        /// </summary>
        Hide = 0,

        /// <summary>
        /// Activates and displays a window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when displaying the window
        /// for the first time.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Activates the window and displays it as a minimized window.
        /// </summary>
        ShowMinimized = 2,

        /// <summary>
        /// Maximizes the specified window.
        /// </summary>
        Maximize = 3, // is this the right value?

        /// <summary>
        /// Activates the window and displays it as a maximized window.
        /// </summary>
        ShowMaximized = 3,

        /// <summary>
        /// Displays a window in its most recent size and position. This value
        /// is similar to <see cref="Normal"/>, except
        /// the window is not activated.
        /// </summary>
        ShowNoActivate = 4,

        /// <summary>
        /// Activates the window and displays it in its current size and position.
        /// </summary>
        Show = 5,

        /// <summary>
        /// Minimizes the specified window and activates the next top-level
        /// window in the Z order.
        /// </summary>
        Minimize = 6,

        /// <summary>
        /// Displays the window as a minimized window. This value is similar to
        /// <see cref="ShowMinimized"/>, except the
        /// window is not activated.
        /// </summary>
        ShowMinNoActive = 7,

        /// <summary>
        /// Displays the window in its current size and position. This value is
        /// similar to <see cref="Show"/>, except the
        /// window is not activated.
        /// </summary>
        ShowNA = 8,

        /// <summary>
        /// Activates and displays the window. If the window is minimized or
        /// maximized, the system restores it to its original size and position.
        /// An application should specify this flag when restoring a minimized window.
        /// </summary>
        Restore = 9,

        /// <summary>
        /// Sets the show state based on the SW_* value specified in the
        /// STARTUPINFO structure passed to the CreateProcess function by the
        /// program that started the application.
        /// </summary>
        ShowDefault = 10,

        /// <summary>
        ///  <b>Windows 2000/XP:</b> Minimizes a window, even if the thread
        /// that owns the window is not responding. This flag should only be
        /// used when minimizing windows from a different thread.
        /// </summary>
        ForceMinimize = 11
    }

    [Flags]
    internal enum SetWindowPosFlags : uint
    {
        /// <summary>If the calling thread and the thread that owns the window are attached to different input queues,
        /// the system posts the request to the thread that owns the window. This prevents the calling thread from
        /// blocking its execution while other threads process the request.</summary>
        /// <remarks>SWP_ASYNCWINDOWPOS</remarks>
        AsynchronousWindowPosition = 0x4000,

        /// <summary>Prevents generation of the WM_SYNCPAINT message.</summary>
        /// <remarks>SWP_DEFERERASE</remarks>
        DeferErase = 0x2000,

        /// <summary>Draws a frame (defined in the window's class description) around the window.</summary>
        /// <remarks>SWP_DRAWFRAME</remarks>
        DrawFrame = 0x0020,

        /// <summary>Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
        /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
        /// is sent only when the window's size is being changed.</summary>
        /// <remarks>SWP_FRAMECHANGED</remarks>
        FrameChanged = 0x0020,

        /// <summary>Hides the window.</summary>
        /// <remarks>SWP_HIDEWINDOW</remarks>
        HideWindow = 0x0080,

        /// <summary>Does not activate the window. If this flag is not set, the window is activated and moved to the
        /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
        /// parameter).</summary>
        /// <remarks>SWP_NOACTIVATE</remarks>
        NoActivate = 0x0010,

        /// <summary>Discards the entire contents of the client area. If this flag is not specified, the valid
        /// contents of the client area are saved and copied back into the client area after the window is sized or
        /// repositioned.</summary>
        /// <remarks>SWP_NOCOPYBITS</remarks>
        NoCopyBits = 0x0100,

        /// <summary>Retains the current position (ignores X and Y parameters).</summary>
        /// <remarks>SWP_NOMOVE</remarks>
        NoMove = 0x0002,

        /// <summary>Does not change the owner window's position in the Z order.</summary>
        /// <remarks>SWP_NOOWNERZORDER</remarks>
        NoOwnerZOrder = 0x0200,

        /// <summary>Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
        /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
        /// window uncovered as a result of the window being moved. When this flag is set, the application must
        /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.</summary>
        /// <remarks>SWP_NOREDRAW</remarks>
        NoRedraw = 0x0008,

        /// <summary>Same as the SWP_NOOWNERZORDER flag.</summary>
        /// <remarks>SWP_NOREPOSITION</remarks>
        NoReposition = 0x0200,

        /// <summary>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</summary>
        /// <remarks>SWP_NOSENDCHANGING</remarks>
        NoSendChangingEvent = 0x0400,

        /// <summary>Retains the current size (ignores the cx and cy parameters).</summary>
        /// <remarks>SWP_NOSIZE</remarks>
        NoSize = 0x0001,

        /// <summary>Retains the current Z order (ignores the hWndInsertAfter parameter).</summary>
        /// <remarks>SWP_NOZORDER</remarks>
        NoZOrder = 0x0004,

        /// <summary>Displays the window.</summary>
        /// <remarks>SWP_SHOWWINDOW</remarks>
        ShowWindow = 0x0040,
    }

    [Flags]
    internal enum WindowPlacementFlags : uint
    {
        /// <summary>
        /// If the calling thread and the thread that owns the window are attached to different input queues,
        /// the system posts the request to the thread that owns the window.
        /// This prevents the calling thread from blocking its execution while other threads process the request.
        /// </summary>
        AsyncWindowPlacement = 0x0004,

        /// <summary>
        /// The restored window will be maximized, regardless of whether it was maximized before it was minimized.
        /// This setting is only valid the next time the window is restored.
        /// It does not change the default restoration behavior.
        /// This flag is only valid when the SW_SHOWMINIMIZED value is specified for the showCmd member.
        /// </summary>
        RestoreToMaximized = 0x0002,

        /// <summary>
        /// The coordinates of the minimized window may be specified.
        /// This flag must be specified if the coordinates are set in the ptMinPosition member.
        /// </summary>
        SetMinPosition = 0x0001,
    }

    // FIXME: There are additional values for when the hWnd is a dialog box.
    // See DWL values:
    // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongw
    internal enum GetGWLPIndex : int
    {
        /// <summary>
        /// Retrieves the extended window styles.
        /// </summary>
        ExStyle = -20,

        /// <summary>
        /// Retrieves a handle to the application instance.
        /// </summary>
        HInstance = -6,

        /// <summary>
        /// Retrieves a handle to the parent window, if any.
        /// </summary>
        HWNDParent = -8,

        /// <summary>
        /// Retrieves the identifier of the window.
        /// </summary>
        ID = -12,

        /// <summary>
        /// Retrieves the window styles.
        /// </summary>
        Style = -16,

        /// <summary>
        /// Retrieves the user data associated with the window.
        /// This data is intended for use by the application that created the window.
        /// Its value is initially zero.
        /// </summary>
        UserData = -21,

        /// <summary>
        /// Retrieves the address of the window procedure,
        /// or a handle representing the address of the window procedure.
        /// You must use the CallWindowProc function to call the window procedure.
        /// </summary>
        WndProc = -4,
    }

    // FIXME: There are additional values for when the hWnd is a dialog box.
    // See DWL values:
    // https://docs.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowlongw
    internal enum SetGWLPIndex : int
    {
        /// <summary>
        /// Retrieves the extended window styles.
        /// </summary>
        ExStyle = -20,

        /// <summary>
        /// Retrieves a handle to the application instance.
        /// </summary>
        HInstance = -6,

        /// <summary>
        /// Retrieves the identifier of the window.
        /// </summary>
        ID = -12,

        /// <summary>
        /// Retrieves the window styles.
        /// </summary>
        Style = -16,

        /// <summary>
        /// Retrieves the user data associated with the window.
        /// This data is intended for use by the application that created the window.
        /// Its value is initially zero.
        /// </summary>
        UserData = -21,

        /// <summary>
        /// Retrieves the address of the window procedure,
        /// or a handle representing the address of the window procedure.
        /// You must use the CallWindowProc function to call the window procedure.
        /// </summary>
        WndProc = -4,
    }

    [Flags]
    internal enum TME : uint
    {
        /// <summary>
        /// The caller wants to cancel a prior tracking request.
        /// The caller should also specify the type of tracking that it wants to cancel.
        /// For example, to cancel hover tracking,
        /// the caller must pass the <see cref="Cancel"/> and <see cref="Hover"/> flags.
        /// </summary>
        Cancel = 0x80000000,

        /// <summary>
        /// The caller wants hover notification.
        /// Notification is delivered as a <see cref="WM.MOUSEHOVER"/> message.
        /// If the caller requests hover tracking while hover tracking is already active,
        /// the hover timer will be reset.
        /// This flag is ignored if the mouse pointer is not over the specified window or area.
        /// </summary>
        Hover = 0x00000001,

        /// <summary>
        /// The caller wants leave notification.
        /// Notification is delivered as a <see cref="WM.MOUSELEAVE"/> message.
        /// If the mouse is not over the specified window or area,
        /// a leave notification is generated immediately and no further tracking is performed.
        /// </summary>
        Leave = 0x00000002,

        /// <summary>
        /// The caller wants hover and leave notification for the nonclient areas.
        /// Notification is delivered as <see cref="WM.NCMOUSEHOVER"/> and <see cref="WM.NCMOUSELEAVE"/> messages.
        /// </summary>
        NonClient = 0x00000010,

        /// <summary>
        /// The function fills in the structure instead of treating it as a tracking request.
        /// The structure is filled such that had that structure been passed to TrackMouseEvent,
        /// it would generate the current tracking.
        /// The only anomaly is that the hover time-out returned is always the actual time-out and not HOVER_DEFAULT,
        /// if HOVER_DEFAULT was specified during the original TrackMouseEvent request.
        /// </summary>
        Query = 0x40000000,
    }

    internal enum SIZE
    {
        /// <summary>
        /// Message is sent to all pop-up windows when some other window is maximized.
        /// </summary>
        MaxHide = 4,

        /// <summary>
        /// The window has been maximized.
        /// </summary>
        Maximized = 2,

        /// <summary>
        /// Message is sent to all pop-up windows when some other window has been restored to its former size.
        /// </summary>
        MaxShow = 3,

        /// <summary>
        /// The window has been minimized.
        /// </summary>
        Minimized = 1,

        /// <summary>
        /// The window has been resized, but neither the SIZE_MINIMIZED nor SIZE_MAXIMIZED value applies.
        /// </summary>
        Restored = 0,
    }

    [Flags]
    internal enum PFD : uint
    {
        /// <summary>
        /// The buffer is double-buffered. This flag and SUPPORT_GDI are mutually exclusive in the current generic implementation.
        /// </summary>
        DOUBLEBUFFER = 0x00000001,

        /// <summary>
        /// The buffer is stereoscopic. This flag is not supported in the current generic implementation.
        /// </summary>
        STEREO = 0x00000002,

        /// <summary>
        /// The buffer can draw to a window or device surface.
        /// </summary>
        DRAW_TO_WINDOW = 0x00000004,

        /// <summary>
        /// The buffer can draw to a memory bitmap.
        /// </summary>
        DRAW_TO_BITMAP = 0x00000008,

        /// <summary>
        /// The buffer supports GDI drawing.
        /// This flag and DOUBLEBUFFER are mutually exclusive in the current generic implementation.
        /// </summary>
        SUPPORT_GDI = 0x00000010,

        /// <summary>
        /// The buffer supports OpenGL drawing.
        /// </summary>
        SUPPORT_OPENGL = 0x00000020,

        /// <summary>
        /// The pixel format is supported by the GDI software implementation,
        /// which is also known as the generic implementation.
        /// If this bit is clear, the pixel format is supported by a device driver or hardware.
        /// </summary>
        GENERIC_FORMAT = 0x00000040,

        /// <summary>
        /// The buffer uses RGBA pixels on a palette-managed device.
        /// A logical palette is required to achieve the best results for this pixel type.
        /// Colors in the palette should be specified according to the values of the
        /// cRedBits, cRedShift, cGreenBits, cGreenShift, cBluebits, and cBlueShift members.
        /// The palette should be created and realized in the device context before calling wglMakeCurrent.
        /// </summary>
        NEED_PALETTE = 0x00000080,

        /// <summary>
        /// Defined in the pixel format descriptors of hardware that supports one hardware palette in 256-color mode only.
        /// For such systems to use hardware acceleration, the hardware palette must be in a
        /// fixed order (for example, 3-3-2) when in RGBA mode or must match the logical palette
        /// when in color-index mode.When this flag is set, you must call SetSystemPaletteUse in your program
        /// to force a one-to-one mapping of the logical palette and the system palette.
        /// If your OpenGL hardware supports multiple hardware palettes and the device driver can
        /// allocate spare hardware palettes for OpenGL, this flag is typically clear.
        /// This flag is not set in the generic pixel formats.
        /// </summary>
        NEED_SYSTEM_PALETTE = 0x00000100,

        /// <summary>
        /// Specifies the content of the back buffer in the double-buffered main color plane following a buffer swap.
        /// Swapping the color buffers causes the exchange of the back buffer's content with the front buffer's content.
        /// Following the swap, the back buffer's content contains the front buffer's content before the swap.
        /// SWAP_EXCHANGE is a hint only and might not be provided by a driver.
        /// </summary>
        /// <remarks>Can be used when calling glAddSwapHintRectWIN.</remarks>
        SWAP_EXCHANGE = 0x00000200,

        /// <summary>
        /// Specifies the content of the back buffer in the double-buffered main color plane following a buffer swap.
        /// Swapping the color buffers causes the content of the back buffer to be copied to the front buffer.
        /// The content of the back buffer is not affected by the swap.
        /// SWAP_COPY is a hint only and might not be provided by a driver.
        /// <remarks>Can be used when calling glAddSwapHintRectWIN.</remarks>
        /// </summary>
        SWAP_COPY = 0x00000400,

        /// <summary>
        /// Indicates whether a device can swap individual layer planes with pixel formats
        /// that include double-buffered overlay or underlay planes.
        /// Otherwise all layer planes are swapped together as a group.
        /// When this flag is set, wglSwapLayerBuffers is supported.
        /// </summary>
        SWAP_LAYER_BUFFERS = 0x00000800,

        /// <summary>
        /// The pixel format is supported by a device driver that accelerates the generic implementation. If this flag is clear and the GENERIC_FORMAT flag is set, the pixel format is supported by the generic implementation only.
        /// </summary>
        GENERIC_ACCELERATED = 0x00001000,

        /// <summary>
        /// The pixel buffer supports DirectDraw drawing,
        /// which allows applications to have low-level control of the output drawing surface.
        /// </summary>
        SUPPORT_DIRECTDRAW = 0x00002000,

        /// <summary>
        /// The pixel buffer supports Direct3D drawing, which accellerated rendering in three dimensions.
        /// </summary>
        DIRECT3D_ACCELERATED = 0x00004000,

        /// <summary>
        /// The pixel buffer supports compositing,
        /// which indicates that source pixels MAY overwrite or be combined with background pixels.
        /// </summary>
        /// <remarks>
        /// Windows uses this with OpenGL drawing only.
        /// Windows NT 3.1, Windows NT 3.51, Windows NT 4.0, Windows 98,
        /// Windows 2000, Windows Millennium Edition, Windows XP,
        /// and Windows Server 2003 do not support this flag.
        /// </remarks>
        SUPPORT_COMPOSITION = 0x00008000,

        /// <summary>
        /// The requested pixel format can either have or not have a depth buffer.
        /// To select a pixel format without a depth buffer, you must specify this flag.
        /// The requested pixel format can be with or without a depth buffer.
        /// Otherwise, only pixel formats with a depth buffer are considered.
        /// </summary>
        /// <remarks>Can be used when calling ChoosePixelFormat.</remarks>
        DEPTH_DONTCARE = 0x20000000,

        /// <summary>
        /// The requested pixel format can be either single- or double-buffered.
        /// </summary>
        /// <remarks>Can be used when calling ChoosePixelFormat.</remarks>
        DOUBLEBUFFER_DONTCARE = 0x40000000,

        /// <summary>
        /// The requested pixel format can be either monoscopic or stereoscopic.
        /// </summary>
        /// <remarks>Can be used when calling ChoosePixelFormat.</remarks>
        STEREO_DONTCARE = 0x80000000,
    }

    internal enum PFDType : byte
    {
        /// <summary>
        /// RGBA pixels. Each pixel has four components in this order: red, green, blue, and alpha.
        /// </summary>
        TYPE_RGBA = 0,

        /// <summary>
        /// Color-index pixels. Each pixel uses a color-index value.
        /// </summary>
        TYPE_COLORINDEX = 1,
    }

    internal enum PFDPlane : byte
    {
        MAIN = 0,
        OVERLAY = 1,
        UNDERLAY = 0xFF, // -1
    }

    internal enum IDC : int
    {
        /// <summary>
        /// Standard arrow and small hourglass
        /// </summary>
        AppStarting = 32650,

        /// <summary>
        /// Standard arrow
        /// </summary>
        Arrow = 32512,

        /// <summary>
        /// Crosshair
        /// </summary>
        Cross = 32515,

        /// <summary>
        /// Hand
        /// </summary>
        Hand = 32649,

        /// <summary>
        /// Arrow and question mark
        /// </summary>
        Help = 32651,

        /// <summary>
        /// I-beam
        /// </summary>
        IBeam = 32513,

        /// <summary>
        /// Obsolete for applications marked version 4.0 or later.
        /// </summary>
        Icon = 32641,

        /// <summary>
        /// Slashed circle
        /// </summary>
        No = 32648,

        /// <summary>
        /// Obsolete for applications marked version 4.0 or later. Use SIZEALL.
        /// </summary>
        Size = 32640,

        /// <summary>
        /// Four-pointed arrow pointing north, south, east, and west
        /// </summary>
        SizeAll = 32646,

        /// <summary>
        /// Double-pointed arrow pointing northeast and southwest
        /// </summary>
        SizeNESW = 32643,

        /// <summary>
        /// Double-pointed arrow pointing north and south
        /// </summary>
        SizeNS = 32645,

        /// <summary>
        /// Double-pointed arrow pointing northwest and southeast
        /// </summary>
        SizeNWSE = 32642,

        /// <summary>
        /// Double-pointed arrow pointing west and east
        /// </summary>
        SizeWE = 32644,

        /// <summary>
        /// Vertical arrow
        /// </summary>
        UpArrow = 32516,

        /// <summary>
        /// Hour
        /// </summary>
        Wait = 32514,
    }

    internal enum OCR
    {
        /// <summary>
        /// Standard arrow and small hourglass
        /// </summary>
        AppStarting = 32650,

        /// <summary>
        /// Standard arrow
        /// </summary>
        Normal = 32512,

        /// <summary>
        /// Crosshair
        /// </summary>
        Cross = 32515,

        /// <summary>
        /// Hand
        /// </summary>
        Hand = 32649,

        /// <summary>
        /// Arrow and question mark
        /// </summary>
        Help = 32651,

        /// <summary>
        /// I-beam
        /// </summary>
        IBeam = 32513,

        /// <summary>
        /// Slashed circle
        /// </summary>
        No = 32648,

        /// <summary>
        /// Four-pointed arrow pointing north, south, east, and west
        /// </summary>
        SizeAll = 32646,

        /// <summary>
        /// Double-pointed arrow pointing northeast and southwest
        /// </summary>
        SizeNESW = 32643,

        /// <summary>
        /// Double-pointed arrow pointing north and south
        /// </summary>
        SizeNS = 32645,

        /// <summary>
        /// Double-pointed arrow pointing northwest and southeast
        /// </summary>
        SizeNWSE = 32642,

        /// <summary>
        /// Double-pointed arrow pointing west and east
        /// </summary>
        SizeWE = 32644,

        /// <summary>
        /// Vertical arrow
        /// </summary>
        Up = 32516,

        /// <summary>
        /// Hourglass
        /// </summary>
        Wait = 32514,
    }

    internal enum OIC
    {
        Sample = 32512,
        Hand = 32513,
        Ques = 32514,
        Bang = 32515,
        Note = 32516,
        WinLogo = 32517,
        Warning = Bang,
        Error = Hand,
        Information = Note,
        Shield = 32518,
    }

    internal enum ImageType : uint
    {
        /// <summary>
        /// Loads a bitmap.
        /// </summary>
        Bitmap = 0,

        /// <summary>
        /// Loads a cursor.
        /// </summary>
        Cursor = 2,

        /// <summary>
        /// Loads an icon.
        /// </summary>
        Icon = 1,
    }

    internal enum DIB
    {
        /// <summary>
        /// The color table contains literal RGB values.
        /// </summary>
        RGBColors = 0x00,

        /// <summary>
        /// The color table consists of an array of 16-bit indexes
        /// into the LogPalette object (section <see href="https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-emf/758f047d-765e-424c-9204-a833b7b4e527">2.2.17</see>) that is
        /// currently defined in the <see href="https://docs.microsoft.com/en-us/openspecs/windows_protocols/ms-emf/6de331ec-81f6-4ab2-8982-673a232e0a5c#gt_32591a2b-a9d0-4ccf-a5b8-7177e1ea8d45">playback device context</see>.
        /// </summary>
        PALColors = 0x01,

        /// <summary>
        /// No color table exists.
        /// The pixels in the DIB are indices into the current
        /// logical palette in the playback device context.
        /// </summary>
        PALIndices = 0x02,
    }

    internal enum BI : uint
    {
        /// <summary>
        /// An uncompressed format.
        /// </summary>
        RGB,

        /// <summary>
        /// A run-length encoded (RLE) format for bitmaps with 8 bpp.
        /// The compression format is a 2-byte format consisting of
        /// a count byte followed by a byte containing a color index.
        /// For more information, see Bitmap Compression.
        /// </summary>
        RLE8,

        /// <summary>
        /// An RLE format for bitmaps with 4 bpp.
        /// The compression format is a 2-byte format consisting of
        /// a count byte followed by two word-length color indexes.
        /// For more information, see Bitmap Compression.
        /// </summary>
        RLE4,

        /// <summary>
        /// Specifies that the bitmap is not compressed and that
        /// the color table consists of three DWORD color masks
        /// that specify the red, green, and blue components,
        /// respectively, of each pixel.
        /// This is valid when used with 16- and 32-bpp bitmaps.
        /// </summary>
        Bitfields,

        /// <summary>
        /// Indicates that the image is a JPEG image.
        /// </summary>
        JPEG,

        /// <summary>
        /// Indicates that the image is a PNG image.
        /// </summary>
        PNG,
    }

    [Flags]
    internal enum LR : uint
    {
        /// <summary>
        /// When the uType parameter specifies IMAGE_BITMAP,
        /// causes the function to return a DIB section bitmap
        /// rather than a compatible bitmap.
        /// This flag is useful for loading a bitmap without
        /// mapping it to the colors of the display device.
        /// </summary>
        CreatedIBSection = 0x00002000,

        /// <summary>
        /// The default flag; it does nothing. All it means is "not <see cref="LR.Monochrome"/>".
        /// </summary>
        DefaultColor = 0x00000000,

        /// <summary>
        /// Uses the width or height specified by the system
        /// metric values for cursors or icons,
        /// if the cxDesired or cyDesired values are set to zero.
        /// If this flag is not specified and cxDesired and cyDesired
        /// are set to zero,
        /// the function uses the actual resource size.
        /// If the resource contains multiple images,
        /// the function uses the size of the first image.
        /// </summary>
        DefaultSize = 0x00000040,

        /// <summary>
        /// Loads the stand-alone image from the file specified
        /// by lpszName (icon, cursor, or bitmap file).
        /// </summary>
        LoadFromFile = 0x00000010,

        /// <summary>
        /// Searches the color table for the image and replaces
        /// the following shades of gray with the corresponding 3-D color.
        /// <list type="3D colors">
        /// <item>Dk Gray, RGB(128,128,128) with COLOR_3DSHADOW</item>
        /// <item>Gray, RGB(192,192,192) with COLOR_3DFACE</item>
        /// <item>Lt Gray, RGB(223,223,223) with COLOR_3DLIGHT</item>
        /// </list>
        /// Do not use this option if you are loading a bitmap
        /// with a color depth greater than 8bpp.
        /// </summary>
        LoadMap3DColors = 0x00001000,

        /// <summary>
        /// Retrieves the color value of the first pixel in the image
        /// and replaces the corresponding entry in the color table
        /// with the default window color (COLOR_WINDOW).
        /// All pixels in the image that use that entry become
        /// the default window color.
        /// This value applies only to images that have
        /// corresponding color tables.
        ///
        /// Do not use this option if you are loading a bitmap
        /// with a color depth greater than 8bpp.
        ///
        /// If fuLoad includes both the LR_LOADTRANSPARENT and
        /// LR_LOADMAP3DCOLORS values, LR_LOADTRANSPARENT takes precedence.
        /// However, the color table entry is replaced with COLOR_3DFACE
        /// rather than COLOR_WINDOW.
        /// </summary>
        LoadTransparent = 0x00000020,

        /// <summary>
        /// Loads the image in black and white.
        /// </summary>
        Monochrome = 0x00000001,

        /// <summary>
        /// Shares the image handle if the image is loaded multiple times.
        /// If <see cref="Shared"/> is not set, a second call to LoadImage
        /// for the same resource will load the image again and
        /// return a different handle.
        ///
        /// When you use this flag, the system will destroy the resource
        /// when it is no longer needed.
        ///
        /// Do not use <see cref="Shared"/> for images that have
        /// non-standard sizes, that may change after loading,'
        /// or that are loaded from a file.
        ///
        /// When loading a system icon or cursor,
        /// you must use <see cref="Shared"/> or the function
        /// will fail to load the resource.
        ///
        /// This function finds the first image in the cache
        /// with the requested resource name,
        /// regardless of the size requested.
        /// </summary>
        Shared = 0x00008000,

        /// <summary>
        /// Uses true VGA colors.
        /// </summary>
        VGAColor = 0x00000080,
    }

    [Flags]
    internal enum PM : uint
    {
        /// <summary>
        /// Messages are not removed from the queue after processing by PeekMessage.
        /// </summary>
        NOREMOVE = 0,

        /// <summary>
        /// Messages are removed from the queue after processing by PeekMessage.
        /// </summary>
        REMOVE = 1,

        /// <summary>
        /// Prevents the system from releasing any thread that is waiting for the caller to go idle (see WaitForInputIdle).
        /// Combine this value with either PM_NOREMOVE or PM_REMOVE.
        /// </summary>
        NOYIELD = 2,
    }

    internal enum SystemMetric : int
    {
        /// <summary>
        /// The flags that specify how the system arranged minimized windows. For more information, see the Remarks section in this topic.
        /// </summary>
        Arrange = 56,

        /// <summary>
        /// The value that specifies how the system is started:
        /// 0 Normal boot
        /// 1 Fail-safe boot
        /// 2 Fail-safe with network boot
        /// A fail-safe boot (also called SafeBoot, Safe Mode, or Clean Boot) bypasses the user startup files.
        /// </summary>
        CleanBoot = 67,

        /// <summary>
        /// The number of display monitors on a desktop. For more information, see the Remarks section in this topic.
        /// </summary>
        CMonitors = 80,

        /// <summary>
        /// The number of buttons on a mouse, or zero if no mouse is installed.
        /// </summary>
        CMouseButtons = 43,

        /// <summary>
        /// The width of a window border, in pixels. This is equivalent to the CXEDGE value for windows with the 3-D look.
        /// </summary>
        CXBorder = 5,

        /// <summary>
        /// The width of a cursor, in pixels. The system cannot create cursors of other sizes.
        /// </summary>
        CXCursor = 13,

        /// <summary>
        /// This value is the same as <see cref="CXFIXEDFRAME"/>.
        /// </summary>
        CXDLGFrame = 7,

        /// <summary>
        /// The width of the rectangle around the location of a first click in a double-click sequence, in pixels. ,
        /// The second click must occur within the rectangle that is defined by CXDOUBLECLK and CYDOUBLECLK for the system
        /// to consider the two clicks a double-click. The two clicks must also occur within a specified time.
        /// To set the width of the double-click rectangle, call SystemParametersInfo with SPI_SETDOUBLECLKWIDTH.
        /// </summary>
        CXDoubleClk = 36,

        /// <summary>
        /// The number of pixels on either side of a mouse-down point that the mouse pointer can move before a drag operation begins.
        /// This allows the user to click and release the mouse button easily without unintentionally starting a drag operation.
        /// If this value is negative, it is subtracted from the left of the mouse-down point and added to the right of it.
        /// </summary>
        CXDrag = 68,

        /// <summary>
        /// The width of a 3-D border, in pixels. This metric is the 3-D counterpart of CXBORDER.
        /// </summary>
        CXEdge = 45,

        /// <summary>
        /// The thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels.
        /// CXFIXEDFRAME is the height of the horizontal border, and CYFIXEDFRAME is the width of the vertical border.
        /// This value is the same as CXDLGFRAME.
        /// </summary>
        CXFixedFrame = 7,

        /// <summary>
        /// The width of the left and right edges of the focus rectangle that the DrawFocusRectdraws.
        /// This value is in pixels.
        /// Windows 2000:  This value is not supported.
        /// </summary>
        CXFocusBorder = 83,

        /// <summary>
        /// This value is the same as CXSIZEFRAME.
        /// </summary>
        CXFrame = 32,

        /// <summary>
        /// The width of the client area for a full-screen window on the primary display monitor, in pixels.
        /// To get the coordinates of the portion of the screen that is not obscured by the system taskbar or by application desktop toolbars,
        /// call the SystemParametersInfofunction with the SPI_GETWORKAREA value.
        /// </summary>
        CXFullscreen = 16,

        /// <summary>
        /// The width of the arrow bitmap on a horizontal scroll bar, in pixels.
        /// </summary>
        CXHScroll = 21,

        /// <summary>
        /// The width of the thumb box in a horizontal scroll bar, in pixels.
        /// </summary>
        CXHThumb = 10,

        /// <summary>
        /// The default width of an icon, in pixels. The LoadIcon function can load only icons with the dimensions
        /// that CXICON and CYICON specifies.
        /// </summary>
        CXIcon = 11,

        /// <summary>
        /// The width of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size
        /// CXICONSPACING by CYICONSPACING when arranged. This value is always greater than or equal to CXICON.
        /// </summary>
        CXIconSpacing = 38,

        /// <summary>
        /// The default width, in pixels, of a maximized top-level window on the primary display monitor.
        /// </summary>
        CXMaximized = 61,

        /// <summary>
        /// The default maximum width of a window that has a caption and sizing borders, in pixels.
        /// This metric refers to the entire desktop. The user cannot drag the window frame to a size larger than these dimensions.
        /// A window can override this value by processing the WM_GETMINMAXINFO message.
        /// </summary>
        CXMaxTrack = 59,

        /// <summary>
        /// The width of the default menu check-mark bitmap, in pixels.
        /// </summary>
        CXMenuCheck = 71,

        /// <summary>
        /// The width of menu bar buttons, such as the child window close button that is used in the multiple document interface, in pixels.
        /// </summary>
        CXMenuSize = 54,

        /// <summary>
        /// The minimum width of a window, in pixels.
        /// </summary>
        CXMin = 28,

        /// <summary>
        /// The width of a minimized window, in pixels.
        /// </summary>
        CXMinimized = 57,

        /// <summary>
        /// The width of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged.
        /// This value is always greater than or equal to CXMINIMIZED.
        /// </summary>
        CXMinSpacing = 47,

        /// <summary>
        /// The minimum tracking width of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions.
        /// A window can override this value by processing the WM_GETMINMAXINFO message.
        /// </summary>
        CXMinTrack = 34,

        /// <summary>
        /// The amount of border padding for captioned windows, in pixels. Windows XP/2000:  This value is not supported.
        /// </summary>
        CXPaddedBorder = 92,

        /// <summary>
        /// The width of the screen of the primary display monitor, in pixels. This is the same value obtained by calling
        /// GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, HORZRES).
        /// </summary>
        CXScreen = 0,

        /// <summary>
        /// The width of a button in a window caption or title bar, in pixels.
        /// </summary>
        CXSize = 30,

        /// <summary>
        /// The thickness of the sizing border around the perimeter of a window that can be resized, in pixels.
        /// CXSIZEFRAME is the width of the horizontal border, and CYSIZEFRAME is the height of the vertical border.
        /// This value is the same as CXFRAME.
        /// </summary>
        CXSizeFrame = 32,

        /// <summary>
        /// The recommended width of a small icon, in pixels. Small icons typically appear in window captions and in small icon view.
        /// </summary>
        CXSMIcon = 49,

        /// <summary>
        /// The width of small caption buttons, in pixels.
        /// </summary>
        CXSMSize = 52,

        /// <summary>
        /// The width of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors.
        /// The XVIRTUALSCREEN metric is the coordinates for the left side of the virtual screen.
        /// </summary>
        CXVirtualScreen = 78,

        /// <summary>
        /// The width of a vertical scroll bar, in pixels.
        /// </summary>
        CXVScroll = 2,

        /// <summary>
        /// The height of a window border, in pixels. This is equivalent to the CYEDGE value for windows with the 3-D look.
        /// </summary>
        CYBorder = 6,

        /// <summary>
        /// The height of a caption area, in pixels.
        /// </summary>
        CYCaption = 4,

        /// <summary>
        /// The height of a cursor, in pixels. The system cannot create cursors of other sizes.
        /// </summary>
        CYCursor = 14,

        /// <summary>
        /// This value is the same as CYFIXEDFRAME.
        /// </summary>
        CYDLGFrame = 8,

        /// <summary>
        /// The height of the rectangle around the location of a first click in a double-click sequence, in pixels.
        /// The second click must occur within the rectangle defined by CXDOUBLECLK and CYDOUBLECLK for the system to consider
        /// the two clicks a double-click. The two clicks must also occur within a specified time. To set the height of the double-click
        /// rectangle, call SystemParametersInfo with SPI_SETDOUBLECLKHEIGHT.
        /// </summary>
        CYDoubleClick = 37,

        /// <summary>
        /// The number of pixels above and below a mouse-down point that the mouse pointer can move before a drag operation begins.
        /// This allows the user to click and release the mouse button easily without unintentionally starting a drag operation.
        /// If this value is negative, it is subtracted from above the mouse-down point and added below it.
        /// </summary>
        CYDrag = 69,

        /// <summary>
        /// The height of a 3-D border, in pixels. This is the 3-D counterpart of CYBORDER.
        /// </summary>
        CYEdge = 46,

        /// <summary>
        /// The thickness of the frame around the perimeter of a window that has a caption but is not sizable, in pixels.
        /// CXFIXEDFRAME is the height of the horizontal border, and CYFIXEDFRAME is the width of the vertical border.
        /// This value is the same as CYDLGFRAME.
        /// </summary>
        CYFixedFrame = 8,

        /// <summary>
        /// The height of the top and bottom edges of the focus rectangle drawn byDrawFocusRect.
        /// This value is in pixels.
        /// Windows 2000:  This value is not supported.
        /// </summary>
        CYFocusBorder = 84,

        /// <summary>
        /// This value is the same as CYSIZEFRAME.
        /// </summary>
        CYFrame = 33,

        /// <summary>
        /// The height of the client area for a full-screen window on the primary display monitor, in pixels.
        /// To get the coordinates of the portion of the screen not obscured by the system taskbar or by application desktop toolbars,
        /// call the SystemParametersInfo function with the SPI_GETWORKAREA value.
        /// </summary>
        CYFullscreen = 17,

        /// <summary>
        /// The height of a horizontal scroll bar, in pixels.
        /// </summary>
        CYHScroll = 3,

        /// <summary>
        /// The default height of an icon, in pixels. The LoadIcon function can load only icons with the dimensions CXICON and CYICON.
        /// </summary>
        CYIcon = 12,

        /// <summary>
        /// The height of a grid cell for items in large icon view, in pixels. Each item fits into a rectangle of size
        /// CXICONSPACING by CYICONSPACING when arranged. This value is always greater than or equal to CYICON.
        /// </summary>
        CYIconSpacing = 39,

        /// <summary>
        /// For double byte character set versions of the system, this is the height of the Kanji window at the bottom of the screen, in pixels.
        /// </summary>
        CYKanjiWindow = 18,

        /// <summary>
        /// The default height, in pixels, of a maximized top-level window on the primary display monitor.
        /// </summary>
        CYMaximized = 62,

        /// <summary>
        /// The default maximum height of a window that has a caption and sizing borders, in pixels. This metric refers to the entire desktop.
        /// The user cannot drag the window frame to a size larger than these dimensions. A window can override this value by processing
        /// the WM_GETMINMAXINFO message.
        /// </summary>
        CYMaxTrack = 60,

        /// <summary>
        /// The height of a single-line menu bar, in pixels.
        /// </summary>
        CYMenu = 15,

        /// <summary>
        /// The height of the default menu check-mark bitmap, in pixels.
        /// </summary>
        CYMenuCheck = 72,

        /// <summary>
        /// The height of menu bar buttons, such as the child window close button that is used in the multiple document interface, in pixels.
        /// </summary>
        CYMenuSize = 55,

        /// <summary>
        /// The minimum height of a window, in pixels.
        /// </summary>
        CYMin = 29,

        /// <summary>
        /// The height of a minimized window, in pixels.
        /// </summary>
        CYMinimized = 58,

        /// <summary>
        /// The height of a grid cell for a minimized window, in pixels. Each minimized window fits into a rectangle this size when arranged.
        /// This value is always greater than or equal to CYMINIMIZED.
        /// </summary>
        CYMinSpacing = 48,

        /// <summary>
        /// The minimum tracking height of a window, in pixels. The user cannot drag the window frame to a size smaller than these dimensions.
        /// A window can override this value by processing the WM_GETMINMAXINFO message.
        /// </summary>
        CYMinTrack = 35,

        /// <summary>
        /// The height of the screen of the primary display monitor, in pixels. This is the same value obtained by calling
        /// GetDeviceCaps as follows: GetDeviceCaps( hdcPrimaryMonitor, VERTRES).
        /// </summary>
        CYScreen = 1,

        /// <summary>
        /// The height of a button in a window caption or title bar, in pixels.
        /// </summary>
        CYSize = 31,

        /// <summary>
        /// The thickness of the sizing border around the perimeter of a window that can be resized, in pixels.
        /// CXSIZEFRAME is the width of the horizontal border, and CYSIZEFRAME is the height of the vertical border.
        /// This value is the same as CYFRAME.
        /// </summary>
        CYSizeFrame = 33,

        /// <summary>
        /// The height of a small caption, in pixels.
        /// </summary>
        CYSMCaption = 51,

        /// <summary>
        /// The recommended height of a small icon, in pixels. Small icons typically appear in window captions and in small icon view.
        /// </summary>
        CYSMIcon = 50,

        /// <summary>
        /// The height of small caption buttons, in pixels.
        /// </summary>
        CYSMSize = 53,

        /// <summary>
        /// The height of the virtual screen, in pixels. The virtual screen is the bounding rectangle of all display monitors.
        /// The YVIRTUALSCREEN metric is the coordinates for the top of the virtual screen.
        /// </summary>
        CYVirtualScreen = 79,

        /// <summary>
        /// The height of the arrow bitmap on a vertical scroll bar, in pixels.
        /// </summary>
        CYVScroll = 20,

        /// <summary>
        /// The height of the thumb box in a vertical scroll bar, in pixels.
        /// </summary>
        CYVThumb = 9,

        /// <summary>
        /// Nonzero if User32.dll supports DBCS; otherwise, 0.
        /// </summary>
        DBCSEnabled = 42,

        /// <summary>
        /// Nonzero if the debug version of User.exe is installed; otherwise, 0.
        /// </summary>
        Debug = 22,

        /// <summary>
        /// Nonzero if the current operating system is Windows 7 or Windows Server 2008 R2 and the Tablet PC Input
        /// service is started; otherwise, 0. The return value is a bitmask that specifies the type of digitizer input supported by the device.
        /// For more information, see Remarks.
        /// Windows Server 2008, Windows Vista, and Windows XP/2000:  This value is not supported.
        /// </summary>
        Digitizer = 94,

        /// <summary>
        /// Nonzero if Input Method Manager/Input Method Editor features are enabled; otherwise, 0.
        /// IMMENABLED indicates whether the system is ready to use a Unicode-based IME on a Unicode application.
        /// To ensure that a language-dependent IME works, check DBCSENABLED and the system ANSI code page.
        /// Otherwise the ANSI-to-Unicode conversion may not be performed correctly, or some components like fonts
        /// or registry settings may not be present.
        /// </summary>
        IMMEnabled = 82,

        /// <summary>
        /// Nonzero if there are digitizers in the system; otherwise, 0. MAXIMUMTOUCHES returns the aggregate maximum of the
        /// maximum number of contacts supported by every digitizer in the system. If the system has only single-touch digitizers,
        /// the return value is 1. If the system has multi-touch digitizers, the return value is the number of simultaneous contacts
        /// the hardware can provide. Windows Server 2008, Windows Vista, and Windows XP/2000:  This value is not supported.
        /// </summary>
        MaximumTouches = 95,

        /// <summary>
        /// Nonzero if the current operating system is the Windows XP, Media Center Edition, 0 if not.
        /// </summary>
        MediaCenter = 87,

        /// <summary>
        /// Nonzero if drop-down menus are right-aligned with the corresponding menu-bar item; 0 if the menus are left-aligned.
        /// </summary>
        MenuDropAlignment = 40,

        /// <summary>
        /// Nonzero if the system is enabled for Hebrew and Arabic languages, 0 if not.
        /// </summary>
        MidEastEnabled = 74,

        /// <summary>
        /// Nonzero if a mouse is installed; otherwise, 0. This value is rarely zero, because of support for virtual mice and because
        /// some systems detect the presence of the port instead of the presence of a mouse.
        /// </summary>
        MousePresent = 19,

        /// <summary>
        /// Nonzero if a mouse with a horizontal scroll wheel is installed; otherwise 0.
        /// </summary>
        MouseHorizontalWheelPresent = 91,

        /// <summary>
        /// Nonzero if a mouse with a vertical scroll wheel is installed; otherwise 0.
        /// </summary>
        MouseWheelPresent = 75,

        /// <summary>
        /// The least significant bit is set if a network is present; otherwise, it is cleared. The other bits are reserved for future use.
        /// </summary>
        Network = 63,

        /// <summary>
        /// Nonzero if the Microsoft Windows for Pen computing extensions are installed; zero otherwise.
        /// </summary>
        PenWindows = 41,

        /// <summary>
        /// This system metric is used in a Terminal Services environment to determine if the current Terminal Server session is
        /// being remotely controlled. Its value is nonzero if the current session is remotely controlled; otherwise, 0.
        /// You can use terminal services management tools such as Terminal Services Manager (tsadmin.msc) and shadow.exe to
        /// control a remote session. When a session is being remotely controlled, another user can view the contents of that session
        /// and potentially interact with it.
        /// </summary>
        RemoteControl = 0x2001,

        /// <summary>
        /// This system metric is used in a Terminal Services environment. If the calling process is associated with a Terminal Services
        /// client session, the return value is nonzero. If the calling process is associated with the Terminal Services console session,
        /// the return value is 0.
        /// Windows Server 2003 and Windows XP:  The console session is not necessarily the physical console.
        /// For more information, seeWTSGetActiveConsoleSessionId.
        /// </summary>
        RemoteSession = 0x1000,

        /// <summary>
        /// Nonzero if all the display monitors have the same color format, otherwise, 0. Two displays can have the same bit depth,
        /// but different color formats. For example, the red, green, and blue pixels can be encoded with different numbers of bits,
        /// or those bits can be located in different places in a pixel color value.
        /// </summary>
        SameDisplayFormat = 81,

        /// <summary>
        /// This system metric should be ignored; it always returns 0.
        /// </summary>
        Secure = 44,

        /// <summary>
        /// The build number if the system is Windows Server 2003 R2; otherwise, 0.
        /// </summary>
        ServerR2 = 89,

        /// <summary>
        /// Nonzero if the user requires an application to present information visually in situations where it would otherwise present
        /// the information only in audible form; otherwise, 0.
        /// </summary>
        ShowSounds = 70,

        /// <summary>
        /// Nonzero if the current session is shutting down; otherwise, 0. Windows 2000:  This value is not supported.
        /// </summary>
        ShuttingDown = 0x2000,

        /// <summary>
        /// Nonzero if the computer has a low-end (slow) processor; otherwise, 0.
        /// </summary>
        SlowMachine = 73,

        /// <summary>
        /// Nonzero if the current operating system is Windows 7 Starter Edition, Windows Vista Starter, or Windows XP Starter Edition; otherwise, 0.
        /// </summary>
        Starter = 88,

        /// <summary>
        /// Nonzero if the meanings of the left and right mouse buttons are swapped; otherwise, 0.
        /// </summary>
        SwapButton = 23,

        /// <summary>
        /// Nonzero if the current operating system is the Windows XP Tablet PC edition or if the current operating system is Windows Vista
        /// or Windows 7 and the Tablet PC Input service is started; otherwise, 0. The DIGITIZER setting indicates the type of digitizer
        /// input supported by a device running Windows 7 or Windows Server 2008 R2. For more information, see Remarks.
        /// </summary>
        TabletPC = 86,

        /// <summary>
        /// The coordinates for the left side of the virtual screen. The virtual screen is the bounding rectangle of all display monitors.
        /// The CXVIRTUALSCREEN metric is the width of the virtual screen.
        /// </summary>
        XVirtualScreen = 76,

        /// <summary>
        /// The coordinates for the top of the virtual screen. The virtual screen is the bounding rectangle of all display monitors.
        /// The CYVIRTUALSCREEN metric is the height of the virtual screen.
        /// </summary>
        YVirtualScreen = 77,
    }

    internal enum MonitorDpiType : int
    {
        EffectiveDpi = 0,
        AngularDpi = 1,
        RawDpi = 2,
        Default = EffectiveDpi,
    }

    internal enum ProcessDPIAwareness : int
    {
        DpiUnaware = 0,
        SystemDpiAware = 1,
        PerMonitorDpiAware = 2,
    }

    internal enum DpiAwarenessContext : int
    {
        Unaware = -1,
        SystemAware = -2,
        PerMonitorAware = -3,
        PerMonitorAwareV2 = -4,
        UnawareGDIScaled = -5,
    }

    internal enum DBT : int
    {
        /// <summary>
        /// A device has been added to or removed from the system.
        /// </summary>
        DevNodesChanged = 0x0007,

        /// <summary>
        /// Permission is requested to change the current configuration (dock or undock).
        /// </summary>
        QueryChangeConfig = 0x0017,

        /// <summary>
        /// The current configuration has changed, due to a dock or undock.
        /// </summary>
        ConfigChanged = 0x0018,

        /// <summary>
        /// A request to change the current configuration (dock or undock) has been canceled.
        /// </summary>
        ConfigChangedCanceled = 0x0019,

        /// <summary>
        /// A device or piece of media has been inserted and is now available.
        /// </summary>
        DeviceArrival = 0x8000,

        /// <summary>
        /// Permission is requested to remove a device or piece of media. Any application can deny this request and cancel the removal.
        /// </summary>
        DeviceQueryRemove = 0x8001,

        /// <summary>
        /// A request to remove a device or piece of media has been canceled.
        /// </summary>
        DeviceQueryMoveFailed = 0x8002,

        /// <summary>
        /// A request to remove a device or piece of media has been canceled.
        /// </summary>
        DeviceRemovePending = 0x8003,

        /// <summary>
        /// A device or piece of media has been removed.
        /// </summary>
        DeviceRemoveComplete = 0x8004,

        /// <summary>
        /// A device-specific event has occurred.
        /// </summary>
        DeviceTypeSpecific = 0x8005,

        /// <summary>
        /// A custom event has occurred.
        /// </summary>
        CustomEvent = 0x8006,

        /// <summary>
        /// The meaning of this message is user-defined.
        /// </summary>
        UserDefined = 0xFFFF,
    }

    [Flags]
    internal enum DM : uint
    {
        /// <summary> dmOrientation member is present </summary>
        Orientation = 0x00000001,

        /// <summary> dmPaperSize member is present </summary>
        PaperSize = 0x00000002,

        /// <summary> dmPaperength member is present </summary>
        Paperength = 0x00000004,

        /// <summary> dmPaperWidth member is present </summary>
        PaperWidth = 0x00000008,

        /// <summary> dmScale member is present </summary>
        Scale = 0x00000010,

        /// <summary> dmCopies member is present </summary>
        Copies = 0x00000100,

        /// <summary> dmDefaultSource member is present </summary>
        DefaultSource = 0x00000200,

        /// <summary> dmPrintQuality member is present </summary>
        PrintQuality = 0x00000400,

        /// <summary> dmPosition member is present </summary>
        Position = 0x00000020,

        /// <summary> dmDisplayOrientation member is present </summary>
        DisplayOrientation = 0x00000080,

        /// <summary> dmDisplayFixedOutput member is present </summary>
        DisplayFixedOutput = 0x20000000,

        /// <summary> dmColor member is present </summary>
        Color = 0x00000800,

        /// <summary> dmDuplex member is present </summary>
        Duplex = 0x00001000,

        /// <summary> dmYResolution member is present </summary>
        YResolution = 0x00002000,

        /// <summary> dmTTOption member is present </summary>
        TTOption = 0x00004000,

        /// <summary> dmCollate member is present </summary>
        Collate = 0x00008000,

        /// <summary> dmFormName member is present </summary>
        FormName = 0x00010000,

        /// <summary> dmogPixels member is present </summary>
        ogPixels = 0x00020000,

        /// <summary> dmBitsPerPel member is present </summary>
        BitsPerPel = 0x00040000,

        /// <summary> dmPelsWidth member is present </summary>
        PelsWidth = 0x00080000,

        /// <summary> dmPelsHeight member is present </summary>
        PelsHeight = 0x00100000,

        /// <summary> dmDisplayFlags member is present </summary>
        DisplayFlags = 0x00200000,

        /// <summary> dmNup member is present </summary>
        Nup = 0x00000040,

        /// <summary> dmDisplayFrequency member is present </summary>
        DisplayFrequency = 0x00400000,

        /// <summary> dmICMMethod member is present </summary>
        ICMMethod = 0x00800000,

        /// <summary> dmICMIntent member is present </summary>
        ICMIntent = 0x01000000,

        /// <summary> dmMediaType member is present </summary>
        MediaType = 0x02000000,

        /// <summary> dmDitherType member is present </summary>
        DitherType = 0x04000000,

        /// <summary> dmPanningWidth member is present </summary>
        PanningWidth = 0x08000000,

        /// <summary> dmPanningHeight member is present </summary>
        PanningHeight = 0x10000000,
    }

    internal enum DMDFO : uint
    {
        /// <summary>
        /// The display's default setting.
        /// </summary>
        DMDFO_DEFAULT = 0,

        /// <summary>
        /// The low-resolution image is centered in the larger screen space.
        /// </summary>
        DMDFO_CENTER = 1,

        /// <summary>
        /// The low-resolution image is stretched to fill the larger screen space.
        /// </summary>
        DMDFO_STRETCH = 2,
    }

    [Flags]
    internal enum DisplayDeviceStateFlags : uint
    {
        /// <summary>
        /// <see cref="Active"/> specifies whether a monitor
        /// is presented as being "on" by the respective GDI view.
        ///
        /// Windows Vista:
        /// EnumDisplayDevices will only enumerate monitors that
        /// can be presented as being "on."
        /// </summary>
        Active = 0x00000001,

        /// <summary>
        /// TODO: Find documentation.
        /// </summary>
        MultiDriver = 0x00000002,

        /// <summary>
        /// Represents a pseudo device used to mirror application
        /// drawing for remoting or other purposes.
        /// An invisible pseudo monitor is associated with this device.
        /// For example, NetMeeting uses it.
        /// Note that GetSystemMetrics (<see cref="SystemMetric.CMonitors"/>)
        /// only accounts for visible display monitors.
        /// </summary>
        MirroringDriver = 0x00000008,

        /// <summary>
        /// The device has more display modes than its output devices support.
        /// </summary>
        ModesPruned = 0x08000000,

        /// <summary>
        /// The primary desktop is on the device.
        /// For a system with a single display card, this is always set.
        /// For a system with multiple display cards, only one device can have this set.
        /// </summary>
        PrimaryDevice = 0x00000004,

        /// <summary>
        /// The device is removable; it cannot be the primary display.
        /// </summary>
        Removable = 0x00000020,

        /// <summary>
        /// The device is VGA compatible.
        /// </summary>
        VGACompatible = 0x00000010,

        /// <summary>
        /// TODO: Find documentation.
        /// </summary>
        Remote = 0x04000000,

        /// <summary>
        /// TODO: Find documentation.
        /// </summary>
        Disconnect = 0x02000000,
    }

    internal enum RegOption : uint
    {
        /// <summary>
        /// The key is a symbolic link.
        /// Registry symbolic links should only be used when absolutely necessary.
        /// </summary>
        OpenLink = 0x00000008
    }

    /// <remarks>Sometimes called REGSAM.</remarks>
    [Flags]
    internal enum AccessMask : uint
    {
        // --- Standard access rights ---

        /// <summary>
        /// The right to delete the object.
        /// </summary>
        Delete = 0x00010000,

        /// <summary>
        /// The right to read the information in the object's security descriptor,
        /// not including the information in the system access control list (SACL).
        /// </summary>
        ReadControl = 0x00020000,

        /// <summary>
        /// The right to modify the discretionary access control list (DACL)
        /// in the object's security descriptor.
        /// </summary>
        WriteDAC = 0x00040000,

        /// <summary>
        /// The right to change the owner in the object's security descriptor.
        /// </summary>
        WriteOwner = 0x00080000,

        /// <summary>
        /// The right to use the object for synchronization.
        /// This enables a thread to wait until the object is in the signaled state.
        /// Some object types do not support this access right.
        /// </summary>
        Synchronize = 0x00100000,

        // --- Registry access rights ---

        /// <summary>
        /// Combines the STANDARD_RIGHTS_REQUIRED, KEY_QUERY_VALUE,
        /// KEY_SET_VALUE, KEY_CREATE_SUB_KEY, KEY_ENUMERATE_SUB_KEYS,
        /// KEY_NOTIFY, and KEY_CREATE_LINK access rights.
        /// </summary>
        KeyAllAccess = 0xF003F,

        /// <summary>
        /// Reserved for system use.
        /// </summary>
        KeyCreateLink = 0x0020,

        /// <summary>
        /// Required to create a subkey of a registry key.
        /// </summary>
        KeyCreateSubKey = 0x0004,

        /// <summary>
        /// Required to enumerate the subkeys of a registry key.
        /// </summary>
        KeyEnumerateSubKeys = 0x0008,

        /// <summary>
        /// Equivalent to KEY_READ.
        /// </summary>
        KeyExecute = 0x20019,

        /// <summary>
        /// Required to request change notifications for a
        /// registry key or for subkeys of a registry key.
        /// </summary>
        KeyNotify = 0x0010,

        /// <summary>
        /// Required to query the values of a registry key.
        /// </summary>
        KeyQueryValue = 0x0001,

        /// <summary>
        /// Combines the STANDARD_RIGHTS_READ, KEY_QUERY_VALUE,
        /// KEY_ENUMERATE_SUB_KEYS, and KEY_NOTIFY values.
        /// </summary>
        KeyRead = 0x20019,

        /// <summary>
        /// Required to create, delete, or set a registry value.
        /// </summary>
        KeySetValue = 0x0002,

        /// <summary>
        /// Indicates that an application on 64-bit Windows should
        /// operate on the 32-bit registry view.
        /// This flag is ignored by 32-bit Windows.
        /// For more information, see Accessing an Alternate Registry View.
        /// This flag must be combined using the OR operator with the
        /// other flags in this table that either query or access registry values.
        /// Windows 2000: This flag is not supported.
        /// </summary>
        KeyWOW64_32Key = 0x0200,

        /// <summary>
        /// Indicates that an application on 64-bit Windows should
        /// operate on the 64-bit registry view.
        /// This flag is ignored by 32-bit Windows.
        /// For more information, see Accessing an Alternate Registry View.
        /// This flag must be combined using the OR operator with the
        /// other flags in this table that either query or access registry values.
        /// Windows 2000: This flag is not supported.
        /// </summary>
        KeyWOW64_64Key = 0x0100,

        /// <summary>
        /// Combines the STANDARD_RIGHTS_WRITE, KEY_SET_VALUE,
        /// and KEY_CREATE_SUB_KEY access rights.
        /// </summary>
        KeyWrite = 0x20006,
    }

    internal enum PredefinedKeys : uint {
        /// <summary>
        /// Registry entries subordinate to this key define types (or classes)
        /// of documents and the properties associated with those types.
        /// Shell and COM applications use the information stored under this key.
        /// This key also provides backward compatibility with the Windows 3.1 registration
        /// database by storing information for DDE and OLE support.
        /// File viewers and user interface extensions store their OLE class identifiers
        /// in HKEY_CLASSES_ROOT, and in-process servers are registered in this key.
        /// This handle should not be used in a service or an application that
        /// impersonates different users.
        /// For more information, see HKEY_CLASSES_ROOT.
        /// </summary>
        HKEY_CLASSES_ROOT = 0x80000000,

        /// <summary>
        /// Registry entries subordinate to this key define the preferences of the current user.
        /// These preferences include the settings of environment variables,
        /// data about program groups, colors, printers, network connections,
        /// and application preferences.
        /// This key makes it easier to establish the current user's settings;
        /// the key maps to the current user's branch in HKEY_USERS.
        /// In HKEY_CURRENT_USER, software vendors store the current user-specific preferences
        /// to be used within their applications.
        /// Microsoft, for example, creates the HKEY_CURRENT_USER\Software\Microsoft key
        /// for its applications to use,
        /// with each application creating its own subkey under the Microsoft key.
        /// 
        /// The mapping between HKEY_CURRENT_USER and HKEY_USERS is per process and
        /// is established the first time the process references HKEY_CURRENT_USER.
        /// The mapping is based on the security context of the first thread to
        /// reference HKEY_CURRENT_USER.
        /// If this security context does not have a registry hive loaded in HKEY_USERS,
        /// the mapping is established with HKEY_USERS\.Default.
        /// After this mapping is established it persists,
        /// even if the security context of the thread changes.
        /// 
        /// All registry entries in HKEY_CURRENT_USER except those under
        /// HKEY_CURRENT_USER\Software\Classes are included in the per-user registry
        /// portion of a roaming user profile.
        /// To exclude other entries from a roaming user profile,
        /// store them in HKEY_CURRENT_USER_LOCAL_SETTINGS.
        /// This handle should not be used in a service or an application
        /// that impersonates different users.
        /// Instead, call the RegOpenCurrentUser function.
        /// For more information, see HKEY_CURRENT_USER.
        /// </summary>
        HKEY_CURRENT_USER = 0x80000001,

        /// <summary>
        /// Registry entries subordinate to this key define the physical state of the computer,
        /// including data about the bus type, system memory, and installed hardware and software.
        /// It contains subkeys that hold current configuration data,
        /// including Plug and Play information
        /// (the Enum branch, which includes a complete list of all hardware that
        /// has ever been on the system), network logon preferences,
        /// network security information, software-related information
        /// (such as server names and the location of the server),
        /// and other system information.
        /// For more information, see HKEY_LOCAL_MACHINE.
        /// </summary>
        HKEY_LOCAL_MACHINE = 0x80000002,

        /// <summary>
        /// Registry entries subordinate to this key define the default user configuration
        /// for new users on the local computer and the user configuration for the current user.
        /// </summary>
        HKEY_USERS = 0x80000003,

        /// <summary>
        /// Registry entries subordinate to this key allow you to access performance data.
        /// The data is not actually stored in the registry;
        /// the registry functions cause the system to collect the data from its source.
        /// </summary>
        HKEY_PERFORMANCE_DATA = 0x80000004,

        /// <summary>
        /// Contains information about the current hardware profile of the local computer system.
        /// The information under HKEY_CURRENT_CONFIG describes only the differences
        /// between the current hardware configuration and the standard configuration.
        /// Information about the standard hardware configuration is stored under the
        /// Software and System keys of HKEY_LOCAL_MACHINE.
        /// HKEY_CURRENT_CONFIG is an alias for
        /// HKEY_LOCAL_MACHINE\System\CurrentControlSet\Hardware Profiles\Current.
        /// For more information, see HKEY_CURRENT_CONFIG.
        /// </summary>
        HKEY_CURRENT_CONFIG = 0x80000005,

        HKEY_DYN_DATA = 0x80000006,

        /// <summary>
        /// Registry entries subordinate to this key reference the text strings
        /// that describe counters in US English.
        /// These entries are not available to Regedit.exe and Regedt32.exe.
        /// Windows 2000: This key is not supported.
        /// </summary>
        HKEY_PERFORMANCE_TEXT = 0x80000050,

        /// <summary>
        /// Registry entries subordinate to this key reference the text strings
        /// that describe counters in the local language of the area in which the
        /// computer system is running.
        /// These entries are not available to Regedit.exe and Regedt32.exe.
        /// Windows 2000: This key is not supported.
        /// </summary>
        HKEY_PERFORMANCE_NLSTEXT = 0x80000060,
    }

    [Flags]
    internal enum RRF : uint
    {
        /// <summary>
        /// No type restriction.
        /// </summary>
        TypeAny = 0x0000ffff,

        /// <summary>
        /// Restrict type to 32-bit TypeRegBinary | TypeRegDword.
        /// </summary>
        TypeDword = 0x00000018,

        /// <summary>
        /// Restrict type to 64-bit TypeRegBinary | TypeRegQword.
        /// </summary>
        TypeQword = 0x00000048,

        /// <summary>
        /// A 32-bit number in little-endian format.
        /// Windows is designed to run on little-endian computer architectures.
        /// Therefore, this value is defined as REG_DWORD in the Windows header files.
        /// </summary>
        REG_DWORD_LITTLE_ENDIAN,

        /// <summary>
        /// A 32-bit number in big-endian format. Some UNIX systems support big-endian architectures.
        /// </summary>
        REG_DWORD_BIG_ENDIAN,

        /// <summary>
        /// Restrict type to REG_BINARY.
        /// </summary>
        TypeRegBinary = 0x00000008,

        /// <summary>
        /// Restrict type to REG_DWORD.
        /// </summary>
        TypeRegDword = 0x00000010,

        /// <summary>
        /// Restrict type to REG_EXPAND_SZ.
        /// </summary>
        TypeRegExpandSZ = 0x00000004,

        /// <summary>
        /// Restrict type to REG_MULTI_SZ.
        /// </summary>
        TypeRegMultiSZ = 0x00000020,

        /// <summary>
        /// Restrict type to REG_NONE.
        /// </summary>
        TypeRegNone = 0x00000001,

        /// <summary>
        /// Restrict type to REG_QWORD.
        /// </summary>
        TypeRegQword = 0x00000040,

        /// <summary>
        /// Restrict type to REG_SZ.
        /// </summary>
        TypeRegSZ = 0x00000002,

        // This parameter can also include one or more of the following values.

        /// <summary>
        /// Do not automatically expand environment strings if the value is of type REG_EXPAND_SZ.
        /// </summary>
        NoExpand = 0x10000000,

        /// <summary>
        /// If pvData is not NULL, set the contents of the buffer to zeroes on failure.
        /// </summary>
        ZeroOnFailure = 0x20000000,

        /// <summary>
        /// If lpSubKey is not NULL, open the subkey that lpSubKey specifies with the KEY_WOW64_64KEY access rights.
        /// For information about these access rights, see Registry Key Security and Access Rights.
        /// You cannot specify SubKeyWOW6464Key in combination with SubKeyWOW6432Key.
        /// </summary>
        SubKeyWOW6464Key = 0x00010000,

        /// <summary>
        /// If lpSubKey is not NULL, open the subkey that lpSubKey specifies with the KEY_WOW64_32KEY access rights.
        /// For information about these access rights, see Registry Key Security and Access Rights.
        /// You cannot specify SubKeyWOW6432Key in combination with SubKeyWOW6464Key.
        /// </summary>
        SubKeyWOW6432Key = 0x00020000,

    }

    internal enum RegValueType : uint
    {
        /* Maybe we want to include these?
            #define REG_RESOURCE_LIST           ( 8 )   // Resource list in the resource map
            #define REG_FULL_RESOURCE_DESCRIPTOR ( 9 )  // Resource list in the hardware description
            #define REG_RESOURCE_REQUIREMENTS_LIST ( 10 )
        */

        /// <summary>
        /// Binary data in any form.
        /// </summary>
        Binary = 3,

        /// <summary>
        /// A 32-bit number.
        /// </summary>
        Dword = 4,

        /// <summary>
        /// A 32-bit number in little-endian format.
        /// Windows is designed to run on little-endian computer architectures.
        /// Therefore, this value is defined as REG_DWORD in the Windows header files.
        /// </summary>
        DwordLittleEndian = 4,

        /// <summary>
        /// A 32-bit number in big-endian format. Some UNIX systems support big-endian architectures.
        /// </summary>
        DwordBigEndian = 5,

        /// <summary>
        /// A null-terminated string that contains unexpanded references to
        /// environment variables (for example, "%PATH%").
        /// It will be a Unicode or ANSI string depending on whether you use the Unicode or ANSI functions.
        /// To expand the environment variable references, use the ExpandEnvironmentStrings function.
        /// </summary>
        ExpandSZ = 2,

        /// <summary>
        /// A null-terminated Unicode string that contains the target path of a symbolic
        /// link that was created by calling the RegCreateKeyEx function with REG_OPTION_CREATE_LINK.
        /// </summary>
        Link = 6,

        /// <summary>
        /// A sequence of null-terminated strings, terminated by an empty string (\0).
        /// The following is an example:
        /// String1\0String2\0String3\0LastString\0\0
        /// The first \0 terminates the first string,
        /// the second to the last \0 terminates the last string,
        /// and the final \0 terminates the sequence.
        /// Note that the final terminator must be factored into the length of the string.
        /// </summary>
        MultiSZ = 7,

        /// <summary>
        /// No defined value type.
        /// </summary>
        None = 0,

        /// <summary>
        /// A 64-bit number.
        /// </summary>
        Qword = 11,

        /// <summary>
        /// A 64-bit number in little-endian format.
        /// Windows is designed to run on little-endian computer architectures.
        /// Therefore, this value is defined as REG_QWORD in the Windows header files.
        /// </summary>
        QwordLittleEndian = 11,

        /// <summary>
        /// A null-terminated string.
        /// This will be either a Unicode or an ANSI string,
        /// depending on whether you use the Unicode or ANSI functions.
        /// </summary>
        SZ = 1,
    }

    [Flags]
    internal enum FormatMessage : uint
    {
        /// <summary>
        /// The function allocates a buffer large enough to hold the formatted message,
        /// and places a pointer to the allocated buffer at the address specified by lpBuffer.
        /// The lpBuffer parameter is a pointer to an LPTSTR;
        /// you must cast the pointer to an LPTSTR (for example, (LPTSTR)&lpBuffer).
        /// The nSize parameter specifies the minimum number of TCHARs to allocate for an output message buffer.
        /// The caller should use the LocalFree function to free the buffer when it is no longer needed.
        /// 
        /// If the length of the formatted message exceeds 128K bytes,
        /// then FormatMessage will fail and a subsequent call to GetLastError will return ERROR_MORE_DATA.
        ///
        /// In previous versions of Windows, this value was not available for use when compiling Windows Store apps.
        /// As of Windows 10 this value can be used.
        ///
        /// Windows Server 2003 and Windows XP:  
        ///
        /// If the length of the formatted message exceeds 128K bytes,
        /// then FormatMessage will not automatically fail with an error of ERROR_MORE_DATA.
        /// </summary>
        AllocateBuffer = 0x00000100,

        /// <summary>
        /// The Arguments parameter is not a va_list structure, but is a pointer to an array of values that represent the arguments.
        /// This flag cannot be used with 64-bit integer values. If you are using a 64-bit integer, you must use the va_list structure.
        /// </summary>
        ArgumentArray = 0x00002000,

        /// <summary>
        /// The lpSource parameter is a module handle containing the message-table resource(s) to search.
        /// If this lpSource handle is NULL, the current process's application image file will be searched.
        /// This flag cannot be used with FORMAT_MESSAGE_FROM_STRING.
        /// 
        /// If the module has no message table resource, the function fails with ERROR_RESOURCE_TYPE_NOT_FOUND.
        /// </summary>
        FromHModule = 0x00000800,

        /// <summary>
        /// The lpSource parameter is a pointer to a null-terminated string that contains a message definition.
        /// The message definition may contain insert sequences,
        /// just as the message text in a message table resource may.
        /// This flag cannot be used with FORMAT_MESSAGE_FROM_HMODULE or FORMAT_MESSAGE_FROM_SYSTEM.
        /// </summary>
        FromString = 0x00000400,

        /// <summary>
        /// The function should search the system message-table resource(s) for the requested message.
        /// If this flag is specified with FORMAT_MESSAGE_FROM_HMODULE,
        /// the function searches the system message table if the message is not found in the module specified by lpSource.
        /// This flag cannot be used with FORMAT_MESSAGE_FROM_STRING.
        /// 
        /// If this flag is specified, an application can pass the result of the GetLastError function to retrieve the message text for a system-defined error.
        /// </summary>
        FromSystem = 0x00001000,

        /// <summary>
        /// Insert sequences in the message definition such as %1 are to be ignored and passed through to the output buffer unchanged.
        /// This flag is useful for fetching a message for later formatting.
        /// If this flag is set, the Arguments parameter is ignored.
        /// </summary>
        IgnoreInserts = 0x00000200,
    }

    internal enum HIDUsagePage : ushort
    {
        /// <summary>
        /// Generic Desktop Controls
        /// </summary>
        Generic = 0x01,

        /// <summary>
        /// VR Controls
        /// </summary>
        VR = 0x03,

        /// <summary>
        /// Game Controls
        /// </summary>
        Game = 0x05,

        /// <summary>
        /// LEDs
        /// </summary>
        LED = 0x08,

        /// <summary>
        /// Button
        /// </summary>
        Button = 0x09,

        /// <summary>
        /// Haptics
        /// </summary>
        Haptics = 0x0E,
    }

    internal enum HIDUsageGeneric : ushort
    {
        Pointer = 0x01,
        Mouse = 0x02,
        Joystick = 0x04,
        Gamepad = 0x05,
        Keyboard = 0x06,
        Keypad = 0x07,
        MultiAxisController = 0x08,
    }

    [Flags]
    internal enum RIDEV : uint
    {
        /// <summary>
        /// If set, this removes the top level collection from the inclusion list. This tells the operating system to stop reading from a device which matches the top level collection.
        /// </summary>
        Remove = 0x00000001,

        /// <summary>
        /// If set, this specifies the top level collections to exclude when reading a complete usage page. This flag only affects a TLC whose usage page is already specified with RIDEV_PAGEONLY.
        /// </summary>
        Exclude = 0x00000010,

        /// <summary>
        /// If set, this specifies all devices whose top level collection is from the specified usUsagePage. Note that usUsage must be zero. To exclude a particular top level collection, use RIDEV_EXCLUDE.
        /// </summary>
        PageOnly = 0x00000020,

        /// <summary>
        /// If set, this prevents any devices specified by usUsagePage or usUsage from generating legacy messages. This is only for the mouse and keyboard. See Remarks.
        /// </summary>
        NoLegacy = 0x00000030,

        /// <summary>
        /// If set, this enables the caller to receive the input even when the caller is not in the foreground. Note that hwndTarget must be specified.
        /// </summary>
        InputSink = 0x00000100,

        /// <summary>
        /// If set, the mouse button click does not activate the other window. RIDEV_CAPTUREMOUSE can be specified only if RIDEV_NOLEGACY is specified for a mouse device.
        /// </summary>
        CaptureMouse = 0x00000200,

        /// <summary>
        /// If set, the application-defined keyboard device hotkeys are not handled. However, the system hotkeys; for example, ALT+TAB and CTRL+ALT+DEL, are still handled. By default, all keyboard hotkeys are handled. RIDEV_NOHOTKEYS can be specified even if RIDEV_NOLEGACY is not specified and hwndTarget is NULL.
        /// </summary>
        NoHotkeys = 0x00000200,

        /// <summary>
        /// If set, the application command keys are handled. RIDEV_APPKEYS can be specified only if RIDEV_NOLEGACY is specified for a keyboard device.
        /// </summary>
        AppKeys = 0x00000400,

        /// <summary>
        /// If set, this enables the caller to receive input in the background only if the foreground application does not process it. In other words, if the foreground application is not registered for raw input, then the background application that is registered will receive the input.
        ///
        /// Windows XP: This flag is not supported until Windows Vista
        /// </summary>
        ExInputSink = 0x00001000,

        /// <summary>
        /// If set, this enables the caller to receive WM_INPUT_DEVICE_CHANGE notifications for device arrival and device removal.
        ///
        /// Windows XP: This flag is not supported until Windows Vista
        /// </summary>
        DevNotify = 0x00002000,
    }

    internal enum RID : uint
    {
        /// <summary>
        /// Get the header information from the RAWINPUT structure.
        /// </summary>
        Header = 0x10000005,

        /// <summary>
        /// Get the raw data from the RAWINPUT structure.
        /// </summary>
        Input = 0x10000003,
    }

    internal enum RIM : uint
    {
        /// <summary>
        /// Raw input comes from the mouse.
        /// </summary>
        TypeMouse = 0,

        /// <summary>
        /// Raw input comes from the keyboard.
        /// </summary>
        TypeKeyboard = 1,

        /// <summary>
        /// Raw input comes from some device that is not a keyboard or a mouse.
        /// </summary>
        TypeHID = 2,
    }

    internal enum RIDI : uint
    {
        /// <summary>
        /// pData is a PHIDP_PREPARSED_DATA pointer to a buffer for a top-level collection's preparsed data.
        /// </summary>
        PreParsedData = 0x20000005,

        /// <summary>
        /// pData points to a string that contains the device interface name.
        /// If this device is opened with Shared Access Mode then you can call CreateFile with this name to open a HID collection and use returned handle for calling ReadFile to read input reports and WriteFile to send output reports.
        /// 
        /// For more information, see Opening HID Collections and Handling HID Reports.
        ///
        /// For this uiCommand only, the value in pcbSize is the character count (not the byte count).
        /// </summary>
        DeviceName = 0x20000007,

        /// <summary>
        /// pData points to an RID_DEVICE_INFO structure.
        /// </summary>
        DeviceInfo = 0x2000000b,
    }

    internal enum GIDC : ulong
    {
        /// <summary>
        /// A new device has been added to the system.
        ///
        /// You can call GetRawInputDeviceInfo to get more information regarding the device.
        /// </summary>
        Arrival = 1,

        /// <summary>
        /// A device has been removed from the system.
        /// </summary>
        Removal = 2,
    }

    // FIXME: Proper name
    [Flags]
    internal enum RID_MOUSE_DEVICE_PROPERTIES : uint
    {
        /// <summary>
        /// HID mouse
        /// </summary>
        MOUSE_HID_HARDWARE = 0x0080,

        /// <summary>
        /// HID wheel mouse
        /// </summary>
        WHEELMOUSE_HID_HARDWARE = 0x0100,

        /// <summary>
        /// Mouse with horizontal wheel
        /// </summary>
        HORIZONTAL_WHEEL_PRESENT = 0x8000,
    }

    public enum HIDPStatus : uint
    {
        // These values are taken from: https://github.com/tpn/winsdk-10/blob/9b69fd26ac0c7d0b83d378dba01080e93349c2ed/Include/10.0.16299.0/shared/hidpi.h#L1801

        HIDP_STATUS_SUCCESS = 0 << 28 | 0x11 << 16 | 0,

        HIDP_STATUS_NULL = unchecked((uint)0x8 << 28 | 0x11 << 16 | 1),

        /// <summary>
        /// The specified preparsed data is invalid.
        /// </summary>
        HIDP_STATUS_INVALID_PREPARSED_DATA = unchecked((uint)0xC << 28 | 0x11 << 16 | 1),

        HIDP_STATUS_INVALID_REPORT_TYPE = unchecked((uint)0xC << 28 | 0x11 << 16 | 2),

        HIDP_STATUS_INVALID_REPORT_LENGTH = unchecked((uint)0xC << 28 | 0x11 << 16 | 3),

        HIDP_STATUS_USAGE_NOT_FOUND = unchecked((uint)0xC << 28 | 0x11 << 16 | 4),

        HIDP_STATUS_VALUE_OUT_OF_RANGE = unchecked((uint)0xC << 28 | 0x11 << 16 | 5),

        HIDP_STATUS_BAD_LOG_PHY_VALUES = unchecked((uint)0xC << 28 | 0x11 << 16 | 6),

        HIDP_STATUS_BUFFER_TOO_SMALL = unchecked((uint)0xC << 28 | 0x11 << 16 | 7),

        HIDP_STATUS_INTERNAL_ERROR = unchecked((uint)0xC << 28 | 0x11 << 16 | 8),

        HIDP_STATUS_I8042_TRANS_UNKNOWN = unchecked((uint)0xC << 28 | 0x11 << 16 | 9),

        HIDP_STATUS_INCOMPATIBLE_REPORT_ID = unchecked((uint)0xC << 28 | 0x11 << 16 | 0xA),

        HIDP_STATUS_NOT_VALUE_ARRAY = unchecked((uint)0xC << 28 | 0x11 << 16 | 0xB),

        HIDP_STATUS_IS_VALUE_ARRAY = unchecked((uint)0xC << 28 | 0x11 << 16 | 0xC),

        HIDP_STATUS_DATA_INDEX_NOT_FOUND = unchecked((uint)0xC << 28 | 0x11 << 16 | 0xD),

        HIDP_STATUS_DATA_INDEX_OUT_OF_RANGE = unchecked((uint)0xC << 28 | 0x11 << 16 | 0xE),

        HIDP_STATUS_BUTTON_NOT_PRESSED = unchecked((uint)0xC << 28 | 0x11 << 16 | 0xF),

        HIDP_STATUS_REPORT_DOES_NOT_EXIST = unchecked((uint)0xC << 28 | 0x11 << 16 | 0x10),

        HIDP_STATUS_NOT_IMPLEMENTED = unchecked((uint)0xC << 28 | 0x11 << 16 | 0x20),
    }

    /// <summary>
    /// Windows Messages
    /// Defined in winuser.h from Windows SDK v6.1
    /// Documentation pulled from MSDN.
    /// </summary>
    public enum WM : uint
    {
        /// <summary>
        /// The WM_NULL message performs no operation. An application sends the WM_NULL message if it wants to post a message that the recipient window will ignore.
        /// </summary>
        NULL = 0x0000,

        /// <summary>
        /// The WM_CREATE message is sent when an application requests that a window be created by calling the CreateWindowEx or CreateWindow function. (The message is sent before the function returns.) The window procedure of the new window receives this message after the window is created, but before the window becomes visible.
        /// </summary>
        CREATE = 0x0001,

        /// <summary>
        /// The WM_DESTROY message is sent when a window is being destroyed. It is sent to the window procedure of the window being destroyed after the window is removed from the screen.
        /// This message is sent first to the window being destroyed and then to the child windows (if any) as they are destroyed. During the processing of the message, it can be assumed that all child windows still exist.
        /// /// </summary>
        DESTROY = 0x0002,

        /// <summary>
        /// The WM_MOVE message is sent after a window has been moved.
        /// </summary>
        MOVE = 0x0003,

        /// <summary>
        /// The WM_SIZE message is sent to a window after its size has changed.
        /// </summary>
        SIZE = 0x0005,

        /// <summary>
        /// The WM_ACTIVATE message is sent to both the window being activated and the window being deactivated. If the windows use the same input queue, the message is sent synchronously, first to the window procedure of the top-level window being deactivated, then to the window procedure of the top-level window being activated. If the windows use different input queues, the message is sent asynchronously, so the window is activated immediately.
        /// </summary>
        ACTIVATE = 0x0006,

        /// <summary>
        /// The WM_SETFOCUS message is sent to a window after it has gained the keyboard focus.
        /// </summary>
        SETFOCUS = 0x0007,

        /// <summary>
        /// The WM_KILLFOCUS message is sent to a window immediately before it loses the keyboard focus.
        /// </summary>
        KILLFOCUS = 0x0008,

        /// <summary>
        /// The WM_ENABLE message is sent when an application changes the enabled state of a window. It is sent to the window whose enabled state is changing. This message is sent before the EnableWindow function returns, but after the enabled state (WS_DISABLED style bit) of the window has changed.
        /// </summary>
        ENABLE = 0x000A,

        /// <summary>
        /// An application sends the WM_SETREDRAW message to a window to allow changes in that window to be redrawn or to prevent changes in that window from being redrawn.
        /// </summary>
        SETREDRAW = 0x000B,

        /// <summary>
        /// An application sends a WM_SETTEXT message to set the text of a window.
        /// </summary>
        SETTEXT = 0x000C,

        /// <summary>
        /// An application sends a WM_GETTEXT message to copy the text that corresponds to a window into a buffer provided by the caller.
        /// </summary>
        GETTEXT = 0x000D,

        /// <summary>
        /// An application sends a WM_GETTEXTLENGTH message to determine the length, in characters, of the text associated with a window.
        /// </summary>
        GETTEXTLENGTH = 0x000E,

        /// <summary>
        /// The WM_PAINT message is sent when the system or another application makes a request to paint a portion of an application's window. The message is sent when the UpdateWindow or RedrawWindow function is called, or by the DispatchMessage function when the application obtains a WM_PAINT message by using the GetMessage or PeekMessage function.
        /// </summary>
        PAINT = 0x000F,

        /// <summary>
        /// The WM_CLOSE message is sent as a signal that a window or an application should terminate.
        /// </summary>
        CLOSE = 0x0010,

        /// <summary>
        /// The WM_QUERYENDSESSION message is sent when the user chooses to end the session or when an application calls one of the system shutdown functions. If any application returns zero, the session is not ended. The system stops sending WM_QUERYENDSESSION messages as soon as one application returns zero.
        /// After processing this message, the system sends the WM_ENDSESSION message with the wParam parameter set to the results of the WM_QUERYENDSESSION message.
        /// </summary>
        QUERYENDSESSION = 0x0011,

        /// <summary>
        /// The WM_QUERYOPEN message is sent to an icon when the user requests that the window be restored to its previous size and position.
        /// </summary>
        QUERYOPEN = 0x0013,

        /// <summary>
        /// The WM_ENDSESSION message is sent to an application after the system processes the results of the WM_QUERYENDSESSION message. The WM_ENDSESSION message informs the application whether the session is ending.
        /// </summary>
        ENDSESSION = 0x0016,

        /// <summary>
        /// The WM_QUIT message indicates a request to terminate an application and is generated when the application calls the PostQuitMessage function. It causes the GetMessage function to return zero.
        /// </summary>
        QUIT = 0x0012,

        /// <summary>
        /// The WM_ERASEBKGND message is sent when the window background must be erased (for example, when a window is resized). The message is sent to prepare an invalidated portion of a window for painting.
        /// </summary>
        ERASEBKGND = 0x0014,

        /// <summary>
        /// This message is sent to all top-level windows when a change is made to a system color setting.
        /// </summary>
        SYSCOLORCHANGE = 0x0015,

        /// <summary>
        /// The WM_SHOWWINDOW message is sent to a window when the window is about to be hidden or shown.
        /// </summary>
        SHOWWINDOW = 0x0018,

        /// <summary>
        /// An application sends the WM_WININICHANGE message to all top-level windows after making a change to the WIN.INI file. The SystemParametersInfo function sends this message after an application uses the function to change a setting in WIN.INI.
        /// Note  The WM_WININICHANGE message is provided only for compatibility with earlier versions of the system. Applications should use the WM_SETTINGCHANGE message.
        /// </summary>
        WININICHANGE = 0x001A,

        /// <summary>
        /// An application sends the WM_WININICHANGE message to all top-level windows after making a change to the WIN.INI file. The SystemParametersInfo function sends this message after an application uses the function to change a setting in WIN.INI.
        /// Note  The WM_WININICHANGE message is provided only for compatibility with earlier versions of the system. Applications should use the WM_SETTINGCHANGE message.
        /// </summary>
        SETTINGCHANGE = WININICHANGE,

        /// <summary>
        /// The WM_DEVMODECHANGE message is sent to all top-level windows whenever the user changes device-mode settings.
        /// </summary>
        DEVMODECHANGE = 0x001B,

        /// <summary>
        /// The WM_ACTIVATEAPP message is sent when a window belonging to a different application than the active window is about to be activated. The message is sent to the application whose window is being activated and to the application whose window is being deactivated.
        /// </summary>
        ACTIVATEAPP = 0x001C,

        /// <summary>
        /// An application sends the WM_FONTCHANGE message to all top-level windows in the system after changing the pool of font resources.
        /// </summary>
        FONTCHANGE = 0x001D,

        /// <summary>
        /// A message that is sent whenever there is a change in the system time.
        /// </summary>
        TIMECHANGE = 0x001E,

        /// <summary>
        /// The WM_CANCELMODE message is sent to cancel certain modes, such as mouse capture. For example, the system sends this message to the active window when a dialog box or message box is displayed. Certain functions also send this message explicitly to the specified window regardless of whether it is the active window. For example, the EnableWindow function sends this message when disabling the specified window.
        /// </summary>
        CANCELMODE = 0x001F,

        /// <summary>
        /// The WM_SETCURSOR message is sent to a window if the mouse causes the cursor to move within a window and mouse input is not captured.
        /// </summary>
        SETCURSOR = 0x0020,

        /// <summary>
        /// The WM_MOUSEACTIVATE message is sent when the cursor is in an inactive window and the user presses a mouse button. The parent window receives this message only if the child window passes it to the DefWindowProc function.
        /// </summary>
        MOUSEACTIVATE = 0x0021,

        /// <summary>
        /// The WM_CHILDACTIVATE message is sent to a child window when the user clicks the window's title bar or when the window is activated, moved, or sized.
        /// </summary>
        CHILDACTIVATE = 0x0022,

        /// <summary>
        /// The WM_QUEUESYNC message is sent by a computer-based training (CBT) application to separate user-input messages from other messages sent through the WH_JOURNALPLAYBACK Hook procedure.
        /// </summary>
        QUEUESYNC = 0x0023,

        /// <summary>
        /// The WM_GETMINMAXINFO message is sent to a window when the size or position of the window is about to change. An application can use this message to override the window's default maximized size and position, or its default minimum or maximum tracking size.
        /// </summary>
        GETMINMAXINFO = 0x0024,

        /// <summary>
        /// Windows NT 3.51 and earlier: The WM_PAINTICON message is sent to a minimized window when the icon is to be painted. This message is not sent by newer versions of Microsoft Windows, except in unusual circumstances explained in the Remarks.
        /// </summary>
        PAINTICON = 0x0026,

        /// <summary>
        /// Windows NT 3.51 and earlier: The WM_ICONERASEBKGND message is sent to a minimized window when the background of the icon must be filled before painting the icon. A window receives this message only if a class icon is defined for the window; otherwise, WM_ERASEBKGND is sent. This message is not sent by newer versions of Windows.
        /// </summary>
        ICONERASEBKGND = 0x0027,

        /// <summary>
        /// The WM_NEXTDLGCTL message is sent to a dialog box procedure to set the keyboard focus to a different control in the dialog box.
        /// </summary>
        NEXTDLGCTL = 0x0028,

        /// <summary>
        /// The WM_SPOOLERSTATUS message is sent from Print Manager whenever a job is added to or removed from the Print Manager queue.
        /// </summary>
        SPOOLERSTATUS = 0x002A,

        /// <summary>
        /// The WM_DRAWITEM message is sent to the parent window of an owner-drawn button, combo box, list box, or menu when a visual aspect of the button, combo box, list box, or menu has changed.
        /// </summary>
        DRAWITEM = 0x002B,

        /// <summary>
        /// The WM_MEASUREITEM message is sent to the owner window of a combo box, list box, list view control, or menu item when the control or menu is created.
        /// </summary>
        MEASUREITEM = 0x002C,

        /// <summary>
        /// Sent to the owner of a list box or combo box when the list box or combo box is destroyed or when items are removed by the LB_DELETESTRING, LB_RESETCONTENT, CB_DELETESTRING, or CB_RESETCONTENT message. The system sends a WM_DELETEITEM message for each deleted item. The system sends the WM_DELETEITEM message for any deleted list box or combo box item with nonzero item data.
        /// </summary>
        DELETEITEM = 0x002D,

        /// <summary>
        /// Sent by a list box with the LBS_WANTKEYBOARDINPUT style to its owner in response to a WM_KEYDOWN message.
        /// </summary>
        VKEYTOITEM = 0x002E,

        /// <summary>
        /// Sent by a list box with the LBS_WANTKEYBOARDINPUT style to its owner in response to a WM_CHAR message.
        /// </summary>
        CHARTOITEM = 0x002F,

        /// <summary>
        /// An application sends a WM_SETFONT message to specify the font that a control is to use when drawing text.
        /// </summary>
        SETFONT = 0x0030,

        /// <summary>
        /// An application sends a WM_GETFONT message to a control to retrieve the font with which the control is currently drawing its text.
        /// </summary>
        GETFONT = 0x0031,

        /// <summary>
        /// An application sends a WM_SETHOTKEY message to a window to associate a hot key with the window. When the user presses the hot key, the system activates the window.
        /// </summary>
        SETHOTKEY = 0x0032,

        /// <summary>
        /// An application sends a WM_GETHOTKEY message to determine the hot key associated with a window.
        /// </summary>
        GETHOTKEY = 0x0033,

        /// <summary>
        /// The WM_QUERYDRAGICON message is sent to a minimized (iconic) window. The window is about to be dragged by the user but does not have an icon defined for its class. An application can return a handle to an icon or cursor. The system displays this cursor or icon while the user drags the icon.
        /// </summary>
        QUERYDRAGICON = 0x0037,

        /// <summary>
        /// The system sends the WM_COMPAREITEM message to determine the relative position of a new item in the sorted list of an owner-drawn combo box or list box. Whenever the application adds a new item, the system sends this message to the owner of a combo box or list box created with the CBS_SORT or LBS_SORT style.
        /// </summary>
        COMPAREITEM = 0x0039,

        /// <summary>
        /// Active Accessibility sends the WM_GETOBJECT message to obtain information about an accessible object contained in a server application.
        /// Applications never send this message directly. It is sent only by Active Accessibility in response to calls to AccessibleObjectFromPoint, AccessibleObjectFromEvent, or AccessibleObjectFromWindow. However, server applications handle this message.
        /// </summary>
        GETOBJECT = 0x003D,

        /// <summary>
        /// The WM_COMPACTING message is sent to all top-level windows when the system detects more than 12.5 percent of system time over a 30- to 60-second interval is being spent compacting memory. This indicates that system memory is low.
        /// </summary>
        COMPACTING = 0x0041,

        /// <summary>
        /// WM_COMMNOTIFY is Obsolete for Win32-Based Applications
        /// </summary>
        [Obsolete("Obsolete for Win32 Based Applications")]
        COMMNOTIFY = 0x0044,

        /// <summary>
        /// The WM_WINDOWPOSCHANGING message is sent to a window whose size, position, or place in the Z order is about to change as a result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        WINDOWPOSCHANGING = 0x0046,

        /// <summary>
        /// The WM_WINDOWPOSCHANGED message is sent to a window whose size, position, or place in the Z order has changed as a result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        WINDOWPOSCHANGED = 0x0047,

        /// <summary>
        /// Notifies applications that the system, typically a battery-powered personal computer, is about to enter a suspended mode.
        /// Use: POWERBROADCAST
        /// </summary>
        [Obsolete("Provided only for compatibility with 16-bit Windows-based applications")]
        POWER = 0x0048,

        /// <summary>
        /// An application sends the WM_COPYDATA message to pass data to another application.
        /// </summary>
        COPYDATA = 0x004A,

        /// <summary>
        /// The WM_CANCELJOURNAL message is posted to an application when a user cancels the application's journaling activities. The message is posted with a NULL window handle.
        /// </summary>
        CANCELJOURNAL = 0x004B,

        /// <summary>
        /// Sent by a common control to its parent window when an event has occurred or the control requires some information.
        /// </summary>
        NOTIFY = 0x004E,

        /// <summary>
        /// The WM_INPUTLANGCHANGEREQUEST message is posted to the window with the focus when the user chooses a new input language, either with the hotkey (specified in the Keyboard control panel application) or from the indicator on the system taskbar. An application can accept the change by passing the message to the DefWindowProc function or reject the change (and prevent it from taking place) by returning immediately.
        /// </summary>
        INPUTLANGCHANGEREQUEST = 0x0050,

        /// <summary>
        /// The WM_INPUTLANGCHANGE message is sent to the topmost affected window after an application's input language has been changed. You should make any application-specific settings and pass the message to the DefWindowProc function, which passes the message to all first-level child windows. These child windows can pass the message to DefWindowProc to have it pass the message to their child windows, and so on.
        /// </summary>
        INPUTLANGCHANGE = 0x0051,

        /// <summary>
        /// Sent to an application that has initiated a training card with Microsoft Windows Help. The message informs the application when the user clicks an authorable button. An application initiates a training card by specifying the HELP_TCARD command in a call to the WinHelp function.
        /// </summary>
        TCARD = 0x0052,

        /// <summary>
        /// Indicates that the user pressed the F1 key. If a menu is active when F1 is pressed, WM_HELP is sent to the window associated with the menu; otherwise, WM_HELP is sent to the window that has the keyboard focus. If no window has the keyboard focus, WM_HELP is sent to the currently active window.
        /// </summary>
        HELP = 0x0053,

        /// <summary>
        /// The WM_USERCHANGED message is sent to all windows after the user has logged on or off. When the user logs on or off, the system updates the user-specific settings. The system sends this message immediately after updating the settings.
        /// </summary>
        USERCHANGED = 0x0054,

        /// <summary>
        /// Determines if a window accepts ANSI or Unicode structures in the WM_NOTIFY notification message. WM_NOTIFYFORMAT messages are sent from a common control to its parent window and from the parent window to the common control.
        /// </summary>
        NOTIFYFORMAT = 0x0055,

        /// <summary>
        /// The WM_CONTEXTMENU message notifies a window that the user clicked the right mouse button (right-clicked) in the window.
        /// </summary>
        CONTEXTMENU = 0x007B,

        /// <summary>
        /// The WM_STYLECHANGING message is sent to a window when the SetWindowLong function is about to change one or more of the window's styles.
        /// </summary>
        STYLECHANGING = 0x007C,

        /// <summary>
        /// The WM_STYLECHANGED message is sent to a window after the SetWindowLong function has changed one or more of the window's styles
        /// </summary>
        STYLECHANGED = 0x007D,

        /// <summary>
        /// The WM_DISPLAYCHANGE message is sent to all windows when the display resolution has changed.
        /// </summary>
        DISPLAYCHANGE = 0x007E,

        /// <summary>
        /// The WM_GETICON message is sent to a window to retrieve a handle to the large or small icon associated with a window. The system displays the large icon in the ALT+TAB dialog, and the small icon in the window caption.
        /// </summary>
        GETICON = 0x007F,

        /// <summary>
        /// An application sends the WM_SETICON message to associate a new large or small icon with a window. The system displays the large icon in the ALT+TAB dialog box, and the small icon in the window caption.
        /// </summary>
        SETICON = 0x0080,

        /// <summary>
        /// The WM_NCCREATE message is sent prior to the WM_CREATE message when a window is first created.
        /// </summary>
        NCCREATE = 0x0081,

        /// <summary>
        /// The WM_NCDESTROY message informs a window that its nonclient area is being destroyed. The DestroyWindow function sends the WM_NCDESTROY message to the window following the WM_DESTROY message. WM_DESTROY is used to free the allocated memory object associated with the window.
        /// The WM_NCDESTROY message is sent after the child windows have been destroyed. In contrast, WM_DESTROY is sent before the child windows are destroyed.
        /// </summary>
        NCDESTROY = 0x0082,

        /// <summary>
        /// The WM_NCCALCSIZE message is sent when the size and position of a window's client area must be calculated. By processing this message, an application can control the content of the window's client area when the size or position of the window changes.
        /// </summary>
        NCCALCSIZE = 0x0083,

        /// <summary>
        /// The WM_NCHITTEST message is sent to a window when the cursor moves, or when a mouse button is pressed or released. If the mouse is not captured, the message is sent to the window beneath the cursor. Otherwise, the message is sent to the window that has captured the mouse.
        /// </summary>
        NCHITTEST = 0x0084,

        /// <summary>
        /// The WM_NCPAINT message is sent to a window when its frame must be painted.
        /// </summary>
        NCPAINT = 0x0085,

        /// <summary>
        /// The WM_NCACTIVATE message is sent to a window when its nonclient area needs to be changed to indicate an active or inactive state.
        /// </summary>
        NCACTIVATE = 0x0086,

        /// <summary>
        /// The WM_GETDLGCODE message is sent to the window procedure associated with a control. By default, the system handles all keyboard input to the control; the system interprets certain types of keyboard input as dialog box navigation keys. To override this default behavior, the control can respond to the WM_GETDLGCODE message to indicate the types of input it wants to process itself.
        /// </summary>
        GETDLGCODE = 0x0087,

        /// <summary>
        /// The WM_SYNCPAINT message is used to synchronize painting while avoiding linking independent GUI threads.
        /// </summary>
        SYNCPAINT = 0x0088,

        /// <summary>
        /// The WM_NCMOUSEMOVE message is posted to a window when the cursor is moved within the nonclient area of the window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCMOUSEMOVE = 0x00A0,

        /// <summary>
        /// The WM_NCLBUTTONDOWN message is posted when the user presses the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCLBUTTONDOWN = 0x00A1,

        /// <summary>
        /// The WM_NCLBUTTONUP message is posted when the user releases the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCLBUTTONUP = 0x00A2,

        /// <summary>
        /// The WM_NCLBUTTONDBLCLK message is posted when the user double-clicks the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCLBUTTONDBLCLK = 0x00A3,

        /// <summary>
        /// The WM_NCRBUTTONDOWN message is posted when the user presses the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCRBUTTONDOWN = 0x00A4,

        /// <summary>
        /// The WM_NCRBUTTONUP message is posted when the user releases the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCRBUTTONUP = 0x00A5,

        /// <summary>
        /// The WM_NCRBUTTONDBLCLK message is posted when the user double-clicks the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCRBUTTONDBLCLK = 0x00A6,

        /// <summary>
        /// The WM_NCMBUTTONDOWN message is posted when the user presses the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCMBUTTONDOWN = 0x00A7,

        /// <summary>
        /// The WM_NCMBUTTONUP message is posted when the user releases the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCMBUTTONUP = 0x00A8,

        /// <summary>
        /// The WM_NCMBUTTONDBLCLK message is posted when the user double-clicks the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCMBUTTONDBLCLK = 0x00A9,

        /// <summary>
        /// The WM_NCXBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCXBUTTONDOWN = 0x00AB,

        /// <summary>
        /// The WM_NCXBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCXBUTTONUP = 0x00AC,

        /// <summary>
        /// The WM_NCXBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        NCXBUTTONDBLCLK = 0x00AD,

        /// <summary>
        /// The WM_INPUT_DEVICE_CHANGE message is sent to the window that registered to receive raw input. A window receives this message through its WindowProc function.
        /// </summary>
        INPUT_DEVICE_CHANGE = 0x00FE,

        /// <summary>
        /// The WM_INPUT message is sent to the window that is getting raw input.
        /// </summary>
        INPUT = 0x00FF,

        /// <summary>
        /// This message filters for keyboard messages.
        /// </summary>
        KEYFIRST = 0x0100,

        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        KEYDOWN = 0x0100,

        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        KEYUP = 0x0101,

        /// <summary>
        /// The WM_CHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. The WM_CHAR message contains the character code of the key that was pressed.
        /// </summary>
        CHAR = 0x0102,

        /// <summary>
        /// The WM_DEADCHAR message is posted to the window with the keyboard focus when a WM_KEYUP message is translated by the TranslateMessage function. WM_DEADCHAR specifies a character code generated by a dead key. A dead key is a key that generates a character, such as the umlaut (double-dot), that is combined with another character to form a composite character. For example, the umlaut-O character (Ö) is generated by typing the dead key for the umlaut character, and then typing the O key.
        /// </summary>
        DEADCHAR = 0x0103,

        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user presses the F10 key (which activates the menu bar) or holds down the ALT key and then presses another key. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter.
        /// </summary>
        SYSKEYDOWN = 0x0104,

        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user releases a key that was pressed while the ALT key was held down. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter.
        /// </summary>
        SYSKEYUP = 0x0105,

        /// <summary>
        /// The WM_SYSCHAR message is posted to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage function. It specifies the character code of a system character key — that is, a character key that is pressed while the ALT key is down.
        /// </summary>
        SYSCHAR = 0x0106,

        /// <summary>
        /// The WM_SYSDEADCHAR message is sent to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage function. WM_SYSDEADCHAR specifies the character code of a system dead key — that is, a dead key that is pressed while holding down the ALT key.
        /// </summary>
        SYSDEADCHAR = 0x0107,

        /// <summary>
        /// The WM_UNICHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. The WM_UNICHAR message contains the character code of the key that was pressed.
        /// The WM_UNICHAR message is equivalent to WM_CHAR, but it uses Unicode Transformation Format (UTF)-32, whereas WM_CHAR uses UTF-16. It is designed to send or post Unicode characters to ANSI windows and it can can handle Unicode Supplementary Plane characters.
        /// </summary>
        UNICHAR = 0x0109,

        /// <summary>
        /// This message filters for keyboard messages.
        /// </summary>
        KEYLAST = 0x0108,

        /// <summary>
        /// Sent immediately before the IME generates the composition string as a result of a keystroke. A window receives this message through its WindowProc function.
        /// </summary>
        IME_STARTCOMPOSITION = 0x010D,

        /// <summary>
        /// Sent to an application when the IME ends composition. A window receives this message through its WindowProc function.
        /// </summary>
        IME_ENDCOMPOSITION = 0x010E,

        /// <summary>
        /// Sent to an application when the IME changes composition status as a result of a keystroke. A window receives this message through its WindowProc function.
        /// </summary>
        IME_COMPOSITION = 0x010F,

        IME_KEYLAST = 0x010F,

        /// <summary>
        /// The WM_INITDIALOG message is sent to the dialog box procedure immediately before a dialog box is displayed. Dialog box procedures typically use this message to initialize controls and carry out any other initialization tasks that affect the appearance of the dialog box.
        /// </summary>
        INITDIALOG = 0x0110,

        /// <summary>
        /// The WM_COMMAND message is sent when the user selects a command item from a menu, when a control sends a notification message to its parent window, or when an accelerator keystroke is translated.
        /// </summary>
        COMMAND = 0x0111,

        /// <summary>
        /// A window receives this message when the user chooses a command from the Window menu, clicks the maximize button, minimize button, restore button, close button, or moves the form. You can stop the form from moving by filtering this out.
        /// </summary>
        SYSCOMMAND = 0x0112,

        /// <summary>
        /// The WM_TIMER message is posted to the installing thread's message queue when a timer expires. The message is posted by the GetMessage or PeekMessage function.
        /// </summary>
        TIMER = 0x0113,

        /// <summary>
        /// The WM_HSCROLL message is sent to a window when a scroll event occurs in the window's standard horizontal scroll bar. This message is also sent to the owner of a horizontal scroll bar control when a scroll event occurs in the control.
        /// </summary>
        HSCROLL = 0x0114,

        /// <summary>
        /// The WM_VSCROLL message is sent to a window when a scroll event occurs in the window's standard vertical scroll bar. This message is also sent to the owner of a vertical scroll bar control when a scroll event occurs in the control.
        /// </summary>
        VSCROLL = 0x0115,

        /// <summary>
        /// The WM_INITMENU message is sent when a menu is about to become active. It occurs when the user clicks an item on the menu bar or presses a menu key. This allows the application to modify the menu before it is displayed.
        /// </summary>
        INITMENU = 0x0116,

        /// <summary>
        /// The WM_INITMENUPOPUP message is sent when a drop-down menu or submenu is about to become active. This allows an application to modify the menu before it is displayed, without changing the entire menu.
        /// </summary>
        INITMENUPOPUP = 0x0117,

        /// <summary>
        /// The WM_MENUSELECT message is sent to a menu's owner window when the user selects a menu item.
        /// </summary>
        MENUSELECT = 0x011F,

        /// <summary>
        /// The WM_MENUCHAR message is sent when a menu is active and the user presses a key that does not correspond to any mnemonic or accelerator key. This message is sent to the window that owns the menu.
        /// </summary>
        MENUCHAR = 0x0120,

        /// <summary>
        /// The WM_ENTERIDLE message is sent to the owner window of a modal dialog box or menu that is entering an idle state. A modal dialog box or menu enters an idle state when no messages are waiting in its queue after it has processed one or more previous messages.
        /// </summary>
        ENTERIDLE = 0x0121,

        /// <summary>
        /// The WM_MENURBUTTONUP message is sent when the user releases the right mouse button while the cursor is on a menu item.
        /// </summary>
        MENURBUTTONUP = 0x0122,

        /// <summary>
        /// The WM_MENUDRAG message is sent to the owner of a drag-and-drop menu when the user drags a menu item.
        /// </summary>
        MENUDRAG = 0x0123,

        /// <summary>
        /// The WM_MENUGETOBJECT message is sent to the owner of a drag-and-drop menu when the mouse cursor enters a menu item or moves from the center of the item to the top or bottom of the item.
        /// </summary>
        MENUGETOBJECT = 0x0124,

        /// <summary>
        /// The WM_UNINITMENUPOPUP message is sent when a drop-down menu or submenu has been destroyed.
        /// </summary>
        UNINITMENUPOPUP = 0x0125,

        /// <summary>
        /// The WM_MENUCOMMAND message is sent when the user makes a selection from a menu.
        /// </summary>
        MENUCOMMAND = 0x0126,

        /// <summary>
        /// An application sends the WM_CHANGEUISTATE message to indicate that the user interface (UI) state should be changed.
        /// </summary>
        CHANGEUISTATE = 0x0127,

        /// <summary>
        /// An application sends the WM_UPDATEUISTATE message to change the user interface (UI) state for the specified window and all its child windows.
        /// </summary>
        UPDATEUISTATE = 0x0128,

        /// <summary>
        /// An application sends the WM_QUERYUISTATE message to retrieve the user interface (UI) state for a window.
        /// </summary>
        QUERYUISTATE = 0x0129,

        /// <summary>
        /// The WM_CTLCOLORMSGBOX message is sent to the owner window of a message box before Windows draws the message box. By responding to this message, the owner window can set the text and background colors of the message box by using the given display device context handle.
        /// </summary>
        CTLCOLORMSGBOX = 0x0132,

        /// <summary>
        /// An edit control that is not read-only or disabled sends the WM_CTLCOLOREDIT message to its parent window when the control is about to be drawn. By responding to this message, the parent window can use the specified device context handle to set the text and background colors of the edit control.
        /// </summary>
        CTLCOLOREDIT = 0x0133,

        /// <summary>
        /// Sent to the parent window of a list box before the system draws the list box. By responding to this message, the parent window can set the text and background colors of the list box by using the specified display device context handle.
        /// </summary>
        CTLCOLORLISTBOX = 0x0134,

        /// <summary>
        /// The WM_CTLCOLORBTN message is sent to the parent window of a button before drawing the button. The parent window can change the button's text and background colors. However, only owner-drawn buttons respond to the parent window processing this message.
        /// </summary>
        CTLCOLORBTN = 0x0135,

        /// <summary>
        /// The WM_CTLCOLORDLG message is sent to a dialog box before the system draws the dialog box. By responding to this message, the dialog box can set its text and background colors using the specified display device context handle.
        /// </summary>
        CTLCOLORDLG = 0x0136,

        /// <summary>
        /// The WM_CTLCOLORSCROLLBAR message is sent to the parent window of a scroll bar control when the control is about to be drawn. By responding to this message, the parent window can use the display context handle to set the background color of the scroll bar control.
        /// </summary>
        CTLCOLORSCROLLBAR = 0x0137,

        /// <summary>
        /// A static control, or an edit control that is read-only or disabled, sends the WM_CTLCOLORSTATIC message to its parent window when the control is about to be drawn. By responding to this message, the parent window can use the specified device context handle to set the text and background colors of the static control.
        /// </summary>
        CTLCOLORSTATIC = 0x0138,

        /// <summary>
        /// Use WM_MOUSEFIRST to specify the first mouse message. Use the PeekMessage() Function.
        /// </summary>
        MOUSEFIRST = 0x0200,

        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. If the mouse is not captured, the message is posted to the window that contains the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        MOUSEMOVE = 0x0200,

        /// <summary>
        /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        LBUTTONDOWN = 0x0201,

        /// <summary>
        /// The WM_LBUTTONUP message is posted when the user releases the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        LBUTTONUP = 0x0202,

        /// <summary>
        /// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        LBUTTONDBLCLK = 0x0203,

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        RBUTTONDOWN = 0x0204,

        /// <summary>
        /// The WM_RBUTTONUP message is posted when the user releases the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        RBUTTONUP = 0x0205,

        /// <summary>
        /// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        RBUTTONDBLCLK = 0x0206,

        /// <summary>
        /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        MBUTTONDOWN = 0x0207,

        /// <summary>
        /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        MBUTTONUP = 0x0208,

        /// <summary>
        /// The WM_MBUTTONDBLCLK message is posted when the user double-clicks the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        MBUTTONDBLCLK = 0x0209,

        /// <summary>
        /// The WM_MOUSEWHEEL message is sent to the focus window when the mouse wheel is rotated. The DefWindowProc function propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
        /// </summary>
        MOUSEWHEEL = 0x020A,

        /// <summary>
        /// The WM_XBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        XBUTTONDOWN = 0x020B,

        /// <summary>
        /// The WM_XBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        XBUTTONUP = 0x020C,

        /// <summary>
        /// The WM_XBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        XBUTTONDBLCLK = 0x020D,

        /// <summary>
        /// The WM_MOUSEHWHEEL message is sent to the focus window when the mouse's horizontal scroll wheel is tilted or rotated. The DefWindowProc function propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
        /// </summary>
        MOUSEHWHEEL = 0x020E,

        /// <summary>
        /// Use WM_MOUSELAST to specify the last mouse message. Used with PeekMessage() Function.
        /// </summary>
        MOUSELAST = 0x020E,

        /// <summary>
        /// The WM_PARENTNOTIFY message is sent to the parent of a child window when the child window is created or destroyed, or when the user clicks a mouse button while the cursor is over the child window. When the child window is being created, the system sends WM_PARENTNOTIFY just before the CreateWindow or CreateWindowEx function that creates the window returns. When the child window is being destroyed, the system sends the message before any processing to destroy the window takes place.
        /// </summary>
        PARENTNOTIFY = 0x0210,

        /// <summary>
        /// The WM_ENTERMENULOOP message informs an application's main window procedure that a menu modal loop has been entered.
        /// </summary>
        ENTERMENULOOP = 0x0211,

        /// <summary>
        /// The WM_EXITMENULOOP message informs an application's main window procedure that a menu modal loop has been exited.
        /// </summary>
        EXITMENULOOP = 0x0212,

        /// <summary>
        /// The WM_NEXTMENU message is sent to an application when the right or left arrow key is used to switch between the menu bar and the system menu.
        /// </summary>
        NEXTMENU = 0x0213,

        /// <summary>
        /// The WM_SIZING message is sent to a window that the user is resizing. By processing this message, an application can monitor the size and position of the drag rectangle and, if needed, change its size or position.
        /// </summary>
        SIZING = 0x0214,

        /// <summary>
        /// The WM_CAPTURECHANGED message is sent to the window that is losing the mouse capture.
        /// </summary>
        CAPTURECHANGED = 0x0215,

        /// <summary>
        /// The WM_MOVING message is sent to a window that the user is moving. By processing this message, an application can monitor the position of the drag rectangle and, if needed, change its position.
        /// </summary>
        MOVING = 0x0216,

        /// <summary>
        /// Notifies applications that a power-management event has occurred.
        /// </summary>
        POWERBROADCAST = 0x0218,

        /// <summary>
        /// Notifies an application of a change to the hardware configuration of a device or the computer.
        /// </summary>
        DEVICECHANGE = 0x0219,

        /// <summary>
        /// An application sends the WM_MDICREATE message to a multiple-document interface (MDI) client window to create an MDI child window.
        /// </summary>
        MDICREATE = 0x0220,

        /// <summary>
        /// An application sends the WM_MDIDESTROY message to a multiple-document interface (MDI) client window to close an MDI child window.
        /// </summary>
        MDIDESTROY = 0x0221,

        /// <summary>
        /// An application sends the WM_MDIACTIVATE message to a multiple-document interface (MDI) client window to instruct the client window to activate a different MDI child window.
        /// </summary>
        MDIACTIVATE = 0x0222,

        /// <summary>
        /// An application sends the WM_MDIRESTORE message to a multiple-document interface (MDI) client window to restore an MDI child window from maximized or minimized size.
        /// </summary>
        MDIRESTORE = 0x0223,

        /// <summary>
        /// An application sends the WM_MDINEXT message to a multiple-document interface (MDI) client window to activate the next or previous child window.
        /// </summary>
        MDINEXT = 0x0224,

        /// <summary>
        /// An application sends the WM_MDIMAXIMIZE message to a multiple-document interface (MDI) client window to maximize an MDI child window. The system resizes the child window to make its client area fill the client window. The system places the child window's window menu icon in the rightmost position of the frame window's menu bar, and places the child window's restore icon in the leftmost position. The system also appends the title bar text of the child window to that of the frame window.
        /// </summary>
        MDIMAXIMIZE = 0x0225,

        /// <summary>
        /// An application sends the WM_MDITILE message to a multiple-document interface (MDI) client window to arrange all of its MDI child windows in a tile format.
        /// </summary>
        MDITILE = 0x0226,

        /// <summary>
        /// An application sends the WM_MDICASCADE message to a multiple-document interface (MDI) client window to arrange all its child windows in a cascade format.
        /// </summary>
        MDICASCADE = 0x0227,

        /// <summary>
        /// An application sends the WM_MDIICONARRANGE message to a multiple-document interface (MDI) client window to arrange all minimized MDI child windows. It does not affect child windows that are not minimized.
        /// </summary>
        MDIICONARRANGE = 0x0228,

        /// <summary>
        /// An application sends the WM_MDIGETACTIVE message to a multiple-document interface (MDI) client window to retrieve the handle to the active MDI child window.
        /// </summary>
        MDIGETACTIVE = 0x0229,

        /// <summary>
        /// An application sends the WM_MDISETMENU message to a multiple-document interface (MDI) client window to replace the entire menu of an MDI frame window, to replace the window menu of the frame window, or both.
        /// </summary>
        MDISETMENU = 0x0230,

        /// <summary>
        /// The WM_ENTERSIZEMOVE message is sent one time to a window after it enters the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns.
        /// The system sends the WM_ENTERSIZEMOVE message regardless of whether the dragging of full windows is enabled.
        /// </summary>
        ENTERSIZEMOVE = 0x0231,

        /// <summary>
        /// The WM_EXITSIZEMOVE message is sent one time to a window, after it has exited the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns.
        /// </summary>
        EXITSIZEMOVE = 0x0232,

        /// <summary>
        /// Sent when the user drops a file on the window of an application that has registered itself as a recipient of dropped files.
        /// </summary>
        DROPFILES = 0x0233,

        /// <summary>
        /// An application sends the WM_MDIREFRESHMENU message to a multiple-document interface (MDI) client window to refresh the window menu of the MDI frame window.
        /// </summary>
        MDIREFRESHMENU = 0x0234,

        /// <summary>
        /// Sent to an application when a window is activated. A window receives this message through its WindowProc function.
        /// </summary>
        IME_SETCONTEXT = 0x0281,

        /// <summary>
        /// Sent to an application to notify it of changes to the IME window. A window receives this message through its WindowProc function.
        /// </summary>
        IME_NOTIFY = 0x0282,

        /// <summary>
        /// Sent by an application to direct the IME window to carry out the requested command. The application uses this message to control the IME window that it has created. To send this message, the application calls the SendMessage function with the following parameters.
        /// </summary>
        IME_CONTROL = 0x0283,

        /// <summary>
        /// Sent to an application when the IME window finds no space to extend the area for the composition window. A window receives this message through its WindowProc function.
        /// </summary>
        IME_COMPOSITIONFULL = 0x0284,

        /// <summary>
        /// Sent to an application when the operating system is about to change the current IME. A window receives this message through its WindowProc function.
        /// </summary>
        IME_SELECT = 0x0285,

        /// <summary>
        /// Sent to an application when the IME gets a character of the conversion result. A window receives this message through its WindowProc function.
        /// </summary>
        IME_CHAR = 0x0286,

        /// <summary>
        /// Sent to an application to provide commands and request information. A window receives this message through its WindowProc function.
        /// </summary>
        IME_REQUEST = 0x0288,

        /// <summary>
        /// Sent to an application by the IME to notify the application of a key press and to keep message order. A window receives this message through its WindowProc function.
        /// </summary>
        IME_KEYDOWN = 0x0290,

        /// <summary>
        /// Sent to an application by the IME to notify the application of a key release and to keep message order. A window receives this message through its WindowProc function.
        /// </summary>
        IME_KEYUP = 0x0291,

        /// <summary>
        /// The WM_MOUSEHOVER message is posted to a window when the cursor hovers over the client area of the window for the period of time specified in a prior call to TrackMouseEvent.
        /// </summary>
        MOUSEHOVER = 0x02A1,

        /// <summary>
        /// The WM_MOUSELEAVE message is posted to a window when the cursor leaves the client area of the window specified in a prior call to TrackMouseEvent.
        /// </summary>
        MOUSELEAVE = 0x02A3,

        /// <summary>
        /// The WM_NCMOUSEHOVER message is posted to a window when the cursor hovers over the nonclient area of the window for the period of time specified in a prior call to TrackMouseEvent.
        /// </summary>
        NCMOUSEHOVER = 0x02A0,

        /// <summary>
        /// The WM_NCMOUSELEAVE message is posted to a window when the cursor leaves the nonclient area of the window specified in a prior call to TrackMouseEvent.
        /// </summary>
        NCMOUSELEAVE = 0x02A2,

        /// <summary>
        /// The WM_WTSSESSION_CHANGE message notifies applications of changes in session state.
        /// </summary>
        WTSSESSION_CHANGE = 0x02B1,

        TABLET_FIRST = 0x02c0,

        TABLET_LAST = 0x02df,

        /// <summary>
        /// Sent when the effective dots per inch (dpi) for a window has changed. The DPI is the scale factor for a window.
        /// </summary>
        DPICHANGED = 0x02E0,

        /// <summary>
        /// An application sends a WM_CUT message to an edit control or combo box to delete (cut) the current selection, if any, in the edit control and copy the deleted text to the clipboard in CF_TEXT format.
        /// </summary>
        CUT = 0x0300,

        /// <summary>
        /// An application sends the WM_COPY message to an edit control or combo box to copy the current selection to the clipboard in CF_TEXT format.
        /// </summary>
        COPY = 0x0301,

        /// <summary>
        /// An application sends a WM_PASTE message to an edit control or combo box to copy the current content of the clipboard to the edit control at the current caret position. Data is inserted only if the clipboard contains data in CF_TEXT format.
        /// </summary>
        PASTE = 0x0302,

        /// <summary>
        /// An application sends a WM_CLEAR message to an edit control or combo box to delete (clear) the current selection, if any, from the edit control.
        /// </summary>
        CLEAR = 0x0303,

        /// <summary>
        /// An application sends a WM_UNDO message to an edit control to undo the last operation. When this message is sent to an edit control, the previously deleted text is restored or the previously added text is deleted.
        /// </summary>
        UNDO = 0x0304,

        /// <summary>
        /// The WM_RENDERFORMAT message is sent to the clipboard owner if it has delayed rendering a specific clipboard format and if an application has requested data in that format. The clipboard owner must render data in the specified format and place it on the clipboard by calling the SetClipboardData function.
        /// </summary>
        RENDERFORMAT = 0x0305,

        /// <summary>
        /// The WM_RENDERALLFORMATS message is sent to the clipboard owner before it is destroyed, if the clipboard owner has delayed rendering one or more clipboard formats. For the content of the clipboard to remain available to other applications, the clipboard owner must render data in all the formats it is capable of generating, and place the data on the clipboard by calling the SetClipboardData function.
        /// </summary>
        RENDERALLFORMATS = 0x0306,

        /// <summary>
        /// The WM_DESTROYCLIPBOARD message is sent to the clipboard owner when a call to the EmptyClipboard function empties the clipboard.
        /// </summary>
        DESTROYCLIPBOARD = 0x0307,

        /// <summary>
        /// The WM_DRAWCLIPBOARD message is sent to the first window in the clipboard viewer chain when the content of the clipboard changes. This enables a clipboard viewer window to display the new content of the clipboard.
        /// </summary>
        DRAWCLIPBOARD = 0x0308,

        /// <summary>
        /// The WM_PAINTCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and the clipboard viewer's client area needs repainting.
        /// </summary>
        PAINTCLIPBOARD = 0x0309,

        /// <summary>
        /// The WM_VSCROLLCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's vertical scroll bar. The owner should scroll the clipboard image and update the scroll bar values.
        /// </summary>
        VSCROLLCLIPBOARD = 0x030A,

        /// <summary>
        /// The WM_SIZECLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and the clipboard viewer's client area has changed size.
        /// </summary>
        SIZECLIPBOARD = 0x030B,

        /// <summary>
        /// The WM_ASKCBFORMATNAME message is sent to the clipboard owner by a clipboard viewer window to request the name of a CF_OWNERDISPLAY clipboard format.
        /// </summary>
        ASKCBFORMATNAME = 0x030C,

        /// <summary>
        /// The WM_CHANGECBCHAIN message is sent to the first window in the clipboard viewer chain when a window is being removed from the chain.
        /// </summary>
        CHANGECBCHAIN = 0x030D,

        /// <summary>
        /// The WM_HSCROLLCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window. This occurs when the clipboard contains data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's horizontal scroll bar. The owner should scroll the clipboard image and update the scroll bar values.
        /// </summary>
        HSCROLLCLIPBOARD = 0x030E,

        /// <summary>
        /// This message informs a window that it is about to receive the keyboard focus, giving the window the opportunity to realize its logical palette when it receives the focus.
        /// </summary>
        QUERYNEWPALETTE = 0x030F,

        /// <summary>
        /// The WM_PALETTEISCHANGING message informs applications that an application is going to realize its logical palette.
        /// </summary>
        PALETTEISCHANGING = 0x0310,

        /// <summary>
        /// This message is sent by the OS to all top-level and overlapped windows after the window with the keyboard focus realizes its logical palette.
        /// This message enables windows that do not have the keyboard focus to realize their logical palettes and update their client areas.
        /// </summary>
        PALETTECHANGED = 0x0311,

        /// <summary>
        /// The WM_HOTKEY message is posted when the user presses a hot key registered by the RegisterHotKey function. The message is placed at the top of the message queue associated with the thread that registered the hot key.
        /// </summary>
        HOTKEY = 0x0312,

        /// <summary>
        /// The WM_PRINT message is sent to a window to request that it draw itself in the specified device context, most commonly in a printer device context.
        /// </summary>
        PRINT = 0x0317,

        /// <summary>
        /// The WM_PRINTCLIENT message is sent to a window to request that it draw its client area in the specified device context, most commonly in a printer device context.
        /// </summary>
        PRINTCLIENT = 0x0318,

        /// <summary>
        /// The WM_APPCOMMAND message notifies a window that the user generated an application command event, for example, by clicking an application command button using the mouse or typing an application command key on the keyboard.
        /// </summary>
        APPCOMMAND = 0x0319,

        /// <summary>
        /// The WM_THEMECHANGED message is broadcast to every window following a theme change event. Examples of theme change events are the activation of a theme, the deactivation of a theme, or a transition from one theme to another.
        /// </summary>
        THEMECHANGED = 0x031A,

        /// <summary>
        /// Sent when the contents of the clipboard have changed.
        /// </summary>
        CLIPBOARDUPDATE = 0x031D,

        /// <summary>
        /// The system will send a window the WM_DWMCOMPOSITIONCHANGED message to indicate that the availability of desktop composition has changed.
        /// </summary>
        DWMCOMPOSITIONCHANGED = 0x031E,

        /// <summary>
        /// WM_DWMNCRENDERINGCHANGED is called when the non-client area rendering status of a window has changed. Only windows that have set the flag DWM_BLURBEHIND.fTransitionOnMaximized to true will get this message.
        /// </summary>
        DWMNCRENDERINGCHANGED = 0x031F,

        /// <summary>
        /// Sent to all top-level windows when the colorization color has changed.
        /// </summary>
        DWMCOLORIZATIONCOLORCHANGED = 0x0320,

        /// <summary>
        /// WM_DWMWINDOWMAXIMIZEDCHANGE will let you know when a DWM composed window is maximized. You also have to register for this message as well. You'd have other windowd go opaque when this message is sent.
        /// </summary>
        DWMWINDOWMAXIMIZEDCHANGE = 0x0321,

        /// <summary>
        /// Sent to request extended title bar information. A window receives this message through its WindowProc function.
        /// </summary>
        GETTITLEBARINFOEX = 0x033F,

        HANDHELDFIRST = 0x0358,

        HANDHELDLAST = 0x035F,

        AFXFIRST = 0x0360,

        AFXLAST = 0x037F,

        PENWINFIRST = 0x0380,

        PENWINLAST = 0x038F,

        /// <summary>
        /// The WM_APP constant is used by applications to help define private messages, usually of the form WM_APP+X, where X is an integer value.
        /// </summary>
        APP = 0x8000,

        /// <summary>
        /// The WM_USER constant is used by applications to help define private messages for use by private window classes, usually of the form WM_USER+X, where X is an integer value.
        /// </summary>
        USER = 0x0400,

        /// <summary>
        /// An application sends the WM_CPL_LAUNCH message to Windows Control Panel to request that a Control Panel application be started.
        /// </summary>
        CPL_LAUNCH = USER + 0x1000,

        /// <summary>
        /// The WM_CPL_LAUNCHED message is sent when a Control Panel application, started by the WM_CPL_LAUNCH message, has closed. The WM_CPL_LAUNCHED message is sent to the window identified by the wParam parameter of the WM_CPL_LAUNCH message that started the application.
        /// </summary>
        CPL_LAUNCHED = USER + 0x1001,

        /// <summary>
        /// WM_SYSTIMER is a well-known yet still undocumented message. Windows uses WM_SYSTIMER for internal actions like scrolling.
        /// </summary>
        SYSTIMER = 0x118,

        /// <summary>
        /// The accessibility state has changed.
        /// </summary>
        HSHELL_ACCESSIBILITYSTATE = 11,

        /// <summary>
        /// The shell should activate its main window.
        /// </summary>
        HSHELL_ACTIVATESHELLWINDOW = 3,

        /// <summary>
        /// The user completed an input event (for example, pressed an application command button on the mouse or an application command key on the keyboard), and the application did not handle the WM_APPCOMMAND message generated by that input.
        /// If the Shell procedure handles the WM_COMMAND message, it should not call CallNextHookEx. See the Return Value section for more information.
        /// </summary>
        HSHELL_APPCOMMAND = 12,

        /// <summary>
        /// A window is being minimized or maximized. The system needs the coordinates of the minimized rectangle for the window.
        /// </summary>
        HSHELL_GETMINRECT = 5,

        /// <summary>
        /// Keyboard language was changed or a new keyboard layout was loaded.
        /// </summary>
        HSHELL_LANGUAGE = 8,

        /// <summary>
        /// The title of a window in the task bar has been redrawn.
        /// </summary>
        HSHELL_REDRAW = 6,

        /// <summary>
        /// The user has selected the task list. A shell application that provides a task list should return TRUE to prevent Windows from starting its task list.
        /// </summary>
        HSHELL_TASKMAN = 7,

        /// <summary>
        /// A top-level, unowned window has been created. The window exists when the system calls this hook.
        /// </summary>
        HSHELL_WINDOWCREATED = 1,

        /// <summary>
        /// A top-level, unowned window is about to be destroyed. The window still exists when the system calls this hook.
        /// </summary>
        HSHELL_WINDOWDESTROYED = 2,

        /// <summary>
        /// The activation has changed to a different top-level, unowned window.
        /// </summary>
        HSHELL_WINDOWACTIVATED = 4,

        /// <summary>
        /// A top-level window is being replaced. The window exists when the system calls this hook.
        /// </summary>
        HSHELL_WINDOWREPLACED = 13
    }

    public enum WGLPixelFormatAttribute : int
    {
        /// <summary>
        /// The number of pixel formats for the device context. The
        /// iLayerPlane and iPixelFormat parameters are ignored if this
        /// attribute is specified.
        /// </summary>
        NUMBER_PIXEL_FORMATS_ARB = 0x2000,

        /// <summary>
        /// True if the pixel format can be used with a window. The
        /// iLayerPlane parameter is ignored if this attribute is
        /// specified.
        /// </summary>
        DRAW_TO_WINDOW_ARB = 0x2001,

        /// <summary>
        /// True if the pixel format can be used with a memory bitmap. The
        /// iLayerPlane parameter is ignored if this attribute is
        /// specified.
        /// </summary>
        DRAW_TO_BITMAP_ARB = 0x2002,

        /// <summary>
        /// Indicates whether the pixel format is supported by the driver.
        /// If this is set to WGL_NO_ACCELERATION_ARB then only the software
        /// renderer supports this pixel format; if this is set to
        /// WGL_GENERIC_ACCELERATION_ARB then the pixel format is supported
        /// by an MCD driver; if this is set to WGL_FULL_ACCELERATION_ARB
        /// then the pixel format is supported by an ICD driver.
        /// </summary>
        ACCELERATION_ARB = 0x2003,

        /// <summary>
        /// A logical palette is required to achieve the best results for
        /// this pixel format. The iLayerPlane parameter is ignored if
        /// this attribute is specified.
        /// </summary>
        NEED_PALETTE_ARB = 0x2004,

        /// <summary>
        /// The hardware supports one hardware palette in 256-color mode
        /// only. The iLayerPlane parameter is ignored if this attribute
        /// is specified.
        /// </summary>
        NEED_SYSTEM_PALETTE_ARB = 0x2005,

        /// <summary>
        /// True if the pixel format supports swapping layer planes
        /// independently of the main planes. If the pixel format does not
        /// support a back buffer then this is set to FALSE. The
        /// iLayerPlane parameter is ignored if this attribute is
        /// specified.
        /// </summary>
        SWAP_LAYER_BUFFERS_ARB = 0x2006,

        /// <summary>
        /// If the pixel format supports a back buffer, then this indicates
        /// how they are swapped. If this attribute is set to
        /// WGL_SWAP_EXCHANGE_ARB then swapping exchanges the front and back
        /// buffer contents; if it is set to WGL_SWAP_COPY_ARB then swapping
        /// copies the back buffer contents to the front buffer; if it is
        /// set to WGL_SWAP_UNDEFINED_ARB then the back buffer contents are
        /// copied to the front buffer but the back buffer contents are
        /// undefined after the operation. If the pixel format does not
        /// support a back buffer then this parameter is set to
        /// WGL_SWAP_UNDEFINED_ARB. The <iLayerPlane> parameter is ignored
        /// if this attribute is specified.
        /// </summary>
        SWAP_METHOD_ARB = 0x2007,

        /// <summary>
        /// The number of overlay planes. The <iLayerPlane> parameter is
        /// ignored if this attribute is specified.
        /// </summary>
        NUMBER_OVERLAYS_ARB = 0x2008,

        /// <summary>
        /// The number of underlay planes. The <iLayerPlane> parameter is
        /// ignored if this attribute is specified.
        /// </summary>
        NUMBER_UNDERLAYS_ARB = 0x2009,

        /// <summary>
        /// True if transparency is supported.
        /// </summary>
        TRANSPARENT_ARB = 0x200A,

        /// <summary>
        /// Specifies the transparent red color value. Typically this value
        /// is the same for all layer planes. This value is undefined if
        /// transparency is not supported.
        /// </summary>
        TRANSPARENT_RED_VALUE_ARB = 0x2037,

        /// <summary>
        /// Specifies the transparent green value. Typically this value is
        /// the same for all layer planes. This value is undefined if
        /// transparency is not supported.
        /// </summary>
        TRANSPARENT_GREEN_VALUE_ARB = 0x2038,

        /// <summary>
        /// Specifies the transparent blue color value. Typically this value
        /// is the same for all layer planes. This value is undefined if
        /// transparency is not supported.
        /// </summary>
        TRANSPARENT_BLUE_VALUE_ARB = 0x2039,

        /// <summary>
        /// Specifies the transparent alpha value. This is reserved for
        /// future use.
        /// </summary>
        TRANSPARENT_ALPHA_VALUE_ARB = 0x203A,

        /// <summary>
        /// Specifies the transparent color index value. Typically this
        /// value is the same for all layer planes. This value is undefined
        /// if transparency is not supported.
        /// </summary>
        TRANSPARENT_INDEX_VALUE_ARB = 0x203B,

        /// <summary>
        /// True if the layer plane shares the depth buffer with the main
        /// planes. If <iLayerPlane> is zero, this is always true.
        /// </summary>
        SHARE_DEPTH_ARB = 0x200C,

        /// <summary>
        ///  True if the layer plane shares the stencil buffer with the main
        /// planes. If <iLayerPlane> is zero, this is always true.
        /// </summary>
        SHARE_STENCIL_ARB = 0x200D,

        /// <summary>
        /// True if the layer plane shares the accumulation buffer with the
        /// main planes. If <iLayerPlane> is zero, this is always true.
        /// </summary>
        SHARE_ACCUM_ARB = 0x200E,

        /// <summary>
        /// True if GDI rendering is supported.
        /// </summary>
        SUPPORT_GDI_ARB = 0x200F,

        /// <summary>
        /// True if OpenGL is supported.
        /// </summary>
        SUPPORT_OPENGL_ARB = 0x2010,

        /// <summary>
        /// True if the color buffer has back/front pairs.
        /// </summary>
        DOUBLE_BUFFER_ARB = 0x2011,

        /// <summary>
        /// True if the color buffer has left/right pairs.
        /// </summary>
        STEREO_ARB = 0x2012,

        /// <summary>
        ///  The type of pixel data. This can be set to WGL_TYPE_RGBA_ARB or
        /// WGL_TYPE_COLORINDEX_ARB.
        /// </summary>
        PIXEL_TYPE_ARB = 0x2013,

        /// <summary>
        /// The number of color bitplanes in each color buffer. For RGBA
        /// pixel types, it is the size of the color buffer, excluding the
        /// alpha bitplanes. For color-index pixels, it is the size of the
        /// color index buffer.
        /// </summary>
        COLOR_BITS_ARB = 0x2014,

        /// <summary>
        /// The number of red bitplanes in each RGBA color buffer.
        /// </summary>
        RED_BITS_ARB = 0x2015,

        /// <summary>
        /// The shift count for red bitplanes in each RGBA color buffer.
        /// </summary>
        RED_SHIFT_ARB = 0x2016,

        /// <summary>
        /// The number of green bitplanes in each RGBA color buffer.
        /// </summary>
        GREEN_BITS_ARB = 0x2017,

        /// <summary>
        /// The shift count for green bitplanes in each RGBA color buffer.
        /// </summary>
        GREEN_SHIFT_ARB = 0x2018,

        /// <summary>
        /// The number of blue bitplanes in each RGBA color buffer.
        /// </summary>
        BLUE_BITS_ARB = 0x2019,

        /// <summary>
        /// The shift count for blue bitplanes in each RGBA color buffer.
        /// </summary>
        BLUE_SHIFT_ARB = 0x201A,

        /// <summary>
        /// The number of alpha bitplanes in each RGBA color buffer.
        /// </summary>
        ALPHA_BITS_ARB = 0x201B,

        /// <summary>
        /// The shift count for alpha bitplanes in each RGBA color buffer.
        /// </summary>
        ALPHA_SHIFT_ARB = 0x201C,

        /// <summary>
        /// The total number of bitplanes in the accumulation buffer.
        /// </summary>
        ACCUM_BITS_ARB = 0x201D,

        /// <summary>
        /// The number of red bitplanes in the accumulation buffer.
        /// </summary>
        ACCUM_RED_BITS_ARB = 0x201E,

        /// <summary>
        /// The number of green bitplanes in the accumulation buffer.
        /// </summary>
        ACCUM_GREEN_BITS_ARB = 0x201F,

        /// <summary>
        /// The number of blue bitplanes in the accumulation buffer.
        /// </summary>
        ACCUM_BLUE_BITS_ARB = 0x2020,

        /// <summary>
        /// The number of alpha bitplanes in the accumulation buffer.
        /// </summary>
        ACCUM_ALPHA_BITS_ARB = 0x2021,

        /// <summary>
        /// The depth of the depth (z-axis) buffer.
        /// </summary>
        DEPTH_BITS_ARB = 0x2022,

        /// <summary>
        /// The depth of the stencil buffer.
        /// </summary>
        STENCIL_BITS_ARB = 0x2023,

        /// <summary>
        /// The number of auxiliary buffers.
        /// </summary>
        AUX_BUFFERS_ARB = 0x2024,

        // ### WGL_ARB_multisample ###
        SAMPLE_BUFFERS_ARB = 0x2041,
        SAMPLES_ARB = 0x2042,

        // ### WGL_ARB_framebuffer_sRGB ###
        FRAMEBUFFER_SRGB_CAPABLE_ARB = 0x20A9,
    }

    /// <summary>
    /// Indicates whether the pixel format is supported by the driver.
    /// </summary>
    public enum WGLAcceleration
    {
        /// <summary>
        /// Only the software renderer supports this pixel format.
        /// </summary>
        NO_ACCELERATION_ARB = 0x2025,

        /// <summary>
        /// The pixel format is supported by an MCD driver.
        /// </summary>
        GENERIC_ACCELERATION_ARB = 0x2026,

        /// <summary>
        ///  The pixel format is supported by an ICD driver.
        /// </summary>
        FULL_ACCELERATION_ARB = 0x2027,
    }

    public enum WGLSwapMethod
    {
        /// <summary>
        /// Swapping exchanges the front and back buffer contents.
        /// </summary>
        SWAP_EXCHANGE_ARB = 0x2028,

        /// <summary>
        /// Swapping copies the back buffer contents to the front buffer.
        /// </summary>
        SWAP_COPY_ARB = 0x2029,

        /// <summary>
        /// the back buffer contents are
        /// copied to the front buffer but the back buffer contents are
        /// undefined after the operation. If the pixel format does not
        /// support a back buffer then this parameter is set to
        /// WGL_SWAP_UNDEFINED_ARB.
        /// </summary>
        SWAP_UNDEFINED_ARB = 0x202A,
    }

    public enum WGLColorType
    {
        TYPE_RGBA_ARB = 0x202B,
        TYPE_COLORINDEX_ARB = 0x202C,
    }

    public enum WGLContextAttribs : int
    {
        CONTEXT_MAJOR_VERSION_ARB = 0x2091,
        CONTEXT_MINOR_VERSION_ARB = 0x2092,
        CONTEXT_LAYER_PLANE_ARB = 0x2093,
        CONTEXT_FLAGS_ARB = 0x2094,
        CONTEXT_PROFILE_MASK_ARB = 0x9126,
    }
}
