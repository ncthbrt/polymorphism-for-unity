#nullable enable
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    public class SearchableMenuTreeElement<T>: VisualElement
    {
        public SearchableMenuTreeNode<T>? Node { get; set; } = null;

        public SearchableMenuTreeElement()
        {
        }
    }
}