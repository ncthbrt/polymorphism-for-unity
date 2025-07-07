#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using static Polymorphism4Unity.Editor.FuncUtils;
using static Polymorphism4Unity.Editor.EnumerableUtils;
using T = Polymorphism4Unity.Editor.TypeUtils;

namespace Polymorphism4Unity.Editor
{
    public abstract class DrawerData
    {
        private const string typeFieldName = "m_Type";
        private const string useForChildrenFieldName = "m_UseForChildren";
        public Type DrawerType { get; private set; }
        public Type TargetType { get; private set; }
        public bool UseForChildren { get; private set; }

        public DrawerData(Type targetType, Type drawerType, bool useForChildren)
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

        protected static IEnumerable<CustomPropertyDrawerAttributeData> CustomPropertyDrawerAttributeDataFromDrawerType(Type drawerType)
        {
            foreach (CustomPropertyDrawer customPropertyDrawer in drawerType.GetCustomAttributes<CustomPropertyDrawer>())
            {
                IDynamicReadonlyInstance dynamicCustomPropertyDrawer = customPropertyDrawer.ToDynamicReadonlyInstance();
                Type? targetType = dynamicCustomPropertyDrawer.GetValue<Type>(typeFieldName);
                bool useForChildren = dynamicCustomPropertyDrawer.GetValue<bool>(useForChildrenFieldName);
                if (targetType is null)
                {
                    LoggerProvider.LogError(nameof(DrawerData), $"{drawerType.Name} has a CustomPropertyDrawer attribute where the type specified is null");
                    continue;
                }
                yield return new CustomPropertyDrawerAttributeData(targetType, useForChildren);
            }
        }
    }

    public class PropertyDrawerData : DrawerData
    {
        private static IEnumerable<PropertyDrawerData> FromDrawerType(Type drawerType)
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
        private static readonly IReadOnlyDictionary<Type, PropertyDrawerData> propertyDrawerMappings =
            propertyDrawersTypes.SelectMany(FromDrawerType).OfType<PropertyDrawerData>().ToLazyDictionary(x => x.TargetType, x => x);

        public static IReadOnlyDictionary<Type, PropertyDrawerData> Data =>
                    propertyDrawerMappings;

        private PropertyDrawerData(Type targetType, Type drawerType, bool useForChildren) : base(targetType, drawerType, useForChildren)
        {
        }
    }

    public class DecoratorDrawerData : DrawerData
    {
        private static IEnumerable<DecoratorDrawerData> FromDrawerType(Type drawerType)
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
        private static readonly IReadOnlyDictionary<Type, DecoratorDrawerData> decoratorDrawerMappings =
            decoratorDrawerTypes.SelectMany(FromDrawerType).ToLazyDictionary(x => x.TargetType, x => x);

        public static IReadOnlyDictionary<Type, DecoratorDrawerData> Mappings =>
            decoratorDrawerMappings;

        private DecoratorDrawerData(Type attributeType, Type drawerType, bool useForChildren) : base(attributeType, drawerType, useForChildren)
        {
        }
    }
}