#if UNITY_6000_0_OR_NEWER
using System;
using UnityEngine;

namespace Polymorphism4Unity
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PolymorphicListAttribute : PropertyAttribute
    {
        public PolymorphicListAttribute() : base(applyToCollection: true)
        {
        }
    }
}
#endif