#nullable enable
using System;
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
        private Type? type = null;

        public Type? Type
        {
            get => type;
            set
            {
                if (value is null)
                {
                    type = null;
                }
                if (!typeof(TBaseType).IsAssignableFrom(value))
                {
                    throw new TypeIsNotSubtypeException<TBaseType>(value);
                }
                type = value;
            }
        }

        public void Set<T>()
            where T : TBaseType
        {
            type = typeof(T);
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (type is null)
            {
                assemblyQualifiedName = string.Empty;
            }
            else
            {
                assemblyQualifiedName = type.AssemblyQualifiedName;
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (string.IsNullOrEmpty(assemblyQualifiedName))
            {
                type = null;
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

        public void Clear() => type = null;
    }
}