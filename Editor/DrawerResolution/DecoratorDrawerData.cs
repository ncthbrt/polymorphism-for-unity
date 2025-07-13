#nullable enable
using System;

namespace Polymorphism4Unity.Editor.DrawerResolution
{
    internal class DecoratorDrawerData : DrawerData
    {
        public DecoratorDrawerData(Type attributeType, Type drawerType, bool useForChildren) : base(attributeType, drawerType, useForChildren)
        {
        }
    }
}