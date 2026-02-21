using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WJS;

public class EnemySpawnManager : MonoSingleton<EnemySpawnManager>
{
    public void SpawnEnemy(EnemyModel enemyModel,Vector3 spawnPosition,float degree)
    {
        if(degree == 0f) degree = Random.Range(0, 360f);
        GameObject enemy = SceneVariants.CreateCharacter(enemyModel.prefab,1, spawnPosition,enemyModel.property, degree,enemyModel.name);
        enemy.GetComponent<ChaState>().GetComponentInChildren<ViewContainer>().transform.GetChild(0).gameObject.AddComponent<UnitLockPosition>();
        enemy.AddComponent<SimpleAI>();
        foreach (var buff in enemyModel.addBuffInfos)
        {
            enemy.GetComponent<ChaState>().AddBuff(buff);
        }
    }
}
