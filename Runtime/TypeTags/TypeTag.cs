#nullable enable
using System;
using Polymorphism4Unity.Safety;
using UnityEngine;

namespace Polymorphism4Unity.TypeTags
{
    public static class TypeTag
    {
        public static TypeTag<TBaseType> FromType<TBaseType>(Type subtype)
        {
            TypeTag<TBaseType> tag = new()
            {
                Type = subtype
            };
            return tag;
        }

        public static TypeTag<TBaseType> FromType<TBaseType, TSubtype>() where TSubtype : TBaseType =>
             FromType<TBaseType>(typeof(TSubtype));
    }

    [Serializable]
    public class TypeTag<TBaseType> : ISerializationCallbackReceiver, IEquatable<TypeTag<TBaseType>>
    {
        [SerializeField]
        private string assemblyQualifiedName = string.Empty;
        private Type? _type;

        public Type? Type
        {
            get => _type;
            set
            {
                if (value is null)
                {
                    _type = null;
                }
                if (!typeof(TBaseType).IsAssignableFrom(value))
                {
                    throw new TypeIsNotSubtypeException<TBaseType>(value);
                }
                _type = value;
            }
        }

        public void Set<T>()
            where T : TBaseType
        {
            _type = typeof(T);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            assemblyQualifiedName = 
                _type is null 
                    ? string.Empty 
                    : Asserts.IsNotNullOrEmpty(_type.AssemblyQualifiedName);
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(assemblyQualifiedName))
            {
                _type = null;
            }
            else
            {
                Type = Type.GetType(assemblyQualifiedName, false);
            }
        }

        public bool Equals(TypeTag<TBaseType>? other) =>
            other is not null
            && (ReferenceEquals(this, other) || (
                Equals(GetType(), other.GetType())
                && Equals(Type, other.Type)
            ));

        public override bool Equals(object? other) =>
            Equals(other as TypeTag<TBaseType>);

        public override int GetHashCode() =>
            HashCode.Combine(Type);

        public void Clear() => _type = null;
    }
}