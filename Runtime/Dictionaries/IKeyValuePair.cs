#nullable enable
namespace Polymorphism4Unity.Dictionaries
{
    public interface IKeyValuePair<TKey, TValue>
    {
        TKey? Key { get; set; }
        TValue? Value { get; set; }
    }
}