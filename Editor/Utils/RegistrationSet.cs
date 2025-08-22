#nullable enable
#if UNITY_2023_2_OR_NEWER
#define HAS_REGISTER_CALLBACK_ONCE
#endif

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Utils
{
    
    internal class RegistrationSet: IDisposable
    {
        private readonly VisualElement _target;
        private readonly Dictionary<(object callback, TrickleDown useTrickleDown), Action> _listeners = new();
        private readonly SynchronizationContext _synchronizationContext;

        public RegistrationSet(VisualElement target)
        {
            _target = target;
            _synchronizationContext = SynchronizationContext.Current;
        }
        
        public void RegisterCallback<TEvent>(EventCallback<TEvent> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEvent: EventBase<TEvent>, new()
        {
            _target.RegisterCallback(callback);
            _listeners.Add((callback, useTrickleDown), () => _target.UnregisterCallback(callback, useTrickleDown));
        }

        public bool UnregisterCallback<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEventType: EventBase<TEventType>, new()
        {
            _target.UnregisterCallback(callback);
            return _listeners.Remove((callback, useTrickleDown));
        }
        
        public void RegisterCallbackOnce<TEventType>(EventCallback<TEventType> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown)
            where TEventType: EventBase<TEventType>, new()
        {
            #if HAS_REGISTER_CALLBACK_ONCE
            _target.RegisterCallbackOnce(callback);
            _listeners.Add((callback, useTrickleDown), () => { _target.UnregisterCallback(callback, useTrickleDown); });
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

        public void AttachedToTask(Task task)
        {
            task.ContinueWith(_ =>
            {
                _synchronizationContext.Post(self =>
                {
                    ((RegistrationSet)self).Dispose();
                }, this);
            });
        }
        
        public void AddManipulator<TManipulator>(TManipulator manipulator)
            where TManipulator: Manipulator
        {
            _target.AddManipulator(manipulator);
            _listeners.Add((manipulator, TrickleDown.NoTrickleDown), () => _target.RemoveManipulator(manipulator));
        }
        
        public void RemoveManipulator<TManipulator>(TManipulator manipulator)
            where TManipulator: Manipulator
        {
            _target.RemoveManipulator(manipulator);
            _listeners.Remove((manipulator, TrickleDown.NoTrickleDown));
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