using System.Text.RegularExpressions;
using Polymorphism4Unity.Safety;
using UnityEditor;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    internal static class SearchableMenuTreesConstants
    {
        public static readonly Regex PathRegex = new(@"^(?:(?<part>(?:\\\/|[^\/])*)\/)*(?<part>[^\/]+)$");
    }
}