#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Polymorphism4Unity.Safety;
using UnityEngine;

namespace Polymorphism4Unity.TypeTags
{
    public static class TypeTagSet
    {
        public static TypeTagSet<T> FromTypeTags<T>(IEnumerable<TypeTag<T>> typeTags)
        {
            TypeTagSet<T> set = new();
            foreach (TypeTag<T> typeTag in typeTags)
            {
                if (typeTag.Type is { } notNullType)
                {
                    set.Add(notNullType);
                }
            }
            return set;
        }
    }

    [Serializable]
    public class TypeTagSet<TBaseType> : ISet<Type>, IEquatable<TypeTagSet<TBaseType>>, ISerializationCallbackReceiver
    {
        [SerializeField]
        private string[] backingDataAssemblyQualifiedTypeNames = Array.Empty<string>();
        private readonly HashSet<Type> _values = new();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            for (int i = 0; i < backingDataAssemblyQualifiedTypeNames.Length; ++i)
            {
                string assemblyQualifiedName = backingDataAssemblyQualifiedTypeNames[i];
                if (!string.IsNullOrEmpty(assemblyQualifiedName))
                {
                    Type? maybeType = Type.GetType(assemblyQualifiedName, false);
                    if (maybeType is null)
                    {
                        continue;
                    }
                    if (!typeof(TBaseType).IsAssignableFrom(maybeType))
                    {
                        continue;
                    }
                    _values.Add(maybeType);
                }
            }
            backingDataAssemblyQualifiedTypeNames = Array.Empty<string>();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            int count = _values.Count;
            backingDataAssemblyQualifiedTypeNames = new string[count];
            int i = 0;
            foreach (Type type in this)
            {
                backingDataAssemblyQualifiedTypeNames[i] = Asserts.IsNotNullOrEmpty(type.AssemblyQualifiedName);
                ++i;
            }
        }

        public bool Add(Type type)
        {
            if (!typeof(TBaseType).IsAssignableFrom(type))
            {
                throw new TypeIsNotSubtypeException<TBaseType>(type);
            }

            return _values.Add(type);
        }

        public bool Add<T>() where T : TBaseType =>
            Add(typeof(T));

        public void ExceptWith(IEnumerable<Type> other) =>
            _values.ExceptWith(other);

        public void IntersectWith(IEnumerable<Type> other) =>
            _values.IntersectWith(other);

        public bool IsProperSubsetOf(IEnumerable<Type> other) =>
            _values.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<Type> other) =>
            _values.IsProperSubsetOf(other);

        public bool IsSubsetOf(IEnumerable<Type> other) =>
            _values.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<Type> other) =>
            _values.IsSubsetOf(other);

        public bool Overlaps(IEnumerable<Type> other) =>
            _values.Overlaps(other);

        public bool SetEquals(IEnumerable<Type> other) =>
            _values.SetEquals(other);

        public void SymmetricExceptWith(IEnumerable<Type> other)
        {
            Type[] otherArr = other.ToArray();
            int otherLength = otherArr.Length;
            for (int i = 0; i < otherLength; ++i)
            {
                Type otherType = otherArr[i];
                if (!typeof(TBaseType).IsAssignableFrom(otherType))
                {
                    throw new TypeIsNotSubtypeException<TBaseType>(otherType);
                }
            }
            _values.SymmetricExceptWith(otherArr);
        }

        public void UnionWith(IEnumerable<Type> other)
        {
            foreach (Type otherType in other)
            {
                if (!typeof(TBaseType).IsAssignableFrom(otherType))
                {
                    throw new TypeIsNotSubtypeException<TBaseType>(otherType);
                }
                _values.Add(otherType);
            }
        }

        void ICollection<Type>.Add(Type item) =>
            Add(item);

        public void Clear() =>
            _values.Clear();

        public bool Contains(Type item) =>
            _values.Contains(item);

        public bool Contains<T>() where T : TBaseType =>
            _values.Contains(typeof(T));

        public void CopyTo(Type[] array, int arrayIndex) =>
            _values.CopyTo(array, arrayIndex);

        public bool Remove(Type item) =>
            _values.Remove(item);

        public bool Remove<T>() where T : TBaseType =>
            _values.Remove(typeof(T));

        public IEnumerator<Type> GetEnumerator() =>
            _values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public bool Equals(TypeTagSet<TBaseType>? other) =>
            ReferenceEquals(this, other)
            || (
                other is not null
                && GetType() == other.GetType()
                && Equals(_values, other._values)
            );

        public override bool Equals(object? other) =>
            Equals(other as TypeTagSet<TBaseType>);

        public override int GetHashCode() =>
            HashCode.Combine(_values);

        public int Count => _values.Count;

        public bool IsReadOnly => false;
    }
}