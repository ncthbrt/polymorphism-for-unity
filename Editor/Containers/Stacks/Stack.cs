#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEditor;
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
        private readonly List<Action> _deregistrations = new();
        private Task _maybeCurrentOperation = Task.CompletedTask;
        private uint _count;
        public uint Count => _count;

        public Stack()
        {
            _registrationSet = new RegistrationSet(this);
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
            RegisterStackAction<PushFrame>(HandlePushFrameCommand);
            RegisterStackAction<PopFrame>(HandlePopFrameCommand);
        }

        private void HandleDetachFromPanelEvent(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;
        }

        protected void RegisterStackAction<TEvent>(Action<TEvent> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where TEvent : StackCommand<TEvent>, new()
        {
            EventCallback<TEvent> eventCallback = Iff(IsThisStack, callback).ToEventCallback();
            Asserts.IsNotNull(_registrationSet).RegisterCallback(eventCallback, useTrickleDown);
        }

        protected bool IsThisStack(IStackCommand cmd) =>
            cmd.StackId == StackId;

        #region Event Handlers
        private void HandlePushFrameCommand(PushFrame cmd)
        {
            _ = Push(cmd.Frame);
        }

        private void HandlePopFrameCommand(PopFrame cmd)
        {
            _ = TryPop();
        }

        #endregion Event Handlers
        
        #region  Public Api
        public Task Push(StackFrame frame)
        {
            Task prevOperation = CurrentOperation;
            frame.StackId = StackId;
            async Task PushFrameInner()
            {
                await prevOperation;
                await PushFrameImpl(frame);
            }
            Task operation = PushFrameInner();
            CurrentOperation = operation.SwallowCancellations();
            return operation;
        }
        
        private Task PushFrameImpl(StackFrame frame)
        {
            frame.StackId = StackId;
            using RegistrationSet registrationSet = new(frame);
            TaskCompletionSource<bool> taskCompletionSource = new();
            if (frame is { Header: not null } stackFrame)
            {
                stackFrame.Header.EnableBackButton = !IsEmpty;
            }
            
            void HandleTransitionEnd(TransitionEndEvent endEvent)
            {
                taskCompletionSource.TrySetResult(true);
            }
            
            void HandleTransitionCancel(TransitionCancelEvent cancelEvent)
            {
                taskCompletionSource.TrySetCanceled();
            }
            registrationSet.RegisterCallback<TransitionEndEvent>(HandleTransitionEnd);
            registrationSet.RegisterCallback<TransitionCancelEvent>(HandleTransitionCancel);
            frame.AddToClassList("animating");
            Add(frame);
            ++_count;
            return taskCompletionSource.Task;
        }


        private Task<StackFrame?> TryPopFrame()
        {
            if (IsEmpty)
            {
                return Task.FromResult<StackFrame?>(null);
            }
            Task prevOperation = CurrentOperation;
            async Task<StackFrame?> PopFrameInner()
            {
                AssertNotEmpty();
                return await PopFrameImpl();
            }
            Task<StackFrame?> newOperation = PopFrameInner();
            CurrentOperation =  Task.WhenAll(prevOperation, newOperation.SwallowCancellations());
            return newOperation;
        }
        
        
        public Task<StackFrame> Pop()
        {
            AssertNotEmpty();
            // Nested function to ensure exceptions based on program invariants
            // are thrown on Task creation rather than when awaited
            // (see https://github.com/SergeyTeplyakov/ErrorProne.NET/blob/master/docs/Rules/EPC37.md for more info)
            async Task<StackFrame> PopFrameInner() =>
                Asserts.IsNotNull(await TryPop());
            return PopFrameInner();
        }

        public Task<StackFrame?> TryPop()
        {
            if (IsEmpty)
            {
                return CurrentOperation.ContinueWith<StackFrame?>(_=> null);
            }
            Task prevOperation = CurrentOperation;
            async Task<StackFrame?> TryPopFrameInner()
            {
                await prevOperation;
                return await TryPopFrame();
            }
            Task<StackFrame?> operation = TryPopFrameInner();
            CurrentOperation = operation.SwallowCancellations();
            return operation;
        }
        

        public StackFrame? TryPeek() =>
            IsEmpty ? null : Peek();
        
        public StackFrame Peek()
        {
            AssertNotEmpty();
            return Asserts.IsType<StackFrame>(contentContainer[(int)(Count - 1u)]);
        }

        public bool IsEmpty => Count is 0;
        public Task CurrentOperation { get; private set; } = Task.CompletedTask;
        #endregion Public Api
    }
}