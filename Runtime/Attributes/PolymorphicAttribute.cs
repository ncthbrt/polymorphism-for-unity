#nullable enable
using System;
using Polymorphism4Unity.Enums;
using UnityEngine;

namespace Polymorphism4Unity.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class PolymorphicAttribute : PropertyAttribute
    {
        public PolymorphicAttribute()
        {

        }
    }
}