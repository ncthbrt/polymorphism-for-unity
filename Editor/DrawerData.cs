#nullable enable
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Polymorphism4Unity.Editor
{
    public class DrawerData
    {
        public enum DrawerKind
        {
            PropertyDrawer,
            DecoratorDrawer
        }
        private const string typeFieldName = "m_Type";
        private const string useForChildrenFieldName = "m_UseForChildren";

        public DrawerKind Kind { get; }
        public Type TargetType { get; private set; }
        public Type DrawerType { get; private set; }
        public bool UseForChildren { get; private set; }


        public DrawerData(Type targetType, Type drawerType, bool useForChildren)
        {
            TargetType = targetType;
            UseForChildren = useForChildren;
            DrawerType = drawerType;
            if (drawerType.Is<DecoratorDrawer>())
            {
                Kind = DrawerKind.DecoratorDrawer;
            }
            else if (drawerType.Is<PropertyDrawer>())
            {
                Kind = DrawerKind.DecoratorDrawer;
            }
            throw Asserts.Fail($"{nameof(drawerType)} is {nameof(DecoratorDrawer)} or ${nameof(PropertyDrawer)}");
        }

        public static IEnumerable<DrawerData> FromDrawerType(Type drawerType)
        {
            foreach (CustomPropertyDrawer attribute in drawerType.GetCustomAttributes<CustomPropertyDrawer>())
            {
                IDynamicInstance dynamicAttributeInstance = attribute.ToDynamicInstance();
                if (dynamicAttributeInstance.TryGetValue(typeFieldName, out Type? targetType) && dynamicAttributeInstance.TryGetValue(useForChildrenFieldName, out bool useForChildren))
                {
                    yield return new DrawerData(Asserts.IsNotNull(targetType), drawerType, useForChildren);
                }
            }
        }
    }
}