#nullable enable
using System;
using Polymorphism4Unity.Enums;
using static Polymorphism4Unity.Enums.TypesFilter;

namespace Polymorphism4Unity.Editor.Utils
{
    public static class TypesFilterExtensions
    {
        public static bool HasFlag(this TypesFilter filter, TypesFilter value)
        {
            return (filter & value) != 0;
        }


        public static bool Matches(TypesFilter filter, Type? t)
        {
            if (t == null)
            {
                return filter.HasFlag(Nulls);
            }
            if (t.IsAbstract && !filter.HasFlag(Abstracts))
            {
                return false;
            }
            if (t.IsInterface & !filter.HasFlag(Interfaces))
            {
                return false;
            }
            if (t.IsGenericType && !filter.HasFlag(Generics))
            {
                return false;
            }
            if (t.IsClass && !filter.HasFlag(Classes))
            {
                return false;
            }
            if (t.IsClass && !filter.HasFlag(Classes))
            {
                return false;
            }
            if (t.IsValueType && !filter.HasFlag(ValueTypes))
            {
                return false;
            }
            if (t.IsValueType && !filter.HasFlag(ValueTypes))
            {
                return false;
            }
            if (!t.HasDefaultPublicConstructor() && filter.HasFlag(HasDefaultPublicConstructor))
            {
                return false;
            }
            bool isPublic = t.IsPublic || t.IsNestedPublic;
            if (!isPublic && filter.HasFlag(IsPublic))
            {
                return false;
            }
            return true;
        }
    }
}