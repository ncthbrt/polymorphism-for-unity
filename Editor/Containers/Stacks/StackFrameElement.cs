#nullable enable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Polymorphism4Unity.Editor.Styling;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEngine;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Containers.Stacks
{
    [UxmlElement(nameof(StackFrameElement))]
    public partial class StackFrameElement : VisualElement
    {
        public sealed override VisualElement contentContainer { get; }
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
                    _maybeHeader ??= new StackFrameHeader();
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
        
        public Action OnNavigateBack { get; set; } = () => { };
        
        public StackFrameElement()
        {
            style.ApplyStyles(new CompactStyle
            {
                flexGrow = 1,
                flexShrink = 0,
                width = Length.Percent(100),
                height = Length.Percent(100),
                flexDirection = FlexDirection.Column,
            });
            ApplyInitial();
            contentContainer = new VisualElement();
            contentContainer.name = nameof(contentContainer);
            contentContainer.style.ApplyStyles(new CompactStyle
            {
                flexShrink = 0,
                flexGrow = 0,
                width = Length.Percent(100),
                height = Length.Percent(100)
            });
            hierarchy.Add(contentContainer);
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanel);
        }

        private void HandleAttachToPanel(AttachToPanelEvent attachToPanelEvent)
        {
            
        }
        
        private void HandleDetachFromPanel(DetachFromPanelEvent detachFromPanelEvent)
        {
        }
        

        public Task AnimateIn(bool enableBackButton)
        {
            visible = true;
            SetEnabled(false);
            RegistrationSet registrationSet = new(this);
            TaskCompletionSource<bool> taskCompletionSource = new();
            if (_maybeHeader is not null)
            {
                _maybeHeader.NavigateBackEnabled = enableBackButton;
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
            ApplyAnimatingIn();
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
            ApplyAnimatingOut();
            return taskCompletionSource.Task;
        }

        public void Hide()
        {
            SetEnabled(false);
            ApplyInitial();
            style.display = DisplayStyle.None;
        }

        public void Appear(bool enabled = true)
        {
            SetEnabled(enabled);
            style.display = DisplayStyle.Flex;
            ApplyStable();
        }

        
        private void PostAnimateIn()
        {
            SetEnabled(true);
            ApplyStable();
        }

        
        private void PostAnimateOut()
        {
            visible = false;
            SetEnabled(false);
            ApplyInitial();
            RemoveFromHierarchy();
        }

        private void ApplyInitial()
        {
            style.ApplyStyles(new VerboseStyle
            {
                translate = new Translate(Length.Percent(110), Length.Percent(0)),
                transitionProperty = new StyleList<StylePropertyName>()
            });
        }

        private void ApplyStable()
        {
            style.ApplyStyles(
                new VerboseStyle
                {
                    transitionProperty = new StyleList<StylePropertyName>()
                }, 
                new CompactStyle
                {
                    translate = new Translate(0f, 0f),
                    transitionDuration = 0
                }
            );
        }

        private void ApplyAnimatingOut()
        {
            style.ApplyStyles(new CompactStyle
            {
                transitionProperty = nameof(IStyle.translate),
                transitionDuration = 0.3f,
                transitionTimingFunction = EasingMode.EaseInOutSine,
                translate = new Translate(Length.Percent(110), 0)
            });
        }
        
        private void ApplyAnimatingIn()
        {
            style.ApplyStyles(new CompactStyle
            {
                transitionProperty = nameof(IStyle.translate),
                transitionDuration = 0.3f,
                transitionTimingFunction = EasingMode.EaseInOutSine,
                translate = new Translate(0, 0)
            });
        }
        

    }
}