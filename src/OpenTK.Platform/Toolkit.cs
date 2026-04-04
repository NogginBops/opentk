using OpenTK.Platform.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using OpenTK.Core.Utility;

namespace OpenTK.Platform
{
    /// <summary>
    /// An event handler delegate for platform events.
    /// </summary>
    /// <param name="args">Information associated with the event, if any.</param>
    public delegate void PlatformEventHandler(EventArgs args);

    /// <summary>
    /// Provides static access to all OpenTK platform abstraction interfaces.
    /// This is the main way to access the OpenTK PAL2 api.
    /// </summary>
    /// <seealso cref="ToolkitOptions"/>
    public static class Toolkit
    {
        private static bool Initialized = false;
        private static bool EnableOpenGL = false;
        private static bool EnableVulkan = false;

        private static IClipboardComponent? _clipboardComponent;
        private static ICursorComponent? _cursorComponent;
        private static IDisplayComponent? _displayComponent;
        private static IIconComponent? _iconComponent;
        private static IKeyboardComponent? _keyboardComponent;
        private static IMouseComponent? _mouseComponent;
        private static IOpenGLComponent? _openGLComponent;
        private static ISurfaceComponent? _surfaceComponent;
        private static IWindowComponent? _windowComponent;
        private static IShellComponent? _shellComponent;
        private static IJoystickComponent? _joystickComponent;
        private static IGamepadComponent? _gamepadComponent;
        private static IDialogComponent? _dialogComponent;
        private static IVulkanComponent? _vulkanComponent;

        /// <summary>
        /// Interface for creating, interacting with, and deleting windows.
        /// </summary>
        public static IWindowComponent Window => _windowComponent ??
            (Initialized ? null! : ThrowNotInitialized<IWindowComponent>());

        /// <summary>
        /// Interface for creating, interacting with, and deleting surfaces.
        /// </summary>
        public static ISurfaceComponent Surface => _surfaceComponent ??
            (Initialized ? null! : ThrowNotInitialized<ISurfaceComponent>());

        /// <summary>
        /// Interface for creating, interacting with, and deleting OpenGL contexts.
        /// </summary>
        public static IOpenGLComponent OpenGL
        {
            get
            {
                if (_openGLComponent != null)
                {
                    return _openGLComponent;
                }
                else if (!Initialized)
                {
                    return ThrowNotInitialized<IOpenGLComponent>();
                }
                else if (!EnableOpenGL)
                {
                    return ThrowFeatureNotEnabled<IOpenGLComponent>("OpenGL", ToolkitFlags.EnableOpenGL);
                }
                else
                {
                    return null!;
                }
            }
        }

        /// <summary>
        /// Interface for querying information about displays attached to the system.
        /// </summary>
        public static IDisplayComponent Display => _displayComponent ??
            (Initialized ? null! : ThrowNotInitialized<IDisplayComponent>());

        /// <summary>
        /// Interface for shell functions such as battery information, preferred theme, etc.
        /// </summary>
        public static IShellComponent Shell => _shellComponent ??
            (Initialized ? null! : ThrowNotInitialized<IShellComponent>());

        /// <summary>
        /// Interface for getting and setting the mouse position, and getting mouse button information.
        /// </summary>
        public static IMouseComponent Mouse => _mouseComponent ??
            (Initialized ? null! : ThrowNotInitialized<IMouseComponent>());

        /// <summary>
        /// Interface for dealing with keyboard layouts, conversions between <see cref="Key"/> and <see cref="Scancode"/>, and IME.
        /// </summary>
        public static IKeyboardComponent Keyboard => _keyboardComponent ??
            (Initialized ? null! : ThrowNotInitialized<IKeyboardComponent>());

        /// <summary>
        /// Interface for creating, interacting with, and deleting mouse cursor images.
        /// </summary>
        public static ICursorComponent Cursor => _cursorComponent ??
            (Initialized ? null! : ThrowNotInitialized<ICursorComponent>());

        /// <summary>
        /// Interface for creating, interacting with, and deleting window icon images.
        /// </summary>
        public static IIconComponent Icon => _iconComponent ??
            (Initialized ? null! : ThrowNotInitialized<IIconComponent>());

        /// <summary>
        /// Interface for getting and setting clipboard data.
        /// </summary>
        public static IClipboardComponent Clipboard => _clipboardComponent ??
            (Initialized ? null! : ThrowNotInitialized<IClipboardComponent>());

        /// <summary>
        /// Interface for getting joystick input.
        /// </summary>
        public static IJoystickComponent Joystick => _joystickComponent ??
            (Initialized ? null! : ThrowNotInitialized<IJoystickComponent>());

        /// <summary>
        /// Interface for getting gamepad input.
        /// </summary>
        public static IGamepadComponent Gamepad => _gamepadComponent ??
            (Initialized ? null! : ThrowNotInitialized<IGamepadComponent>());

        /// <summary>
        /// Interface for opening system dialogs such as file open dialogs.
        /// </summary>
        public static IDialogComponent Dialog => _dialogComponent ??
            (Initialized ? null! : ThrowNotInitialized<IDialogComponent>());

        /// <summary>
        /// Interface for creating Vulkan surfaces.
        /// </summary>
        public static IVulkanComponent Vulkan
        {
            get
            {
                if (_vulkanComponent != null)
                {
                    return _vulkanComponent;
                }
                else if (!Initialized)
                {
                    return ThrowNotInitialized<IVulkanComponent>();
                }
                else if (!EnableVulkan)
                {
                    return ThrowFeatureNotEnabled<IVulkanComponent>("Vulkan", ToolkitFlags.EnableVulkan);
                }
                else
                {
                    return null!;
                }
            }
        }

        /// <summary>
        /// Initialize OpenTK with the given settings.
        /// This function must be called before trying to use the OpenTK API.
        /// </summary>
        /// <param name="options">The options to initialize with.</param>
        /// <seealso cref="ToolkitOptions"/>
        public static void Init(ToolkitOptions options)
        {
            EnableOpenGL = options.FeatureFlags.HasFlag(ToolkitFlags.EnableOpenGL);
            EnableVulkan = options.FeatureFlags.HasFlag(ToolkitFlags.EnableVulkan);

            // FIXME: Figure out where to actually store this setting...
            // - Noggin_bops 2025-07-09
            PlatformComponents.PreferANGLE = options.FeatureFlags.HasFlag(ToolkitFlags.PreferANGLE);

            try { _windowComponent = PlatformComponents.CreateWindowComponent(); } catch (NotSupportedException) { }
            try { _surfaceComponent = PlatformComponents.CreateSurfaceComponent(); } catch (NotSupportedException) { }
            if (EnableOpenGL)
            {
                try { _openGLComponent = PlatformComponents.CreateOpenGLComponent(); } catch (NotSupportedException) { }
            }
            try { _displayComponent = PlatformComponents.CreateDisplayComponent(); } catch (NotSupportedException) { }
            try { _shellComponent = PlatformComponents.CreateShellComponent(); } catch (NotSupportedException) { }
            try { _mouseComponent = PlatformComponents.CreateMouseComponent(); } catch (NotSupportedException) { }
            try { _keyboardComponent = PlatformComponents.CreateKeyboardComponent(); } catch (NotSupportedException) { }
            try { _cursorComponent = PlatformComponents.CreateCursorComponent(); } catch (NotSupportedException) { }
            try { _iconComponent = PlatformComponents.CreateIconComponent(); } catch (NotSupportedException) { }
            try { _clipboardComponent = PlatformComponents.CreateClipboardComponent(); } catch (NotSupportedException) { }
            try { _joystickComponent = PlatformComponents.CreateJoystickComponent(); } catch (NotSupportedException) { }
            try { _gamepadComponent = PlatformComponents.CreateGamepadComponent(); } catch (NotSupportedException) { }
            try { _dialogComponent = PlatformComponents.CreateDialogComponent(); } catch (NotSupportedException) { }
            if (EnableVulkan)
            {
                try { _vulkanComponent = PlatformComponents.CreateVulkanComponent(); } catch (NotSupportedException) { }
            }

            if (_windowComponent != null)
                _windowComponent.Logger = options.Logger;
            if (_surfaceComponent != null)
                _surfaceComponent.Logger = options.Logger;
            if (_openGLComponent != null)
                _openGLComponent.Logger = options.Logger;
            if (_displayComponent != null)
                _displayComponent.Logger = options.Logger;
            if (_shellComponent != null)
                _shellComponent.Logger = options.Logger;
            if (_mouseComponent != null)
                _mouseComponent.Logger = options.Logger;
            if (_keyboardComponent != null)
                _keyboardComponent.Logger = options.Logger;
            if (_cursorComponent != null)
                _cursorComponent.Logger = options.Logger;
            if (_iconComponent != null)
                _iconComponent.Logger = options.Logger;
            if (_clipboardComponent != null)
                _clipboardComponent.Logger = options.Logger;
            if (_joystickComponent != null)
                _joystickComponent.Logger = options.Logger;
            if (_gamepadComponent != null)
                _gamepadComponent.Logger = options.Logger;
            if (_dialogComponent != null)
                _dialogComponent.Logger = options.Logger;
            if (_vulkanComponent != null)
                _vulkanComponent.Logger = options.Logger;

            _windowComponent?.Initialize(options);
            _surfaceComponent?.Initialize(options);
            _openGLComponent?.Initialize(options);
            _displayComponent?.Initialize(options);
            _shellComponent?.Initialize(options);
            _mouseComponent?.Initialize(options);
            _keyboardComponent?.Initialize(options);
            _cursorComponent?.Initialize(options);
            _iconComponent?.Initialize(options);
            _clipboardComponent?.Initialize(options);
            _joystickComponent?.Initialize(options);
            _gamepadComponent?.Initialize(options);
            _dialogComponent?.Initialize(options);
            _vulkanComponent?.Initialize(options);
            Ext.Initialize(options);

            Initialized = true;
        }

        /// <summary>
        /// Uninitialize OpenTK.
        /// This frees any native resources held.
        /// All allocated windows, opengl contexts, etc should be closed before calling this function.
        /// This function does not need to be called when exiting the application.
        /// This function is only useful if the application will keep running after OpenTK has been uninitialized.
        /// </summary>
        /// <remarks>
        /// There are some irreversible settings on some platforms that cannot be undone once OpenTK has been initialized.
        /// What follows is a list of things that cannot be undone:
        /// <list type="bullet">
        /// <item>DPI awareness in windows is a per process setting that can only be set once.</item>
        /// </list>
        /// </remarks>
        public static void Uninit()
        {
            Ext.Uninitialize();
            _vulkanComponent?.Uninitialize();
            _dialogComponent?.Uninitialize();
            _joystickComponent?.Uninitialize();
            _clipboardComponent?.Uninitialize();
            _iconComponent?.Uninitialize();
            _cursorComponent?.Uninitialize();
            _keyboardComponent?.Uninitialize();
            _mouseComponent?.Uninitialize();
            _shellComponent?.Uninitialize();
            _displayComponent?.Uninitialize();
            _openGLComponent?.Uninitialize();
            _surfaceComponent?.Uninitialize();
            _windowComponent?.Uninitialize();

            _vulkanComponent = null;
            _dialogComponent = null;
            _joystickComponent = null;
            _clipboardComponent = null;
            _iconComponent = null;
            _cursorComponent = null;
            _keyboardComponent = null;
            _mouseComponent = null;
            _shellComponent = null;
            _displayComponent = null;
            _openGLComponent = null;
            _surfaceComponent = null;
            _windowComponent = null;

            Initialized = false;
            EnableOpenGL = false;
            EnableVulkan = false;
        }

        [DoesNotReturn]
        private static T ThrowNotInitialized<T>() where T : IPalComponent
        {
            throw new InvalidOperationException("You need to call Toolkit.Init() before you can use it.");
        }

        [DoesNotReturn]
        private static T ThrowFeatureNotEnabled<T>(string featureName, ToolkitFlags flag) where T : IPalComponent
        {
            throw new InvalidOperationException($"You need to enable {featureName} by adding {nameof(ToolkitFlags)}.{flag} to {nameof(ToolkitOptions)}.{nameof(ToolkitOptions.FeatureFlags)} before you can use {featureName}.");
        }

        /// <summary>
        /// Event component for interacting with platform events.
        /// </summary>
        public static class Event
        {
            /// <summary>
            /// Invoked when an event is raised.
            /// </summary>
            public static event PlatformEventHandler? EventRaised;

            /// <summary>
            /// Raise an event without notifying waiters.
            /// </summary>
            /// <param name="args">The event to raise.</param>
            public static void RaiseEvent(EventArgs args)
            {
                EventRaised?.Invoke(args);
            }

            /// <summary>
            /// Raise an event and notify anyone waiting for events.
            /// </summary>
            /// <param name="args">The event to raise.</param>
            public static void RaiseEventNotify(EventArgs args)
            {
                Toolkit.Window.PostUserEvent(args);
            }

            /// <summary>
            /// Process platform events and send them to the <see cref="EventRaised"/> callback.
            /// </summary>
            /// <param name="waitForEvents">Specifies if this function should wait for events or return immediately if there are no events.</param>
            public static void ProcessEvents(bool waitForEvents)
            {
                Toolkit.Window.ProcessEvents(waitForEvents);
            }
        }

        /// <summary>
        /// PAL2 Extension Registry.
        /// </summary>
        public static class Ext
        {
            private static bool _isInit = false;
            private static ILogger? _logger;
            private static readonly object _lockObject = new object();
            private static readonly List<IPalExtension> _extensions = new List<IPalExtension>();
            private static readonly Dictionary<string, IPalExtension> _byName = new Dictionary<string, IPalExtension>();
            private static readonly Dictionary<int, IPalExtension> _byAtom = new Dictionary<int, IPalExtension>();

            /// <summary>
            /// List of extensions currently available in the registry.
            /// </summary>
            public static IReadOnlyList<IPalExtension> Available { get; }

            static Ext()
            {
                Available = _extensions.AsReadOnly();
            }

            /// <summary>
            /// Register a new extension.
            /// </summary>
            /// <typeparam name="T">Type of the new extension to register.</typeparam>
            public static void RegisterExtension<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)]T>() where T : class, IPalExtension, new() => RegisterExtension(new T());

            /// <summary>
            /// Register a new extension.
            /// </summary>
            /// <typeparam name="T">Type of the new extension to register.</typeparam>
            public static void RegisterExtension<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] T>( T extension) where T : class, IPalExtension
            {
                lock (_lockObject)
                {
                    if (_isInit)
                    {
                        extension.Logger = _logger;
                        extension.Initialize();
                    }

                    _byName.Add(extension.Name, extension);
                    _byAtom.Add(Atom<T>.Id, extension);
                    _extensions.Add(extension);

                    // For this extension system to work, we must browse the type hierarchy and find ancestor
                    // interfaces and their type atoms.
                    // Otherwise, we cannot require extensions by their ancestor interface.

                    // What are the NativeAOT caveats here? Users may be forced to use strings as keys. - mixed.

                    Type type = typeof(T);
                    foreach (Type iface in type.GetInterfaces().Where(x => x == typeof(IPalExtension)))
                    {
                        _byAtom.Add(Atom.Get(iface).Id, extension);
                    }

                    _logger?.LogDebug($"Extension {extension.Name} has been registered.");
                }
            }

            internal static void Initialize(ToolkitOptions options)
            {
                if (_isInit)
                    return;

                _logger = options.Logger;

                _isInit = true;
                foreach (IPalExtension extension in _extensions)
                {
                    extension.Logger = _logger;
                    extension.Initialize();
                }
            }

            internal static void Uninitialize()
            {
                foreach (IPalExtension extension in _extensions)
                {
                    extension.Uninitialize();
                }

                _extensions.Clear();
                _byAtom.Clear();
                _byName.Clear();

                _isInit = false;
            }

            /// <summary>
            /// Require an extension.
            /// </summary>
            /// <typeparam name="T">Type of the extension class or its extension interface.</typeparam>
            /// <returns>The extension instance.</returns>
            public static T Require<T>() where T : IPalExtension
            {
                return (T)_byAtom[Atom<T>.Id];
            }

            /// <summary>
            /// Require an extension.
            /// </summary>
            /// <param name="name">Name of the extension.</param>
            /// <returns>The extension instance.</returns>
            public static IPalExtension Require(string name)
            {
                return _byName[name];
            }

            /// <summary>
            /// Require an extension.
            /// </summary>
            /// <param name="name">Name of the extension.</param>
            /// <typeparam name="T">Type of the extension class or its extension interface.</typeparam>
            /// <returns>The extension instance.</returns>
            public static T Require<T>(string name) where T : IPalExtension
            {
                return (T)_byName[name];
            }

            /// <summary>
            /// Check if an extension is available.
            /// </summary>
            /// <typeparam name="T">Type of the extension class or its extension interface.</typeparam>
            /// <returns>The extension instance.</returns>
            public static bool IsAvailable<T>() where T : IPalExtension
            {
                return _byAtom.ContainsKey(Atom<T>.Id);
            }

            /// <summary>
            /// Check if an extension is available.
            /// </summary>
            /// <param name="name">Name of the extension.</param>
            /// <returns>The extension instance.</returns>
            public static bool IsAvailable(string name)
            {
                return _byName.ContainsKey(name);
            }

            // This whole section of code is a micro optimization. It accelerates type lookup without depending
            // on behavior of Type.GetHashCode() or Type.Equals by abusing generics.
            // Feel free to take this out if you think this is too complex.

            private struct Atom
            {
                public int Id { get; }
                public Type Type { get; }

                private Atom(Type type)
                {
                    Type = type;
                    Id = TakeAtom();
                }

                private static int _atoms = 0;
                private static readonly object _lockObject = new object();
                private static readonly Dictionary<int, Atom> _byId = new Dictionary<int, Atom>();
                private static readonly Dictionary<Type, Atom> _byType = new Dictionary<Type, Atom>();

                private int TakeAtom() => Interlocked.Increment(ref _atoms);

                public static Atom? Get(int id) => _byId.GetValueOrDefault(id);
                public static Atom Get(Type type)
                {
                    if (_byType.TryGetValue(type, out Atom atom))
                        return atom;

                    // Type is not registered, try to acquire lock.
                    lock (_lockObject)
                    {
                        // Maybe somebody else registered this type whilst acquiring the lock.
                        if (_byType.TryGetValue(type, out atom))
                            return atom;

                        // Register the type if applicable and leave.
                        atom = new Atom(type);
                        _byId.Add(atom.Id, atom);
                        _byType.Add(type, atom);
                        return atom;
                    }
                }
            }

            private struct Atom<T> where T : IPalExtension
            {
                public static Atom Value { get; } = Atom.Get(typeof(T));
                public static int Id => Value.Id;
                public static Type Type => Value.Type;
            }
        }
    }
}
