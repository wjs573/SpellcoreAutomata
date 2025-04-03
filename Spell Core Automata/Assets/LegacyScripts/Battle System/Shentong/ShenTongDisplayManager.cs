using System.Collections.Generic;
using System.Linq;
using JinShan;
using TMPro;
using UnityEngine;

public class ShenTongDisplayManager : MonoBehaviour
{
    public List<ShenTongCard> cards = new List<ShenTongCard>(3);
    private List<EnhancedEffectObject> availableShenTongs;
    private EnhancedEffectObject[] displayedShenTongs = new EnhancedEffectObject[3];
    public TextMeshProUGUI refreshCountText;
    public int refreshCount = 3; // 假设初始刷新次数为5

    void Start()
    {
        EnhancedEffectObject[] Shentongs = Resources.LoadAll<EnhancedEffectObject>("Inventory/ScriptableObjects/技能强化");
        availableShenTongs = Shentongs.ToList();
        InitializeCards();
    }

    public void Update()
    {
        refreshCountText.text = $"剩余刷新次数：{refreshCount}";
    }

    public void InitializeCards()
    {
        for (int i = 0; i < displayedShenTongs.Length; i++)
        {
            displayedShenTongs[i] = GetRandomShenTong(displayedShenTongs);
            cards[i].Initialize(i,this);
            cards[i].RefreshShenTong(displayedShenTongs[i]);
        }
    }

    public EnhancedEffectObject GetRandomShenTong(EnhancedEffectObject[] exclude)
    {
        var availableOptions = availableShenTongs.Except(exclude).ToList();
        if (availableOptions.Any())
        {
            int index = Random.Range(0, availableOptions.Count);
            return availableOptions[index];
        }
        return null;
    }

    public void RefreshShenTong(int cardIndex)
    {
        if (refreshCount > 0)
        {
            EnhancedEffectObject newShenTong = GetRandomShenTong(displayedShenTongs);
            if (newShenTong != null)
            {
                displayedShenTongs[cardIndex] = newShenTong;
                cards[cardIndex].RefreshShenTong(newShenTong);
            }
            refreshCount--;
        }
        else
        {
            Debug.Log("No more refreshes available.");
        }
    }
}
