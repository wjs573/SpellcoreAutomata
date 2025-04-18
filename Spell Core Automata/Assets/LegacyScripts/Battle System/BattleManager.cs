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
        CreateSkeletonsAtRandomPosition(10);
    }

    public void CreateSkeletonsAtRandomPosition(int count)
    {
        for (int i = 0; i < count; i++)
        {
            AddBuffInfo buff = new AddBuffInfo(DesignerTables.Buff.data["Resurrect"],null,null,1,1.3f,true);
            GameObject enemy = GameManager.Instance.CreateCharacter("Skeleton", 2, new Vector3(Random.Range(-5,5), 0, Random.Range(-5,5)), 
            new ChaProperty(100,100,100,100,10,100,10,0,10,10,10,1.5f,0.25f,0.05f,0.25f,0.25f,MoveType.ground,false),
            0f, "骷髅");
            enemy.GetComponent<ChaState>().AddBuff(buff);
        }
    }
}