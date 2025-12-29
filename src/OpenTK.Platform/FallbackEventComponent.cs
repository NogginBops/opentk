using System;
using OpenTK.Core.Utility;

namespace OpenTK.Platform
{
    /// <summary>
    /// Temporary object to handle old PAL drivers with the new event API.
    /// </summary>
    /// <param name="windowComponent">The window component.</param>
    public class FallbackEventComponent(IWindowComponent windowComponent) : IEventComponent
    {
        /// <inheritdoc />
        public string Name { get; } = "OpenTK PAL2 Fallback Event Component";

        /// <inheritdoc />
        public PalComponents Provides { get; } = PalComponents.Event;

        /// <inheritdoc />
        public ILogger? Logger { get; set; }

        /// <inheritdoc />
        public event PlatformEventHandler? EventRaised;

        /// <inheritdoc />
        public event PlatformEventHandlerEx? EventRaisedEx;

        /// <inheritdoc />
        public void Initialize(ToolkitOptions options)
        {
            Logger?.LogWarning($"Initializing IEventComponent with FallbackEventComponent. The platform driver '{windowComponent.Name}' should port to the new IEventComponent.");
            EventQueue.EventRaised += OnEventRaised;
        }

        /// <inheritdoc />
        public void Uninitialize()
        {
            EventQueue.EventRaised -= OnEventRaised;
        }

        private void OnEventRaised(PalHandle? handle, PlatformEventType type, EventArgs args)
        {
            EventRaised?.Invoke(handle, type, args);
            EventRaisedEx?.Invoke(handle, args);
        }

        /// <inheritdoc />
        public void PostUserEvent(EventArgs @event)
        {
            windowComponent.PostUserEvent(@event);
        }

        /// <inheritdoc />
        public void ProcessEvents(bool waitForEvents)
        {
            windowComponent.ProcessEvents(waitForEvents);
        }
    }
}
