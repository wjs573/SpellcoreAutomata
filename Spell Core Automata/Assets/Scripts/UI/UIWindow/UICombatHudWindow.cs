using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WJS;

public class UICombatHudWindow : UIWindow
{
    private ChaState targetChaState;
    public Image hpBar;
    public Image mpBar;
    // Start is called before the first frame update
    private void Start()
    {
        // 获取主角的ChaState
        if (GameManager.Instance.mainCharacter != null)
        {
            targetChaState = GameManager.Instance.mainCharacter.GetComponent<ChaState>();
            UIWindowStack.Instance.PushWindow("UICombatHudWindow");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (targetChaState == null)
        {
            return;
        }
        // 更新UI
        hpBar.fillAmount = (float)targetChaState.resource.hp / targetChaState.property.hp;
        mpBar.fillAmount = (float)targetChaState.resource.mp / targetChaState.property.mp;

    }
}
