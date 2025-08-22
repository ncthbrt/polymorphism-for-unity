#nullable enable
using JetBrains.Annotations;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    [PublicAPI]
    public class SearchableMenuTreeIndexEntry<T>
    {
        public string SearchTerm { get; }
        public SearchableMenuTreeLeafNode<T>? Item { get; }
            
        public SearchableMenuTreeIndexEntry(string searchTerm, SearchableMenuTreeLeafNode<T>? item = null)
        {
            SearchTerm = searchTerm;
            Item = item;
        }
    }
}