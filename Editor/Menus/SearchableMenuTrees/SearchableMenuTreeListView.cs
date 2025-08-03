#nullable enable
using System.Collections.Generic;
using Polymorphism4Unity.Editor.Commands;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEngine.UIElements;
using System.Linq;
using Polymorphism4Unity.Editor.Styling;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    public class SearchableMenuTreeListView<T>: ListView
        where T: VisualElement, new()
    {   
        private RegistrationSet? _registrationSet = null;
        
        public SearchableMenuTreeListView(List<T> results)
        {
            itemsSource = results;
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
            if (childCount > 0)
            {
                SetSelection(0);
                ScrollToItem(0);
            }
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanelEvent);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanelEvent);
        }

        private VisualElement MakeItem()
        {
            VisualElement element = new();
            element.style.ApplyStyles(new CompactStyle
            {
                margin = 0,
                padding = 0,
            });
            return element;
        }

        private void BindItem(VisualElement element, int index)
        {
            Asserts.IsLess(index, itemsSource.Count);
            element.Add((VisualElement)itemsSource[index]);
        }

        private void UnbindItem(VisualElement element, int index)
        {
            element.Clear();
        }

        protected virtual void HandleAttachToPanelEvent(AttachToPanelEvent _)
        {
            Asserts.IsNull(_registrationSet);
            _registrationSet = new RegistrationSet(this);

        }

        protected virtual void HandleDetachFromPanelEvent(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;
        }
        
    }
}