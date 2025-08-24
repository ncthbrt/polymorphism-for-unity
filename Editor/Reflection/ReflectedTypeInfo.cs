#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework.Internal;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;

namespace Polymorphism4Unity.Editor.Reflection
{
    internal abstract class ReflectedTypeInfo
    {
        // public Type Type { get; }
        // public List<ReflectedTypeInfo> TypeParams { get; }  
        //
        // public ReflectedTypeInfo(Type type)
        // {
        //     Type = type;
        //     TypeParams = new List<ReflectedTypeInfo>();
        //     if(Type.Is)
        // }
    }

    public class ReflectedGenericParameter
    {
        public Type BaseType { get; }
        public IReadOnlyList<Type> TypeConstraints { get; }
        
        public bool HasReferenceTypeConstraint => GenericParameterAttributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint);
        public bool HasDefaultConstructorConstraint => GenericParameterAttributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint);
        public bool HasNotNullableValueTypeConstraint => GenericParameterAttributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint);
        public bool IsCovariant => GenericParameterAttributes.HasFlag(GenericParameterAttributes.Covariant);
        
        public GenericParameterAttributes GenericParameterAttributes { get; }
        
        public ReflectedGenericParameter(Type type)
        {
            Asserts.IsTrue(type.IsGenericTypeParameter);
            BaseType = type;
            TypeConstraints = BaseType.GetGenericParameterConstraints().ToList();
            GenericParameterAttributes = BaseType.GenericParameterAttributes;
            
        }

        public bool TypeQualifies(Type otherType)
        {
            // This method only accepts
            Asserts.IsTrue(!otherType.IsGenericType || otherType.IsConstructedGenericType);
            
            if (BaseType != typeof(object) && !otherType.Is(BaseType))
            {
                return false;
            }
            
            if (TypeConstraints.Any(x => !otherType.Is(x)))
            {
                return false;
            }
            
            if (HasReferenceTypeConstraint && !otherType.IsClass)
            {
                return false;
            }

            if (HasNotNullableValueTypeConstraint && !otherType.IsValueType)
            {
                return false;
            }

            if (HasDefaultConstructorConstraint && !otherType.HasDefaultPublicConstructor())
            {
                return false;
            }
            
            return true;
        }
    }
}