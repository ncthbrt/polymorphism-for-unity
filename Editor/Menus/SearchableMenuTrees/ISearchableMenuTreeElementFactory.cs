using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    public interface ISearchableMenuTreeElementFactory<T>
    {
        VisualElement CreateItem();
        
        
    }
}