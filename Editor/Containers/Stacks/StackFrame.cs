#nullable enable
using System.Threading.Tasks;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEditor;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Containers.Stacks
{
    [UxmlElement(nameof(StackFrame))]
    public sealed partial class StackFrame : VisualElement
    {
        public override VisualElement contentContainer { get; }

        private StackFrameHeader? _maybeHeader;
        public StackFrameHeader? Header
        {
            get => _maybeHeader;
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
        
        public string? StackId { get; set; }
        
        public StackFrame()
        {
            this.AddStackStyles();
            AddToClassList("poly-stackframe__root");
            contentContainer = new VisualElement();
            contentContainer.name = nameof(contentContainer);
            contentContainer.AddToClassList("poly-stackframe__content");
        }

        public Task AnimateIn()
        {
            
        }
        
        public Task AnimateOut()
        {
            
        }
    }
}