#nullable enable
namespace Polymorphism4Unity.Safety
{
    public static partial class Asserts
    {
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
}