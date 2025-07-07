#nullable enable
namespace Polymorphism4Unity.Safety
{
    public static partial class Asserts
    {
        public class UniformBinaryAssertionException<TA> : BinaryAssertionException<TA, TA>
        {
            public UniformBinaryAssertionException(TA a, TA b, string assertion) : base(a, b, assertion)
            {
            }

            public UniformBinaryAssertionException(TA a, TA b, string assertion, string message) : base(a, b, assertion, message)
            {
            }
        }
    }
}