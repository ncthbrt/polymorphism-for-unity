#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Polymorphism4Unity.Editor.Commands;
using Polymorphism4Unity.Editor.Containers.Stacks;
using Polymorphism4Unity.Editor.Styling;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using Raffinert.FuzzySharp.Extractor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;
using FuzzSearch = Raffinert.FuzzySharp.Process;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    [PublicAPI]
    public abstract class SearchableMenuTree<T>: VisualElement
        where T: class
    {
        private RegistrationSet? _registrationSet;
        private readonly StackView _stack;
        private readonly StackFrameElement _initialStackFrame;
        private SearchableMenuTreeIndexEntry<T>[]? _searchIndex;
        protected abstract IEnumerable<SearchableMenuTreeEntry<T>> Items { get; }

        public Action<T?> OnSelected { get; set; } = _ => { };
        
        [UxmlAttribute]
        public string HeaderText
        {
            get => _initialStackFrame.HeaderText;
            set => _initialStackFrame.HeaderText = value;
        }
        
        protected SearchableMenuTree()
        {
            _stack = new StackView();
            _initialStackFrame = new StackFrameElement();
            _searchToolbar = new Toolbar
            {
                name = "SearchToolbar"
            };
            Add(_searchToolbar);
            Add(_stack);
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanelEvent);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanelEvent);
        }

        protected virtual void HandleAttachToPanelEvent(AttachToPanelEvent _)
        {
            Asserts.IsNull(_registrationSet);
            PopulateSearchToolbar();
            _registrationSet = new RegistrationSet(this);
            _registrationSet.RegisterCallback<NavigateBackCommand>(HandleNavigateBackCommand);
            _registrationSet.RegisterCallback<NavigateSearchCommand>(HandleNavigateSearchCommand);
            _registrationSet.RegisterCallback<NavigateSubmitCommand>(HandleNavigateSubmitCommand);
            _registrationSet.RegisterCallback<NavigateBottomCommand>(HandleNavigateBottomCommand);
            _registrationSet.RegisterCallback<NavigateDownCommand>(HandleNavigateDownCommand);
            _registrationSet.RegisterCallback<NavigatePageDownCommand>(HandleNavigatePageDownCommand);
            _registrationSet.RegisterCallback<NavigatePageUpCommand>(HandleNavigatePageUpCommand);
            _registrationSet.RegisterCallback<NavigateTopCommand>(HandleNavigateTopCommand);
            _registrationSet.RegisterCallback<NavigateUpCommand>(HandleNavigateUpCommand);
            RefreshItems();
        }

        protected virtual void HandleDetachFromPanelEvent(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;
            _stack.ClearAll();
            _searchToolbar.Clear();
        }
        
        private async void HandleNavigateBackCommand(NavigateBackCommand command)
        {
            if (_stack.IsEmpty)
            {
                OnSelected.SafelyInvoke(null);
            }
            else
            {
                await _stack.TryPopAsync().SwallowAndLogExceptions();
            }
        }
        
        
        private void HandleNavigateBottomCommand(NavigateBottomCommand command)
        {
            if (itemsSource.Count > 0)
            {
                int index = itemsSource.Count - 1;
                SetSelection(index);
                ScrollToItem(index);
            }
        }
        
        private void HandleNavigateDownCommand(NavigateDownCommand command)
        {
            int? maybeIndex = selectedIndices.FirstOrDefault();
            if (maybeIndex is { } index)
            {
                index = index + 1 >= itemsSource.Count ? 0 : index + 1; 
                ScrollToItem(index);
                SetSelection(index);
            }
        }
        
        private void HandleNavigatePageDownCommand(NavigatePageDownCommand command)
        {
            
        }
        
        private void HandleNavigatePageUpCommand(NavigatePageUpCommand command)
        {
            
        }
        
        private void HandleNavigateTopCommand(NavigateTopCommand command)
        {
            if (itemsSource.Count > 0)
            {
                SetSelection(0);
                ScrollToItem(0);
            }
        }
        
        private void HandleNavigateUpCommand(NavigateUpCommand command)
        {
            int? maybeIndex = selectedIndices.FirstOrDefault();
            if (maybeIndex is { } index)
            {
                index = index - 1 < 0 ? itemsSource.Count - 1 : index - 1; 
                SetSelection(index);
                ScrollToItem(index);
            }   
        }
        
        private void HandleNavigateSearchCommand(NavigateSearchCommand command)
        {
            // _searchToolbar.Focus();   
        }
        
        private async void HandleNavigateSubmitCommand(NavigateSubmitCommand command)
        {
            IEventHandler commandTarget = command.target;
            SearchableMenuTreeNode<T> node = Asserts.IsType<SearchableMenuTreeNode<T>>(commandTarget);
            if (node is SearchableMenuTreeLeafNode<T> leaf)
            {
                OnSelected.SafelyInvoke(leaf.Value);
            }   
            else
            {
                SearchMenuTreeParentNode<T> parentNode = Asserts.IsType<SearchMenuTreeParentNode<T>>(node);
                StackFrameElement stackFrame = new();
                stackFrame.HeaderText = parentNode.text;
                stackFrame.AddRange(parentNode.ChildNodes);
                await _stack.PushAsync(stackFrame).SwallowAndLogExceptions();
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
            SearchableMenuTreeNode<T>[] treeRoots = ConstructTree(items);
            _searchIndex = ConstructIndex(items);
            _stack.ClearAll();
            _initialStackFrame.Clear();
            _initialStackFrame.AddRange(treeRoots);
            _stack.Add(_initialStackFrame);
            _stack.PushWithoutAnimate(_initialStackFrame);
        }

        private static void CreateShortcuts(SearchableMenuTreeNode<T>[] nodes)
        {
            // TODO: this is a little complex and will require changes to the Stack element
            // Lets get things working solidly first 
        }
        
        private static SearchableMenuTreeNode<T>[] ConstructTree(IEnumerable<SearchableMenuTreeEntry<T>> items)
        {
            SearchMenuTreeParentNode<T> intermediate = new(string.Empty);
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
                        SearchMenuTreeParentNode<T> parent = new(key);
                        current.ChildNodes.Insert(0, parent);
                        current = parent;
                    }
                    else if (maybeElement is SearchMenuTreeParentNode<T> parent)
                    {
                        current = parent;
                    }
                }
                string leafName = parts[^1];
                SearchableMenuTreeLeafNode<T> leafNode = new(leafName, item.Value);
                current.ChildNodes.Add(leafNode);
            }
            SearchableMenuTreeNode<T>[] results = intermediate.ChildNodes.ToArray();
            Array.Sort(results);
            CreateShortcuts(results);
            return results;
        }

        protected virtual StackFrameElement CreateFrame(SearchMenuTreeParentNode<T> parentNode)
        {
            StackFrameElement stackFrame = new()
            {
                name = parentNode.Key,
                HeaderText = parentNode.Key
            };
            VisualElement searchToolbar = CreateSearchToolbar(parentNode);
            stackFrame.Add(searchToolbar);
            ListView listView = new()
            {
                showBorder = false,
                showAddRemoveFooter = false,
                showFoldoutHeader = false,
                showBoundCollectionSize = false,
                fixedItemHeight = EditorGUIUtility.singleLineHeight,
                virtualizationMethod = CollectionVirtualizationMethod.FixedHeight
            };
            listView.style.ApplyStyles(new CompactStyle
            {
                flex = 1
            });
            stackFrame.Add(listView);
            return stackFrame;
        }

        protected virtual SearchableMenuTreeElement<T> CreateListElement()
        {
            SearchableMenuTreeElement<T> element = new();
            // visualElement.style.ApplyStyles(new CompactStyle()
            // {
            //     justifyContent = Justify.SpaceBetween,
            //     
            // });
            return element;
        }

        private void BindItem(SearchableMenuTreeElement<T> element, int index)
        {
            
            // element.Node = node;
        }

        protected virtual VisualElement CreateSearchToolbar(SearchMenuTreeParentNode<T> parentNode)
        {
            Toolbar toolbar = new()
            {
                name = "SearchToolbar"
            };
            ToolbarSearchField searchField = new()
            {
                name = "SearchField"
            };
            searchField.style.ApplyStyles(new CompactStyle
            {
                flex = 1 
            });
            return toolbar;
        }
    }
}