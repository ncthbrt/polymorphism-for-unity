#nullable enable
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Events
{
    public class ItemSelectedEvent<T>: EventBase<ItemSelectedEvent<T>>
    {
        public T? Value { get; set; }

        public static ItemSelectedEvent<T> GetPooled(T? value)
        {
            ItemSelectedEvent<T> itemSelectedEvent = GetPooled();
            itemSelectedEvent.Value = value;
            return itemSelectedEvent;
        }
    }
}