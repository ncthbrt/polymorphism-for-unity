using System;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Polymorphism4Unity.Editor.Fields.Types
{
    public class TypeDropdownField<T>: BaseField<Type>
    {
        public TypeDropdownField(string label) : base(label, CreateFieldInput())
        {
        }
        
        private static VisualElement CreateFieldInput()
        {
            VisualElement input = new()
            {
                name = "TypePopup",
            };

            input.AddToClassList("");
            return input;
        }

    }
}