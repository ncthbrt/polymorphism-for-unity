#nullable enable
namespace Polymorphism4Unity.Editor.Utils
{
    internal static class TupleUtils
    {
        public static TA First<TA, TB>(this (TA a, TB b) value) => value.a;
        public static TA First<TA, TB, TC>(this (TA a, TB b, TC c) value) => value.a;
        public static TA First<TA, TB, TC, TD>(this (TA a, TB b, TC c, TD d) value) => value.a;
        public static TA First<TA, TB, TC, TD, TE>(this (TA a, TB b, TC c, TD d, TE e) value) => value.a;
        public static TA First<TA, TB, TC, TD, TE, TF>(this (TA a, TB b, TC c, TD d, TE e, TF f) value) => value.a;
        public static TA Second<TA, TB>(this (TA a, TB b) value) => value.a;
        public static TB Second<TA, TB, TC>(this (TA a, TB b, TC c) value) => value.b;
        public static TB Second<TA, TB, TC, TD>(this (TA a, TB b, TC c, TD d) value) => value.b;
        public static TB Second<TA, TB, TC, TD, TE>(this (TA a, TB b, TC c, TD d, TE e) value) => value.b;
        public static TB Second<TA, TB, TC, TD, TE, TF>(this (TA a, TB b, TC c, TD d, TE e, TF f) value) => value.b;

        public static TB Rest<TA, TB>((TA a, TB b) value) => value.b;
        public static (TB, TC) Rest<TA, TB, TC>((TA a, TB b, TC c) value) => (value.b, value.c);
        public static (TB, TC, TD) Rest<TA, TB, TC, TD>((TA a, TB b, TC c, TD d) value) => (value.b, value.c, value.d);
        public static (TB, TC, TD, TE) Rest<TA, TB, TC, TD, TE>((TA a, TB b, TC c, TD d, TE e) value) => (value.b, value.c, value.d, value.e);
        public static (TB, TC, TD, TE, TF) Rest<TA, TB, TC, TD, TE, TF>((TA a, TB b, TC c, TD d, TE e, TF f) value) => (value.b, value.c, value.d, value.e, value.f);
    }
}