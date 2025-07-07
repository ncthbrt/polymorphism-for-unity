using System;
using UnityEngine;

namespace Polymorphism4Unity
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public sealed class PolymorphicAttribute : PropertyAttribute
    {
    }
}