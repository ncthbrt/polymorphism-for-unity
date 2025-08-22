#nullable enable
using System;
using System.Collections.Generic;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEngine.UIElements;
using Polymorphism4Unity.Editor.Styling;
using UnityEditor;
using UnityEngine;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    public class SearchableMenuTreeListView<T, TElement>: ListView
        where TElement: SearchableMenuTreeElement<T>, new()
    {   
        private RegistrationSet? _registrationSet;
        private int PageSize => Mathf.CeilToInt(resolvedStyle.height / EditorGUIUtility.singleLineHeight);

        public List<SearchableMenuTreeNode<T>> Nodes
        {
            get => (List<SearchableMenuTreeNode<T>>)(itemsSource);
            set => itemsSource = value;
        }
        

        public SearchableMenuTreeListView(List<SearchableMenuTreeNode<T>>? nodes = null)
        {
            Nodes = nodes ?? new List<SearchableMenuTreeNode<T>>();
            showAddRemoveFooter = false;
            showBorder = false;
            showAlternatingRowBackgrounds = AlternatingRowBackground.All;
            showBoundCollectionSize = false;
            showFoldoutHeader = false;
            selectionType = SelectionType.Single;
            makeItem = MakeItem;
            bindItem = BindItem;
            unbindItem = UnbindItem;
            delegatesFocus = true;
            fixedItemHeight = EditorGUIUtility.singleLineHeight;
            virtualizationMethod = CollectionVirtualizationMethod.FixedHeight;
            if (childCount > 0)
            {
                SetSelection(0);
                ScrollToItem(0);
            }
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanelEvent);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanelEvent);
        }

        private static VisualElement MakeItem()
        {
            TElement element = new();
            return element;
        }

        private void BindItem(VisualElement element, int index)
        {
            Asserts.IsLess(index, itemsSource.Count);
            TElement searchableMenuTreeElement = (TElement)element;
            searchableMenuTreeElement.Node = (SearchableMenuTreeNode<T>)itemsSource[index]; 
        }

        private void UnbindItem(VisualElement element, int index)
        {
            TElement searchableMenuTreeElement = (TElement)element;
            searchableMenuTreeElement.Node = null;
        }

        private void HandleAttachToPanelEvent(AttachToPanelEvent _)
        {
            Asserts.IsNull(_registrationSet);
            _registrationSet = new RegistrationSet(this);
        }

        private void HandleDetachFromPanelEvent(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;
        }


        public void PageDown()
        {
            int pageSize = PageSize;
            int newIndex = Math.Min(selectedIndex + pageSize, itemsSource.Count - 1);
            Select(newIndex);
            Focus();
        }
        
        public void PageUp()
        {
            int pageSize = PageSize;
            int newIndex = Math.Max(selectedIndex - pageSize, 0);
            Select(newIndex);
            Focus();
        }

        public void Select(int index)
        {
            selectedIndex = index;
            ScrollToItem(selectedIndex);
        }

    }
}