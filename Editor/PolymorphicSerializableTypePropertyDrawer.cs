#nullable enable
using UnityEngine;
using UnityEditor;
using Polymorphism4Unity.Abstractions;

namespace Polymorphism4Unity.Editor
{
    [CustomPropertyDrawer(typeof(PolymorphicTypeAttribute), useForChildren: false)]
    public class PolymorphicSerializableTypePropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            base.OnGUI(position, property, label);
        }
    }
}
