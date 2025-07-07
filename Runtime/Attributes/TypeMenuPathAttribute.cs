#nullable enable
using System;

namespace Polymorphism4Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public class TypeMenuPathAttribute : Attribute
    {
        public string MenuPath { get; }

        public TypeMenuPathAttribute(string menuPath)
        {
            MenuPath = menuPath;
        }
    }
}