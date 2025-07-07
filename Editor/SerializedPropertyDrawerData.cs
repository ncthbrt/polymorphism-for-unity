#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Polymorphism4Unity.Editor
{
    internal class SerializedPropertyDrawerData
    {
        private static readonly ICachedEnumerable<Type> propertyDrawersTypes =
            TypeCache.GetTypesDerivedFrom<PropertyDrawer>().Where(x => x.IsConcreteType() && x.HasDefaultPublicConstructor()).Cached();

        private static readonly ICachedEnumerable<Type> decoratorDrawerTypes =
            TypeCache.GetTypesDerivedFrom<DecoratorDrawer>().Where(x => x.IsConcreteType() && x.HasDefaultPublicConstructor()).Cached();

        private static readonly IReadOnlyDictionary<Type, DrawerData> propertyDrawerMappings =
            propertyDrawersTypes.SelectMany(DrawerData.FromDrawerType).ToLazyDictionary(x => x.TargetType, x => x);
        private static readonly IReadOnlyDictionary<Type, DrawerData> decoratorDrawerMappings =
            decoratorDrawerTypes.SelectMany(DrawerData.FromDrawerType).ToLazyDictionary(x => x.TargetType, x => x);

        private readonly Cache<Type, PropertyDrawer> subDrawers =
            new Cache<Type, PropertyDrawer>((type) =>
            {
                throw new Exception();
            });

        private readonly List<DrawerData> fieldDecorators;
        private readonly List<DrawerData> elementDecorators;
        private readonly Dictionary<Type, DrawerData> propertyDrawerFieldTypeMapping;
        private readonly Dictionary<Type, DrawerData> propertyDrawerElementMapping;
        private readonly string propertyPath;
        public ISet<Type> PossibleTypes { get; } = new HashSet<Type>();

        public SerializedPropertyDrawerData(SerializedProperty serializedProperty, FieldInfo fieldInfo)
        {
            propertyPath = serializedProperty.propertyPath;
            Type fieldType = fieldInfo.FieldType;
            List<PropertyAttribute> propertyAttributes = fieldInfo.GetCustomAttributes<PropertyAttribute>().ToList();
            List<DrawerData> decorators =
                propertyAttributes
                    .OrderBy(x => x.order)
                    .SelectWhere(x =>
                    {
                        if (decoratorDrawerMappings.TryGetValue(x.GetType(), out DrawerData drawerData) && drawerData.Kind is DrawerData.DrawerKind.DecoratorDrawer)
                        {
                            return (drawerData, true);
                        }
                        return (null, false);
                    }).ToList();
#if UNITY_6000_0_OR_NEWER
#else
            // In
            propertyDrawerElementMapping = propertyAttributes.ToArray();
#endif

            this.propertyDrawerFieldTypeMapping = new();
        }
    }
}