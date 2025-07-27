#nullable enable
using System;
using Polymorphism4Unity.Editor.Commands;
using Polymorphism4Unity.Safety;
using UnityEngine;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Manipulators
{
    public class BackButtonNavigationManipulator: Manipulator
    {
        public Action<INavigationCommand, EventBase> NavigationHandler { get; set; }
        private readonly Clickable _clickable;

        public BackButtonNavigationManipulator(Action<INavigationCommand, EventBase> navigationHandler)
        {
            NavigationHandler = navigationHandler;
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
            Asserts.IsNotNull(NavigationHandler);
            try
            {
                NavigationHandler.Invoke(navigateBackCommand, clickEvent);
            }
            catch (Exception e)
            {
                Debug.LogError($"{nameof(NavigationHandler)} raised an exception for {target}");
                Debug.LogException(e);
            }
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.RemoveManipulator(_clickable);
        }
    }
}