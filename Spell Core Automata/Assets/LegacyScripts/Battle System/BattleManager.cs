using JinShan;
using Sirenix.OdinInspector;
using UnityEngine;

public class BattleManager : MonoSingleton<BattleManager>
{
    [Button("Start Battle")]
    /// <summary>
    /// 开启一场战斗
    /// </summary>
    public void StartBattle()
    {
        //初始化战斗场景 包括
        //创建地图 创建主角 给主角添加buff
        GameManager.Instance.IniBattle();
        GameManager.Instance.CreateCharacter("Skeleton",1,new Vector3(2,0,3),ChaProperty.zero,0f,"Skeleton");
    }
}