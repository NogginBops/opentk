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
    internal class EventComponentProxy : IEventComponent, IDisposable
    {
        // FIXME: is this pattern actually called the proxy pattern, or is my lack of education failing me - mixed.

        public string Name { get; } = "OpenTK Event Queue Proxy";
        public PalComponents Provides { get; } = PalComponents.Event;
        public ILogger? Logger { get; set; }

        public event PlatformEventHandler? EventRaised;
        public event PlatformEventHandlerEx? EventRaisedEx;

        private readonly List<Unsubscriber> _unsubscribers = new List<Unsubscriber>();
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


        public IDisposable Subscribe(IEventObserver observer)
        {
            AssertNotDisposed();

            Unsubscriber unsubscriber = new Unsubscriber(new WeakReference<EventComponentProxy>(this), observer);
            lock (_unsubscribers)
            {
                _unsubscribers.Add(unsubscriber);
            }

            return unsubscriber;
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

                lock (_unsubscribers)
                {
                    foreach (Unsubscriber unsubscriber in _unsubscribers)
                    {
                        unsubscriber.ActualUnsubscriber = component.Subscribe(unsubscriber.Observer);
                    }
                }
            }
        }

        private void Unsubscribe(Unsubscriber unsubscriber)
        {
            if (_isDisposed)
                return;

            lock (_unsubscribers)
            {
                _unsubscribers.Remove(unsubscriber);
            }
        }

        public void Dispose()
        {
            // TODO: does this function need to be thread safe?

            if (_isDisposed) return;
            _isDisposed = true;

            _unsubscribers.Clear();
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

        private class Unsubscriber(WeakReference<EventComponentProxy> proxyHandle, IEventObserver observer) : IDisposable
        {
            public IEventObserver Observer { get; } = observer;
            public IDisposable? ActualUnsubscriber = null;

            public void Dispose()
            {
                if (proxyHandle.TryGetTarget(out EventComponentProxy? proxy))
                {
                    proxy.Unsubscribe(this);
                }

                ActualUnsubscriber?.Dispose();
            }
        }
    }
}
