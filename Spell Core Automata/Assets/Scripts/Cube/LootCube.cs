using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WJS;

public class LootCube : MonoBehaviour
{
    public string equipmentName = "星辉棒";
    // 抛出参数范围
    public float minThrowForce = 3f;
    public float maxThrowForce = 10f;
    public float minUpwardForce = 2f;
    public float maxUpwardForce = 8f;

    // 触地回调事件
    public event Action<GameObject> OnHitCharacter;

    private Rigidbody rb;
    private bool hasBeenThrown = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        // 初始时禁用物理
        rb.isKinematic = true;
    }

    // 抛出Cube的方法
    // 随机抛出Cube的方法
    public void RandomThrow()
    {
        if (hasBeenThrown) return;

        hasBeenThrown = true;
        rb.isKinematic = false;

        // 随机角度 (0-360度)
        float randomAngle = UnityEngine.Random.Range(0f, 360f);

        // 将角度转换为XZ平面方向向量
        Vector3 direction = new Vector3(
            Mathf.Sin(randomAngle * Mathf.Deg2Rad),
            0,
            Mathf.Cos(randomAngle * Mathf.Deg2Rad)
        );

        // 随机力度
        float throwForce = UnityEngine.Random.Range(minThrowForce, maxThrowForce);
        float upwardForce = UnityEngine.Random.Range(minUpwardForce, maxUpwardForce);

        // 应用力
        Vector3 force = direction * throwForce + Vector3.up * upwardForce;
        rb.AddForce(force, ForceMode.Impulse);
    }

    // 碰撞检测
    private void OnCollisionEnter(Collision collision)
    {
        // 检查是否碰撞到地面
        if (collision.gameObject.GetComponent<ChaState>() && collision.gameObject==SceneVariants.mainChacter)
        {
            // 触发回调
            OnHitCharacter?.Invoke(gameObject);
            SceneVariants.CreateSightEffect("FlashExplosionRadial", transform.position, 0f);
            gameObject.AddComponent<UnitRemover>().duration = 0.2f;
            if(EquipmentData.data.ContainsKey(equipmentName))
            SceneVariants.mainChacter.GetComponent<UnitBackpack>().AddItem(new EquipmentObj(EquipmentData.data[equipmentName]));
        }
    }

    
}
