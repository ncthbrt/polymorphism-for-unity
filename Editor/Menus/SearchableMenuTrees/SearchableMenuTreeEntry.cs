#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    [UxmlObject]
    public partial class SearchableMenuTreeEntry<T>
    {
        [UxmlAttribute]
        public List<string> SearchTerms { get; set; } = new();

        [UxmlAttribute]
        public string Path { get; set; } = String.Empty;
        
#nullable disable
        [UxmlAttribute]
        public T Value { get; set; }
#nullable enable
    }
}