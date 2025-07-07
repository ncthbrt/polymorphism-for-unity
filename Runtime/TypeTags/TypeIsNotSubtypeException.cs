using System;

namespace Polymorphism4Unity.TypeTags
{
    public class TypeIsNotSubtypeException : ArgumentException
    {
        public Type NotSubtype { get; }
        public Type BaseType { get; }
        public TypeIsNotSubtypeException(Type notSubtype, Type baseType) : base($"{notSubtype.Name} is not a subtype of {baseType.Name}")
        {
            NotSubtype = notSubtype;
            BaseType = baseType;
        }
    }

    public class TypeIsNotSubtypeException<TBaseType> : TypeIsNotSubtypeException
    {
        public TypeIsNotSubtypeException(Type notSubtype) : base(notSubtype, typeof(TBaseType))
        {
        }
    }
}