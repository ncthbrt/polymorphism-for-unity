#nullable enable
using System;
using System.Linq;
namespace Polymorphism4Unity.Editor.Utils
{
    internal static class FuncUtils
    {
        public static Func<TResult> F<TResult>(Func<TResult> func) => func;
        public static Func<TA, TResult> F<TA, TResult>(Func<TA, TResult> func) => func;
        public static Func<TA, TB, TResult> F<TA, TB, TResult>(Func<TA, TB, TResult> func) => func;
        public static Func<TA, TB, TC, TResult> F<TA, TB, TC, TResult>(Func<TA, TB, TC, TResult> func) => func;
        public static Func<TA, TB, TC, TD, TResult> F<TA, TB, TC, TD, TResult>(Func<TA, TB, TC, TD, TResult> func) => func;
        public static Func<TA, TB, TC, TD, TE, TResult> F<TA, TB, TC, TD, TE, TResult>(Func<TA, TB, TC, TD, TE, TResult> func) => func;
        public static Func<TA, TB, TC, TD, TE, TF, TResult> F<TA, TB, TC, TD, TE, TF, TResult>(Func<TA, TB, TC, TD, TE, TF, TResult> func) => func;
        public static Func<TA, TB, TC, TD, TE, TF, TG, TResult> F<TA, TB, TC, TD, TE, TF, TG, TResult>(Func<TA, TB, TC, TD, TE, TF, TG, TResult> func) => func;

        public static Func<TA, TC> Then<TA, TB, TC>(this Func<TA, TB> a, Func<TB, TC> b) =>
            value => b(a(value));

        public static Func<T, bool> And<T>(Func<T, bool> term1, Func<T, bool> term2, params Func<T, bool>[] terms) =>
            value => term1(value) && term2(value) && terms.Apply(value).All();

        public static Func<T, bool> And<T>(this Func<T, bool> term1, Func<T, bool> term2) =>
            value => term1(value) && term2(value);

        public static Func<T, bool> Or<T>(Func<T, bool> term1, Func<T, bool> term2, params Func<T, bool>[] terms) =>
            value => term1(value) || term2(value) || terms.Apply(value).Any();
        public static Func<T, bool> Or<T>(this Func<T, bool> term1, Func<T, bool> term2) =>
            value => term1(value) || term2(value);

        public static Func<T, bool> Xor<T>(this Func<T, bool> term1, Func<T, bool> term2) =>
            value =>
            {
                bool a = term1(value);
                bool b = term2(value);
                return (a != b) && (a || b);
            };

        public static Func<T, bool> Xor<T>(Func<T, bool> term1, Func<T, bool> term2, params Func<T, bool>[] terms) =>
            value => terms.Prepend(term2).Prepend(term1).Apply(value).Where(x => x).Take(2).Count() == 1;


        public static Func<T, bool> Not<T>(this Func<T, bool> a) => value => !a(value);

        public static Func<TResult> Apply<TResult, TIn1>(
            Func<TIn1, TResult> a,
            TIn1 value1
        ) => () => a(value1);

        public static Func<TIn2, TResult> Apply<TResult, TIn1, TIn2>(
            Func<TIn1, TIn2, TResult> a,
            TIn1 value1
        ) => value2 => a(value1, value2);

        public static Func<TIn2, TIn3, TResult> Apply<TResult, TIn1, TIn2, TIn3>(
            Func<TIn1, TIn2, TIn3, TResult> a,
            TIn1 value1
        ) => (value2, value3) => a(value1, value2, value3);

        public static Func<TIn2, TIn3, TIn4, TResult> Apply<TResult, TIn1, TIn2, TIn3, TIn4>(
            Func<TIn1, TIn2, TIn3, TIn4, TResult> a,
            TIn1 value1
        ) => (value2, value3, value4) => a(value1, value2, value3, value4);


        public static Func<TResult> Apply<TResult, TIn1, TIn2>(
            Func<TIn1, TIn2, TResult> a,
            TIn1 value1, TIn2 value2
        ) => () => a(value1, value2);

        public static Func<TIn3, TResult> Apply<TResult, TIn1, TIn2, TIn3>(
            Func<TIn1, TIn2, TIn3, TResult> a,
            TIn1 value1, TIn2 value2
        ) => (value3) => a(value1, value2, value3);

        public static Func<TIn3, TIn4, TResult> Apply<TResult, TIn1, TIn2, TIn3, TIn4>(
            Func<TIn1, TIn2, TIn3, TIn4, TResult> a,
            TIn1 value1, TIn2 value2
        ) => (value3, value4) => a(value1, value2, value3, value4);

        public static Func<TResult> Apply<TResult, TIn1, TIn2, TIn3>(
            Func<TIn1, TIn2, TIn3, TResult> a,
            TIn1 value1, TIn2 value2, TIn3 value3
        ) => () => a(value1, value2, value3);

        public static Func<TIn4, TResult> Apply<TResult, TIn1, TIn2, TIn3, TIn4>(
            Func<TIn1, TIn2, TIn3, TIn4, TResult> a,
            TIn1 value1, TIn2 value2, TIn3 value3
        ) => (value4) => a(value1, value2, value3, value4);

        public static Func<TResult> Apply<TResult, TIn1, TIn2, TIn3, TIn4>(
            Func<TIn1, TIn2, TIn3, TIn4, TResult> a,
            TIn1 value1, TIn2 value2, TIn3 value3, TIn4 value4
        ) => () => a(value1, value2, value3, value4);

        public static Func<TIn2, TIn1, TResult> Swivel<TIn1, TIn2, TResult>(
            Func<TIn1, TIn2, TResult> a
        ) => (value2, value1) => a(value1, value2);


        public static Comparison<TIn> ToComparison<TIn, TCompare>(this Func<TIn, TCompare> selector)
            where TCompare : IComparable<TCompare> =>
                (input1, input2) => selector(input1).CompareTo(selector(input2));

        public static Comparison<TIn> ToComparison<TIn, TCompare>(Func<TIn, TCompare> selector1, Func<TIn, TCompare> selector2)
                    where TCompare : IComparable<TCompare> =>
                        (input1, input2) => selector1(input1).CompareTo(selector2(input2));

    }
}