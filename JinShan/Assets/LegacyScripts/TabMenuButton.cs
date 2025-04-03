using System.Collections;
using System.Collections.Generic;
using JinShan;
using UnityEngine;
using UnityEngine.EventSystems;

public class TabMenuButton : MonoBehaviour
{
    public GameObject iconFocus;
    public UIEventListener listener;
    public List<ItemType> itemTypes;
    private void Start()
    {
        listener = GetComponent<UIEventListener>();
    }
}
