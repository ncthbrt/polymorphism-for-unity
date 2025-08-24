#nullable enable
using System;
using JetBrains.Annotations;
using Polymorphism4Unity.Safety;
using UnityEngine;
using UnityEngine.UIElements;
using BPF = UnityEngine.UIElements.BasePopupField<string, string>;
namespace Polymorphism4Unity.Editor.Fields.Types
{
    
    [PublicAPI]
    public class TypePopupField : BaseField<Type>
    {
        public Type BaseType { get; }
        private const string ButtonName = "TypeButton";
        private readonly Button _button;
        public TypePopupField(Type baseType) : base("Label", CreateFieldInput())
        {
            BaseType = baseType;
            AddToClassList(BPF.ussClassName);
            labelElement.AddToClassList(BPF.labelUssClassName);
            _button = Asserts.IsNotNull(this.Q<Button>(name: ButtonName));

            // this.RegisterCallback<PointerDownEvent>(new EventCallback<PointerDownEvent>(this.OnPointerDownEvent));
            // this.RegisterCallback<PointerUpEvent>(new EventCallback<PointerUpEvent>(this.OnPointerUpEvent));
            // this.RegisterCallback<PointerMoveEvent>(new EventCallback<PointerMoveEvent>(this.OnPointerMoveEvent));
            // this.RegisterCallback<MouseDownEvent>((EventCallback<MouseDownEvent>) (e =>
            // {
            //     if (e.button != 0)
            //         return;
            //     e.StopPropagation();
            // }));
            // this.RegisterCallback<NavigationSubmitEvent>(new EventCallback<NavigationSubmitEvent>(this.OnNavigationSubmit));
        }
        
        private static VisualElement CreateFieldInput()
        {
            Button button = new()
            {
                name = ButtonName
            }; 
            button.AddToClassList(BPF.inputUssClassName);
            
            TextElement textElement = button.Q<TextElement>();
            textElement.pickingMode = PickingMode.Ignore;
            textElement.AddToClassList(BPF.textUssClassName);
            
            VisualElement arrowElement = new();
            arrowElement.AddToClassList(BPF.arrowUssClassName);
            arrowElement.pickingMode = PickingMode.Ignore;
            button.Add(arrowElement);
            return button;
        }
        
        
        
        private void OnPointerDownEvent(PointerDownEvent evt)
        {
            ProcessPointerDown(evt);
        }

        private void OnPointerUpEvent(PointerUpEvent evt)
        {
            if (evt.button != 0)
            {
                return;
            }
            evt.StopPropagation();
        }

        private void OnPointerMoveEvent(PointerMoveEvent evt)
        {
            if (evt.button != 0 || (evt.pressedButtons & 1) == 0)
            {
                return;
            }
            ProcessPointerDown(evt);
        }

        private void ProcessPointerDown<T>(PointerEventBase<T> evt) where T : PointerEventBase<T>, new()
        {
            if (evt.button != 0)
            {
                return;
            }
            schedule.Execute(ShowMenu);
            evt.StopPropagation();
        }

        private void OnNavigationSubmit(NavigationSubmitEvent evt)
        {
            this.ShowMenu();
            evt.StopPropagation();
        }

        private void ShowMenu()
        {
            Debug.Log("Showing Menu");
        }
        
        
    }
    
    [PublicAPI]
    [UxmlElement(nameof(TypePopupField))]
    public partial class TypePopupField<T>: TypePopupField
    {
        public TypePopupField(): base(typeof(T))
        {
        }
    }
    
}