using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Commands
{
    public interface INavigationCommand
    {
        
    }
    
    public class AbstractNavigationCommand<T> : EventBase<T>, INavigationCommand
        where T: AbstractNavigationCommand<T>, new()
    {
    }
}