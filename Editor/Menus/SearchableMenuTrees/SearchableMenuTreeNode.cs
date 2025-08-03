#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using Polymorphism4Unity.Editor.Styling;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    public abstract class SearchableMenuTreeNode<T>
    {
        public string Key { get; }
      
        protected SearchableMenuTreeNode(string key)
        {
            Key = key;
            // _nextIcon.style.ApplyStyles(new CompactStyle
            // {
            //     backgroundImage = EditorGUIUtility.IconContent("Arrownavigationright@2x").image as Texture2D,
            //     height = 15,
            //     width = 15,
            //     margin = 0,
            //     padding = 0,
            //     borderWidth = 0,
            //     alignSelf = Align.Center,
            //     backgroundPosition = new BackgroundPosition(BackgroundPositionKeyword.Center)
            // });
            // style.ApplyStyles(new CompactStyle
            // {
            //     justifyContent = Justify.SpaceBetween
            // });
            // ShowNextIcon = showNextIcon;
        }

        public int CompareTo(SearchableMenuTreeNode<T> other)
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;

            switch (this, other)
            {
                case ({ } thisNode, {} otherNode) when thisNode.GetType() == otherNode.GetType():
                    return currentCulture.CompareInfo.Compare(thisNode.Key, otherNode.Key);
                case (SearchableMenuTreeLeafNode<T>, _):
                    return 1;
                case (_, SearchMenuTreeParentNode<T>):
                    return -1;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }  
    }
    
    public class SearchMenuTreeParentNode<T>: SearchableMenuTreeNode<T>
    {
        public List<SearchableMenuTreeNode<T>> ChildNodes { get; } = new();
            
        public SearchMenuTreeParentNode(string name): base(name)
        {
        }
    }     

    public class SearchableMenuTreeLeafNode<T> : SearchableMenuTreeNode<T>
    {
        public T Value { get; set; }

        public SearchableMenuTreeLeafNode(string name, T value): base(name)
        {
            Value = value;
        }
    }
}