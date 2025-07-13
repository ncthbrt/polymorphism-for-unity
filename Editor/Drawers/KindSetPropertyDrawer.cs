#nullable enable
using UnityEditor;
using Polymorphism4Unity.KindSets;

namespace Polymorphism4Unity.Editor.Drawers
{
    [CustomPropertyDrawer(typeof(KindSet<>), useForChildren: true)]
    public class KindSetPropertyDrawer : PropertyDrawer
    {
    }
}
