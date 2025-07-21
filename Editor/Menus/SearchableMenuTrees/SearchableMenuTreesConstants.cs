using System.Text.RegularExpressions;
using Polymorphism4Unity.Safety;
using UnityEditor;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    internal static class SearchableMenuTreesConstants
    {
        private const string FolderPath = Constants.AssemblyPath + nameof(Menus) + "/" + nameof(SearchableMenuTrees) + "/";
        private const string StylePath = FolderPath + "SearchableMenuTreeStyles.uss";
        private static readonly StyleSheet StackStyleSheet;
        
        public static readonly Regex PathRegex = new(@"^(?:(?<part>(?:\\\/|[^\/])*)\/)*(?<part>[^\/]+)$");

        
        static SearchableMenuTreesConstants()
        {
            StackStyleSheet = Asserts.IsNotNull(AssetDatabase.LoadAssetAtPath<StyleSheet>(StylePath));
        }

        public static void AddSearchableMenuTreeStyles(this VisualElement element)
        {
            element.styleSheets.Add(StackStyleSheet);
        }
    }
}