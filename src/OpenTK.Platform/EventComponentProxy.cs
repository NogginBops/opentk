using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using OpenTK.Core.Utility;

namespace OpenTK.Platform
{
    /// <summary>
    /// Provides temporary services before the true event component is
    /// </summary>
    internal class EventComponentProxy : IEventComponent
    {
        // FIXME: is this pattern actually called the proxy pattern, or is my lack of education failing me - mixed.

        public string Name { get; } = "OpenTK Event Queue Proxy";
        public PalComponents Provides { get; } = PalComponents.Event;
        public ILogger? Logger { get; set; }

        public event PlatformEventHandler? EventRaised;
        public event PlatformEventHandlerEx? EventRaisedEx;

        private bool _isDisposed = false;

        public void Initialize(ToolkitOptions options)
        {
            // I knowingly call Debug.Fail() here, this is not a normal log item. But I don't think it should cause an exception. - mixed.
            Debug.Fail("EventComponentProxy.Initialize() should never have been called.");
        }

        public void Uninitialize()
        {
            Debug.Fail("EventComponentProxy.Uninitialize() should never have been called.");
        }

        public void PostUserEvent(EventArgs @event)
        {
            ThrowNotInitialized();
        }

        public void ProcessEvents(bool waitForEvents)
        {
            ThrowNotInitialized();
        }

        public void TransferSubscribers(IEventComponent component)
        {
            AssertNotDisposed();

            // This isn't exactly pretty, but I think it should prevent most race conditions.
            lock (this)
            {
                if (EventRaised != null)
                    component.EventRaised += EventRaised;

                if (EventRaisedEx != null)
                    component.EventRaisedEx += EventRaisedEx;
            }
        }

        [DoesNotReturn]
        private static void ThrowNotInitialized()
        {
            throw new InvalidOperationException("You need to call Toolkit.Init() before you can use it.");
        }

        private void AssertNotDisposed()
        {
            if (_isDisposed)
                throw new InvalidOperationException("The event queue proxy was disposed.");
        }
    }
}
