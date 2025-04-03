using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
namespace JinShan
{
    public abstract class UserInterface : UIWindow
    {

        public InventoryObject inventory;


        public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        void Start()
        {
            //slotsOnInterface.UpdateSlotDisplay();
            for (int i = 0; i < inventory.GetSlots.Length; i++)
            {
                inventory.GetSlots[i].parent = this;
                inventory.GetSlots[i].OnAfterUpdate += OnSlotUpdate;
            }
            CreateSlots();
            gameObject.AddComponent<UIEventListener>();
            GetComponent<UIEventListener>().PointerEnter += OnEnterInterface;
            GetComponent<UIEventListener>().PointerExit += OnExitInterface;
        }

        private void OnSlotUpdate(InventorySlot _slot)
        {
            if (_slot.item.Id >= 0)
            {
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.ItemObject.uiDisplay;
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = _slot.amount == 1 ? "" : _slot.amount.ToString("n0");
            }
            else
            {
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.slotDisplay.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                _slot.slotDisplay.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }

        public abstract void CreateSlots();


        public void OnExitInterface(PointerEventData eventData)
        {

        }

        public void OnEnterInterface(PointerEventData eventData)
        {

        }


        public void OnClick(PointerEventData eventData)
        {

        }



        public void OnEnter(PointerEventData eventData)
        {

            MouseData.slotHoveredOver = eventData.pointerEnter.transform.parent.gameObject;
            MouseData.interfaceMouseIsOver = slotsOnInterface[MouseData.slotHoveredOver].parent;
        }
        public void OnExit(PointerEventData eventData)
        {
            MouseData.slotHoveredOver = null;
            MouseData.interfaceMouseIsOver = null;
        }
        public void OnDragStart(PointerEventData eventData)
        {
            if (slotsOnInterface.ContainsKey(eventData.pointerEnter.transform.parent.gameObject))//若选择的插槽在字典中存在
            {
                MouseData.tempSlotBeingDragged = eventData.pointerEnter.transform.parent.gameObject;
                MouseData.tempSlotInterfaceMouseIsOver = MouseData.tempSlotBeingDragged.transform.parent.GetComponent<UserInterface>();
                MouseData.tempItemBeingDragged = CreateTempItem(eventData.pointerEnter.transform.parent.gameObject);//创建一个临时的物品 以供玩家拖动
                if (MouseData.tempItemBeingDragged == null)//若插槽为空 临时物品为null 直接返回
                {
                    return;
                }
                MouseData.tempItemBeingDragged.GetComponent<Image>().raycastTarget = false;
            }

        }
        public GameObject CreateTempItem(GameObject obj)
        {
            GameObject tempItem = null;

            if (slotsOnInterface[obj].item.Id >= 0)
            {
                tempItem = new GameObject();
                var rt = tempItem.AddComponent<RectTransform>();
                rt.sizeDelta = new Vector2(50, 50);
                tempItem.transform.SetParent(transform.parent);
                var img = tempItem.AddComponent<Image>();
                img.sprite = slotsOnInterface[obj].ItemObject.uiDisplay;
                img.raycastTarget = false;
            }
            return tempItem;
        }

        public void OnDragEnd(PointerEventData eventData)
        {

            Destroy(MouseData.tempItemBeingDragged);
            Debug.Log(MouseData.interfaceMouseIsOver);
            //如果光标下面是非用户界面 删除物品
            if (MouseData.interfaceMouseIsOver == null)
            {
                slotsOnInterface[eventData.pointerEnter.transform.parent.gameObject].RemoveItem();
                return;
            }

            //如果光标下的插槽存在
            if (MouseData.slotHoveredOver != null)
            {
                Debug.Log(MouseData.slotHoveredOver);
                //通过obj 获取对应的插槽
                InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
                Debug.Log(MouseData.interfaceMouseIsOver);
                Debug.Log(mouseHoverSlotData.item.Name);
                //Debug.Log(slotsOnInterface[eventData.pointerEnter.transform.parent.gameObject].item.Name);//在拖拽结束后 两个swap的两个参数变成了相同的
                inventory.SwapItem(MouseData.tempSlotInterfaceMouseIsOver.slotsOnInterface[MouseData.tempSlotBeingDragged], mouseHoverSlotData);
            }
        }
        public void OnDrag(PointerEventData eventData)
        {
            if (MouseData.tempItemBeingDragged != null)
            {
                MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
                //MouseData.tempItemBeingDragged.GetComponent<Image>().raycastTarget = false;
            }
            //Debug.Log(MouseData.interfaceMouseIsOver);
        }



    }

    public static class MouseData
    {
        /// <summary>
        /// 拖拽开始时,光标下的slot
        /// </summary>
        public static GameObject slotOnDragStart;

        /// <summary>
        /// 上一次点击的inventoryslot
        /// </summary>
        public static InventorySlot InventorySlotLastClick;
        public static float InventorySlotLastClickTime;
        public static GameObject InventorySlotGameobjectLastClick;

        public static GameObject tempItemBeingDragged;
        public static GameObject slotHoveredOver;
        public static UserInterface interfaceMouseIsOver;
        public static GameObject tempSlotBeingDragged;
        public static UserInterface tempSlotInterfaceMouseIsOver;
        //光标下的物品
        public static ItemObject itemObjectMouseIsOver;
    }

    public static class ExtensionMethods
    {
        public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
        {
            foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
            {
                if (_slot.Value.item.Id >= 0)
                {
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.Value.ItemObject.uiDisplay;
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                    _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
                }
                else
                {
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                    _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                    _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
                }
            }
        }
    }
}
