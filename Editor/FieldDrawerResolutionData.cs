#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using T = Polymorphism4Unity.Editor.TypeUtils;
using static Polymorphism4Unity.Editor.FuncUtils;

namespace Polymorphism4Unity.Editor
{
    public sealed class FieldDrawerResolutionData : IEquatable<FieldDrawerResolutionData>
    {
        private static readonly Cache<Type, Type[]> concreteSubtypes = new(
            type => TypeCache.GetTypesDerivedFrom(type).Where(And<Type>(T.IsConcreteType, T.HasDefaultPublicConstructor)).ToArray()
        );

        private static readonly Cache<FieldInfo, PropertyAttribute[]> propertyAttributes = new(
            fieldInfo => fieldInfo
                .GetCustomAttributes<PropertyAttribute>()
                .OrderBy(x => x.order)
                .ThenBy(x => x.GetType().AssemblyQualifiedName)
                .ToArray()
        );

        private static readonly Cache<Type, IReadOnlyList<Type>> interfaces = new(
            type =>
            {
                Asserts.IsFalse(type.IsInterface);
                ISet<Type> interfacesSet = type.GetInterfaces().ToHashSet();
                Type? baseType = type.BaseType;
                if (baseType is not null)
                {
                    interfacesSet.ExceptWith(baseType.GetInterfaces());
                }
                Type[] interfaces = interfacesSet.ToArray();
                static int comparision(Type a, Type b)
                {
                    // a and b should not be exactly the same type
                    Asserts.IsNotEqual(a, b);
                    bool aIsB = a.Is(b);
                    bool bIsA = b.Is(a);
                    // If interfaces aren't related we consider them equal for the purposes of this sort
                    // To break the tie and ensure predictable ordering, we sort by assembly qualified name
                    if ((aIsB, bIsA) is (false, false))
                    {
                        return a.AssemblyQualifiedName.CompareTo(b.AssemblyQualifiedName);
                    }
                    // We want a to be before b if it is the larger (derived) interface
                    return aIsB ? -1 : -1;
                }
                Array.Sort(interfaces, comparision);
                return interfaces;
            }
        );
        private static readonly Cache<Type, IReadOnlyList<PropertyDrawerData>> propertyDrawers = new(ResolvePropertyDrawerData);

        private static IReadOnlyList<PropertyDrawerData> ResolvePropertyDrawerData(Type subtype)
        {
            List<PropertyDrawerData> result = new();
            void TryAdd(Type t)
            {
                if (PropertyDrawerData.Data.TryGetValue(t, out PropertyDrawerData? propertyDrawerData))
                {
                    result.Add(propertyDrawerData);
                }
            }
            TryAdd(subtype);
            if (subtype.IsGenericType)
            {
                Type currentGenericType = subtype.GetGenericTypeDefinition();
                TryAdd(currentGenericType);
            }
            IReadOnlyList<Type> subInterfaces = interfaces[subtype];
            foreach (Type sub in subInterfaces)
            {
                TryAdd(sub);
                if (sub.IsGenericType)
                {
                    Type currentGenericInterface = sub.GetGenericTypeDefinition();
                    TryAdd(currentGenericInterface);
                }
            }
            if (subtype.BaseType is not null)
            {
                IReadOnlyList<PropertyDrawerData> dataItems = propertyDrawers[subtype.BaseType];
                foreach (PropertyDrawerData data in dataItems)
                {
                    if (data.UseForChildren)
                    {
                        result.Add(data);
                    }
                }
            }
            return result;
        }

        public string PropertyPath { get; }
        public Type Type { get; }
        public Lazy<IReadOnlyList<Type>> AllSubtypes { get; }
        public Cache<Type, IReadOnlyList<PropertyDrawerData>> PropertyDrawers { get; }
        public IReadOnlyList<DecoratorDrawerData> DecoratorAttributes { get; }
        public IReadOnlyList<PropertyDrawerData> PropertyAttributes { get; }
        public bool IsRootField { get; }
        private FieldDrawerResolutionData(string propertyPath, PropertyAttribute[] propertyAttributes, Type type, bool isRootField)
        {
            Type = type;
            PropertyPath = propertyPath;
            IsRootField = isRootField;
            AllSubtypes = new(() => concreteSubtypes[Type]);

            DecoratorDrawerData? MaybeGetAttributeDecoratorDrawerData(PropertyAttribute attribute)
            {
#if UNITY_6000_0_OR_NEWER
                if (attribute.applyToCollection && isRootField)
                {
                    return null;
                }
#endif
                _ = DecoratorDrawerData.Mappings.TryGetValue(attribute.GetType(), out DecoratorDrawerData? drawerData);
                return drawerData;
            }
            DecoratorAttributes = propertyAttributes.Select(MaybeGetAttributeDecoratorDrawerData).WhereNotNull().ToList();

            PropertyDrawerData? MaybeGetAttributePropertyDrawerData(PropertyAttribute attribute)
            {
#if UNITY_6000_0_OR_NEWER
                if (attribute.applyToCollection && isRootField)
                {
                    return null;
                }
#endif
                _ = PropertyDrawerData.Data.TryGetValue(attribute.GetType(), out PropertyDrawerData? drawerData);
                return drawerData;
            }
            PropertyAttributes = propertyAttributes.Select(MaybeGetAttributePropertyDrawerData).WhereNotNull().ToList();
            PropertyDrawers = new Cache<Type, IReadOnlyList<PropertyDrawerData>>((subtype) =>
            {
                if (subtype.IsNot(Type))
                {
                    throw new ArgumentException($"Expected {subtype.Name} to be the same as or derived from {Type.Name}");
                }
                return propertyDrawers[subtype];
            });
        }

        public static FieldDrawerResolutionData CreateForField(SerializedProperty property, FieldInfo fieldInfo) =>
            new(property.propertyPath, propertyAttributes[fieldInfo], fieldInfo.FieldType, true);

        public static FieldDrawerResolutionData CreateForCollectionMember(SerializedProperty property, FieldInfo fieldInfo, Type elementType) =>
            new(property.propertyPath, propertyAttributes[fieldInfo], elementType, false);

        public bool Equals(FieldDrawerResolutionData? other) =>
            other is not null
            && (
                ReferenceEquals(this, other)
                || (PropertyPath == other.PropertyPath
                    && Type == other.Type
                    && IsRootField == other.IsRootField)
            );

        public override bool Equals(object other) =>
            Equals(other as FieldDrawerResolutionData);

        public override int GetHashCode() =>
            HashCode.Combine(PropertyPath, Type, IsRootField);
    }
}