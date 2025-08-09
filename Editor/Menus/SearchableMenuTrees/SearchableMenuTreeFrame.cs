#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Polymorphism4Unity.Editor.Commands;
using Polymorphism4Unity.Editor.Containers.Stacks;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEngine;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    public class SearchableMenuTreeFrame<T, TSearchToolbar, TElement> : StackFrameElement
        where TSearchToolbar: SearchableMenuTreeSearchToolbar<T>, new()
        where TElement: SearchableMenuTreeElement<T>, new()
    {
        private readonly SearchableMenuTreeListView<T, TElement> _listView;
        private readonly TSearchToolbar _toolbar;
        private RegistrationSet? _registrationSet;
        public SearchableMenuTreeFrame(List<SearchableMenuTreeNode<T>> nodes)
        {
            _toolbar = new TSearchToolbar();
            _listView = new SearchableMenuTreeListView<T, TElement>(nodes);
            Add(_toolbar);
            Add(_listView);
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanel);
        }

        private void HandleAttachToPanel(AttachToPanelEvent _)
        {
            Asserts.IsNull(_registrationSet);
            _registrationSet = new RegistrationSet(this);
            _registrationSet.RegisterCallback<NavigateSearchCommand>(HandleNavigateSearchCommand);
            _registrationSet.RegisterCallback<NavigateBottomCommand>(HandleNavigateBottomCommand);
            _registrationSet.RegisterCallback<NavigateDownCommand>(HandleNavigateDownCommand);
            _registrationSet.RegisterCallback<NavigatePageDownCommand>(HandleNavigatePageDownCommand);
            _registrationSet.RegisterCallback<NavigatePageUpCommand>(HandleNavigatePageUpCommand);
            _registrationSet.RegisterCallback<NavigateTopCommand>(HandleNavigateTopCommand);
            _registrationSet.RegisterCallback<NavigateUpCommand>(HandleNavigateUpCommand);
        }

        private void HandleDetachFromPanel(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;
        }
        
        
        private void HandleNavigateBottomCommand(NavigateBottomCommand command)
        {
            if (_listView.itemsSource.Count > 0)
            {
                int index = _listView.itemsSource.Count - 1;
                _listView.SetSelection(index);
                _listView.ScrollToItem(index);
            }
            command.StopPropagation();
        }
        
        private void HandleNavigateDownCommand(NavigateDownCommand command)
        {
            int count = _listView.itemsSource.Count;
            if (_listView.selectedIndex is var index and >=0)
            {
                ++index;
                if (index >= count)
                {
                    _listView.ClearSelection();
                    _toolbar.Focus();
                }
                else
                {
                    _listView.Select(index);
                }
            }
            else if (count > 0)
            {   
                _listView.Select(0);
                _listView.Focus();
            }
            command.StopPropagation();
        }
        
        private void HandleNavigatePageDownCommand(NavigatePageDownCommand command)
        {
            _listView.PageDown();
            command.StopPropagation();
        }
        
        private void HandleNavigatePageUpCommand(NavigatePageUpCommand command)
        {
            _listView.PageUp();
            command.StopPropagation();
        }
        
        private void HandleNavigateTopCommand(NavigateTopCommand command)
        {
            if (_listView.itemsSource.Count > 0)
            {
                _listView.Select(0);
                _listView.Focus();
            }
            command.StopPropagation();
        }
        
        private void HandleNavigateUpCommand(NavigateUpCommand command)
        {
            int? maybeIndex = _listView.selectedIndex >= 0 
                ? _listView.selectedIndex
                : null;
            int count = _listView.itemsSource.Count;
            if (maybeIndex is >= 1)
            {
                int index = Math.Min(maybeIndex.Value - 1, count);
                _listView.Select(index);
                _listView.Focus();
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