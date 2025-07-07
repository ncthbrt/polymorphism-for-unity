#nullable enable
using System;
using UnityEngine;

namespace Polymorphism4Unity
{
    [AttributeUsage(AttributeTargets.Class)]
    public class KindSetAttribute : PropertyAttribute
    {
    }
}