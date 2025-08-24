#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using Polymorphism4Unity.Editor.Commands;
using Polymorphism4Unity.Editor.Containers.Stacks;
using Polymorphism4Unity.Editor.Manipulators;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using Raffinert.FuzzySharp.Extractor;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    public class SearchableMenuTreeFrame<T, TSearchToolbar, TElement> : StackFrameElement
        where TSearchToolbar: SearchableMenuTreeSearchToolbar<T>, new()
        where TElement: SearchableMenuTreeElement<T>, new()
    {
        private readonly SearchableMenuTreeListView<T, TElement> _treeListView;
        private readonly SearchableMenuTreeListView<T, TElement> _searchResultsListView;

        private readonly TSearchToolbar _toolbar;
        private RegistrationSet? _registrationSet;
        private RegistrationSet? _searchToolbarRegistrationSet;
        private readonly MenuTreeNavigationManipulator _menuTreeNavigationManipulator;
        public SearchableMenuTreeFrame(List<SearchableMenuTreeNode<T>> nodes)
        {
            _toolbar = new TSearchToolbar();
            _menuTreeNavigationManipulator = new MenuTreeNavigationManipulator();
            _treeListView = new SearchableMenuTreeListView<T, TElement>(nodes);
            _searchResultsListView = new SearchableMenuTreeListView<T, TElement>(new List<SearchableMenuTreeNode<T>>())
            {
                style =
                {
                    display = DisplayStyle.None
                }
            };
            _toolbar.Index = ConstructIndex(nodes);
            Add(_toolbar);
            Add(_treeListView);
            Add(_searchResultsListView);
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanel);
        }

        private List<SearchableMenuTreeIndexEntry<T>> ConstructIndex(List<SearchableMenuTreeNode<T>> nodes)
        {
            Stack<SearchableMenuTreeNode<T>> stack = new(nodes);
            List<SearchableMenuTreeIndexEntry<T>> results = new();
            while (stack.Count > 0)
            {
                SearchableMenuTreeNode<T> node = stack.Pop();
                if (node is SearchMenuTreeParentNode<T> parentNode)
                {
                    stack.Push(parentNode);
                }
                else
                {
                    SearchableMenuTreeLeafNode<T> leafNode = (SearchableMenuTreeLeafNode<T>)node;
                    results.AddRange(leafNode.SearchTerms.Select(x => new SearchableMenuTreeIndexEntry<T>(x, leafNode)));
                }
            }
            return results;
        }

        private void HandleAttachToPanel(AttachToPanelEvent _)
        {
            Asserts.IsNull(_registrationSet);
            Asserts.IsNull(_searchToolbarRegistrationSet);
            _registrationSet = new RegistrationSet(this);
            _registrationSet.AddManipulator(_menuTreeNavigationManipulator);
            _searchToolbarRegistrationSet = new RegistrationSet(_toolbar);
            _registrationSet.RegisterCallback<NavigateSearchCommand>(HandleNavigateSearchCommand);
            _registrationSet.RegisterCallback<NavigateBottomCommand>(HandleNavigateBottomCommand);
            _registrationSet.RegisterCallback<NavigateDownCommand>(HandleNavigateDownCommand);
            _registrationSet.RegisterCallback<NavigatePageDownCommand>(HandleNavigatePageDownCommand);
            _registrationSet.RegisterCallback<NavigatePageUpCommand>(HandleNavigatePageUpCommand);
            _registrationSet.RegisterCallback<NavigateTopCommand>(HandleNavigateTopCommand);
            _registrationSet.RegisterCallback<NavigateUpCommand>(HandleNavigateUpCommand);
            _searchToolbarRegistrationSet.RegisterCallback<ChangeEvent<List<SearchableMenuTreeNode<T>>?>>(HandleSearchChanged);
        }

        private void HandleSearchChanged(ChangeEvent<List<SearchableMenuTreeNode<T>>?> changeEvent)
        {
            if (changeEvent.newValue is { } results)
            {
                _searchResultsListView.itemsSource = results;
                _searchResultsListView.style.display = DisplayStyle.Flex;
                _treeListView.style.display = DisplayStyle.None;
            }
            else
            {
                _searchResultsListView.itemsSource = new List<SearchableMenuTreeNode<T>>();
                _treeListView.style.display = DisplayStyle.Flex;
                _searchResultsListView.style.display = DisplayStyle.None;
            }
        }

        private void HandleDetachFromPanel(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;
        }
        
        
        private void HandleNavigateBottomCommand(NavigateBottomCommand command)
        {
            if (_treeListView.itemsSource.Count > 0)
            {
                int index = _treeListView.itemsSource.Count - 1;
                _treeListView.SetSelection(index);
                _treeListView.ScrollToItem(index);
            }
            command.StopPropagation();
        }
        
        private void HandleNavigateDownCommand(NavigateDownCommand command)
        {
            int count = _treeListView.itemsSource.Count;
            if (_treeListView.selectedIndex is var index and >=0)
            {
                ++index;
                if (index >= count)
                {
                    _treeListView.ClearSelection();
                    _toolbar.Focus();
                }
                else
                {
                    _treeListView.Select(index);
                }
            }
            else if (count > 0)
            {   
                _treeListView.Select(0);
                _treeListView.Focus();
            }
            command.StopPropagation();
        }
        
        private void HandleNavigatePageDownCommand(NavigatePageDownCommand command)
        {
            _treeListView.PageDown();
            command.StopPropagation();
        }
        
        private void HandleNavigatePageUpCommand(NavigatePageUpCommand command)
        {
            _treeListView.PageUp();
            command.StopPropagation();
        }
        
        private void HandleNavigateTopCommand(NavigateTopCommand command)
        {
            if (_treeListView.itemsSource.Count > 0)
            {
                _treeListView.Select(0);
                _treeListView.Focus();
            }
            command.StopPropagation();
        }
        
        private void HandleNavigateUpCommand(NavigateUpCommand command)
        {
            int? maybeIndex = _treeListView.selectedIndex >= 0 
                ? _treeListView.selectedIndex
                : null;
            int count = _treeListView.itemsSource.Count;
            if (maybeIndex is >= 1)
            {
                int index = Math.Min(maybeIndex.Value - 1, count);
                _treeListView.Select(index);
                _treeListView.Focus();
            }   
            else
            {
                _toolbar.Focus();
            }
            command.StopPropagation();
        }
        
        private void HandleNavigateSearchCommand(NavigateSearchCommand command)
        {
            _toolbar.Focus(command.Character);
        }
    }
}