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

        private static readonly Cache<(Type type, TypesFilter filter), Type[]> _subtypes = new(
            (args) => TypeCache.GetTypesDerivedFrom(args.type).Where(Matches(args.filter)).ToArray()
        );

        public static Type[] GetSubtypes(Type type, TypesFilter filter = TypesFilter.Concretes) =>
            _subtypes[(type, filter)];


        public static bool IsConcreteConstructedType(this Type t)
        {
            return t.IsAbstract is false &&
                t.IsInterface is false &&
                (t.IsGenericType is false || t.IsConstructedGenericType);
        }
        
        public static ConstructorInfo? MaybeGetDefaultPublicConstructor(this Type t)
        {
            if (!IsConcreteConstructedType(t))
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

        public static DynamicReadonlyInstance<TBaseType> ToDynamicReadonlyInstance<TBaseType>(this TBaseType value) => new(value);
    }
}