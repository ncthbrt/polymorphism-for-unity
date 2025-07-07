#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polymorphism4Unity
{
    [Serializable]
    public class PolymorphicDictionary<TKey, TValue, TKeyValuePair> : Dictionary<TKey?, TValue?>, ISerializationCallbackReceiver
        where TKeyValuePair : IKeyValuePair<TKey, TValue>, new()
    {
        [SerializeField]
        private TKeyValuePair[] backingData = new TKeyValuePair[0];

        public void OnAfterDeserialize()
        {
            Clear();
            int length = backingData.Length;
            for (int i = 0; i < length; ++i)
            {
                TKeyValuePair entry = backingData[i];
                this[entry.Key] = entry.Value;
            }
            backingData = new TKeyValuePair[0];
        }

        public void OnBeforeSerialize()
        {
            int count = Count;
            backingData = new TKeyValuePair[Count];
            IEnumerator<KeyValuePair<TKey, TValue>> enumerator = GetEnumerator();
            for (int i = 0; i < count; ++i, enumerator.MoveNext())
            {
                (TKey key, TValue value) = enumerator.Current;
                TKeyValuePair keyValuePair = new()
                {
                    Key = key,
                    Value = value
                };
                backingData[i] = keyValuePair;
            }
        }
    }

    public class PolymorphicDictionary<TKey, TValue> : PolymorphicDictionary<TKey, TValue, PolymorphicKeyValuePair<TKey, TValue>>
    {
    }
}