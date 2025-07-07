using System;
using UnityEditor;
using UnityEngine;

namespace Polymorphism4Unity
{
    [AttributeUsage(AttributeTargets.Field)]
    public class PolymorphicListAttribute : PropertyAttribute
    {
        public PolymorphicListAttribute()
#if UNITY_6000_0_OR_NEWER
                : base(applyToCollection: true)
#endif
        {
        }
    }
}