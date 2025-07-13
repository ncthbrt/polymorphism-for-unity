#nullable enable
using System;
using JetBrains.Annotations;

namespace Polymorphism4Unity.Safety
{
    [PublicAPI]
    public class AssertionException : Exception
    {
        public string Assertion { get; private set; }

        public AssertionException(string assertion, string message) : base(message)
        {
            Assertion = assertion;
        }

        public AssertionException(string assertion)
        {
            Assertion = assertion;
        }
    }
}