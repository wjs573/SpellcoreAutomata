using JinShan;

public class BattleManager : MonoSingleton<BattleManager>
{
    /// <summary>
    /// 开启一场战斗
    /// </summary>
    public void StartBattle()
    {
        //初始化战斗场景 包括
        //创建地图 创建主角 给主角添加buff
        GameManager.Instance.IniBattle();

    }
}