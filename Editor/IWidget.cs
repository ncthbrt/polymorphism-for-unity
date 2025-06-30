#nullable enable
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor
{
    public interface IPropertyDrawerWidget
    {
        void OnGUI(Rect rect);
        float GetPropertyHeight(GUIContent label);

        VisualElement CreatePropertyGUI();
    }
}