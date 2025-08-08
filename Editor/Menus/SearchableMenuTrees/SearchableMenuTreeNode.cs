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
        public bool HasNext { get; }

        protected SearchableMenuTreeNode(string key, bool hasNext)
        {
            Key = key;
            HasNext = hasNext;
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
        public List<SearchableMenuTreeNode<T>> ChildNodes { get; }
            
        public SearchMenuTreeParentNode(string name, List<SearchableMenuTreeNode<T>> childNodes): base(name, true)
        {
            ChildNodes = childNodes;
        }
    }

    public class SearchableMenuTreeLeafNode<T> : SearchableMenuTreeNode<T>
    {
        public T Value { get; }

        public SearchableMenuTreeLeafNode(string name, T value, bool hasNext): base(name, hasNext)
        {
            Value = value;
        }
    }
}