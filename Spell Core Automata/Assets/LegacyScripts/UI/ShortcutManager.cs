using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JinShan;
public class ShortcutManager : MonoSingleton<ShortcutManager>
{
    public float dummyCreationCooldown = 0.20f;
    private float lastDummyCreationTime = -0.20f;

    private void Update()
    {
        // 检测按键输入
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameSoundManager.Instance.PlaySoundOneTimes("ButtonClick2");
            // 打开暂停页面
            OpenPauseMenu();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            GameSoundManager.Instance.PlaySoundOneTimes("ButtonClick2");
            // 打开背包
            OpenInventory();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            // 放置假人
            if (Time.time - lastDummyCreationTime >= dummyCreationCooldown)
            {
                CreateTestDummy();
                lastDummyCreationTime = Time.time;
            }
        }
    }

    private void CreateTestDummy()
    {
        // 获取鼠标屏幕坐标
        Vector3 mousePosition = Input.mousePosition;

        // 将鼠标屏幕坐标转换为世界坐标
        Camera mainCamera = Camera.main;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, mainCamera.nearClipPlane));

        // 设置 y 坐标为 0
        worldPosition.y = 0;

        // 创建假人
        SceneVariants.CreateCharacter("SkeletonGiant", 2, worldPosition,
            new ChaProperty(
                100, 0, 100,
                10000, 10, 1000, 10,
                0, 100, 0, 0, 0, 0, 0,
                0.5f, 0.5f, MoveType.ground, false), 0f, "骷髅巨人");
    }

    private void OpenPauseMenu()
    {
        // 在这里执行打开暂停页面的操作
        GameManager.Instance.TogglePause();
    }

    private void OpenInventory()
    {

    }
}