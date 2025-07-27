#nullable enable
using System;
using JetBrains.Annotations;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Containers.Stacks
{
    
    [UxmlElement]
    public partial class StackFrameHeader : VisualElement
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
                _contents =
                    _navigateBackEnabled
                        ? MakeButton()
                        : MakeLabel();
                _contents.text = _headerText;
                Add(_contents);
            }
        }
        

        public Action OnNavigateBack { get; set; } = () => { };
        
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
        

        public StackFrameHeader()
        {
            this.AddStackStyles();
        }

        private TextElement MakeLabel()
        {
            Label label = new()
            {
                text = _headerText
            };
            label.AddToClassList("poly-stackframe-header__root");
            return label;
        }

        private TextElement MakeButton()
        {
            Button button = new(OnNavigateBack)
            {
                text = _headerText,
            };
            VisualElement backIcon = new()
            {
                pickingMode = PickingMode.Ignore
            };
            button.AddToClassList("poly-stackframe-header__root");
            backIcon.AddToClassList("unity-button");
            backIcon.AddToClassList("poly-stackframe-header__back-icon");
            button.Add(backIcon);
            return button;
        }
    }
}