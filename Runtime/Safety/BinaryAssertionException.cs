#nullable enable
namespace Polymorphism4Unity.Safety
{
    public static partial class Asserts
    {
        public class BinaryAssertionException<TA, TB> : AssertionException
        {
            public TA A { get; private set; }
            public TB B { get; private set; }

            public BinaryAssertionException(TA a, TB b, string assertion) : base(assertion)
            {
                A = a;
                B = b;
            }
            public BinaryAssertionException(TA a, TB b, string assertion, string message) : base(assertion, message)
            {
                A = a;
                B = b;
            }
        }
    }
}