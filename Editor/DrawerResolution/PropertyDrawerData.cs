#nullable enable
using System;

namespace Polymorphism4Unity.Editor.DrawerResolution
{
    public class PropertyDrawerData : DrawerData
    {
        public PropertyDrawerData(Type targetType, Type drawerType, bool useForChildren) : base(targetType, drawerType, useForChildren)
        {
        }
    }
}