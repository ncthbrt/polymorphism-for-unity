#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Polymorphism4Unity.Safety;
using UnityEngine;

namespace Polymorphism4Unity.KindSets
{

    [Serializable, PublicAPI]
    public class KindSet<TBaseKind> : ICollection<TBaseKind>, ISerializationCallbackReceiver
    {
        [SerializeReference]
        private TBaseKind[] backingValues = Array.Empty<TBaseKind>();
        private readonly Dictionary<Type, TBaseKind> _values = new();
        public int Count => _values.Count;
        public bool IsReadOnly => false;
        public ICollection<Type> Types => _values.Keys;
        public ICollection<TBaseKind> Values => _values.Values;

        public TBaseKind this[Type t] => _values[t];

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            _values.Clear();
            for (int i = 0; i < backingValues.Length; ++i)
            {
                TBaseKind item = Asserts.IsNotNull(backingValues[i])!;
                Type itemKind = item.GetType();
                Asserts.IsFalse(_values.ContainsKey(itemKind));
                _values[itemKind] = item;
            }
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            backingValues = new TBaseKind[_values.Count];
            CopyTo(backingValues, 0);
        }

        public bool Add(TBaseKind item)
        {
            if (item is null)
            {
                return false;
            }
            Type itemKind = item.GetType();
            return _values.TryAdd(itemKind, item);
        }

        public bool Remove(Type kind) =>
            _values.Remove(kind);

        public bool Remove<TSubkind>()
            where TSubkind : TBaseKind =>
            Remove(typeof(TSubkind));

        public IEnumerator<TBaseKind> GetEnumerator() =>
            _values.Values.Cast<TBaseKind>().GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() =>
            GetEnumerator();

        public void Clear()
        {
            _values.Clear();
            backingValues = Array.Empty<TBaseKind>();
        }

        public bool ContainsType(Type subkind) =>
            _values.ContainsKey(subkind);

        public void CopyTo(TBaseKind[] destination, int destinationStartIndex)
        {
            ICollection<TBaseKind> values = _values.Values;
            values.CopyTo(destination, destinationStartIndex);
        }

        void ICollection<TBaseKind>.Add(TBaseKind item)
        {
            if (Add(item))
            {
                return;
            }
            throw new ArgumentException($"An item with the same kind already exists in this {nameof(KindSet<TBaseKind>)}");
        }

        public bool Contains(TBaseKind item)
        {
            if (item is null)
            {
                return false;
            }
            Type itemKind = item.GetType();
            return Equals(_values[itemKind], item);
        }

        public bool Remove(TBaseKind item)
        {
            if (item is null || !Contains(item))
            {
                return false;
            }
            _values.Remove(item.GetType());
            return true;
        }
    }
}