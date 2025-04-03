using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public PanelGroup panelGroup;

    public List<TabButton> tabButtons;
    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabAcitve;

    public List<GameObject> objectsToSwap;

    public TabButton selectedTabButton;
    //添加tab button
    public void Subscribe(TabButton tabButton)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }
        tabButtons.Add(tabButton);
    }

    public void OnTabEnter(TabButton tabButton)
    {
        GameSoundManager.Instance.PlaySoundOneTimes("ButtonHover");
        ResetTabs();
        if (selectedTabButton == null || tabButton != selectedTabButton)
        {
            tabButton.background.sprite = tabHover;
            tabButton.background.color = new Color(255, 255, 255, 100);
        }
    }

    public void OnTabExit(TabButton tabButton)
    {
        ResetTabs();
    }

    public void OnTabSelected(TabButton tabButton)
    {
        GameSoundManager.Instance.PlaySoundOneTimes("ButtonClick");
        if (selectedTabButton != null)
        {
            selectedTabButton.DeSelected();
        }

        selectedTabButton = tabButton;

        selectedTabButton.Select();

        ResetTabs();
        tabButton.background.sprite = tabAcitve;
        tabButton.background.color = Color.white;

        int index = tabButton.transform.GetSiblingIndex();
        for (int i = 0; i < objectsToSwap.Count; i++)
        {
            if (i == index)
            {
                objectsToSwap[i].GetComponent<CanvasGroup>().alpha = 1;
                objectsToSwap[i].GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                objectsToSwap[i].GetComponent<CanvasGroup>().alpha = 0;
                objectsToSwap[i].GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }

        if (panelGroup != null)
        {
            panelGroup.SetPageIndex(tabButton.transform.GetSiblingIndex());
        }
    }

    public void ResetTabs()
    {
        foreach (TabButton tab in tabButtons)
        {
            if (tab == selectedTabButton)
            {
                continue;
            }
            tab.background.sprite = tabIdle;
            tab.background.color = new Color(116, 125, 140, 255);
        }
    }
}
