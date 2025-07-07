#nullable enable
using System;
using Polymorphism4Unity.Enums;

namespace Polymorphism4Unity.TypeTags
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeTagSettingsAttribute : Attribute
    {
        public TypesFilter Filter { get; } = TypesFilter.ConcretesAndNulls;
    }
}