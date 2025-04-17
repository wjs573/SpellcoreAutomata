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
        GameObject enemy = GameManager.Instance.CreateCharacter("Skeleton", 1, new Vector3(2, 0, 3), 
        new ChaProperty(100,100,100,100,10,100,10,0,10,10,10,1.5f,0.25f,0.05f,0.25f,0.25f,MoveType.ground,false),
        0f, "骷髅");
        enemy.AddComponent<UnitSimpleAI>();
    }
}