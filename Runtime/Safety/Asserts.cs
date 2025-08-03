#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace Polymorphism4Unity.Safety
{
    [PublicAPI]
    public static class Asserts
    {
        [ContractAnnotation(" => halt")]
        public static Exception Fail(string assertion)
        {
            throw new AssertionException(assertion);
        }

        [ContractAnnotation(" => halt")]
        public static Exception Never(string assertion)
        {
            throw new AssertionException(assertion);
        }


        [AssertionMethod]
        [ContractAnnotation("a:null => halt; a:notnull => a:notnull")]
        public static T IsNotNull<T>([NoEnumeration, AssertionCondition(AssertionConditionType.IS_NOT_NULL)] T? a)
        {
            if (a is null)
            {
                throw new AssertionException($"{nameof(a)} is not null");
            }
            return a;
        }

        [AssertionMethod]
        public static string IsNotNullOrEmpty(string? a)
        {
            return IsNotEqual(IsNotNull(a), string.Empty);
        }
        
        [AssertionMethod]
        public static TCollection IsNotNullOrEmpty<T, TCollection>(TCollection? a)
            where TCollection: ICollection<T>
        {
            IsNotEqual(IsNotNull(a).Count, 0);
            return a!;
        }
        
        [AssertionMethod]
        public static T[] IsNotNullOrEmpty<T>(T[]? a)
        {
            IsNotEqual(IsNotNull(a).Length, 0);
            return a!;
        }

        [AssertionMethod]
        public static string? IsNullOrEmpty(string? a)
        {
            if (a is null)
            {
                return null;
            }
            return IsEqual(a, string.Empty);
        }

        [ContractAnnotation("a:null => null; a:notnull => halt")]
        [AssertionMethod]
        public static T? IsNull<T>([NoEnumeration, AssertionCondition(AssertionConditionType.IS_NULL)] T? a)
        {
            if (a is not null)
            {
                throw new UnaryAssertionException<T>(a, $"{nameof(a)} is null");
            }
            return default;
        }

        public static Match HasMatch(string a, [RegexPattern] string pattern)
        {
            Regex regex = new(pattern);
            if (regex.Match(a) is { Value: not "" } match)
            {
                return match;
            }
            throw new BinaryAssertionException<string, Regex>(a, regex, $"(new {nameof(Regex)}({nameof(pattern)})).IsMatch(a)");
        }

        public static MatchCollection HasMatches(string a, [RegexPattern] string pattern)
        {
            Regex regex = new(pattern);
            if (regex.Matches(a) is { Count: > 0 } matches)
            {
                return matches;
            }
            throw new BinaryAssertionException<string, Regex>(a, regex, $"(new {nameof(Regex)}({nameof(pattern)})).IsMatch(a)");
        }

        public static string HasNoMatch(string a, [RegexPattern] string pattern)
        {
            Regex regex = new(pattern);
            if (regex.Match(a) is { Value: "" })
            {
                return a;
            }
            throw new BinaryAssertionException<string, Regex>(a, regex, $"(new {nameof(Regex)}({nameof(pattern)}).IsMatch(a)) == false");
        }

        public static void HasLength<T>(ICollection collection, int length)
        {
            IsEqual(collection.Count, length);
        }
        
        public static void ShorterThan<T>(ICollection collection, int length)
        {
            IsLess(collection.Count, length);
        }
        
        public static void LongerThan<T>(ICollection collection, int length)
        {
            IsGreater(collection.Count, length);
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
            where T : IComparable<T>
        {
            if (a.CompareTo(b) > 0)
            {
                throw new UniformBinaryAssertionException<T>(a, b, $"{nameof(a)} > {nameof(b)})");
            }
            return a;
        }

        public static T IsLess<T>(T a, T b)
            where T : IComparable<T>
        {
            if (a.CompareTo(b) < 0)
            {
                throw new UniformBinaryAssertionException<T>(a, b, $"{nameof(a)} < {nameof(b)})");
            }
            return a;
        }

        public static T IsGreaterOrEqual<T>(T a, T b)
            where T : IComparable<T>
        {
            if (a.CompareTo(b) >= 0)
            {
                throw new UniformBinaryAssertionException<T>(a, b, $"{nameof(a)} >= {nameof(b)})");
            }

            return a;
        }

        public static T IsLessOrEqual<T>(T a, T b)
            where T : IComparable<T>
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

        [AssertionMethod]
        [ContractAnnotation("a:true => true; a:false => halt")]
        public static bool IsTrue([AssertionCondition(AssertionConditionType.IS_TRUE)] bool a)
        {
            if (!a)
            {
                throw new AssertionException($"{nameof(a)} is true");
            }
            return true;
        }

        [AssertionMethod]
        [ContractAnnotation("a:false => false; a:true => halt")]
        public static bool IsFalse([AssertionCondition(AssertionConditionType.IS_FALSE)] bool a)
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