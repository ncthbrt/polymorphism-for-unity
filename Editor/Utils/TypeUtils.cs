#nullable enable
using System;
using System.Linq;
using System.Reflection;
using Polymorphism4Unity.Editor.Collections;
using Polymorphism4Unity.Enums;
using UnityEditor;


namespace Polymorphism4Unity.Editor.Utils
{
    internal static class TypeUtils
    {
        private static Func<Type, bool> Matches(TypesFilter filter) =>
             t => TypesFilterExtensions.Matches(filter, t);

        private static readonly Cache<(Type type, TypesFilter filter), Type[]> subtypes = new(
            (args) => TypeCache.GetTypesDerivedFrom(args.type).Where(Matches(args.filter)).ToArray()
        );

        public static Type[] GetSubtypes(Type type, TypesFilter filter = TypesFilter.Concretes) =>
            subtypes[(type, filter)];


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

        public static bool Is<TParentType>(this Type childType) =>
            typeof(TParentType).IsAssignableFrom(childType);
        public static bool IsNot<TParentType>(this Type childType) =>
            !typeof(TParentType).IsAssignableFrom(childType);

        public static bool Is(this Type childType, Type parentType) =>
            parentType.IsAssignableFrom(childType);

        public static bool IsNot(this Type childType, Type parentType) =>
            !parentType.IsAssignableFrom(childType);

        public static IDynamicReadonlyInstance ToDynamicReadonlyInstance<TBaseType>(this TBaseType value) =>
            new DynamicReadonlyInstance<TBaseType>(value);
    }
}