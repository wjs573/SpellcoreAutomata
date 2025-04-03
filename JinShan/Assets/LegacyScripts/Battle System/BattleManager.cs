using JinShan;

public class BattleManager : MonoSingleton<BattleManager>
{
    /// <summary>
    /// 开启一场战斗
    /// </summary>
    public void StartBattle()
    {
        //加载页面
        UIManager.Instance.GetWindow<UILoadingWindow>().Loading(0.5f);

        //初始化战斗场景 包括
        //创建地图 创建主角 给主角添加buff
        GameManager.Instance.IniBattle();

        //打开战斗hud
        UIManager.Instance.GetWindow<UICombatHUDWindow>().SetVisible(true);
        UIManager.Instance.GetWindow<UICombatHUDWindow>().SetMainCharacter(GameManager.Instance.mainActor);
    }
}