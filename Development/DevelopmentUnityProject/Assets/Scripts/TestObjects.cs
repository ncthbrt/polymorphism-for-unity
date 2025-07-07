#nullable enable
using System;
using JetBrains.Annotations;
using UnityEngine;

namespace Polymorphism4Unity.DevelopmentEnvironment
{
    [Serializable, UsedImplicitly]
    public abstract class TestObject
    {
        [SerializeField]
        private int value1;
    }

    [Serializable, UsedImplicitly]
    public class TestObjectA : TestObject
    {
        [SerializeField]
        private string value2 = string.Empty;
    }

    [Serializable, UsedImplicitly]
    public class TestObjectB : TestObject
    {
        [SerializeReference]
        private TestObject value3 = new TestObjectA();
    }
}