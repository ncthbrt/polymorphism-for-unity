#nullable enable
using System;

namespace Polymorphism4Unity.Editor
{
    public static class Asserts
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

        public class UniformBinaryAssertionException<TA> : BinaryAssertionException<TA, TA>
        {
            public UniformBinaryAssertionException(TA a, TA b, string assertion) : base(a, b, assertion)
            {
            }

            public UniformBinaryAssertionException(TA a, TA b, string assertion, string message) : base(a, b, assertion, message)
            {
            }
        }

        public static Exception Fail(string assertion)
        {
            throw new AssertionException(assertion);
        }

        public static T IsNotNull<T>(T? a)
        {
            if (a is null)
            {
                throw new AssertionException($"{nameof(a)} is not null");
            }
            return a;
        }

        public static void IsNull<T>(T? a)
        {
            if (a is not null)
            {
                throw new UnaryAssertionException<T>(a, $"{nameof(a)} is null");
            }
        }

        public static void IsEqual<T>(T a, T b)
        {
            if (!Equals(a, b))
            {
                throw new UniformBinaryAssertionException<T>(a, b, $"Equals({nameof(a)}, {nameof(b)})");
            }
        }

        public static void IsNotEqual<T>(T a, T b)
        {
            if (Equals(a, b))
            {
                throw new UniformBinaryAssertionException<T>(a, b, $"!Equals({nameof(a)}, {nameof(b)})");
            }
        }

        public static void IsTrue(bool a)
        {
            if (!a)
            {
                throw new AssertionException($"{nameof(a)} is true");
            }
        }

        public static void IsFalse(bool a)
        {
            if (a)
            {
                throw new AssertionException($"{nameof(a)} is true");
            }
        }

        public static TB IsType<TA, TB>(TA a)
            where TB : TA
        {
            if (a is TB b)
            {
                return b;
            }
            throw new BinaryAssertionException<TA, Type>(a, typeof(TB), $"a is {typeof(TB).Name}");
        }

        public static TB? IsTypeOrNull<TA, TB>(TA? a)
            where TB : class?, TA
        {
            if (a is TB b)
            {
                return b;
            }
            if (a is null)
            {
                return null;
            }
            throw new BinaryAssertionException<TA, Type>(a, typeof(TB), $"a is {typeof(TB).Name}");
        }

        public static void IsNotType<TA, TB>(TA a)
          where TB : TA
        {
            if (a is TB)
            {
                throw new BinaryAssertionException<TA, Type>(a, typeof(TB), $"a is not {typeof(TB).Name}");
            }
        }
    }
}