#nullable enable
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable InconsistentNaming
// Following Unity UIElements naming conventions for this file

namespace Polymorphism4Unity.Editor.Styling
{
    public class CompactStyle: IStyle
    {
        public StyleInt unitySlice
        {
            set
            {
                unitySliceBottom = value;
                unitySliceLeft = value;
                unitySliceRight = value;
                unitySliceTop = value;
            }
        }

        public StyleLength padding
        {
            set
            {
                paddingBottom = value;
                paddingLeft = value;
                paddingRight = value;
                paddingTop = value;
            }
        }
        
        public StyleLength margin
        {
            set
            {
                marginBottom = value;
                marginLeft = value;
                marginRight = value;
                marginTop = value;
            }
        }

        public StyleColor borderColor
        {
            set
            {
                borderBottomColor = value;
                borderRightColor = value;
                borderLeftColor = value;
                borderTopColor = value;
            }
        }
        
        public StyleLength borderRadius
        {
            set
            {                
                borderBottomLeftRadius = value;
                borderBottomRightRadius = value;
                borderTopLeftRadius = value;
                borderTopRightRadius = value;
            }
        }

        public StyleFloat borderWidth
        {
            set
            {
                borderBottomWidth = value;
                borderLeftWidth = value;
                borderRightWidth = value;
                borderTopWidth = value;
            }
        }

        public StyleBackgroundPosition backgroundPosition
        {
            set
            {
                backgroundPositionX = value;
                backgroundPositionY = value;
            }
        }

        public StyleEnum<Align> alignContent { get; set; } = StyleKeyword.Null;
        public StyleEnum<Align> alignItems { get; set; } = StyleKeyword.Null;
        public StyleEnum<Align> alignSelf { get; set; } = StyleKeyword.Null;
        public StyleColor backgroundColor { get; set; } = StyleKeyword.Null;
        public StyleBackground backgroundImage { get; set; } = StyleKeyword.Null;
        public StyleBackgroundPosition backgroundPositionX { get; set; } = StyleKeyword.Null;
        public StyleBackgroundPosition backgroundPositionY { get; set; } = StyleKeyword.Null;
        public StyleBackgroundRepeat backgroundRepeat { get; set; } = StyleKeyword.Null;
        public StyleBackgroundSize backgroundSize { get; set; } = StyleKeyword.Null;
        public StyleColor borderBottomColor { get; set; } = StyleKeyword.Null;
        public StyleLength borderBottomLeftRadius { get; set; } = StyleKeyword.Null;
        public StyleLength borderBottomRightRadius { get; set; } = StyleKeyword.Null;
        public StyleFloat borderBottomWidth { get; set; } = StyleKeyword.Null;
        public StyleColor borderLeftColor { get; set; } = StyleKeyword.Null;
        public StyleFloat borderLeftWidth { get; set; } = StyleKeyword.Null;
        public StyleColor borderRightColor { get; set; } = StyleKeyword.Null;
        public StyleFloat borderRightWidth { get; set; } = StyleKeyword.Null;
        public StyleColor borderTopColor { get; set; } = StyleKeyword.Null;
        public StyleLength borderTopLeftRadius { get; set; } = StyleKeyword.Null;
        public StyleLength borderTopRightRadius { get; set; } = StyleKeyword.Null;
        public StyleFloat borderTopWidth { get; set; } = StyleKeyword.Null;
        public StyleLength bottom { get; set; } = StyleKeyword.Null;
        public StyleColor color { get; set; } = StyleKeyword.Null;
        public StyleCursor cursor { get; set; } = StyleKeyword.Null;
        public StyleEnum<DisplayStyle> display { get; set; } = StyleKeyword.Null;
        public StyleLength flexBasis { get; set; } = StyleKeyword.Null;
        public StyleEnum<FlexDirection> flexDirection { get; set; } = StyleKeyword.Null;
        public StyleFloat flexGrow { get; set; } = StyleKeyword.Null;
        public StyleFloat flexShrink { get; set; } = StyleKeyword.Null;
        public StyleEnum<Wrap> flexWrap { get; set; } = StyleKeyword.Null;
        public StyleLength fontSize { get; set; } = StyleKeyword.Null;
        public StyleLength height { get; set; } = StyleKeyword.Null;
        public StyleEnum<Justify> justifyContent { get; set; } = StyleKeyword.Null;
        public StyleLength left { get; set; } = StyleKeyword.Null;
        public StyleLength letterSpacing { get; set; } = StyleKeyword.Null;
        public StyleLength marginBottom { get; set; } = StyleKeyword.Null;
        public StyleLength marginLeft { get; set; } = StyleKeyword.Null;
        public StyleLength marginRight { get; set; } = StyleKeyword.Null;
        public StyleLength marginTop { get; set; } = StyleKeyword.Null;
        public StyleLength maxHeight { get; set; } = StyleKeyword.Null;
        public StyleLength maxWidth { get; set; } = StyleKeyword.Null;
        public StyleLength minHeight { get; set; } = StyleKeyword.Null;
        public StyleLength minWidth { get; set; } = StyleKeyword.Null;
        public StyleFloat opacity { get; set; } = StyleKeyword.Null;
        public StyleEnum<Overflow> overflow { get; set; } = StyleKeyword.Null;
        public StyleLength paddingBottom { get; set; } = StyleKeyword.Null;
        public StyleLength paddingLeft { get; set; } = StyleKeyword.Null;
        public StyleLength paddingRight { get; set; } = StyleKeyword.Null;
        public StyleLength paddingTop { get; set; } = StyleKeyword.Null;
        public StyleEnum<Position> position { get; set; } = StyleKeyword.Null;
        public StyleLength right { get; set; } = StyleKeyword.Null;
        public StyleRotate rotate { get; set; } = StyleKeyword.Null;
        public StyleScale scale { get; set; } = StyleKeyword.Null;
        public StyleEnum<TextOverflow> textOverflow { get; set; } = StyleKeyword.Null;
        public StyleTextShadow textShadow { get; set; } = StyleKeyword.Null;
        public StyleLength top { get; set; } = StyleKeyword.Null;
        public StyleTransformOrigin transformOrigin { get; set; } = StyleKeyword.Null;
        
        public TimeValue transitionDelay 
        { 
            set => ((IStyle)this).transitionDelay = value.AsStyleList(); 
        }
        
        StyleList<TimeValue> IStyle.transitionDelay { get; set; } = StyleKeyword.Null;

        public TimeValue transitionDuration
        {
            set => ((IStyle)this).transitionDuration = value.AsStyleList();
        }
        
        StyleList<TimeValue> IStyle.transitionDuration { get; set; } = StyleKeyword.Null;
        
        public StylePropertyName transitionProperty
        {
            set => ((IStyle)this).transitionProperty = value.AsStyleList();
        }
        
        StyleList<StylePropertyName> IStyle.transitionProperty { get; set; } = StyleKeyword.Null;
        public EasingFunction transitionTimingFunction
        {
            set => ((IStyle)this).transitionTimingFunction = value.AsStyleList();
        }
        
        StyleList<EasingFunction> IStyle.transitionTimingFunction { get; set; } = StyleKeyword.Null;
        
        public StyleTranslate translate { get; set; } = StyleKeyword.Null;
        public StyleColor unityBackgroundImageTintColor { get; set; } = StyleKeyword.Null;
        public StyleEnum<EditorTextRenderingMode> unityEditorTextRenderingMode { get; set; } = StyleKeyword.Null;
        public StyleFont unityFont { get; set; } = StyleKeyword.Null;
        public StyleFontDefinition unityFontDefinition { get; set; } = StyleKeyword.Null;
        public StyleEnum<FontStyle> unityFontStyleAndWeight { get; set; } = StyleKeyword.Null;
        public StyleEnum<OverflowClipBox> unityOverflowClipBox { get; set; } = StyleKeyword.Null;
        public StyleLength unityParagraphSpacing { get; set; } = StyleKeyword.Null;
        public StyleInt unitySliceBottom { get; set; } = StyleKeyword.Null;
        public StyleInt unitySliceLeft { get; set; } = StyleKeyword.Null;
        public StyleInt unitySliceRight { get; set; } = StyleKeyword.Null;
        public StyleFloat unitySliceScale { get; set; } = StyleKeyword.Null;
        public StyleInt unitySliceTop { get; set; } = StyleKeyword.Null;
#if UNITY_6000_0_OR_NEWER
        public StyleEnum<SliceType> unitySliceType { get; set; }
#endif
        
        public StyleEnum<TextAnchor> unityTextAlign { get; set; } = StyleKeyword.Null;
        public StyleEnum<TextGeneratorType> unityTextGenerator { get; set; } = StyleKeyword.Null;
        public StyleColor unityTextOutlineColor { get; set; } = StyleKeyword.Null;
        public StyleFloat unityTextOutlineWidth { get; set; } = StyleKeyword.Null;
        public StyleEnum<TextOverflowPosition> unityTextOverflowPosition { get; set; } = StyleKeyword.Null;
        public StyleEnum<Visibility> visibility { get; set; } = StyleKeyword.Null;
        public StyleEnum<WhiteSpace> whiteSpace { get; set; } = StyleKeyword.Null;
        public StyleLength width { get; set; } = StyleKeyword.Null;
        public StyleLength wordSpacing { get; set; } = StyleKeyword.Null;
        public StyleEnum<ScaleMode> unityBackgroundScaleMode { get; set; } = StyleKeyword.Null;
    }
}