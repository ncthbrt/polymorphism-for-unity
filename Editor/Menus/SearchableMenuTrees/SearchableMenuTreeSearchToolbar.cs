#nullable enable
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEditor.UIElements;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    [UsedImplicitly]
    public class SearchableMenuTreeSearchToolbar<T>: Toolbar
        where T: class
    {
        protected readonly ToolbarSearchField SearchField;

        public List<SearchableMenuTreeIndexEntry<T>> Index { get; set; } = new();
        
        public SearchableMenuTreeSearchToolbar()
        {
            SearchField = new ToolbarSearchField();
            Add(SearchField);
        }

        public void Focus(char? initialCharacter)
        {
            base.Focus();
            SearchField.value += initialCharacter ?? '\0';
        }

        public void ResetSearch()
        {
            SearchField.value = "";
        }
    }
}