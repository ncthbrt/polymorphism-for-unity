#nullable enable
using UnityEngine;

namespace Polymorphism4Unity.Editor.Menus.TypeMenus
{
    public enum TypeMenuDisplayMode : uint
    {
        [InspectorName("Default")]
        Default = 0,
        [InspectorName("By Namespace")]
        GroupedByNamespace = 1,
        Flat = 2
    }
}