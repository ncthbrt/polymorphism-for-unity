#nullable enable
using System.Collections.Generic;
using System.Linq;
using Polymorphism4Unity.Editor.Commands;
using Polymorphism4Unity.Editor.Containers.Stacks;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    public class SearchableMenuTreeFrame<T> : StackFrameElement
    {
        private readonly SearchableMenuTreeListView<T> _listView;
        private RegistrationSet? _registrationSet;
        public SearchableMenuTreeFrame(List<SearchableMenuTreeNode<T>> nodes)
        {
            _listView = new SearchableMenuTreeListView<T>(nodes);
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
        }
        
        private void HandleNavigateDownCommand(NavigateDownCommand command)
        {
            int? maybeIndex = _listView.selectedIndices.FirstOrDefault();
            if (maybeIndex is { } index)
            {
                index = index + 1 >= _listView.itemsSource.Count ? 0 : index + 1; 
                _listView.ScrollToItem(index);
                _listView.SetSelection(index);
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
            if (_listView.itemsSource.Count > 0)
            {
                _listView.SetSelection(0);
                _listView.ScrollToItem(0);
            }
        }
        
        private void HandleNavigateUpCommand(NavigateUpCommand command)
        {
            int? maybeIndex =_listView. selectedIndices.FirstOrDefault();
            if (maybeIndex is { } index)
            {
                index = index - 1 < 0 ? _listView.itemsSource.Count - 1 : index - 1; 
                _listView.SetSelection(index);
                _listView.ScrollToItem(index);
            }   
        }
        
        private void HandleNavigateSearchCommand(NavigateSearchCommand command)
        {
            // _searchToolbar.Focus();   
        }
    }
}