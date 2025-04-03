using UnityEngine;

public class PanelGroup : MonoBehaviour
{
    public GameObject[] panels;
    public TabGroup tabGroup;
    public int panelIndex;

    private void Awake()
    {
        ShowCurrentPanel();
    }

    void ShowCurrentPanel()
    {
        for (int i = 0; i < panels.Length; i++)
        {
            if (i == panelIndex)
            {
                panels[i].gameObject.GetComponent<CanvasGroup>().alpha = 1;
                panels[i].gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;
            }
            else
            {
                panels[i].gameObject.GetComponent<CanvasGroup>().alpha = 0;
                panels[i].gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;
            }
        }
    }

    public void SetPageIndex(int index)
    {
        panelIndex = index;
        ShowCurrentPanel();
    }
}
