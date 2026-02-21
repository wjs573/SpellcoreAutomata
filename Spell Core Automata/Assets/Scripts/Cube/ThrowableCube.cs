using UnityEngine;
using System;
using Sirenix.OdinInspector;
using WJS;

public class ThrowableCube : MonoBehaviour
{
    // 抛出参数范围
    public float minThrowForce = 5f;
    public float maxThrowForce = 15f;
    public float minUpwardForce = 2f;
    public float maxUpwardForce = 8f;

    // 地面标签
    public string groundTag = "Ground";

    // 触地回调事件
    public event Action<GameObject> OnGroundHit;

    private Rigidbody rb;
    private bool hasBeenThrown = false;
    private bool hasHitGround = false;

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
        if (!hasBeenThrown || hasHitGround) return;

        // 检查是否碰撞到地面
        if (collision.gameObject.CompareTag(groundTag))
        {
            hasHitGround = true;

            // 触发回调
            OnGroundHit?.Invoke(gameObject);
            SceneVariants.CreateSightEffect("FlashExplosionRadial", transform.position, 0f);
            gameObject.AddComponent<UnitRemover>().duration = 0.2f;

            EnemySpawnManager.Instance.SpawnEnemy(EnemyData.data["Skeleton"],transform.position, 0f);
        }
    }

    // 重置Cube状态
    public void ResetCube()
    {
        hasBeenThrown = false;
        hasHitGround = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}