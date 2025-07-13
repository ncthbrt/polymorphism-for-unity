#nullable enable
using System.Threading.Tasks;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Containers.Stacks
{
    [UxmlElement(nameof(StackFrame))]
    public sealed partial class StackFrame : VisualElement
    {
        private const string StableClassName = "poly-stackframe__stable";
        public override VisualElement contentContainer { get; }

        private StackFrameHeader? _maybeHeader;

        private string _headerText = string.Empty;
        private bool _navigateBackEnabled = false;
        

        [UxmlAttribute]
        public string HeaderText
        {
            get => _headerText;
            set
            {
                _headerText = value;
                if (_headerText is { Length: > 0 } notEmpty)
                {
                    if (_maybeHeader is null)
                    {
                        _maybeHeader = new StackFrameHeader();
                    }
                    _maybeHeader.HeaderText = notEmpty;
                    hierarchy.Add(_maybeHeader);
                }
                else
                {
                    _maybeHeader?.RemoveFromHierarchy();
                    _maybeHeader = null;
                }
            }
        }

        [UxmlAttribute]
        public bool NavigateBackEnabled
        {
            get => _navigateBackEnabled;
            set
            {
                _navigateBackEnabled = value;
                if (_maybeHeader is not null)
                {
                    _maybeHeader.NavigateBackEnabled = _navigateBackEnabled;
                }
            }
        }

        public StackFrameHeader Header
        {
            get => Asserts.IsNotNull(_maybeHeader);
            set
            {
                _maybeHeader?.RemoveFromHierarchy();
                _maybeHeader = value;
                if (_maybeHeader is not null)
                {
                    hierarchy.Insert(0, _maybeHeader);
                }
            }
        }

        public StackFrame()
        {
            this.AddStackStyles();
            AddToClassList("poly-stackframe__root");
            contentContainer = new VisualElement();
            contentContainer.name = nameof(contentContainer);
            contentContainer.AddToClassList("poly-stackframe__content");
        }

        public Task<bool> AnimateIn(bool enableBackButton, string stackId)
        {
            visible = true;
            const string animatingInClassName = "poly-stackframe__animating-in";
            using RegistrationSet registrationSet = new(this);
            TaskCompletionSource<bool> taskCompletionSource = new();
            if (_maybeHeader is not null)
            {
                _maybeHeader.NavigateBackEnabled = enableBackButton;
                _maybeHeader.StackId = stackId;
            }

            void HandleTransitionEnd(TransitionEndEvent _)
            {
                taskCompletionSource.TrySetResult(true);
                SetEnabled(true);
                RemoveFromClassList(animatingInClassName);
                AddToClassList(StableClassName);
            }

            registrationSet.RegisterCallbackOnce<TransitionEndEvent>(HandleTransitionEnd);
            AddToClassList(animatingInClassName);
            return taskCompletionSource.Task;
        }

        public Task<bool> AnimateOut()
        {
            const string animatingOutClassName = "poly-stackframe__animating-out";
            using RegistrationSet registrationSet = new(this);
            TaskCompletionSource<bool> taskCompletionSource = new();
            SetEnabled(false);
            void HandleTransitionEnd(TransitionEndEvent _)
            {
                visible = false;
                RemoveFromHierarchy();
                RemoveFromClassList(animatingOutClassName);
                RemoveFromClassList(StableClassName);
                taskCompletionSource.TrySetResult(true);
            }
            registrationSet.RegisterCallbackOnce<TransitionEndEvent>(HandleTransitionEnd);
            AddToClassList(animatingOutClassName);
            return taskCompletionSource.Task;
        }
    }
}