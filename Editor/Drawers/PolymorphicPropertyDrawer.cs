#nullable enable
using UnityEngine;
using UnityEditor;
using Polymorphism4Unity.Attributes;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Polymorphism4Unity.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PolymorphicAttribute))]
    public class PolymorphicPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            Debug.Log("Creating");
            PropertyField propertyField = new(property);
            IResolvedStyle resolvedStyle = propertyField.resolvedStyle;
            float minHeight = Mathf.Max(resolvedStyle.height, resolvedStyle.minHeight.value);
            Debug.Log(minHeight);
            return propertyField;
        }
    }
}
