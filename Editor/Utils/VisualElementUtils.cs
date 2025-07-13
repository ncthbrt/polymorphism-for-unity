#nullable enable
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Utils
{
    internal static class VisualElementUtils
    {
        public static void AddTemplateContentsToElement(this VisualElement visualElement, VisualTreeAsset template)
        {
            VisualElement rootElement = template.Instantiate();
            VisualElement[] children = rootElement.Children().ToArray();
            rootElement.Clear();
            foreach (VisualElement child in children)
            {
                visualElement.hierarchy.Add(child);
            }
        }

        public static void AddRange(this VisualElement visualElement, IEnumerable<VisualElement> children)
        {
            foreach (VisualElement child in children)
            {
                visualElement.Add(child);
            }
        }

    }
}