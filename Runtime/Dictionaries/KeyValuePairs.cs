using System;
using UnityEngine;

namespace Polymorphism4Unity.Dictionaries
{
    [Serializable]
    public class PolymorphicKeyValuePair<TKey, TValue> : IKeyValuePair<TKey, TValue>
    {
        [SerializeReference]
        private TKey key;
        [SerializeReference]
        private TValue value;
        public TKey Key { get => key; set => key = value; }
        public TValue Value { get => value; set => this.value = value; }
    }

    [Serializable]
    public class DiscreteKeyPolymorphicValuePair<TKey, TValue> : IKeyValuePair<TKey, TValue>
    {
        [SerializeField]
        private TKey key;

        [SerializeReference]
        private TValue value;
        public TKey Key { get => key; set => key = value; }
        public TValue Value { get => value; set => this.value = value; }
    }

    [Serializable]
    public class PolymorphicKeyDiscreteValuePair<TKey, TValue> : IKeyValuePair<TKey, TValue>
    {
        [SerializeReference]
        private TKey key;

        [SerializeField]
        private TValue value;
        public TKey Key { get => key; set => key = value; }
        public TValue Value { get => value; set => this.value = value; }
    }
}