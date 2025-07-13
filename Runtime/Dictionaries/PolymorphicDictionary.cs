#nullable enable
using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Polymorphism4Unity.Dictionaries
{
    [Serializable,  PublicAPI]
    public class PolymorphicDictionary<TKey, TValue, TKeyValuePair> : Dictionary<TKey?, TValue?>, ISerializationCallbackReceiver
        where TKeyValuePair : IKeyValuePair<TKey, TValue>, new()
    {
        [SerializeField]
        private TKeyValuePair?[] backingData = Array.Empty<TKeyValuePair>();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            Clear();
            int length = backingData.Length;
            for (int i = 0; i < length; ++i)
            {
                if (backingData[i] is {  Key: not null } entry)
                {
                    this[entry.Key] = entry.Value;    
                }
            }
            backingData = Array.Empty<TKeyValuePair>();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
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
    
    [PublicAPI]
    public class PolymorphicDictionary<TKey, TValue> : PolymorphicDictionary<TKey, TValue, PolymorphicKeyValuePair<TKey, TValue>>
    {
    }
}