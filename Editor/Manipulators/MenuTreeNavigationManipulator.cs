#nullable enable
using System;
using Polymorphism4Unity.Editor.Commands;
using UnityEngine;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Manipulators
{
    public class MenuTreeNavigationManipulator : Manipulator
    {
        private readonly KeyboardNavigationManipulator _keyboardNavigationManipulator;

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
            if (upEvent.keyCode == KeyCode.Backspace)
            {
                NavigateBackCommand command = NavigateBackCommand.GetPooled();
                command.target = target;
                command.BaseEvent = upEvent;
                upEvent.StopPropagation();
                target.SendEvent(command);
            }
            else if (upEvent.keyCode == KeyCode.Escape)
            {
                NavigateCancelCommand command = NavigateCancelCommand.GetPooled();
                command.target = target;
                command.BaseEvent = upEvent;
                upEvent.StopPropagation();
                target.SendEvent(command);
            }
            char upEventCharacter = upEvent.character;
            if (upEventCharacter != '\0')
            {
                NavigateSearchCommand command = NavigateSearchCommand.GetPooled();
                command.Character = upEventCharacter;
                upEvent.StopPropagation();
                command.target = target;
                ((INavigationCommand)command).BaseEvent = upEvent;
                target.SendEvent(command);
            }
        }

        private void HandleKeyboardNavigationEvent(KeyboardNavigationOperation navigationOperation, EventBase baseEvent)
        {
            EventBase command;
            switch (navigationOperation)
            {
                case KeyboardNavigationOperation.None:
                case KeyboardNavigationOperation.SelectAll:
                    return;
                case KeyboardNavigationOperation.MoveLeft:
                case KeyboardNavigationOperation.Cancel:
                {
                    
                    command = NavigateBackCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.MoveRight:
                case KeyboardNavigationOperation.Submit:
                {
                    command = NavigateSubmitCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.Previous:
                {
                    command = NavigateUpCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.Next:
                {
                    command = NavigateDownCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.PageUp:
                {
                    command = NavigatePageUpCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.PageDown:
                {
                    command = NavigatePageDownCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.Begin:
                {
                    command = NavigateTopCommand.GetPooled();
                    break;
                }
                case KeyboardNavigationOperation.End:
                {
                    command = NavigateBottomCommand.GetPooled();
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(navigationOperation), navigationOperation, null);
                }
            }
            command.target = target;
            ((INavigationCommand)command).BaseEvent = baseEvent;
            target.SendEvent(command);
        }
    }
}