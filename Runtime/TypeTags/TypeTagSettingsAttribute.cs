#nullable enable
using System;

namespace Polymorphism4Unity.TypeTags
{
    [AttributeUsage(AttributeTargets.Field)]
    public class TypeTagSettingsAttribute : Attribute
    {
        public TypeTagFilter Filter { get; } = TypeTagFilter.ShowOnlyConcreteTypes;
        public GenericBehaviour GenericBehaviour { get; } = GenericBehaviour.GenericOrConstructed;
    }
}