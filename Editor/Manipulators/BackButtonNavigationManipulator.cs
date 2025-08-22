#nullable enable
using Polymorphism4Unity.Editor.Commands;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Manipulators
{
    public class BackButtonNavigationManipulator: Manipulator
    {
        private readonly Clickable _clickable;

        public BackButtonNavigationManipulator()
        {
            _clickable = new Clickable(HandleClickEvent);
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.AddManipulator(_clickable);
        }

        private void HandleClickEvent() 
        {
            ClickEvent clickEvent = ClickEvent.GetPooled();
            clickEvent.target = target;
            NavigateBackCommand navigateBackCommand = NavigateBackCommand.GetPooled();
            navigateBackCommand.target = target;
            navigateBackCommand.BaseEvent = clickEvent;
            target.SendEvent(navigateBackCommand);    
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.RemoveManipulator(_clickable);
        }
    }
}