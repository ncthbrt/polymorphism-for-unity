#nullable enable
using System;

namespace Polymorphism4Unity.Safety
{
    public static partial class Asserts
    {
        public static Exception Fail(string assertion)
        {
            throw new AssertionException(assertion);
        }

        public static Exception Never(string assertion)
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

        public static T? IsNull<T>(T? a)
        {
            if (a is not null)
            {
                throw new UnaryAssertionException<T>(a, $"{nameof(a)} is null");
            }
            return default;
        }

        public static T IsEqual<T>(T a, T b)
        {
            if (!Equals(a, b))
            {
                throw new UniformBinaryAssertionException<T>(a, b, $"Equals({nameof(a)}, {nameof(b)})");
            }

            return a;
        }

        public static T IsGreater<T>(T a, T b)
            where T : notnull, IComparable<T>
        {
            if (a.CompareTo(b) > 0)
            {
                throw new UniformBinaryAssertionException<T>(a, b, $"{nameof(a)} > {nameof(b)})");
            }
            return a;
        }

        public static T IsLess<T>(T a, T b)
            where T : notnull, IComparable<T>
        {
            if (a.CompareTo(b) < 0)
            {
                throw new UniformBinaryAssertionException<T>(a, b, $"{nameof(a)} < {nameof(b)})");
            }
            return a;
        }

        public static T IsGreaterOrEqual<T>(T a, T b)
            where T : notnull, IComparable<T>
        {
            if (a.CompareTo(b) >= 0)
            {
                throw new UniformBinaryAssertionException<T>(a, b, $"{nameof(a)} >= {nameof(b)})");
            }

            return a;
        }

        public static T IsLessOrEqual<T>(T a, T b)
            where T : notnull, IComparable<T>
        {
            if (a.CompareTo(b) <= 0)
            {
                throw new UniformBinaryAssertionException<T>(a, b, $"{nameof(a)} <= {nameof(b)})");
            }

            return a;
        }

        public static T IsNotEqual<T>(T a, T b)
        {
            if (Equals(a, b))
            {
                throw new UniformBinaryAssertionException<T>(a, b, $"!Equals({nameof(a)}, {nameof(b)})");
            }
            return a;
        }

        public static bool IsTrue(bool a)
        {
            if (!a)
            {
                throw new AssertionException($"{nameof(a)} is true");
            }
            return true;
        }

        public static bool IsFalse(bool a)
        {
            if (a)
            {
                throw new AssertionException($"{nameof(a)} is true");
            }
            return false;
        }

        public static T IsType<T>(object a)
        {
            if (a is not T b)
            {
                throw new BinaryAssertionException<object, Type>(a, typeof(T), $"a is {typeof(T).Name}");
            }
            return b;
        }

        public static bool IsType<T>(Type a)
        {
            if (!typeof(T).IsAssignableFrom(a))
            {
                throw new BinaryAssertionException<Type, Type>(a, typeof(T), $"{a.Name} is {typeof(T).Name}");
            }
            return true;
        }

        public static bool IsType(Type a, Type b)
        {
            if (!b.IsAssignableFrom(a))
            {
                throw new BinaryAssertionException<Type, Type>(a, b, $"{a.Name} is {b.Name}");
            }
            return true;
        }

        public static bool IsNotType<T>(Type a)
        {
            if (typeof(T).IsAssignableFrom(a))
            {
                throw new BinaryAssertionException<Type, Type>(a, typeof(T), $"{a.Name} is not {typeof(T).Name}");
            }
            return true;
        }

        public static bool IsNotType(Type a, Type b)
        {
            if (b.IsAssignableFrom(a))
            {
                throw new BinaryAssertionException<Type, Type>(a, b, $"{a.Name} is not {b.Name}");
            }
            return true;
        }

        public static T? IsTypeOrNull<T>(object? a)
            where T : class?
        {
            if (a is T b)
            {
                return b;
            }
            if (a is null)
            {
                return null;
            }
            throw new BinaryAssertionException<object, Type>(a, typeof(T), $"a is {typeof(T).Name} or null");
        }

        public static TBase IsNotType<TBase, TNotDerived>(TBase a)
          where TNotDerived : TBase
        {
            if (a is TNotDerived)
            {
                throw new BinaryAssertionException<TBase, Type>(a, typeof(TNotDerived), $"a is not {typeof(TNotDerived).Name}");
            }
            return a;
        }
    }
}