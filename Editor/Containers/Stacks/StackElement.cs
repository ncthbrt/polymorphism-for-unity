#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEngine.UIElements;
using static Polymorphism4Unity.Editor.Utils.FuncUtils;

namespace Polymorphism4Unity.Editor.Containers.Stacks
{
    [UxmlElement(nameof(StackElement)), PublicAPI]
    public partial class StackElement : VisualElement
    {
        private const string StackEmptyErrorMessage = "Stack is empty";
        protected static Exception StackEmptyException =>
            new InvalidOperationException(StackEmptyErrorMessage);

        [ContractAnnotation(" => halt")]
        protected static void RaiseStackEmptyException()
        {
            throw StackEmptyException;
        }


        [UxmlAttribute]
        public string StackId { get; set; } = string.Empty;

        private RegistrationSet? _registrationSet; 
        private Stack<StackFrameElement> _frameStack = new();
        public uint Count => (uint) _frameStack.Count;
        
        public StackElement()
        {
            this.AddStackStyles();
            AddToClassList("poly-stack__root");
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanelEvent);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanelEvent);
        }

        protected void AssertNotEmpty() 
        {
            if (IsEmpty)
            {
                RaiseStackEmptyException();
            }
        }
        
        private void HandleAttachToPanelEvent(AttachToPanelEvent attachToPanelEvent)
        {
            Asserts.IsNull(_registrationSet);
            _registrationSet = new RegistrationSet(this);
            StackFrameElement[] children = Children().OfType<StackFrameElement>().Reverse().ToArray();
            children.Take(children.Length - 2).ForEach(x =>
            {
                x.Hide();
            });
            _frameStack = new Stack<StackFrameElement>(children);
            if (TryPeek() is {} stackFrame)
            {
                stackFrame.Appear();
            }
            RegisterStackAction<PushFrame>(HandlePushFrameCommand);
            RegisterStackAction<PopFrame>(HandlePopFrameCommand);
        }

        private void HandleDetachFromPanelEvent(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;
            _frameStack.Clear();
        }

        protected void RegisterStackAction<TEvent>(Action<TEvent> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEvent : StackCommand<TEvent>, new()
        {
            EventCallback<TEvent> eventCallback = Iff(IsThisStack, callback).ToEventCallback();
            Asserts.IsNotNull(_registrationSet).RegisterCallback(eventCallback, useTrickleDown);
        }

        private bool IsThisStack(IStackCommand cmd) =>
            cmd.StackId == StackId;

        #region Event Handlers

        // ReSharper disable once AsyncVoidMethod
        // SwallowAndLogExceptions catches all exceptions
        private async void HandlePushFrameCommand(PushFrame cmd)
        {
            await PushAsync(cmd.Frame).SwallowAndLogExceptions();
        }

        // ReSharper disable once AsyncVoidMethod
        // SwallowAndLogExceptions catches all exceptions 
        private async void HandlePopFrameCommand(PopFrame cmd)
        {
            await TryPopAsync().SwallowAndLogExceptions();
        }

        #endregion Event Handlers
        
        #region  Public Api

        public void PushWithoutAnimate(StackFrameElement frame)
        {
            StackFrameElement? prev = TryPeek();
            prev?.Hide();
            frame.Appear();
            _frameStack.Push(frame);
            Add(frame);
        }
        
        public Task PushAsync(StackFrameElement frame)
        {
            StackFrameElement? prev = TryPeek();
            prev?.SetEnabled(false);
            Add(frame);
            _frameStack.Push(frame);
            Task animateIn = frame.AnimateIn(IsEmpty, StackId);
            return animateIn
                .SwallowAndLogExceptions()
                .ContinueWith(_ =>
                {
                    prev?.Hide();
                });
        }
        
        
        public Task<StackFrameElement> PopAsync()
        {
            AssertNotEmpty();
            // Nested function to ensure exceptions based on program invariants
            // are thrown on Task creation rather than when awaited
            // (see https://github.com/SergeyTeplyakov/ErrorProne.NET/blob/master/docs/Rules/EPC37.md for more info)
            async Task<StackFrameElement> PopFrameInner() =>
                Asserts.IsNotNull(await TryPopAsync());
            return PopFrameInner();
        }

        public Task<StackFrameElement?> TryPopAsync()
        {
            if (IsEmpty)
            {
                return Task.FromResult<StackFrameElement?>(null);
            }
            StackFrameElement current = _frameStack.Pop();
            StackFrameElement? prev = TryPeek();
            prev?.Appear(false);
            return current
                .AnimateOut()
                .SwallowAndLogExceptions()
                .ContinueWith<StackFrameElement?>(_ =>
                {
                    prev?.SetEnabled(true);
                    return current;
                });
        }
        
        
        public StackFrameElement? TryPeek() =>
            IsEmpty ? null : Peek();
        
        public StackFrameElement Peek()
        {
            AssertNotEmpty();
            return _frameStack.Peek();
        }

        public void ClearAll()
        {
            _frameStack.Clear();
            Clear();
        }
        
        public bool IsEmpty => Count is 0;
        
        #endregion Public Api
    }
}