#nullable enable
using UnityEditor;
using Polymorphism4Unity.Dictionaries;

namespace Polymorphism4Unity.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PolymorphicDictionary<,,>))]
    public class PolymorphicDictionaryPropertyDrawer : PropertyDrawer
    {
    }
}
