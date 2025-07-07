#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private string[] backingDataAssemblyQualifiedTypeNames = new string[0];
        private readonly HashSet<Type> values = new();

        public int Count => throw new NotImplementedException();

        public bool IsReadOnly => throw new NotImplementedException();

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
                    values.Add(maybeType!);
                }
            }
            backingDataAssemblyQualifiedTypeNames = new string[0];
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            int count = values.Count;
            backingDataAssemblyQualifiedTypeNames = new string[count];
            int i = 0;
            foreach (Type type in values)
            {
                backingDataAssemblyQualifiedTypeNames[i] = type.AssemblyQualifiedName;
                ++i;
            }
        }

        public bool Add(Type type)
        {
            if (!typeof(TBaseType).IsAssignableFrom(type))
            {
                throw new TypeIsNotSubtypeException<TBaseType>(type);
            }

            return values.Add(type);
        }

        public bool Add<T>() where T : TBaseType =>
            Add(typeof(T));

        public void ExceptWith(IEnumerable<Type> other) =>
            values.ExceptWith(other);

        public void IntersectWith(IEnumerable<Type> other) =>
            values.IntersectWith(other);

        public bool IsProperSubsetOf(IEnumerable<Type> other) =>
            values.IsProperSubsetOf(other);

        public bool IsProperSupersetOf(IEnumerable<Type> other) =>
            values.IsProperSubsetOf(other);

        public bool IsSubsetOf(IEnumerable<Type> other) =>
            values.IsSubsetOf(other);

        public bool IsSupersetOf(IEnumerable<Type> other) =>
            values.IsSubsetOf(other);

        public bool Overlaps(IEnumerable<Type> other) =>
            values.Overlaps(other);

        public bool SetEquals(IEnumerable<Type> other) =>
            values.SetEquals(other);

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
            values.SymmetricExceptWith(otherArr);
        }

        public void UnionWith(IEnumerable<Type> other)
        {
            foreach (Type otherType in other)
            {
                if (!typeof(TBaseType).IsAssignableFrom(otherType))
                {
                    throw new TypeIsNotSubtypeException<TBaseType>(otherType);
                }
                values.Add(otherType);
            }
        }

        void ICollection<Type>.Add(Type item) =>
            Add(item);

        public void Clear() =>
            values.Clear();

        public bool Contains(Type item) =>
            values.Contains(item);

        public bool Contains<T>() where T : TBaseType =>
            values.Contains(typeof(T));

        public void CopyTo(Type[] array, int arrayIndex) =>
            values.CopyTo(array, arrayIndex);

        public bool Remove(Type item) =>
            values.Remove(item);

        public bool Remove<T>(Type item) where T : TBaseType =>
            values.Remove(item);

        public IEnumerator<Type> GetEnumerator() =>
            values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public bool Equals(TypeTagSet<TBaseType>? other) =>
            ReferenceEquals(this, other)
            || (
                other is not null
                && Equals(GetType(), other.GetType())
                && Equals(values, other.values)
            );

        public override bool Equals(object? other) =>
            Equals(other as TypeTagSet<TBaseType>);

        public override int GetHashCode() =>
            HashCode.Combine(values);
    }
}