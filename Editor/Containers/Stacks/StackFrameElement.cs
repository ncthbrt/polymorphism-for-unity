#nullable enable
using System.Threading.Tasks;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Containers.Stacks
{
    [UxmlElement(nameof(StackFrameElement))]
    public sealed partial class StackFrameElement : VisualElement
    {
        
        private const string AnimatingOutClassName = "poly-stackframe__animating-out";
        private const string StableClassName = "poly-stackframe__stable";
        private const string AnimatingInClassName = "poly-stackframe__animating-in";
        public override VisualElement contentContainer { get; }
        private StackFrameHeader? _maybeHeader;

        private string _headerText = string.Empty;
        private bool _navigateBackEnabled;
        

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
                    hierarchy.Insert(0, _maybeHeader);
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

        public StackFrameElement()
        {
            this.AddStackStyles();
            AddToClassList("poly-stackframe__root");
            contentContainer = new VisualElement();
            contentContainer.name = nameof(contentContainer);
            contentContainer.AddToClassList("poly-stackframe__content");
            hierarchy.Add(contentContainer);
        }

        public Task AnimateIn(bool enableBackButton, string stackId)
        {
            visible = true;
            SetEnabled(false);
            RegistrationSet registrationSet = new(this);
            TaskCompletionSource<bool> taskCompletionSource = new();
            if (_maybeHeader is not null)
            {
                _maybeHeader.NavigateBackEnabled = enableBackButton;
                _maybeHeader.StackId = stackId;
            }

            void HandleTransitionEnd(TransitionEndEvent _)
            {
                PostAnimateIn();
                taskCompletionSource.TrySetResult(true);
            }
            
            void HandleTransitionCancel(TransitionCancelEvent _)
            {
                PostAnimateIn();
                taskCompletionSource.SetCanceled();
            }
            registrationSet.RegisterCallbackOnce<TransitionEndEvent>(HandleTransitionEnd);
            registrationSet.RegisterCallbackOnce<TransitionCancelEvent>(HandleTransitionCancel);
            AddToClassList(AnimatingInClassName);
            registrationSet.AttachedToTask(taskCompletionSource.Task);
            return taskCompletionSource.Task;
        }
        public Task AnimateOut()
        {
            using RegistrationSet registrationSet = new(this);
            TaskCompletionSource<bool> taskCompletionSource = new();
            SetEnabled(false);
            void HandleTransitionEnd(TransitionEndEvent _)
            {
                PostAnimateOut();
                taskCompletionSource.TrySetResult(true);
            }
            void HandleTransitionCancel(TransitionCancelEvent _)
            {
                PostAnimateOut();
                taskCompletionSource.SetCanceled();
            }
            registrationSet.RegisterCallbackOnce<TransitionEndEvent>(HandleTransitionEnd);
            registrationSet.RegisterCallbackOnce<TransitionCancelEvent>(HandleTransitionCancel);
            registrationSet.AttachedToTask(taskCompletionSource.Task);
            AddToClassList(AnimatingOutClassName);
            return taskCompletionSource.Task;
        }

        public void Hide()
        {
            SetEnabled(false);
            RemoveFromClassList(StableClassName);
            style.display = DisplayStyle.None;
        }

        public void Appear(bool enabled = true)
        {
            SetEnabled(enabled);
            style.display = DisplayStyle.Flex;
            AddToClassList(StableClassName);
        }

        
        private void PostAnimateIn()
        {
            SetEnabled(true);
            RemoveFromClassList(AnimatingInClassName);
            AddToClassList(StableClassName);
        }

        
        private void PostAnimateOut()
        {
            visible = false;
            SetEnabled(false);
            RemoveFromClassList(AnimatingOutClassName);
            RemoveFromClassList(StableClassName);
            RemoveFromHierarchy();
        }
        

    }
}