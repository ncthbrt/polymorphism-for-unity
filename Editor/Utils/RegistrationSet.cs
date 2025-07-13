#nullable enable
#if UNITY_2023_2_OR_NEWER
#define HAS_REGISTER_CALLBACK_ONCE
#endif

using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Utils
{
    
    internal class RegistrationSet: IDisposable
    {
        private readonly CallbackEventHandler _handler;
        private readonly Dictionary<(object callback, TrickleDown useTrickleDown), Action> _listeners = new();

        public RegistrationSet(CallbackEventHandler handler)
        {
            _handler = handler;
        }
        
        public void RegisterCallback<TEvent>(EventCallback<TEvent> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEvent: EventBase<TEvent>, new()
        {
            _handler.RegisterCallback(callback);
            _listeners.Add((callback, useTrickleDown), () => _handler.UnregisterCallback(callback, useTrickleDown));
        }

        public bool UnregisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEventType: EventBase<TEventType>, new()
        {
            _handler.UnregisterCallback(callback);
            return _listeners.Remove((callback, useTrickleDown));
        }
        
        public void RegisterCallbackOnce<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEventType: EventBase<TEventType>, new()
        {
            #if HAS_REGISTER_CALLBACK_ONCE
            _handler.RegisterCallbackOnce(callback);
            _listeners.Add((callback, useTrickleDown), () => { _handler.UnregisterCallback(callback, useTrickleDown); });
            #else
            void Handler(TEventType evt)
            {
                try
                {
                    callback(evt);
                }
                finally
                {
                    UnregisterCallback(callback);
                }
            }
            _handler.RegisterCallback<TEventType>(Handler);
            _listeners.Add((callback, useTrickleDown), () => { _handler.UnregisterCallback<TEventType>(Handler); });
            #endif
        }
        
        public void Dispose()
        {
            foreach (Action unsub in _listeners.Values)
            {
                try
                {
                    unsub();
                }
                catch
                {
                    // ignore
                }
            }
            _listeners.Clear();
        }
    }
}