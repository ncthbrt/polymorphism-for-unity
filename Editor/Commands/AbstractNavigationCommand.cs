#nullable enable
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Commands
{
    public interface INavigationCommand
    {
        public EventBase? BaseEvent { get; set; }
    }
    
    public class AbstractNavigationCommand<T> : EventBase<T>, INavigationCommand
        where T: AbstractNavigationCommand<T>, new()
    {
        public EventBase? BaseEvent { get; set; } = null;
    }
}