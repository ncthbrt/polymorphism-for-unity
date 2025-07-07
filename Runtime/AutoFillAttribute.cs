#nullable enable
using System;

namespace Polymorphism4Unity
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class AutoFillAttribute : Attribute
    {
    }
}