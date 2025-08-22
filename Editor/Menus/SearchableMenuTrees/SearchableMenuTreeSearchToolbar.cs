#nullable enable
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using Raffinert.FuzzySharp.Extractor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using FuzzSearch = Raffinert.FuzzySharp.Process;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    [UsedImplicitly]
    public class SearchableMenuTreeSearchToolbar<T>: Toolbar, INotifyValueChanged<List<SearchableMenuTreeNode<T>>?>
    {
        protected readonly ToolbarSearchField SearchField;
        private RegistrationSet? _searchFieldRegistrationSet;

        public List<SearchableMenuTreeIndexEntry<T>> Index { get; set; } = new();
        private List<SearchableMenuTreeNode<T>>? _searchResults; 
        
        
        public SearchableMenuTreeSearchToolbar()
        {
            SearchField = new ToolbarSearchField();
            Add(SearchField);
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanel);
        }

        public void Focus(char? initialCharacter)
        {
            base.Focus();
            SearchField.value += initialCharacter ?? '\0';
        }

        public void ResetSearch()
        {
            SearchField.value = string.Empty;
        }
        
        private void HandleAttachToPanel(AttachToPanelEvent _)
        {
            Asserts.IsNull(_searchFieldRegistrationSet);
            _searchFieldRegistrationSet = new RegistrationSet(SearchField);
            _searchFieldRegistrationSet.RegisterCallback<ChangeEvent<string>>(HandleSearchChanged);
        }

        private void HandleSearchChanged(ChangeEvent<string> changeEvent)
        {
            if (string.IsNullOrEmpty(changeEvent.newValue))
            {
                value = null;
            }
            else
            {
                value = SearchIndex(Index, changeEvent.newValue);
            }
        }
        
        private static List<SearchableMenuTreeNode<T>> SearchIndex(List<SearchableMenuTreeIndexEntry<T>> index, string searchTerm)
        {
            SearchableMenuTreeIndexEntry<T> searchIndexEntry = new(searchTerm);
            IEnumerable<ExtractedResult<SearchableMenuTreeIndexEntry<T>>> searchResults = FuzzSearch.ExtractTop(
                searchIndexEntry, 
                index, 
                item => item.SearchTerm,
                limit: int.MaxValue,
                cutoff: 75 // This is a little arbitrary tbqh
            );
            return searchResults.Select(x=> x.Value.Item).Cast<SearchableMenuTreeNode<T>>().ToList();
        }

        private void HandleDetachFromPanel(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_searchFieldRegistrationSet).Dispose();
            _searchFieldRegistrationSet = null;
        }

        public void SetValueWithoutNotify(List<SearchableMenuTreeNode<T>>? newValue)
        {
            _searchResults = newValue?.ToList();
        }
        
        public List<SearchableMenuTreeNode<T>>? value
        {
            get => _searchResults;
            set
            {
                List<SearchableMenuTreeNode<T>>? prev = _searchResults;
                SetValueWithoutNotify(value);
                ChangeEvent<List<SearchableMenuTreeNode<T>>?> changeEvent = ChangeEvent<List<SearchableMenuTreeNode<T>>?>.GetPooled(prev, _searchResults?.ToList());
                SendEvent(changeEvent);
            }
        }
    }
}