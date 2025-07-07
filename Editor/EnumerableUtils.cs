#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using static Polymorphism4Unity.Editor.FuncUtils;

namespace Polymorphism4Unity.Editor
{
    internal static class EnumerableUtils
    {
        public static IEnumerable<T> FromSingleItem<T>(T item)
        {
            yield return item;
        }

        public static bool All(this IEnumerable<bool> bools) => bools.All(x => x);
        public static bool Any(this IEnumerable<bool> bools) => bools.Any(x => x);

        public static LazyDictionary<TInput, TKey, TValue> ToLazyDictionary<TInput, TKey, TValue>(this IEnumerable<TInput> enumerable, Func<TInput, TKey> keyMapper, Func<TInput, TValue> valueMapper) =>
            new LazyDictionary<TInput, TKey, TValue>(enumerable, keyMapper, valueMapper);

        public static CachedEnumerable<TElement> Cached<TElement>(this IEnumerable<TElement> enumerable) => new(enumerable);

        public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> enumerable)
        {
            foreach (T? value in enumerable)
            {
                if (value is not null)
                {
                    yield return value;
                }
            }
            yield break;
        }

        public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate) =>
            enumerable.Where(predicate.Not());

        public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, (bool include, TOut? maybeResult)> f)
        {
            foreach (TIn input in enumerable)
            {
                (bool include, TOut? maybeResult) = f(input);
                if (include)
                {
                    TOut result = Asserts.IsNotNull(maybeResult);
                    yield return result;
                }
            }
        }

        public static IEnumerable<TOut> Apply<TOut, TIn1>(
            this IEnumerable<Func<TIn1, TOut>> enumerable,
            TIn1 value1
        ) => enumerable.Select(x => x(value1));

        public static IEnumerable<TOut> Apply<TOut, TIn1, TIn2>(
            this IEnumerable<Func<TIn1, TIn2, TOut>> enumerable,
            TIn1 value1, TIn2 value2
        ) => enumerable.Select(x => x(value1, value2));

        public static IEnumerable<TOut> Apply<TOut, TIn1, TIn2, TIn3>(
            this IEnumerable<Func<TIn1, TIn2, TIn3, TOut>> enumerable,
            TIn1 value1, TIn2 value2, TIn3 value3
        ) => enumerable.Select(x => x(value1, value2, value3));

        public static IEnumerable<TOut> Apply<TOut, TIn1, TIn2, TIn3, TIn4>(
            this IEnumerable<Func<TIn1, TIn2, TIn3, TIn4, TOut>> enumerable,
            TIn1 value1, TIn2 value2, TIn3 value3, TIn4 value4
        ) => enumerable.Select(x => x(value1, value2, value3, value4));

    }
}