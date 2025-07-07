#nullable enable
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Polymorphism4Unity.Safety;
using UnityEditor;
using UnityEngine.UIElements;

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

    public abstract class ResultPopCommand<TCommand, TResult> : PopCommand<TCommand>
        where TCommand : ResultPopCommand<TCommand, TResult>, new()
    {
        public TResult? MaybeResult { get; private set; }
        public static TCommand GetPooled(string stackId, TResult? result)
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

    public class PopGroupWithResultCommand<TResult> : ResultPopCommand<PopGroupWithResultCommand<TResult>, TResult>
    {
    }

    public class PopAllWithResultCommand<TResult> : ResultPopCommand<PopGroupWithResultCommand<TResult>, TResult>
    {
    }

    [UxmlElement("HorizontalStackFrame"), UsedImplicitly]
    public partial class HorizontalStackFrame : VisualElement
    {
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
            RegisterCallbackOnce<GeometryChangedEvent>(evt =>
            {
                headerButton.RegisterCallback<ClickEvent>(evt =>
                {
                    SendEvent(PopFrameCommand.GetPooled());
                });
            });
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



    [UxmlElement("HorizontalStack"), UsedImplicitly]
    public partial class HorizontalStackContainer : VisualElement
    {
        private readonly VisualElement container;
        private readonly VisualElement contentMover;
        private readonly Stack<HorizontalStackFrameGroup> stack = new();
        public override VisualElement contentContainer => stack.TryPeek(out HorizontalStackFrameGroup result) ? result : this;

        [UxmlAttribute]
        public string StackId { get; set; }

        public HorizontalStackContainer()
        {
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
            RegisterCallbackOnce<GeometryChangedEvent>(evt =>
            {
                RegisterCallback<IStackCommand>(cmd =>
                {
                    if (cmd.StackId != StackId)
                    {
                        return;
                    }                
                });
            });
        }
    }
}