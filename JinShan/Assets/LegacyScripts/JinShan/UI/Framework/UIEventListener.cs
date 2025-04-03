using UnityEngine;
using UnityEngine.EventSystems;

namespace JinShan
{
    public delegate void PointerEventHandler(PointerEventData eventData);

    public class UIEventListener : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler,
        IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler,
        IBeginDragHandler, IDragHandler, IEndDragHandler
    //IDropHandler, IScrollHandler, IUpdateSelectedHandler, ISelectHandler, IDeselectHandler,
    //IMoveHandler, ISubmitHandler, ICancelHandler, IInitializePotentialDragHandler
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

        /*        public void OnCancel(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            throw new System.NotImplementedException();
        }*/
        /*        public void OnDrop(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }*/
        /*        public void OnScroll(PointerEventData eventData)
                {
                    throw new System.NotImplementedException();
                }

                public void OnSelect(BaseEventData eventData)
                {
                    throw new System.NotImplementedException();
                }

                public void OnSubmit(BaseEventData eventData)
                {
                    throw new System.NotImplementedException();
                }

                public void OnUpdateSelected(BaseEventData eventData)
                {
                    throw new System.NotImplementedException();
                }*/

        /*
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            throw new System.NotImplementedException();
        }*/

        /*        public void OnMove(AxisEventData eventData)
                {
                    throw new System.NotImplementedException();
                }*/
    }
}