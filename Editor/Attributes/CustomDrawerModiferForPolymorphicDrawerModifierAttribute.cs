#nullable enable
using System;
namespace Polymorphism4Unity.Attributes
{
    public class CustomDrawerModiferForPolymorphicDrawerAttribute : Attribute
    {
        public Type AttributeType { get; }

        public CustomDrawerModiferForPolymorphicDrawerAttribute(Type attributeType)
        {
            AttributeType = attributeType;
        }
    }
}