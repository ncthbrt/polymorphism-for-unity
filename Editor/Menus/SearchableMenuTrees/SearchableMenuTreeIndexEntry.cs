#nullable enable
using JetBrains.Annotations;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    [PublicAPI]
    public class SearchableMenuTreeIndexEntry<T> where T: class
    {
        public string SearchTerm { get; }
        public SearchableMenuTreeEntry<T>? Item { get; }
            
        public SearchableMenuTreeIndexEntry(string searchTerm, SearchableMenuTreeEntry<T>? item = null)
        {
            SearchTerm = searchTerm;
            Item = item;
        }
    }
}