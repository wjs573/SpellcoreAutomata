using System.Diagnostics;
using JinShan;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShenTongCard : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;
    public UIEventListener refreshButton;
    public UIEventListener selectButon;
    private int id;
    private ShenTongDisplayManager manager;

    public EnhancedEffectObject currentShenTong;

    public void RefreshShenTong(EnhancedEffectObject shenTong)
    {
        currentShenTong = shenTong;
        nameText.text = shenTong.data.Name;
        descriptionText.text = shenTong.description;
    }

    public void Initialize(int id, ShenTongDisplayManager manager)
    {
        this.id = id;
        this.manager = manager;
        // 先取消已注册的事件处理方法
        refreshButton.PointerClick -= RefreshShenTongCard;
        selectButon.PointerClick -= UnlockShenTong;

        // 然后再注册事件处理方法
        refreshButton.PointerClick += RefreshShenTongCard;
        selectButon.PointerClick += UnlockShenTong;
    }

    private void RefreshShenTongCard(PointerEventData eventData)
    {
        manager.RefreshShenTong(id);
    }

    private void UnlockShenTong(PointerEventData eventData)
    {
        GameManager.Instance.SetPause(false);
        MainCharacter.Instance.Skill_Inventory.AddItem(new Item(currentShenTong),1);
    }
}
