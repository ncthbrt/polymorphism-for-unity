#nullable enable
using UnityEngine;

namespace Polymorphism4Unity.Attributes
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