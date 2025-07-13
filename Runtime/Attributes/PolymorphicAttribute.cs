#nullable enable
using System;
using UnityEngine;

namespace Polymorphism4Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class PolymorphicAttribute : PropertyAttribute
    {
    }
}