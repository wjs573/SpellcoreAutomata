using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using WJS;
public class UIGameStartWindow : UIWindow
{
    UIEventListener GameStartBtn;

    private void Start() 
    {
        GameStartBtn = transform.FindChildByName("ButtonStartGame").GetComponent<UIEventListener>();
        GameStartBtn.PointerClick += OnClickGameStart;
        this.SetVisible(true); //默认显示
    }

    private void OnClickGameStart(PointerEventData eventData)
    {
        this.SetVisible(false);
    }
}
