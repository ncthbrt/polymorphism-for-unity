using System;
using UnityEngine;

namespace Polymorphism4Unity.Abstractions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class PolymorphicTypeAttribute : PropertyAttribute
    {
    }
}
