using Polymorphism4Unity.Safety;
using UnityEditor;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Containers.Stacks
{
    internal static class StacksConstants
    {
        public const string FolderPath = Constants.AssemblyPath + nameof(Containers) + "/" + nameof(Stacks) + "/";
        private const string StylePath = FolderPath + "StackStyles.uss";
        public static readonly StyleSheet StackStyleSheet;
            
        static StacksConstants()
        {
            StackStyleSheet = Asserts.IsNotNull(AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath));
        }

        public static void AddStackStyles(this VisualElement element)
        {
            element.styleSheets.Add(StackStyleSheet);
        }
    }
}