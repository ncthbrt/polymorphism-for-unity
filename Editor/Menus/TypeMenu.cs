#nullable enable
using System;
using JetBrains.Annotations;
using Polymorphism4Unity.Attributes;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Enums;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using PopupWindow = UnityEditor.PopupWindow;

namespace Polymorphism4Unity.Editor
{
    [UxmlElement, UsedImplicitly]
    public partial class TypeMenuContentMover : VisualElement
    {

    }

    public class TypeMenu : PopupWindowContent
    {


        public static TypeMenu Open(Rect activatorRect, Type baseType, TypesFilter typeFilter, Action<Type?> onClose)
        {
            TypeMenu typeMenu = new(baseType, typeFilter, onClose);
            PopupWindow.Show(activatorRect, typeMenu);
            return typeMenu;
        }

        public override void OnOpen()
        {
            Subtypes = TypeUtils.GetSubtypes(BaseType, TypeFilter);
            for (int i = 0; i < Subtypes.Length; ++i)
            {
                Type t = Subtypes[i];

            }
            Debug.Log("Popup opened: " + this);
        }

        public override VisualElement CreateGUI()
        {
            VisualElement verticalContainer = new VisualElement();
            ToolbarSearchField toolbarSearchField = new();
            verticalContainer.Add(toolbarSearchField);
            var visualTreeAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Packages/com.ncthbrt.polymorphism-for-unity/Editor/TypeMenuContent.uxml");
            return visualTreeAsset.CloneTree();
        }

        public override void OnClose()
        {
            Debug.Log("Popup closed: " + this);
        }

        public Type BaseType { get; set; }
        public TypeMenuDisplayMode MenuStyle { get; set; }
        public TypesFilter TypeFilter { get; set; }
        public Action<Type?> OnSelect { get; }
        public Type[] Subtypes { get; private set; }

        private TypeMenu(Type baseType, TypesFilter typeFilter, Action<Type?> onClose)
        {
            BaseType = baseType;
            TypeFilter = typeFilter;
            OnSelect = onClose;
            Subtypes = TypeUtils.GetSubtypes(baseType, typeFilter);
        }

    }
}