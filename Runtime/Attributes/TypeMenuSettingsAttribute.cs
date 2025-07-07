#nullable enable
using System;

namespace Polymorphism4Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeMenuSettingsAttribute : Attribute
    {
        public bool Enabled = true;
        public TypeMenuStyle Style = TypeMenuStyle.Default;
    }
}