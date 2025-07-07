#nullable enable
using System;

namespace Polymorphism4Unity.KindSets
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class AutoFillAttribute : Attribute
    {
    }
}