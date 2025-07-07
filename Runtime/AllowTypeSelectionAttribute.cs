#nullable enable
using System;
using UnityEngine;

namespace Polymorphism4Unity
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field)]
    public class AllowTypeSelectionAttribute : PropertyAttribute
    {
    }
}