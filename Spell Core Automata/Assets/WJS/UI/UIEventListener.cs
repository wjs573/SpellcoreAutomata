using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static WJS.GameDelegate;

namespace WJS
{
    public class UIEventListener : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler,
        IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public event PointerEventHandler PointerClick;

        public event PointerEventHandler PointerDown;

        public event PointerEventHandler PointerUp;

        public event PointerEventHandler PointerExit;

        public event PointerEventHandler PointerEnter;

        public event PointerEventHandler DragBegin;

        public event PointerEventHandler DragOn;

        public event PointerEventHandler DragEnd;

        public static UIEventListener GetListener(Transform tf)
        {
            UIEventListener uiEvent = tf.GetComponent<UIEventListener>();
            //Debug.Log(uiEvent);

            if (uiEvent == null)
            {
                uiEvent = tf.gameObject.AddComponent<UIEventListener>();
            }

            //Debug.Log(uiEvent);
            return uiEvent;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (DragBegin != null) DragBegin(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (DragOn != null) DragOn(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (DragEnd != null) DragEnd(eventData);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (PointerClick != null) PointerClick(eventData);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (PointerDown != null) PointerDown(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (PointerEnter != null) PointerEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (PointerExit != null) PointerExit(eventData);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (PointerUp != null) PointerUp(eventData);
        }
    }
}
