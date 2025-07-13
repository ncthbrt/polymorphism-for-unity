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
using JetBrains.Annotations;

namespace Polymorphism4Unity.Editor.DrawerResolution
{
    [PublicAPI]
    internal abstract class DrawerData
    {
        private const string TypeFieldName = "m_Type";
        private const string UseForChildrenFieldName = "m_UseForChildren";
        public static readonly IReadOnlyDictionary<Type, PropertyDrawerData> PropertyDrawerData;
        public static readonly IReadOnlyDictionary<Type, DecoratorDrawerData> DecoratorDrawerData;
        
        public Type DrawerType { get; private set; }
        private Type TargetType { get; set; }
        public bool UseForChildren { get; private set; }

        protected DrawerData(Type targetType, Type drawerType, bool useForChildren)
        {
            Asserts.IsTrue(drawerType.Is<DecoratorDrawer>() || drawerType.Is<PropertyDrawer>());
            TargetType = targetType;
            UseForChildren = useForChildren;
            DrawerType = drawerType;
        }
        
        static DrawerData()
        {
            CachedEnumerable<Type> propertyDrawersTypes = TypeCache.GetTypesDerivedFrom<PropertyDrawer>().Where(And<Type>(T.IsConcreteConstructedType, T.HasDefaultPublicConstructor)).Cached();
            PropertyDrawerData = propertyDrawersTypes.SelectMany(PropertyDrawerDataFromDrawerType).ToLazyDictionary(x => x.TargetType, x => x);
            CachedEnumerable<Type> decoratorDrawerTypes = TypeCache.GetTypesDerivedFrom<DecoratorDrawer>().Where(And<Type>(T.IsConcreteConstructedType, T.HasDefaultPublicConstructor)).Cached();
            DecoratorDrawerData = decoratorDrawerTypes.SelectMany(DecoratorDrawerDataFromDrawerType).ToLazyDictionary(x => x.TargetType, x => x);
        }

        private readonly struct CustomPropertyDrawerAttributeData
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

        private static IEnumerable<CustomPropertyDrawerAttributeData> CustomPropertyDrawerAttributeDataFromDrawerType(Type drawerType)
        {
            foreach (CustomPropertyDrawer customPropertyDrawer in drawerType.GetCustomAttributes<CustomPropertyDrawer>())
            {
                DynamicReadonlyInstance<CustomPropertyDrawer> dynamicCustomPropertyDrawer = customPropertyDrawer.ToDynamicReadonlyInstance();
                Type? targetType = dynamicCustomPropertyDrawer.GetValue<Type?>(TypeFieldName);
                bool useForChildren = dynamicCustomPropertyDrawer.GetValue<bool>(UseForChildrenFieldName);
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