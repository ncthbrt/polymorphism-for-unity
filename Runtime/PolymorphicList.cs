using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Polymorphism4Unity
{
    public interface IPolymorphicList<T> : IList<T>
    {
    }

    [Serializable]
    public abstract class PolymorphicList<T> : IPolymorphicList<T>
    {
        [SerializeReference]
        private readonly List<T> backingData = new();

        public T this[int index] { get => backingData[index]; set => backingData[index] = value; }

        public int Count => backingData.Count;

        public bool IsReadOnly => false;

        public void Add(T item) =>
            backingData.Add(item);

        public void Clear() =>
            backingData.Clear();

        public bool Contains(T item) =>
            backingData.Contains(item);

        public void CopyTo(T[] array, int arrayIndex) =>
            backingData.CopyTo(array, arrayIndex);

        public IEnumerator<T> GetEnumerator() =>
            backingData.GetEnumerator();


        public int IndexOf(T item) =>
            backingData.IndexOf(item);

        public void Insert(int index, T item) =>
            backingData.Insert(index, item);

        public bool Remove(T item) =>
            backingData.Remove(item);

        public void RemoveAt(int index) =>
            backingData.RemoveAt(index);

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();
    }
}