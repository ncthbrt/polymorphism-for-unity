using System;
using UnityEngine;

namespace Polymorphism4Unity
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class PolymorphicAttribute : PropertyAttribute
    {
    }
}
