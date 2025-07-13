#nullable enable
using UnityEditor;
using Polymorphism4Unity.Lists;

namespace Polymorphism4Unity.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(PolymorphicList<>))]
    public class PolymorphicListPropertyDrawer : PropertyDrawer
    {
    }
}
