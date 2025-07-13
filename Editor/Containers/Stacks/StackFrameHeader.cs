#nullable enable
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Containers.Stacks
{
    public class StackFrameHeader : VisualElement
    {
        private readonly string _stackId;
        private readonly string _frameName;
        private bool _enableBackButton;

        public bool EnableBackButton
        {
            set
            {
                bool change = _enableBackButton != value;
                _enableBackButton = value;
                if (!change)
                {
                    return;
                }
                _contents?.RemoveFromHierarchy();
                _contents =
                    _enableBackButton
                        ? MakeButton()
                        : MakeLabel();
                _contents.text = _frameName;
                Add(_contents);
            }
        }

        private TextElement? _contents;


        public StackFrameHeader(string stackId, string frameName)
        {
            _stackId = stackId;
            _frameName = frameName;
            this.AddStackStyles();
        }

        private TextElement MakeLabel()
        {
            Label label = new()
            {
                text = _frameName
            };
            label.AddToClassList("poly-stackframe-header__root");
            return label;
        }

        private TextElement MakeButton()
        {
            Button button = new(HandleClicked);
            button.text = _frameName;
            VisualElement backIcon = new VisualElement()
            {
                pickingMode = PickingMode.Ignore
            };
            backIcon.AddToClassList("unity-button");
            backIcon.AddToClassList("poly-stackframe-header__back-icon");
            button.Add(backIcon);
            return button;
        }

        private void HandleClicked() =>
            SendEvent(PopFrame.GetPooled(_stackId));
    }
}