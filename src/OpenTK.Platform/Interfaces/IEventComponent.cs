using System;

namespace OpenTK.Platform
{
    /// <summary>
    /// Provides event
    /// </summary>
    public interface IEventComponent : IPalComponent
    {
        // FIXME: Remove EventRaisedEx when we decide if type matching is the way to go.

        /// <summary>
        /// Called when an event is raised.
        /// </summary>
        /// <remarks>
        /// This event is thread safe.
        /// </remarks>
        event PlatformEventHandler? EventRaised;

        /// <summary>
        /// Called when an event is raised.
        /// </summary>
        /// <remarks>
        /// This event is thread safe.
        /// </remarks>
        event PlatformEventHandlerEx? EventRaisedEx;

        /// <summary>
        /// Posts a user defined event to the event queue.
        /// This is useful when using the <see cref="ProcessEvents(bool)"/> with <see langword="true"/> to wait for events.
        /// Then this method can be used to manually post an event and wake up the main thread.
        /// Sending events through this function has overhead so unnecessary calls to this function should be avoided.
        /// </summary>
        /// <remarks>
        /// This function is allowed to be called from any thread.
        /// </remarks>
        /// <param name="event">The event object to pass.</param>
        void PostUserEvent(EventArgs @event);

        /// <summary>
        /// Processes platform events and sends them to the <see cref="EventQueue"/>.
        /// </summary>
        /// <param name="waitForEvents">Specifies if this function should wait for events or return immediately if there are no events.</param>
        void ProcessEvents(bool waitForEvents);
    }
}
