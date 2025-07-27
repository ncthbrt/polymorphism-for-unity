#nullable enable
using System;
using Polymorphism4Unity.Editor.Commands;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Manipulators
{
    public class MenuTreeNavigationManipulator<T>: Manipulator
    {
        private readonly KeyboardNavigationManipulator _keyboardNavigationManipulator;
        private readonly 
        
        public MenuTreeNavigationManipulator()
        {
            _keyboardNavigationManipulator = new KeyboardNavigationManipulator(HandleKeyboardNavigationEvent);
        }
        
        protected override void RegisterCallbacksOnTarget()
        {
            target.AddManipulator(_keyboardNavigationManipulator);
            target.RegisterCallback<KeyUpEvent>(HandleKeyUpEvent);
        }

        
        protected override void UnregisterCallbacksFromTarget()
        {
            target.RemoveManipulator(_keyboardNavigationManipulator);
            target.UnregisterCallback<KeyUpEvent>(HandleKeyUpEvent);
        }

        private void HandleKeyUpEvent(KeyUpEvent upEvent)
        {
            char upEventCharacter = upEvent.character;
        }

        private void HandleKeyboardNavigationEvent(KeyboardNavigationOperation navigationOperation, EventBase baseEvent)
        {
            EventBase navigationEvent;
            switch (navigationOperation)
            {
                case KeyboardNavigationOperation.None:
                case KeyboardNavigationOperation.SelectAll:
                    return;
                case KeyboardNavigationOperation.MoveLeft:
                case KeyboardNavigationOperation.Cancel:
                {
                    navigationEvent = NavigateBackCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.MoveRight:
                case KeyboardNavigationOperation.Submit:
                {
                    if (target is IHasReadOnlyValue<T> value)
                    {
                        NavigateSubmitValueCommand<T> submitCommand = NavigateSubmitValueCommand<T>.GetPooled();
                        navigationEvent = submitCommand;
                        submitCommand.Value = value.Value;
                    }
                    else
                    {
                        return;
                    }
                    break;
                }
                case KeyboardNavigationOperation.Previous:
                {
                    navigationEvent = NavigateUpCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.Next:
                {
                    navigationEvent = NavigateDownCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.PageUp:
                {
                    navigationEvent = NavigatePageUpCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.PageDown:
                {
                    navigationEvent = NavigatePageDownCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.Begin:
                {
                    navigationEvent = NavigateTopCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.End:
                {
                    navigationEvent = NavigateEndCommand.GetPooled();
                    break;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(navigationOperation), navigationOperation, null);
            }
            target.SendEvent(navigationEvent);
        }

        
    }
}