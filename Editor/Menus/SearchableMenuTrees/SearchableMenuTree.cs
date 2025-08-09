#nullable enable
// ReSharper disable AsyncVoidMethod
// We're handling all possible exceptions thrown by tasks, Resharper is just not smart enough to know it
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Polymorphism4Unity.Editor.Commands;
using Polymorphism4Unity.Editor.Containers.Stacks;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using Raffinert.FuzzySharp.Extractor;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using FuzzSearch = Raffinert.FuzzySharp.Process;
namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    [PublicAPI]
    public abstract class SearchableMenuTree<T, TSearchToolbar, TElement>: StackView
        where T: class
        where TSearchToolbar: SearchableMenuTreeSearchToolbar<T>, new()
        where TElement: SearchableMenuTreeElement<T>, new()
    {
        private RegistrationSet? _registrationSet;
        private SearchableMenuTreeFrame<T, TSearchToolbar, TElement>? _initialStackFrame;
        private SearchableMenuTreeIndexEntry<T>[]? _searchIndex;
        protected abstract IEnumerable<SearchableMenuTreeEntry<T>> Items { get; }

        public Action<T?> OnSelected { get; set; } = _ => { };

        private string _headerText = "";
        
        [UxmlAttribute]
        public string HeaderText
        {
            get => _headerText;
            set
            {
                _headerText = value;
                if (_initialStackFrame != null)
                {
                    _initialStackFrame.HeaderText = _headerText;
                }
            }
        }

        protected SearchableMenuTree()
        {
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanelEvent);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanelEvent);
        }

        protected virtual void HandleAttachToPanelEvent(AttachToPanelEvent _)
        {
            Asserts.IsNull(_registrationSet);
            _registrationSet = new RegistrationSet(this);
            _registrationSet.RegisterCallback<NavigateBackCommand>(HandleNavigateBackCommand);
            _registrationSet.RegisterCallback<NavigateSubmitCommand>(HandleNavigateSubmitCommand);
            RefreshItems();
        }

        protected virtual void HandleDetachFromPanelEvent(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;
            ClearAll();
        }
        
        private async void HandleNavigateBackCommand(NavigateBackCommand command)
        {
            if (IsEmpty)
            {
                OnSelected.SafelyInvoke(null);
            }
            else
            {
                await TryPopAsync().SwallowAndLogExceptions();
            }
        }
     
        
        private async void HandleNavigateSubmitCommand(NavigateSubmitCommand command)
        {
            IEventHandler commandTarget = command.target;
            SearchableMenuTreeElement<T> treeElement = Asserts.IsType<SearchableMenuTreeElement<T>>(commandTarget);
            if (treeElement.Node is SearchableMenuTreeLeafNode<T> leaf)
            {
                OnSelected.SafelyInvoke(leaf.Value);
            } 
            else if(treeElement.Node is {} node)
            {
                SearchMenuTreeParentNode<T> parentNode = Asserts.IsType<SearchMenuTreeParentNode<T>>(node);
                SearchableMenuTreeFrame<T, TSearchToolbar, TElement> stackFrame = new(parentNode.ChildNodes)
                {
                    HeaderText = parentNode.Key
                };
                await PushAsync(stackFrame).SwallowAndLogExceptions();
            }
        }

        private static SearchableMenuTreeIndexEntry<T>[] ConstructIndex(IEnumerable<SearchableMenuTreeEntry<T>> items)
        {
            return items.SelectMany(item => 
            {
                return item.SearchTerms.Select(term =>  new SearchableMenuTreeIndexEntry<T>(term, item));
            }).ToArray();
        }

        private static ExtractedResult<SearchableMenuTreeIndexEntry<T>>[] SearchIndex(SearchableMenuTreeIndexEntry<T>[] index, string searchTerm)
        {
            SearchableMenuTreeIndexEntry<T> searchIndexEntry = new(searchTerm);
            IEnumerable<ExtractedResult<SearchableMenuTreeIndexEntry<T>>> searchResults = FuzzSearch.ExtractTop(
                searchIndexEntry, 
                index, 
                item => item.SearchTerm,
                limit: int.MaxValue,
                cutoff: 75 // This is a little arbitrary tbqh
            );
            return searchResults.ToArray();
        }

        protected void RefreshItems()
        {
            SearchableMenuTreeEntry<T>[] items = Items.ToArray();
            List<SearchableMenuTreeNode<T>> treeRoots = ConstructTree(items);
            _searchIndex = ConstructIndex(items);
            ClearAll();
            _initialStackFrame = new SearchableMenuTreeFrame<T, TSearchToolbar, TElement>(treeRoots)
            {
                HeaderText = _headerText
            };
            PushWithoutAnimate(_initialStackFrame);
        }

        private static void CreateShortcuts(SearchableMenuTreeNode<T>[] nodes)
        {
            // TODO: this is a little complex and will require changes to the Stack element
            // Lets get things working solidly first 
        }
        
        private static List<SearchableMenuTreeNode<T>> ConstructTree(IEnumerable<SearchableMenuTreeEntry<T>> items)
        {
            SearchMenuTreeParentNode<T> intermediate = new(string.Empty, new List<SearchableMenuTreeNode<T>>());
            foreach (SearchableMenuTreeEntry<T> item in items)
            {
                Match match = SearchableMenuTreesConstants.PathRegex.Match(item.Path);
                if (!match.Success)
                {
                    Debug.LogError($"Invalid Path String {item.Path}");
                    continue;
                }
                Group matchGroup = match.Groups["part"];
                string[] parts = matchGroup.Captures.Select(x => x.Value).ToArray();
                parts = Asserts.IsNotNullOrEmpty(parts);
                SearchMenuTreeParentNode<T> current = intermediate;
                for (int i = 0; i < parts[i].Length - 1; ++i)
                {
                    string key = parts[i];
                    
                    SearchableMenuTreeNode<T>? maybeElement = 
                        current.ChildNodes
                            .FirstOrDefault(x => x.Key == key);
                    
                    if (maybeElement is null or not SearchMenuTreeParentNode<T>)
                    {
                        SearchMenuTreeParentNode<T> parent = new(key, new List<SearchableMenuTreeNode<T>>());
                        current.ChildNodes.Insert(0, parent);
                        current = parent;
                    }
                    else if (maybeElement is SearchMenuTreeParentNode<T> parent)
                    {
                        current = parent;
                    }
                }
                string leafName = parts[^1];
                SearchableMenuTreeLeafNode<T> leafNode = new(leafName, item.Value, item.HasNext);
                current.ChildNodes.Add(leafNode);
            }
            SearchableMenuTreeNode<T>[] results = intermediate.ChildNodes.ToArray();
            Array.Sort(results);
            CreateShortcuts(results);
            return results.ToList();
        }
    }
}