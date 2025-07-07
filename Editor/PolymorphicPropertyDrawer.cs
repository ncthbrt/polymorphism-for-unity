#nullable enable
using UnityEngine;
using UnityEditor;

namespace Polymorphism4Unity.Editor
{
    [CustomPropertyDrawer(typeof(PolymorphicAttribute), useForChildren: false)]
    public class PolymorphicPropertyDrawer : PropertyDrawer
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
