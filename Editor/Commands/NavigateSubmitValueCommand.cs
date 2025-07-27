using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Commands
{
    public class NavigateSubmitValueCommand<T>: AbstractNavigationCommand<NavigateSubmitValueCommand<T>>
    {
        public T Value { get; set; }    
    }
}