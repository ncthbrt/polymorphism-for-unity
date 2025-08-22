#nullable enable
using System;
using Polymorphism4Unity.Editor.Menus.SearchableMenuTrees;
using Polymorphism4Unity.Editor.Styling;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEngine;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Menus.TypeMenus
{
    public class TypeMenuTypeElement: SearchableMenuTreeElement<Type>
    {
        private readonly TextElement _namespaceTextElement;
        private RegistrationSet? _registrationSet; 
        public TypeMenuTypeElement()
        {
            _namespaceTextElement = new TextElement
            {
                name = "Namespace"
            };
            _namespaceTextElement.style.ApplyStyles(new CompactStyle()
            {
                color = Color.gray,
                display = DisplayStyle.None,
                unityTextAlign = TextAnchor.UpperRight
            });
            Insert(1, _namespaceTextElement);
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanel);
        }

        private void HandleAttachToPanel(AttachToPanelEvent _)
        {
            Asserts.IsNull(_registrationSet);
            _registrationSet = new RegistrationSet(this);
            if (Node is SearchableMenuTreeLeafNode<Type> leaf)
            {
                if (leaf.Value.Namespace is { } namespaceName)
                {
                    _namespaceTextElement.text = namespaceName;
                    _namespaceTextElement.style.display = DisplayStyle.Flex;
                }
                else
                {
                    _namespaceTextElement.style.display = DisplayStyle.None;
                }
            }
        }

        private void HandleDetachFromPanel(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;
            _namespaceTextElement.style.display = DisplayStyle.None;
        }
        
        
    }
}