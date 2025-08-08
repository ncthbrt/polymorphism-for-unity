#nullable enable
using Polymorphism4Unity.Editor.Styling;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using TextElement = UnityEngine.UIElements.TextElement;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    public class SearchableMenuTreeElement<T>: TextElement
    {
        private readonly VisualElement _nextIcon;
        private SearchableMenuTreeNode<T>? _node;

        public SearchableMenuTreeNode<T>? Node
        {
            get => _node;
            set
            {
                _node = value;
                base.text = _node?.Key ?? string.Empty;
                _nextIcon.style.display = 
                    _node?.HasNext ?? false 
                        ? DisplayStyle.Flex 
                        : DisplayStyle.None;
            }
        }

        public SearchableMenuTreeElement()
        {
            _nextIcon = new VisualElement
            {
                name = "NextIcon"
            };
            _nextIcon.style.ApplyStyles(new CompactStyle
            {
                backgroundImage = EditorGUIUtility.IconContent("Arrownavigationright@2x").image as Texture2D,
                height = 15,
                width = 15,
                margin = 0,
                padding = 0,
                borderWidth = 0,
                alignSelf = Align.Center,
                backgroundPosition = new BackgroundPosition(BackgroundPositionKeyword.Center),
                display = DisplayStyle.None
            });
            Add(_nextIcon);
            style.ApplyStyles(new CompactStyle
            {
                justifyContent = Justify.SpaceBetween
            });
        }
        
        
    }
}