#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Polymorphism4Unity.Editor.Collections;
using Polymorphism4Unity.Safety;

namespace Polymorphism4Unity.Editor.Utils
{
    internal static class EnumerableUtils
    {
        public static IEnumerable<T> FromSingleItem<T>(T item)
        {
            yield return item;
        }
        
        public static IEnumerable<T> FromItems<T>(params T[] items)
        {
            return items;
        }

        public static bool All(this IEnumerable<bool> bools) => bools.All(x => x);
        public static bool Any(this IEnumerable<bool> bools) => bools.Any(x => x);

        public static LazyDictionary<TInput, TKey, TValue> ToLazyDictionary<TInput, TKey, TValue>(this IEnumerable<TInput> enumerable, Func<TInput, TKey> keyMapper, Func<TInput, TValue> valueMapper) =>
            new(enumerable, keyMapper, valueMapper);

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

        public static void ForEach<T>(this IEnumerable<T> values, Action<T> action)
        {
            foreach (T value in values)
            {
                action(value);
            }
        }

        public static void InvokeAll(this IEnumerable<Action> actions)
        {
            foreach (Action action in actions)
            {
                action();
            }
        }
        
        public static bool Any<TSource, TType>(this IEnumerable<TSource> source) => 
            source.Any(x => x is TType);
        
        public static bool All<TSource, TType>(this IEnumerable<TSource> source) => 
            source.All(x => x is TType);

        public static TSource? FirstOrDefault<TSource, TType>(this IEnumerable<TSource> source) => 
                source.FirstOrDefault(x => x is TType);
        
        public static TSource First<TSource, TType>(this IEnumerable<TSource> source) =>
            source.First(x => x is TType);

        public static IEnumerable<T> Flatten<T, TInner>(this IEnumerable<TInner> enumerable) where TInner : IEnumerable<T>
        {
            return enumerable.SelectMany(x => x);
        }
        
        public static IEnumerable<T> Flatten<T>(this IEnumerable<List<T>> enumerable)
        {
            return enumerable.SelectMany(x => x);
        }
    }
}