#nullable enable
using System;
using System.Collections.Generic;

namespace Polymorphism4Unity.Editor
{
    internal static class EnumerableUtils
    {
        public static IEnumerable<T> FromSingleItem<T>(T item)
        {
            yield return item;
        }

        public static LazyDictionary<TInput, TKey, TValue> ToLazyDictionary<TInput, TKey, TValue>(this IEnumerable<TInput> enumerable, Func<TInput, TKey> keyMapper, Func<TInput, TValue> valueMapper) =>
            new LazyDictionary<TInput, TKey, TValue>(enumerable, keyMapper, valueMapper);

        public static ICachedEnumerable<TElement> Cached<TElement>(this IEnumerable<TElement> enumerable) => Create(enumerable);

        public static ICachedEnumerable<TElement> Create<TElement>(IEnumerable<TElement> enumerable)
        {
            ICachedEnumerable<TElement> inner = new NotStartedCachedEnumerable<TElement>(enumerable);
            return new WrappedCachedEnumerable<TElement>(inner);
        }

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

        public static IEnumerable<TOut> SelectWhere<TIn, TOut>(this IEnumerable<TIn> enumerable, Func<TIn, (TOut? maybeResult, bool include)> f)
        {
            foreach (TIn input in enumerable)
            {
                (TOut? maybeResult, bool include) = f(input);
                if (include)
                {
                    TOut result = Asserts.IsNotNull(maybeResult);
                    yield return result;
                }
            }
        }
    }
}