#nullable enable
using System;

namespace Polymorphism4Unity.Safety
{
    public static partial class Asserts
    {
        public class AssertionException : Exception
        {
            public string Assertion { get; private set; }

            public AssertionException(string assertion, string message) : base(message)
            {
                Assertion = assertion;
            }

            public AssertionException(string assertion) : base()
            {
                Assertion = assertion;
            }
        }
    }
}