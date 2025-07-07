using System;
using UnityEngine;

namespace Polymorphism4Unity
{
#if UNITY_6000_0_OR_NEWER    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Class)]
#else
    [AttributeUsage(AttributeTargets.Class)]
#endif
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