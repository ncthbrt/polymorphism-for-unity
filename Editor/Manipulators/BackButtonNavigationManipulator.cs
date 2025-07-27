using System;
using Polymorphism4Unity.Editor.Commands;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Manipulators
{
    public class BackButtonNavigationManipulator: Manipulator
    {
        public event Action<INavigationCommand> OnNavigate;

        public BackButtonNavigationManipulator(Action<INavigationCommand, EventBase> onNavigate)
        {
            OnNavigate = onNavigate;
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.AddManipulator<Clickable>();
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            throw new System.NotImplementedException();
        }
    }
}