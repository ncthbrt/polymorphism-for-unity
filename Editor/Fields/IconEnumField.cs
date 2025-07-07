using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Fields
{
    [UxmlElement, UsedImplicitly]
    public partial class IconEnumField : EnumField
    {
        [UxmlAttribute("icon"), CanBeNull]
        public Texture2D Icon { get; set; } = null!;

        [UxmlAttribute("built-in-icon-name")]
        public string BuiltInIconName { get; set; }

        [UxmlAttribute("show-value-on-control")]
        public bool ShowValueOnControl { get; set; }

        public IconEnumField()
        {
            InitField();
        }

        private void InitField()
        {
            int index = childCount - 1;
            AddToClassList("poly_icon__enum_field");
            VisualElement visualElement = ElementAt(index);
            VisualElement icon = new();
            icon.pickingMode = PickingMode.Ignore;
            icon.name = "Icon";
            icon.style.display = DisplayStyle.None;
            visualElement.Insert(0, icon);
            if (!ShowValueOnControl)
            {
                TextElement maybeTextElement = visualElement.Q<TextElement>(name = null, textUssClassName);
                if (maybeTextElement != null)
                {
                    maybeTextElement.style.display = DisplayStyle.None;
                }
            }
            RegisterCallback<AttachToPanelEvent>((_) =>
            {
                StyleSheet iconEnumStyleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.ncthbrt.polymorphism-for-unity/Editor/UIElements/IconEnumFieldStyle.uss");
                styleSheets.Add(iconEnumStyleSheet);
                if (Icon)
                {
                    icon.style.backgroundImage = this.Icon;
                    icon.style.display = DisplayStyle.Flex;
                }
                else if (!string.IsNullOrEmpty(BuiltInIconName))
                {
                    icon.style.backgroundImage = (Texture2D)EditorGUIUtility.IconContent(BuiltInIconName).image;
                    icon.style.display = DisplayStyle.Flex;
                }
                else
                {
                    icon.style.display = DisplayStyle.None;
                }
            });

        }


    }
}