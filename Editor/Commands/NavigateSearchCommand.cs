using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Commands
{
    public class NavigateSearchCommand: AbstractNavigationCommand<NavigateSearchCommand>
    {
        public char Character { get; set; }
    }
}   