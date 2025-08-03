#nullable enable
using System;
using JetBrains.Annotations;
using Polymorphism4Unity.Editor.Commands;
using Polymorphism4Unity.Editor.Manipulators;
using Polymorphism4Unity.Editor.Styling;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Containers.Stacks
{
    public class StackFrameHeader : VisualElement
    {
        private string _headerText = "Placeholder Text";
        private bool _navigateBackEnabled;

        [UxmlAttribute, UsedImplicitly]
        public bool NavigateBackEnabled
        {
            get => _navigateBackEnabled;
            set
            {
                bool change = _navigateBackEnabled != value || _contents == null;
                _navigateBackEnabled = value;
                if (!change)
                {
                    return;
                }
                _contents?.RemoveFromHierarchy();
                _contents?.RemoveManipulator(_backButtonNavigationManipulator);
                _contents =
                    _navigateBackEnabled
                        ? MakeButton()
                        : MakeLabel();
                _contents.text = _headerText;
                Add(_contents);
            }
        }
        
        
        [UxmlAttribute, UsedImplicitly]
        public string HeaderText
        {
            get => _headerText;
            set
            {
                _headerText = value;
                if (_contents is not null)
                {
                    _contents.text = value;    
                }
            }
        }
        
        private TextElement? _contents;
        private readonly BackButtonNavigationManipulator _backButtonNavigationManipulator;
        public Action<INavigationCommand, EventBase> NavigationHandler { get; set; } = (_, _) => { };
        public StackFrameHeader()
        {
            // NavigationHandler = navigationHandler;
            _backButtonNavigationManipulator = new BackButtonNavigationManipulator(HandleNavigationEvent);
        }

        private void HandleNavigationEvent(INavigationCommand navigateBackCommand, EventBase baseEvent)
        {
            Asserts.IsNotNull(NavigationHandler);
            NavigationHandler.SafelyInvoke(navigateBackCommand, baseEvent);
        }

        private static readonly IStyle HeaderRootStyle = new CompactStyle
        {
            flexGrow = 0,
            flexShrink = 0,
            flexDirection = FlexDirection.Row,
            padding = 0,
            margin = 0,
            justifyContent = Justify.SpaceBetween,
            borderWidth = 0,
            borderBottomWidth = 1,
            height = 21,
            borderBottomColor = new Color(r: 35 / 255f, g: 35 / 255f, b: 35 / 255f, a: 1f),
            borderRadius = 0,
            alignContent = Align.Center,
            width = Length.Percent(100)
        };
        
        
        private TextElement MakeLabel()
        {
            Label label = new()
            {
                text = _headerText,
            };
            label.style.ApplyStyles(HeaderRootStyle);
            return label;
        }
        

        private TextElement MakeButton()
        {
            Button button = new()
            {
                text = _headerText,
            };
            VisualElement backIcon = new()
            {
                pickingMode = PickingMode.Ignore
            };
            backIcon.style.ApplyStyles(new CompactStyle
            {
                backgroundImage = EditorGUIUtility.IconContent("Arrownavigationleft@2x").image as Texture2D,
                height = 15,
                width = 15,
                margin = 0,
                padding = 0,
                borderWidth = 0,
                alignSelf = Align.Center,
                backgroundPosition = new BackgroundPosition(BackgroundPositionKeyword.Center)
            });
            button.AddManipulator(_backButtonNavigationManipulator);
            button.style.ApplyStyles(HeaderRootStyle);
            button.Add(backIcon);
            return button;
        }
    }
}