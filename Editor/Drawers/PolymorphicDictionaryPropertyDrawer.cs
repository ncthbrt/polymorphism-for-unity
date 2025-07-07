#nullable enable
using UnityEngine;
using UnityEditor;
using System;
using Polymorphism4Unity.Dictionaries;

namespace Polymorphism4Unity.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PolymorphicDictionary<,,>))]
    public class PolymorphicDictionaryPropertyDrawer : PropertyDrawer
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
