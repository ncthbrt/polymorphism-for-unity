#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Polymorphism4Unity.Lists
{
    [Serializable]
    public class PolymorphicList<T> : List<T>, ISerializationCallbackReceiver
    {
        [SerializeReference]
        private T[] backingData = new T[0];

        public void OnAfterDeserialize()
        {
            AddRange(backingData);
            backingData = new T[0];
        }

        public void OnBeforeSerialize()
        {
            backingData = new T[Count];
            ((ICollection<T>)this).CopyTo(backingData, 0);
        }
    }
}