#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Polymorphism4Unity.Editor.Containers.Stacks;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using StackFrame = Polymorphism4Unity.Editor.Containers.Stacks.StackFrame;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    [UxmlObject]
    public partial class SearchableMenuTreeItem<T>
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
    
    public abstract class SearchableMenuTree<T>: VisualElement
        where T: class
    {
        abstract class Node: TextElement, IComparable<Node>
        {
            public sealed override string text
            {
                get => ((INotifyValueChanged<string>)this).value;
                set => ((INotifyValueChanged<string>)this).value = value;
            }

            protected Node(string text)
            {
                this.text = text;
                this.AddSearchableMenuTreeStyles();
            }

            public int CompareTo(Node other)
            {
                CultureInfo currentCulture = CultureInfo.CurrentCulture;

                switch (this, other)
                {
                    case ({ } thisNode, {} otherNode) when thisNode.GetType() == otherNode.GetType():
                        return currentCulture.CompareInfo.Compare(thisNode.text, otherNode.text);    
                    case (LeafNode thisLeafNode, _):
                        return 1;
                    case (_, LeafNode otherLeafNode):
                        return -1;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        
        class ParentNode: Node
        {
            public List<Node> ChildNodes { get; } = new();
            
            public ParentNode(string name): base(name)
            {
            }
        }

        class LeafNode: Node
        {
            public T Value { get; }

            public LeafNode(string name, T value): base(name)
            {
                Value = value;
            }
        }
        
        
        private RegistrationSet? _registrationSet;
        private readonly Stack _stack;
        private readonly StackFrame _initialStackFrame;
        
        protected abstract IEnumerable<SearchableMenuTreeItem<T>> Items { get; }
        
        [UxmlAttribute]
        public string StackId
        {
            get => _stack.StackId;
            set => _stack.StackId = value;
        }

        [UxmlAttribute]
        public string HeaderText
        {
            get => _initialStackFrame.HeaderText;
            set => _initialStackFrame.HeaderText = value;
        }
        
        protected SearchableMenuTree()
        {
            this.AddSearchableMenuTreeStyles();
            AddToClassList("poly-searchable-menu-tree__root");
            _stack = new Stack();
            _initialStackFrame = new StackFrame();
            _stack.Add(_initialStackFrame);
            Add(_stack);
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanelEvent);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanelEvent);
        }

        protected virtual void HandleAttachToPanelEvent(AttachToPanelEvent _)
        {
            Asserts.IsNull(_registrationSet);
            _registrationSet = new RegistrationSet(this);
            RefreshItems();
            List<Node> treeRoots = ConstructTree(Items);
            _initialStackFrame.AddRange(treeRoots);
        }
        

        protected virtual void HandleDetachFromPanelEvent(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;
            _stack.ClearAll();
        }

        protected virtual void RefreshItems()
        {
        }

        private static List<Node> CollapseParentNodesWithSingleItems(List<Node> result)
        {

            return result;
        }
        
        private static List<Node> ConstructTree(IEnumerable<SearchableMenuTreeItem<T>> items)
        {
            List<Node> result = new();
            Dictionary<string, Node> intermediate = new();
            foreach (SearchableMenuTreeItem<T> item in items)
            {
                Match match = SearchableMenuTreesConstants.PathRegex.Match(item.Path);
                if (!match.Success)
                {
                    Debug.LogError($"Invalid Path String {item.Path}");
                    continue;
                }

                Group matchGroup = match.Groups["part"];
                string[] parts = matchGroup.Captures.Select(x => x.Value).ToArray();
                parts = Asserts.IsNotNullOrEmpty(parts);
                if (parts.Length == 1)
                {
                    result.Add(new LeafNode(parts[0],item.Value));
                    continue;
                }
                string head = parts[0];
                if (!intermediate.TryGetValue(head, out Node? prevNode))
                {
                    intermediate.Add();
                }
                ParentNode parent = new ParentNode(head);
                for (int i = 1; i < parts.Length; ++i)
                {
                    string part = parts[i];
                    
                }
            }
            result.Sort();
            return  CollapseParentNodesWithSingleItems(result);
        }

        protected virtual Toolbar CreateSearchToolbar()
        {
            Toolbar toolbar = new()
            {
                name = "SearchMenu"
            };
            
            return toolbar;
        }   
    }
}