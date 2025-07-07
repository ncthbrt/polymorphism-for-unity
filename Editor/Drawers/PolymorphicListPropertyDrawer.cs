#nullable enable
using UnityEngine;
using UnityEditor;
using System;
using Polymorphism4Unity.Lists;

namespace Polymorphism4Unity.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PolymorphicList<>))]
    public class PolymorphicListPropertyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            throw new NotImplementedException();
        }
    }
}
