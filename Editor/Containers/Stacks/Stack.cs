#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEngine;
using UnityEngine.UIElements;
using static Polymorphism4Unity.Editor.Utils.FuncUtils;

namespace Polymorphism4Unity.Editor.Containers.Stacks
{
    [UxmlElement(nameof(Stack)), PublicAPI]
    public partial class Stack : VisualElement
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
        private Stack<StackFrame> _frameStack = new();
        public uint Count => (uint) _frameStack.Count;
        
        public Stack()
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
            StackFrame[] children = Children().OfType<StackFrame>().Reverse().ToArray();
            children.Take(children.Length - 2).ForEach(x =>
            {
                x.Hide();
            });
            _frameStack = new Stack<StackFrame>(children);
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

        public void PushWithoutAnimate(StackFrame frame)
        {
            StackFrame? prev = TryPeek();
            prev?.Hide();
            frame.Appear();
            _frameStack.Push(frame);
            Add(frame);
        }
        
        public Task PushAsync(StackFrame frame)
        {
            StackFrame? prev = TryPeek();
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
        
        
        public Task<StackFrame> PopAsync()
        {
            AssertNotEmpty();
            // Nested function to ensure exceptions based on program invariants
            // are thrown on Task creation rather than when awaited
            // (see https://github.com/SergeyTeplyakov/ErrorProne.NET/blob/master/docs/Rules/EPC37.md for more info)
            async Task<StackFrame> PopFrameInner() =>
                Asserts.IsNotNull(await TryPopAsync());
            return PopFrameInner();
        }

        public Task<StackFrame?> TryPopAsync()
        {
            if (IsEmpty)
            {
                return Task.FromResult<StackFrame?>(null);
            }
            StackFrame current = _frameStack.Pop();
            StackFrame? prev = TryPeek();
            prev?.Appear(false);
            return current
                .AnimateOut()
                .SwallowAndLogExceptions()
                .ContinueWith<StackFrame?>(_ =>
                {
                    prev?.SetEnabled(true);
                    return current;
                });
        }
        
        
        public StackFrame? TryPeek() =>
            IsEmpty ? null : Peek();
        
        public StackFrame Peek()
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