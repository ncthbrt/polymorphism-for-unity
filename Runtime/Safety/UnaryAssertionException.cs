#nullable enable
using JetBrains.Annotations;

namespace Polymorphism4Unity.Safety
{
    [PublicAPI]
    public class UnaryAssertionException<TA> : AssertionException
    {
        public TA A { get; private set; }
        public UnaryAssertionException(TA a, string assertion, string message) : base(assertion, message)
        {
            A = a;
        }

        public UnaryAssertionException(TA a, string assertion) : base(assertion)
        {
            A = a;
        }
    }
}