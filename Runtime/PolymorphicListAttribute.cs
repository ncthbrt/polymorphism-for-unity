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
#if UNITY_6000_0_OR_NEWER
        public PolymorphicListAttribute(bool applyToCollection = false)
            : base(applyToCollection)

        {
        }
#endif
    }
}