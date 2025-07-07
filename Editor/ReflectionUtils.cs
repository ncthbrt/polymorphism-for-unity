#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Polymorphism4Unity.Editor
{
    internal static partial class ReflectionUtils
    {
        /// <summary>
        /// Evalauates whether <paramref name="t"/> is a type that can be constructed.
        /// </summary>
        /// <param name="t">The <see cref="Type"/> to evaluate.</param>
        /// <returns>Whether <paramref name="t"/> is a concrete type.</returns>
        public static bool IsConcreteType(this Type t)
        {
            return t.IsAbstract is false &&
                t.IsInterface is false &&
                (t.IsGenericType is false || t.IsConstructedGenericType)
              ;
        }

        /// <summary>
        /// .Gets the default parameterless <see langword="public" /> constructor for <paramref name="t">, if available.
        /// </summary>
        /// <param name="t">The <see cref="Type"/> to maybe retrieve the constructor for.</param>
        /// <returns>The <see cref="ConstructorInfo"/>, if it is available, else.<see langword="null">.</returns>
        public static ConstructorInfo? MaybeGetDefaultPublicConstructor(this Type t)
        {
            if (!IsConcreteType(t))
            {
                return null;
            }
            return t.GetConstructor(Type.EmptyTypes);
        }

        public static bool HasDefaultPublicConstructor(this Type t) =>
            t.MaybeGetDefaultPublicConstructor() is not null;

        public static IEnumerable<Type> GetConcreteSubtypes<TBaseType>()
        {
            return Enumerable.Empty<Type>();
        }

        public static bool Is<TParentType>(this Type childType)
        {
            return typeof(TParentType).IsAssignableFrom(childType);
        }

        public static bool Is(this Type childType, Type parentType)
        {
            return parentType.IsAssignableFrom(childType);
        }

        public static IDynamicInstance ToDynamicInstance<TBaseType>(this TBaseType value) =>
            new DynamicInstance<TBaseType>(value);
    }
}