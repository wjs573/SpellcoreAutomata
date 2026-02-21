using Sirenix.OdinInspector;
using UnityEngine;
using WJS;

public class UnitCubeThrower : MonoBehaviour
{
    public ThrowableCube cubePrefab;

    public LootCube lootPrefab;
    public Transform throwOrigin;
    [Button("Throw Cube")]
    private void ThrowCube()
    {
        ThrowableCube newCube = Instantiate(cubePrefab, throwOrigin.position, Quaternion.identity, throwOrigin.transform);

        // 订阅触地事件
        newCube.OnGroundHit += HandleCubeHitGround;

        // 抛出
        newCube.RandomThrow();
    }
    [Button("Throw Loot Cube")]
    private void ThrowLootCube()
    {
        LootCube newCube = Instantiate(lootPrefab, throwOrigin.position, Quaternion.identity, throwOrigin.transform);

        // 订阅触地事件
        newCube.OnHitCharacter += HandleCubeHitGround;

        // 抛出
        newCube.RandomThrow();
    }

    private void HandleCubeHitGround(GameObject cube)
    {
        if (cube.GetComponent<ThrowableCube>())
        cube.GetComponent<ThrowableCube>().OnGroundHit -= HandleCubeHitGround;
    }
}