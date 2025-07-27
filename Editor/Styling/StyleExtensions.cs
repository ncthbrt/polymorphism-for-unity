#nullable enable
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Polymorphism4Unity.Editor.Styling
{
    public static class StyleExtensions
    {
        public static void ApplyStyles(this IStyle target, params IStyle[] sources)
        {
            for (int i = 0; i < sources.Length; ++i)
            {
                IStyle source = sources[i];
                if (source.alignContent.keyword != StyleKeyword.Null)
                {
                    target.alignContent = source.alignContent;
                }
                if (source.alignItems.keyword != StyleKeyword.Null)
                {
                    target.alignItems = source.alignItems;
                }
                if (source.alignSelf.keyword != StyleKeyword.Null)
                {
                    target.alignSelf = source.alignSelf;
                }
                if (source.backgroundColor.keyword != StyleKeyword.Null)
                {
                    target.backgroundColor = source.backgroundColor;
                }
                if (source.backgroundImage.keyword != StyleKeyword.Null)
                {
                    target.backgroundImage = source.backgroundImage;
                }
                if (source.backgroundPositionX.keyword != StyleKeyword.Null)
                {
                    target.backgroundPositionX = source.backgroundPositionX;
                }
                if (source.backgroundPositionY.keyword != StyleKeyword.Null)
                {
                    target.backgroundPositionY = source.backgroundPositionY;
                }
                if (source.backgroundRepeat.keyword != StyleKeyword.Null)
                {
                    target.backgroundRepeat = source.backgroundRepeat;
                }
                if (source.backgroundSize.keyword != StyleKeyword.Null)
                {
                    target.backgroundSize = source.backgroundSize;
                }
                if (source.borderBottomColor.keyword != StyleKeyword.Null)
                {
                    target.borderBottomColor = source.borderBottomColor;
                }
                if (source.borderBottomLeftRadius.keyword != StyleKeyword.Null)
                {
                    target.borderBottomLeftRadius = source.borderBottomLeftRadius;
                }
                if (source.borderBottomRightRadius.keyword != StyleKeyword.Null)
                {
                    target.borderBottomRightRadius = source.borderBottomRightRadius;
                }
                if (source.borderBottomWidth.keyword != StyleKeyword.Null)
                {
                    target.borderBottomWidth = source.borderBottomWidth;
                }
                if (source.borderLeftColor.keyword != StyleKeyword.Null)
                {
                    target.borderLeftColor = source.borderLeftColor;
                }
                if (source.borderLeftWidth.keyword != StyleKeyword.Null)
                {
                    target.borderLeftWidth = source.borderLeftWidth;
                }
                if (source.borderRightColor.keyword != StyleKeyword.Null)
                {
                    target.borderRightColor = source.borderRightColor;
                }
                if (source.borderRightWidth.keyword != StyleKeyword.Null)
                {
                    target.borderRightWidth = source.borderRightWidth;
                }
                if (source.borderTopColor.keyword != StyleKeyword.Null)
                {
                    target.borderTopColor = source.borderTopColor;
                }
                if (source.borderTopLeftRadius.keyword != StyleKeyword.Null)
                {
                    target.borderTopLeftRadius = source.borderTopLeftRadius;
                }
                if (source.borderTopRightRadius.keyword != StyleKeyword.Null)
                {
                    target.borderTopRightRadius = source.borderTopRightRadius;
                }
                if (source.borderTopWidth.keyword != StyleKeyword.Null)
                {
                    target.borderTopWidth = source.borderTopWidth;
                }
                if (source.bottom.keyword != StyleKeyword.Null)
                {
                    target.bottom = source.bottom;
                }
                if (source.color.keyword != StyleKeyword.Null)
                {
                    target.color = source.color;
                }
                if (source.cursor.keyword != StyleKeyword.Null)
                {
                    target.cursor = source.cursor;
                }
                if (source.display.keyword != StyleKeyword.Null)
                {
                    target.display = source.display;
                }
                if (source.flexBasis.keyword != StyleKeyword.Null)
                {
                    target.flexBasis = source.flexBasis;
                }
                if (source.flexDirection.keyword != StyleKeyword.Null)
                {
                    target.flexDirection = source.flexDirection;
                }
                if (source.flexGrow.keyword != StyleKeyword.Null)
                {
                    target.flexGrow = source.flexGrow;
                }
                if (source.flexShrink.keyword != StyleKeyword.Null)
                {
                    target.flexShrink = source.flexShrink;
                }
                if (source.flexWrap.keyword != StyleKeyword.Null)
                {
                    target.flexWrap = source.flexWrap;
                }
                if (source.fontSize.keyword != StyleKeyword.Null)
                {
                    target.fontSize = source.fontSize;
                }
                if (source.height.keyword != StyleKeyword.Null)
                {
                    target.height = source.height;
                }
                if (source.justifyContent.keyword != StyleKeyword.Null)
                {
                    target.justifyContent = source.justifyContent;
                }
                if (source.left.keyword != StyleKeyword.Null)
                {
                    target.left = source.left;
                }
                if (source.letterSpacing.keyword != StyleKeyword.Null)
                {
                    target.letterSpacing = source.letterSpacing;
                }
                if (source.marginBottom.keyword != StyleKeyword.Null)
                {
                    target.marginBottom = source.marginBottom;
                }
                if (source.marginLeft.keyword != StyleKeyword.Null)
                {
                    target.marginLeft = source.marginLeft;
                }
                if (source.marginRight.keyword != StyleKeyword.Null)
                {
                    target.marginRight = source.marginRight;
                }
                if (source.marginTop.keyword != StyleKeyword.Null)
                {
                    target.marginTop = source.marginTop;
                }
                if (source.maxHeight.keyword != StyleKeyword.Null)
                {
                    target.maxHeight = source.maxHeight;
                }
                if (source.maxWidth.keyword != StyleKeyword.Null)
                {
                    target.maxWidth = source.maxWidth;
                }
                if (source.minHeight.keyword != StyleKeyword.Null)
                {
                    target.minHeight = source.minHeight;
                }
                if (source.minWidth.keyword != StyleKeyword.Null)
                {
                    target.minWidth = source.minWidth;
                }
                if (source.opacity.keyword != StyleKeyword.Null)
                {
                    target.opacity = source.opacity;
                }
                if (source.overflow.keyword != StyleKeyword.Null)
                {
                    target.overflow = source.overflow;
                }
                if (source.paddingBottom.keyword != StyleKeyword.Null)
                {
                    target.paddingBottom = source.paddingBottom;
                }
                if (source.paddingLeft.keyword != StyleKeyword.Null)
                {
                    target.paddingLeft = source.paddingLeft;
                }
                if (source.paddingRight.keyword != StyleKeyword.Null)
                {
                    target.paddingRight = source.paddingRight;
                }
                if (source.paddingTop.keyword != StyleKeyword.Null)
                {
                    target.paddingTop = source.paddingTop;
                }
                if (source.position.keyword != StyleKeyword.Null)
                {
                    target.position = source.position;
                }
                if (source.right.keyword != StyleKeyword.Null)
                {
                    target.right = source.right;
                }
                if (source.rotate.keyword != StyleKeyword.Null)
                {
                    target.rotate = source.rotate;
                }
                if (source.scale.keyword != StyleKeyword.Null)
                {
                    target.scale = source.scale;
                }
                if (source.textOverflow.keyword != StyleKeyword.Null)
                {
                    target.textOverflow = source.textOverflow;
                }
                if (source.textShadow.keyword != StyleKeyword.Null)
                {
                    target.textShadow = source.textShadow;
                }
                if (source.top.keyword != StyleKeyword.Null)
                {
                    target.top = source.top;
                }
                if (source.transformOrigin.keyword != StyleKeyword.Null)
                {
                    target.transformOrigin = source.transformOrigin;
                }
                if (source.transitionDelay.keyword != StyleKeyword.Null)
                {
                    target.transitionDelay = source.transitionDelay;
                }
                if (source.transitionDuration.keyword != StyleKeyword.Null)
                {
                    target.transitionDuration = source.transitionDuration;
                }
                if (source.transitionProperty.keyword != StyleKeyword.Null)
                {
                    target.transitionProperty = source.transitionProperty;
                }
                if (source.transitionTimingFunction.keyword != StyleKeyword.Null)
                {
                    target.transitionTimingFunction = source.transitionTimingFunction;
                }
                if (source.translate.keyword != StyleKeyword.Null)
                {
                    target.translate = source.translate;
                }
                if (source.unityBackgroundImageTintColor.keyword != StyleKeyword.Null)
                {
                    target.unityBackgroundImageTintColor = source.unityBackgroundImageTintColor;
                }
                if (source.unityEditorTextRenderingMode.keyword != StyleKeyword.Null)
                {
                    target.unityEditorTextRenderingMode = source.unityEditorTextRenderingMode;
                }
                if (source.unityFont.keyword != StyleKeyword.Null)
                {
                    target.unityFont = source.unityFont;
                }
                if (source.unityFontDefinition.keyword != StyleKeyword.Null)
                {
                    target.unityFontDefinition = source.unityFontDefinition;
                }
                if (source.unityFontStyleAndWeight.keyword != StyleKeyword.Null)
                {
                    target.unityFontStyleAndWeight = source.unityFontStyleAndWeight;
                }
                if (source.unityOverflowClipBox.keyword != StyleKeyword.Null)
                {
                    target.unityOverflowClipBox = source.unityOverflowClipBox;
                }
                if (source.unityParagraphSpacing.keyword != StyleKeyword.Null)
                {
                    target.unityParagraphSpacing = source.unityParagraphSpacing;
                }
                if (source.unitySliceBottom.keyword != StyleKeyword.Null)
                {
                    target.unitySliceBottom = source.unitySliceBottom;
                }
                if (source.unitySliceLeft.keyword != StyleKeyword.Null)
                {
                    target.unitySliceLeft = source.unitySliceLeft;
                }
                if (source.unitySliceRight.keyword != StyleKeyword.Null)
                {
                    target.unitySliceRight = source.unitySliceRight;
                }
                if (source.unitySliceScale.keyword != StyleKeyword.Null)
                {
                    target.unitySliceScale = source.unitySliceScale;
                }
                if (source.unitySliceTop.keyword != StyleKeyword.Null)
                {
                    target.unitySliceTop = source.unitySliceTop;
                }
                if (source.unityTextAlign.keyword != StyleKeyword.Null)
                {
                    target.unityTextAlign = source.unityTextAlign;
                }
                if (source.unityTextGenerator.keyword != StyleKeyword.Null)
                {
                    target.unityTextGenerator = source.unityTextGenerator;
                }
                if (source.unityTextOutlineColor.keyword != StyleKeyword.Null)
                {
                    target.unityTextOutlineColor = source.unityTextOutlineColor;
                }
                if (source.unityTextOutlineWidth.keyword != StyleKeyword.Null)
                {
                    target.unityTextOutlineWidth = source.unityTextOutlineWidth;
                }
                if (source.unityTextOverflowPosition.keyword != StyleKeyword.Null)
                {
                    target.unityTextOverflowPosition = source.unityTextOverflowPosition;
                }
                if (source.visibility.keyword != StyleKeyword.Null)
                {
                    target.visibility = source.visibility;
                }
                if (source.whiteSpace.keyword != StyleKeyword.Null)
                {
                    target.whiteSpace = source.whiteSpace;
                }
                if (source.width.keyword != StyleKeyword.Null)
                {
                    target.width = source.width;
                }
                if (source.wordSpacing.keyword != StyleKeyword.Null)
                {
                    target.wordSpacing = source.wordSpacing;
                }
#pragma warning disable CS0618 // Type or member is obsolete.
                // This is not obsolete in all target Unity versions
                if (source.unityBackgroundScaleMode.keyword != StyleKeyword.Null)
                {
                    target.unityBackgroundScaleMode = source.unityBackgroundScaleMode;
                }
#pragma warning restore CS0618 // Type or member is obsolete
            }
        }

        public static StyleList<T> AsStyleList<T>(this T value)
        {
            return new StyleList<T>(new List<T>()
            {
                value
            });
        }
    }
}