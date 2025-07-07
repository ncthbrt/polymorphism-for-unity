#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using Polymorphism4Unity.Safety;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Editor.Collections;
using static Polymorphism4Unity.Editor.Utils.FuncUtils;
using T = Polymorphism4Unity.Editor.Utils.TypeUtils;
using System.Reflection;

namespace Polymorphism4Unity.Editor.DrawerResolution
{
    public abstract class DrawerData
    {
        private const string typeFieldName = "m_Type";
        private const string useForChildrenFieldName = "m_UseForChildren";
        public Type DrawerType { get; private set; }
        public Type TargetType { get; private set; }
        public bool UseForChildren { get; private set; }

        protected DrawerData(Type targetType, Type drawerType, bool useForChildren)
        {
            Asserts.IsTrue(drawerType.Is<DecoratorDrawer>() || drawerType.Is<PropertyDrawer>());
            TargetType = targetType;
            UseForChildren = useForChildren;
            DrawerType = drawerType;
        }

        protected readonly struct CustomPropertyDrawerAttributeData
        {
            public Type TargetType { get; }
            public bool UseForChildren { get; }

            public CustomPropertyDrawerAttributeData(Type targetType, bool useForChildren)
            {
                TargetType = targetType;
                UseForChildren = useForChildren;
            }
        }

        private static IEnumerable<DecoratorDrawerData> DecoratorDrawerDataFromDrawerType(Type drawerType)
        {
            Asserts.IsType<DecoratorDrawer>(drawerType);
            IEnumerable<CustomPropertyDrawerAttributeData> attributeData =
                CustomPropertyDrawerAttributeDataFromDrawerType(drawerType);
            foreach (CustomPropertyDrawerAttributeData data in attributeData)
            {
                yield return new DecoratorDrawerData(data.TargetType, drawerType, data.UseForChildren);
            }
        }

        private static readonly CachedEnumerable<Type> decoratorDrawerTypes =
            TypeCache.GetTypesDerivedFrom<DecoratorDrawer>().Where(And<Type>(T.IsConcreteType, T.HasDefaultPublicConstructor)).Cached();
        private static readonly IReadOnlyDictionary<Type, DecoratorDrawerData> decoratorDrawerData =
            decoratorDrawerTypes.SelectMany(DecoratorDrawerDataFromDrawerType).ToLazyDictionary(x => x.TargetType, x => x);
        public static IReadOnlyDictionary<Type, DecoratorDrawerData> DecoratorDrawerData => decoratorDrawerData;

        private static IEnumerable<PropertyDrawerData> PropertyDrawerDataFromDrawerType(Type drawerType)
        {
            Asserts.IsType<PropertyDrawer>(drawerType);
            IEnumerable<CustomPropertyDrawerAttributeData> attributeData =
                CustomPropertyDrawerAttributeDataFromDrawerType(drawerType);
            foreach (CustomPropertyDrawerAttributeData data in attributeData)
            {
                yield return new PropertyDrawerData(data.TargetType, drawerType, data.UseForChildren);
            }
        }

        private static readonly CachedEnumerable<Type> propertyDrawersTypes =
            TypeCache.GetTypesDerivedFrom<PropertyDrawer>().Where(And<Type>(T.IsConcreteType, T.HasDefaultPublicConstructor)).Cached();
        private static readonly IReadOnlyDictionary<Type, PropertyDrawerData> propertyDrawerData =
            propertyDrawersTypes.SelectMany(PropertyDrawerDataFromDrawerType).OfType<PropertyDrawerData>().ToLazyDictionary(x => x.TargetType, x => x);
        public static IReadOnlyDictionary<Type, PropertyDrawerData> PropertyDrawerData => propertyDrawerData;


        private static IEnumerable<CustomPropertyDrawerAttributeData> CustomPropertyDrawerAttributeDataFromDrawerType(Type drawerType)
        {
            foreach (CustomPropertyDrawer customPropertyDrawer in drawerType.GetCustomAttributes<CustomPropertyDrawer>())
            {
                IDynamicReadonlyInstance dynamicCustomPropertyDrawer = customPropertyDrawer.ToDynamicReadonlyInstance();
                Type? targetType = dynamicCustomPropertyDrawer.GetValue<Type>(typeFieldName);
                bool useForChildren = dynamicCustomPropertyDrawer.GetValue<bool>(useForChildrenFieldName);
                if (targetType is null)
                {
                    UnityEngine.Debug.LogError($"{drawerType.Name} has a CustomPropertyDrawer attribute where the type specified is null");
                    continue;
                }
                yield return new CustomPropertyDrawerAttributeData(targetType, useForChildren);
            }
        }
    }
}