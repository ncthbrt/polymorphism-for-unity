#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Polymorphism4Unity.Editor.Commands;
using Polymorphism4Unity.Editor.Containers.Stacks;
using Polymorphism4Unity.Editor.Events;
using Polymorphism4Unity.Editor.Menus.SearchableMenuTrees;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Enums;
using Polymorphism4Unity.Safety;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Menus.TypeMenus
{
    public class TypeMenuView : StackView
    {
        class TypeMenuSearchableMenuTree : SearchableMenuTree<Type, SearchableMenuTreeSearchToolbar<Type>, TypeMenuTypeElement>
        {
            private TypesFilter _filter;
            private Type _baseType;
            public Type BaseType
            {
                get => _baseType;
                set  
                {
                    _baseType = value;
                    RefreshItems();
                }
            }

            public TypesFilter Filter
            {
                get => _filter;
                set
                {
                    RefreshItems();
                    _filter = value;
                }
            }
            
            protected override IEnumerable<SearchableMenuTreeEntry<Type>> Items => TypeUtils.GetSubtypes(BaseType, Filter).Select(type=> new SearchableMenuTreeEntry<Type>
            {
                Value = type,
                Path = string.Join('/', type.Namespace?.Split('.') ?? Array.Empty<string>()),
                HasNext = type.IsGenericType && !type.IsConstructedGenericType
            });
            
            public TypeMenuSearchableMenuTree(Type baseType, TypesFilter filter)
            {
                _baseType = baseType;
                _filter = filter;
            }
        }
        
        private RegistrationSet? _registrationSet; 
        public Action<Type?> OnClose { get; }

        private TypeMenuView(Type baseType, TypesFilter filter, Action<Type?> onClose)
        {
            OnClose = onClose;
            TypeMenuSearchableMenuTree tree = CreateTree(baseType, filter);
            StackFrameElement frame = new();
            frame.Add(tree);
            PushWithoutAnimate(frame);
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanel);
        }

        private void HandleAttachToPanel(AttachToPanelEvent _)
        {
            Asserts.IsNull(_registrationSet);
            _registrationSet = new RegistrationSet(this);
            _registrationSet.RegisterCallback<ItemSelectedEvent<Type>>(HandleItemSelected);
        }
        
        private void HandleDetachFromPanel(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;

        }

        private void HandleItemSelected(ItemSelectedEvent<Type> @event)
        {
            if (@event.Value is { } type)
            {
                if (!type.IsGenericType || type.IsConstructedGenericType)
                {
                    // We close here
                }
                else
                {
                    // We need to push a new item on the stack that holds the generic menu stack
                    @event.StopPropagation();
                }
            }
            else
            {
                NavigateCloseCommand navigateCloseCommand = NavigateCloseCommand.GetPooled();
                SendEvent(navigateCloseCommand);
            }
        }

        private static TypeMenuSearchableMenuTree CreateTree(Type type, TypesFilter filter)
        {
            TypeMenuSearchableMenuTree tree = new(type, filter);
            return tree;
        }

    }
}