#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using Polymorphism4Unity.Editor.Commands;
using Polymorphism4Unity.Editor.Containers.Stacks;
using Polymorphism4Unity.Editor.Styling;
using Polymorphism4Unity.Editor.Utils;
using Polymorphism4Unity.Safety;
using Raffinert.FuzzySharp.Extractor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using FuzzSearch = Raffinert.FuzzySharp.Process;

namespace Polymorphism4Unity.Editor.Menus.SearchableMenuTrees
{
    [UxmlObject]
    public partial class SearchableMenuTreeItem<T>
    {
        [UxmlAttribute]
        public List<string> SearchTerms { get; set; } = new();

        [UxmlAttribute]
        public string Path { get; set; } = String.Empty;
        
        public bool ShowNextIcon { get; set; }

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

            private bool _showNextIcon = false;
            
            protected bool ShowNextIcon
            {
                get => _showNextIcon;
                set
                {
                    bool prev = _showNextIcon;
                    _showNextIcon = value;
                    if (prev != _showNextIcon)
                    {
                        if (_showNextIcon)
                        {
                            Add(_nextIcon);
                        }
                        else
                        {
                            Remove(_nextIcon);
                        }
                    }
                }
            }

            private readonly VisualElement _nextIcon;
            protected Node(string text, bool showNextIcon)
            {
                base.text = text;
                _nextIcon = new VisualElement();
                _nextIcon.style.ApplyStyles(new CompactStyle()
                {
                    backgroundImage = EditorGUIUtility.IconContent("Arrownavigationright@2x").image as Texture2D,
                    height = 15,
                    width = 15,
                    margin = 0,
                    padding = 0,
                    borderWidth = 0,
                    alignSelf = Align.Center,
                    backgroundPosition = new BackgroundPosition(BackgroundPositionKeyword.Center)
                });
                style.ApplyStyles(new CompactStyle
                {
                    justifyContent = Justify.SpaceBetween
                });
                ShowNextIcon = showNextIcon;
            }

            public int CompareTo(Node other)
            {
                CultureInfo currentCulture = CultureInfo.CurrentCulture;

                switch (this, other)
                {
                    case ({ } thisNode, {} otherNode) when thisNode.GetType() == otherNode.GetType():
                        return currentCulture.CompareInfo.Compare(thisNode.text, otherNode.text);
                    case (LeafNode, _):
                        return 1;
                    case (_, LeafNode):
                        return -1;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        
        class ParentNode: Node
        {
            public Dictionary<string, List<Node>> ChildNodes { get; set; } = new();
            
            public ParentNode(string name): base(name, true)
            {
            }
        }

        class LeafNode: Node, IHasReadOnlyValue<T>
        {
            public T Value { get; set; }

            public LeafNode(string name, T value, bool showNext): base(name, showNext)
            {
                Value = value;
                RegisterCallback<AttachToPanelEvent>(HandleAttachToPanel);
                RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanel);
            }

            void HandleAttachToPanel(AttachToPanelEvent _)
            {   
            }
            
            void HandleDetachFromPanel(DetachFromPanelEvent _)
            {
                
            }
        }
        

        class IndexItem
        {
            public string SearchTerm { get; }
            public SearchableMenuTreeItem<T>? Item { get; }
            
            public IndexItem(string searchTerm, SearchableMenuTreeItem<T>? item = null)
            {
                SearchTerm = searchTerm;
                Item = item;
            }
        }
        
        
        private RegistrationSet? _registrationSet;
        private readonly StackView _stack;
        private readonly StackFrameElement _initialStackFrame;
        private IndexItem[]? _searchIndex = null;
        protected abstract IEnumerable<SearchableMenuTreeItem<T>> Items { get; }

        public Action<T?> OnSelected { get; set; } = (_) => { };
        
        [UxmlAttribute]
        public string HeaderText
        {
            get => _initialStackFrame.HeaderText;
            set => _initialStackFrame.HeaderText = value;
        }
        
        protected SearchableMenuTree()
        {
            _stack = new StackView();
            _initialStackFrame = new StackFrameElement();
            Add(_stack);
            RegisterCallback<AttachToPanelEvent>(HandleAttachToPanelEvent);
            RegisterCallback<DetachFromPanelEvent>(HandleDetachFromPanelEvent);
        }

        protected virtual void HandleAttachToPanelEvent(AttachToPanelEvent _)
        {
            Asserts.IsNull(_registrationSet);
            _registrationSet = new RegistrationSet(this);
            _registrationSet.RegisterCallback<NavigateBackCommand>(HandleNavigateBackCommand);
            _registrationSet.RegisterCallback<NavigateBottomCommand>(HandleNavigateBottomCommand);
            _registrationSet.RegisterCallback<NavigateDownCommand>(HandleNavigateDownCommand);
            _registrationSet.RegisterCallback<NavigatePageDownCommand>(HandleNavigatePageDownCommand);
            _registrationSet.RegisterCallback<NavigatePageUpCommand>(HandleNavigatePageUpCommand);
            _registrationSet.RegisterCallback<NavigateSearchCommand>(HandleNavigateSearchCommand);
            _registrationSet.RegisterCallback<NavigateSubmitCommand>(HandleNavigateSubmitCommand);
            _registrationSet.RegisterCallback<NavigateTopCommand>(HandleNavigateTopCommand);
            _registrationSet.RegisterCallback<NavigateUpCommand>(HandleNavigateUpCommand);
            RefreshItems();
        }

        protected virtual void HandleDetachFromPanelEvent(DetachFromPanelEvent _)
        {
            Asserts.IsNotNull(_registrationSet).Dispose();
            _registrationSet = null;
            _stack.ClearAll();
        }
        
        private void HandleNavigateBackCommand(NavigateBackCommand command)
        {
            
        }
        private void HandleNavigateBottomCommand(NavigateBottomCommand command)
        {
            
        }
        private void HandleNavigateDownCommand(NavigateDownCommand command)
        {
            
        }
        private void HandleNavigatePageDownCommand(NavigatePageDownCommand command)
        {
            
        }
        private void HandleNavigatePageUpCommand(NavigatePageUpCommand command)
        {
            
        }
        private void HandleNavigateSearchCommand(NavigateSearchCommand command)
        {
            
        }
        private void HandleNavigateSubmitCommand(NavigateSubmitCommand command)
        {
            
        }
        private void HandleNavigateTopCommand(NavigateTopCommand command)
        {
            
        }
        private void HandleNavigateUpCommand(NavigateUpCommand command)
        {
            
        }

        private static IndexItem[] ConstructIndex(IEnumerable<SearchableMenuTreeItem<T>> items)
        {
            return items.SelectMany(item => 
            {
                return item.SearchTerms.Select(term =>  new IndexItem(term, item));
            }).ToArray();
        }

        private static ExtractedResult<IndexItem>[] SearchIndex(IndexItem[] index, string searchTerm)
        {
            IndexItem searchIndexItem = new(searchTerm);
            IEnumerable<ExtractedResult<IndexItem>> searchResults = FuzzSearch.ExtractTop(
                searchIndexItem, 
                index, 
                item => item.SearchTerm,
                limit: int.MaxValue,
                cutoff: 75 // This is a little arbitrary tbqh
            );
            return searchResults.ToArray();
        }

        protected void RefreshItems()
        {
            SearchableMenuTreeItem<T>[] items = Items.ToArray();
            Node[] treeRoots = ConstructTree(items);
            _searchIndex = ConstructIndex(items);
            _stack.ClearAll();
            _initialStackFrame.Clear();
            _initialStackFrame.AddRange(treeRoots);
            _stack.Add(_initialStackFrame);
            _stack.PushWithoutAnimate(_initialStackFrame);
        }

        private static void CreateShortcuts(Node[] nodes)
        {
            // TODO: this is a little complex and will require changes to the Stack element
            // Lets get things working solidly first 
        }
        
        private static Node[] ConstructTree(IEnumerable<SearchableMenuTreeItem<T>> items)
        {
            ParentNode intermediate = new(string.Empty);
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
                ParentNode current = intermediate;
                for (int i = 0; i < parts[i].Length - 1; ++i)
                {
                    string key = parts[i];
                    if (!current.ChildNodes.TryGetValue(key, out List<Node> nodes))
                    {
                        nodes = new List<Node>();
                        current.ChildNodes.Add(key, nodes);
                    }

                    if (nodes.FirstOrDefault() is not ParentNode maybeParentNode)
                    {
                        maybeParentNode = new ParentNode(key);
                        nodes.Insert(0, maybeParentNode);
                    }
                    current = maybeParentNode;
                }
                string leafName = parts[^1];
                LeafNode leafNode = new(leafName, item.Value, item.ShowNextIcon);
                if (!current.ChildNodes.TryGetValue(leafName, out List<Node> finalNodes))
                {
                    finalNodes = new List<Node>();
                    current.ChildNodes.Add(leafName, finalNodes);
                }
                finalNodes.Add(leafNode);
            }
            Node[] results = intermediate.ChildNodes.Values.Flatten().ToArray();
            Array.Sort(results);
            CreateShortcuts(results);
            return results;
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