using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using UnityEngine;
using WJS;

public class BattleManager : MonoSingleton<BattleManager>
{

    /// <summary>
    /// 是否处于战斗场景
    /// </summary>
    public bool IsInBattle = true;
    public GameObject mainChacter;
    public GameObject greedyPillar;

    private void Start()
    {
        IniBattle();
    }

    void InitMainCharacterAndGreedyPillar()
    {
        SceneVariants.mainChacter = mainChacter;
        GameManager.Instance.InitChaState(mainChacter, "MainCharacter", 0, new ChaProperty(100, 0, 100, 1000, 10, 1000, 10, 100, 100, 1.5f, 0.25f, 0f, 0.25f, 0.25f));
        GameManager.Instance.InitChaState(greedyPillar, "GreedyPillar", 1, new ChaProperty(0, 0, 100, 1000000, 10, 1000, 10, 100, 100, 1.5f, 0.25f, 0f, 1f, 1f));
        mainChacter.GetComponent<UnitSkillTester>().LearnSkill();
    }

    /// <summary>
    /// 初始化战斗场景
    /// </summary>
    [Button("InitBattle")]
    public void IniBattle()
    {
        if (IsInBattle)
        {
            return;
        }
        IsInBattle = true;

        //初始化人物表
        GameManager.Instance.ResetCharacters();

        InitMainCharacterAndGreedyPillar();
    }

    /// <summary>
    /// 清除战斗场景
    /// </summary>
    public void ClearBattle()
    {
        IsInBattle = false;
        GameManager.Instance.ClearBattle();
    }
}
