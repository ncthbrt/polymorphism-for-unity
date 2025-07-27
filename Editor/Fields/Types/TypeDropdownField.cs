using System;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Fields.Types
{
    
    public class TypeDrowdownField : BaseField<Type>
    {
        public Type Type { get; }
        public TypeDrowdownField(Type type) : base("Label", CreateFieldInput())
        {
            Type = type;
        }
        
        private static VisualElement CreateFieldInput()
        {
            VisualElement input = new()
            {
                name = "TypePopup",
            };

            return input;
        }
    }
    
    [UxmlElement(nameof(TypeDrowdownField))]
    public partial class TypeDropdownField<T>: TypeDrowdownField
    {
        public TypeDropdownField(): base(typeof(T))
        {
        }
    }
    
}