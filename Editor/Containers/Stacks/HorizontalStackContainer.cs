#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine.UIElements;
using static Polymorphism4Unity.Editor.Utils.FuncUtils;
namespace Polymorphism4Unity.Editor.Containers
{
    public interface IStackCommand
    {
        string StackId { get; }
    }

    public abstract class StackCommand<T> : CommandEventBase<T>, IStackCommand
        where T : StackCommand<T>, new()
    {
        public abstract string StackId { get; }
    }

    public abstract class PopCommand<T> : StackCommand<T>
        where T : PopCommand<T>, new()
    {
        private static readonly string cmdName = typeof(T).Name;
        private string stackId = string.Empty;
        public override string StackId =>
            Asserts.IsNotNullOrEmpty(stackId);

        public static new T GetPooled(string stackId)
        {
            T command = GetPooled();
            command.commandName = cmdName;
            command.stackId = Asserts.IsNotNullOrEmpty(stackId);
            return command;
        }
    }

    public abstract class ResultPopCommand<TCommand> : PopCommand<TCommand>
        where TCommand : ResultPopCommand<TCommand>, new()
    {
        public object? MaybeResult { get; private set; }
        public static TCommand GetPooled(string stackId, object? result)
        {
            TCommand command = GetPooled(stackId);
            command.MaybeResult = result;
            return command;
        }
    }

    public abstract class FrameCommand<T> : StackCommand<T>
        where T : FrameCommand<T>, new()
    {
        private static readonly string cmdName = typeof(T).Name;
        private string? stackId = null;
        private string? frameName = null;
        public override string StackId =>
            Asserts.IsNotNullOrEmpty(stackId);
        private VisualElement? frame = null;
        public VisualElement Frame =>
            Asserts.IsNotNull(frame);

        public string FrameName =>
            Asserts.IsNotNullOrEmpty(frameName);

        public static T GetPooled(string stackId, string frameName, VisualElement frame)
        {
            T command = GetPooled();
            command.commandName = cmdName;
            command.stackId = Asserts.IsNotNullOrEmpty(stackId);
            command.frame = Asserts.IsNotNull(frame);
            command.frameName = Asserts.IsNotNullOrEmpty(frameName);
            return command;
        }
    }

    public class PushFrameCommand : FrameCommand<PushFrameCommand>
    {
    }

    public class PopFrameCommand : PopCommand<PopFrameCommand>
    {
    }

    public class PushGroupCommand : FrameCommand<PushGroupCommand>
    {
    }


    public class PopGroupCommand : PopCommand<PopGroupCommand>
    {
    }

    public class ReplaceFrameCommand : FrameCommand<ReplaceFrameCommand>
    {
    }

    public class ReplaceGroupCommand : FrameCommand<ReplaceGroupCommand>
    {
    }

    public class PopAllCommand : PopCommand<PopAllCommand>
    {
    }

    public class PopGroupWithResultCommand : ResultPopCommand<PopGroupWithResultCommand>
    {
    }

    public class PopAllWithResultCommand : ResultPopCommand<PopAllWithResultCommand>
    {
    }

    [UxmlElement("HorizontalStackFrame"), UsedImplicitly]
    public partial class HorizontalStackFrame : VisualElement
    {
        private const string animatingUssClassName = "animating";
        private readonly VisualElement frame;
        private readonly VisualElement content;
        private readonly VisualElement header;
        private readonly Button headerButton;
        public override VisualElement contentContainer => content ?? this;

        public HorizontalStackFrame() : this(showHeader: false, frameName: "Root")
        {
        }

        public HorizontalStackFrame(bool showHeader, string frameName)
        {
            VisualTreeAsset frameTemplate = Asserts.IsNotNull(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.ncthbrt.polymorphism-for-unity/Editor/Containers/Stacks/StackFrame.uxml"));
            StyleSheet styleSheet = Asserts.IsNotNull(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.ncthbrt.polymorphism-for-unity/Editor/Containers/Stacks/StackStyles.uss"));
            styleSheets.Add(styleSheet);
            AddToClassList("poly_horizontalstack__frame");
            frame = frameTemplate.Instantiate();
            VisualElement[] children = frame.Children().ToArray();
            frame.Clear();
            foreach (VisualElement child in children)
            {
                Add(child);
            }
            content = Asserts.IsNotNull(this.Q<VisualElement>("Content"));
            header = Asserts.IsNotNull(this.Q<VisualElement>("Header"));
            headerButton = Asserts.IsNotNull(header.Q<Button>("BackButton"));
            headerButton.text = frameName;
            if (!showHeader)
            {
                header.style.display = DisplayStyle.None;
            }
            Init();
        }

        private void Init()
        {
            RegisterCallback<AttachToPanelEvent>(evt =>
            {
                headerButton.RegisterCallback<ClickEvent>(PopFrame);
            });
            RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                UnregisterCallback<ClickEvent>(PopFrame);
            });
        }

        private void PopFrame(ClickEvent evt)
        {
            SendEvent(PopFrameCommand.GetPooled());
            evt.StopPropagation();
        }
    }

    [UxmlElement("HorizontalStackFrameGroup")]
    public partial class HorizontalStackFrameGroup : VisualElement
    {
        public override VisualElement? contentContainer => TryPeek(out HorizontalStackFrame? frame) ? frame : this;

        public HorizontalStackFrameGroup() : this(null)
        {
        }

        public HorizontalStackFrameGroup(HorizontalStackFrame? initial)
        {
            StyleSheet styleSheet = Asserts.IsNotNull(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.ncthbrt.polymorphism-for-unity/Editor/Containers/Stacks/StackStyles.uss"));
            styleSheets.Add(styleSheet);
            AddToClassList("poly_horizontalstack__group");
            if (initial == null)
            {
                return;
            }
            Push(initial);
        }

        public HorizontalStackFrame Peek()
        {
            return Asserts.IsNotNull((HorizontalStackFrame)hierarchy.ElementAt(hierarchy.childCount - 1));
        }

        public bool TryPeek(out HorizontalStackFrame? horizontalStackFrame)
        {
            if (hierarchy.childCount == 0)
            {
                horizontalStackFrame = null;
                return false;
            }
            horizontalStackFrame = Asserts.IsNotNull((HorizontalStackFrame)hierarchy.ElementAt(hierarchy.childCount - 1));
            return true;
        }

        public HorizontalStackFrame Pop()
        {
            HorizontalStackFrame element = Peek();
            hierarchy.RemoveAt(hierarchy.childCount - 1);
            return element;
        }

        public bool TryPop(out HorizontalStackFrame? horizontalStackFrame)
        {
            if (!TryPeek(out horizontalStackFrame))
            {
                return false;
            }
            hierarchy.RemoveAt(hierarchy.childCount - 1);
            return true;
        }

        public void Push(HorizontalStackFrame horizontalStackFrame)
        {
            Asserts.IsNotNull(horizontalStackFrame);
            hierarchy.Add(horizontalStackFrame);
        }
    }

    public class HorizontalStackContainer<TValueType> : BindableElement, INotifyValueChanged<TValueType?>
    {
        private readonly VisualElement container;
        private readonly VisualElement contentMover;
        private readonly Stack<HorizontalStackFrameGroup> stack = new();
        public override VisualElement contentContainer => stack.TryPeek(out HorizontalStackFrameGroup result) ? result : this;
        private readonly List<Action> deregistrations = new();
        public string StackId { get; }
        private TValueType? currentValue = default;
        public TValueType? value
        {
            get => currentValue;
            set
            {
                TValueType? prevValue = currentValue;
                SetValueWithoutNotify(value);
                ChangeEvent<TValueType?> changeEvent = ChangeEvent<TValueType?>.GetPooled(prevValue, currentValue);
                SendEvent(changeEvent);
            }
        }

        public HorizontalStackContainer(string stackId)
        {
            StackId = stackId;
            StyleSheet styleSheet = Asserts.IsNotNull(AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.ncthbrt.polymorphism-for-unity/Editor/Containers/Stacks/StackStyles.uss"));
            styleSheets.Add(styleSheet);
            AddToClassList("poly_horizontalstack__root");
            VisualTreeAsset containerTemplate = Asserts.IsNotNull(AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.ncthbrt.polymorphism-for-unity/Editor/Containers/Stacks/StackContainer.uxml"));
            container = containerTemplate.Instantiate();
            VisualElement[] children = container.Children().ToArray();
            container.Clear();
            foreach (VisualElement child in children)
            {
                hierarchy.Add(child);
            }
            contentMover = Asserts.IsNotNull(this.Q<VisualElement>("ContentMover"));
            HorizontalStackFrame prevFrame = new();
            HorizontalStackFrameGroup initialFrameGroup = new(prevFrame);
            contentMover.Add(initialFrameGroup);
            stack.Push(initialFrameGroup);
            Init();
        }


        private void Init()
        {
            RegisterCallback<AttachToPanelEvent>(evt =>
            {
                RegisterStackAction<PushFrameCommand>(PushFrame);
                RegisterStackAction<PushGroupCommand>(PushGroup);
                RegisterStackAction<PopFrameCommand>(PopFrame);
                RegisterStackAction<PopGroupCommand>(PopGroup);
                RegisterStackAction<PopAllCommand>(PopAll);
                RegisterStackAction<PopGroupWithResultCommand>(PopGroupWithResult);
                RegisterStackAction<PopAllWithResultCommand>(PopAllWithResult);
                RegisterStackAction<ReplaceFrameCommand>(ReplaceFrame);
                RegisterStackAction<ReplaceGroupCommand>(ReplaceGroup);
                RegisterTrackedCallback<TransitionEndEvent>(HandleTransitionEnd);
                RegisterTrackedCallback<TransitionStartEvent>(HandleTransitionStart);
                RegisterTrackedCallback<TransitionCancelEvent>(HandleTransitionCancel);
            });
            RegisterCallback<DetachFromPanelEvent>(DeregisterAll);
        }

        private void DeregisterAll(DetachFromPanelEvent _)
        {
            deregistrations.InvokeAll();
            deregistrations.Clear();
        }

        private void HandleTransitionCancel(TransitionCancelEvent transitionCancelEvent)
        {

        }


        private void HandleTransitionStart(TransitionStartEvent transitionStartEvent)
        {

        }
        private void HandleTransitionEnd(TransitionEndEvent transitionEndEvent)
        {

        }

        private void RegisterTrackedCallback<T>(EventCallback<T> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where T : EventBase<T>, new()
        {
            deregistrations.Add(() => UnregisterCallback(callback));
            RegisterCallback(callback, useTrickleDown);
        }


        private void RegisterTrackedAction<T>(Action<T> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where T : EventBase<T>, new()
        {
            EventCallback<T> eventCallback = callback.ToEventCallback();
            RegisterTrackedCallback(eventCallback, useTrickleDown);
        }

        private void RegisterStackAction<T>(Action<T> callback, TrickleDown useTrickleDown = TrickleDown.NoTrickleDown) where T : StackCommand<T>, new()
        {
            EventCallback<T> eventCallback = Iff(IsThisStack, callback).ToEventCallback();
            deregistrations.Add(() => UnregisterCallback(eventCallback));
            RegisterCallback(eventCallback, useTrickleDown);
        }

        private bool IsThisStack(IStackCommand cmd) =>
            cmd.StackId == StackId;


        private void PushFrame(PushFrameCommand pushFrameCommand)
        {

        }

        private void PopFrame(PopFrameCommand pushFrameCommand)
        {

        }

        private void PopGroup(PopGroupCommand popGroupCommand)
        {

        }

        private void PushGroup(PushGroupCommand pushGroupCommand)
        {

        }

        private void PopGroupWithResult(PopGroupWithResultCommand popGroupWithResultCommand)
        {

        }

        private void PopAllWithResult(PopAllWithResultCommand popAllWithResultCommand)
        {

        }

        private void PopAll(PopAllCommand popAllCommand)
        {

        }

        private void ReplaceGroup(ReplaceGroupCommand replaceGroupCommand)
        {

        }

        private void ReplaceFrame(ReplaceFrameCommand replaceFrameCommand)
        {

        }

        public void SetValueWithoutNotify(TValueType? newValue)
        {
            currentValue = newValue;
        }
    }
}