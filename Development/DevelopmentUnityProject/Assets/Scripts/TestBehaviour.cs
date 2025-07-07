#nullable enable
using Polymorphism4Unity.Attributes;
using UnityEngine;

namespace Polymorphism4Unity.DevelopmentEnvironment
{
    public class TestBehaviour : MonoBehaviour
    {
        [SerializeReference, Polymorphic]
        private TestObject? testObject = new TestObjectB();
    }
}