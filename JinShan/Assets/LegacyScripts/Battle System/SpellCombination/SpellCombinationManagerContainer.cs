using System.Collections.Generic;
using JinShan;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpellCombinationManagerContainer : MonoBehaviour
{
    [ShowInInspector]
    private Dictionary<FaBao, SpellCombinationManager> managers;
    public SpellCombinationManager currentManager;

    private void Start()
    {
        managers = new Dictionary<FaBao, SpellCombinationManager>();
    }

    public void AddManager(FaBao fabao)
    {
        if (fabao == null)
        {
            return;
        }
        if (!managers.ContainsKey(fabao))
        {
            GameObject managerObject = new GameObject($"SpellCombinationManager_{Random.Range(0, 100)}");
            managerObject.transform.SetParent(transform);
            SpellCombinationManager manager = managerObject.AddComponent<SpellCombinationManager>();
            manager.Initialize(fabao);
            managers.Add(fabao, manager);

            if (currentManager == null)
            {
                currentManager = manager;
            }
        }
    }

    public void RemoveManager(FaBao fabao)
    {
        if (managers.ContainsKey(fabao))
        {
            SpellCombinationManager manager = managers[fabao];
            Destroy(manager.gameObject);
            managers.Remove(fabao);

            if (currentManager == manager)
            {
                currentManager = null;
            }
        }
    }

    public void SwitchManager(FaBao fabao)
    {
        if (fabao != null && managers.ContainsKey(fabao))
        {
            currentManager = managers[fabao];
            currentManager.HasUpdateDrawPile = false;
            UIManager.Instance.GetWindow<UICombatHUDWindow>().currentFaBaoSlot.SetContent(fabao.uiDisplay, 1);
        }
    }

    public SpellCombinationManager GetManager(FaBao fabao)
    {
        if (managers.ContainsKey(fabao))
        {
            return managers[fabao];
        }
        return null;
    }

    public void UseCurrentWand()
    {
        if (currentManager != null)
        {
            currentManager.UseWand();
        }
        else
        {
            Debug.LogWarning("No current SpellCombinationManager is set.");
        }
    }
}
