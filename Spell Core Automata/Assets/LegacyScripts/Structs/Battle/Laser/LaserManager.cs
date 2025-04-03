using System.Collections;
using System.Collections.Generic;
using JinShan;
using UnityEngine;

/// <summary>
/// 激光管理器
/// 管理激光的生命周期，出生到死亡，命中目标的处理
/// </summary>
public class LaserManager : MonoSingleton<LaserManager>
{
    public GameObject[] lasers;

    public void ClearLasers()
    {
        lasers = GameObject.FindGameObjectsWithTag("Laser");
        foreach (var laser in lasers)
        {
            LaserState laserState = laser.GetComponent<LaserState>();

            // 如果没有 LaserState 组件，跳过
            if (laserState == null)
                continue;
            laserState.duration = 0f;
        }
    }

    private void FixedUpdate()
    {
        lasers = GameObject.FindGameObjectsWithTag("Laser");
        float timePassed = Time.fixedDeltaTime;
        foreach (var laser in lasers)
        {
            LaserState laserState = laser.GetComponent<LaserState>();

            // 如果没有 LaserState 组件，跳过
            if (laserState == null)
                continue;

            // 如果是刚创建的激光，执行 onCreate 事件
            if (laserState.timeElapsed <= 0 && laserState.model.onCreate != null)
            {
                laserState.model.onCreate.Invoke(laser);
            }
            //处理激光的转向逻辑
            //有两种模式，玩家和npc
            //玩家模式：获取玩家当前鼠标位置，旋转至朝向该位置的角度。
            //自动瞄准模式：要求该单位装备有UnitAutoAim组件，获取目标单位，
            //如果有，则旋转至该单位所在角度。如果没有，则保持原方向。
            if (laserState.AimType == AimType.MouserPosition)
            {
                laserState.SetRotationToMousePosition();
            }
            else
            {
                //自动瞄准最近的攻击目标
            }

            //处理激光命中纪录信息
            int hIndex = 0;
            while (hIndex < laserState.hitRecords.Count)
            {
                laserState.hitRecords[hIndex].timeToCanHit -= timePassed;
                if (laserState.hitRecords[hIndex].timeToCanHit <= 0 || laserState.hitRecords[hIndex].target == null)
                {
                    //理论上应该支持可以鞭尸，所以即使target dead了也得留着……
                    laserState.hitRecords.RemoveAt(hIndex);
                }
                else
                {
                    hIndex += 1;
                }
            }



            // 处理激光的碰撞和效果
            LineRenderer laserRenderer = laserState.Laser;
            Vector4 length = laserState.Length;

            // 设置激光的纹理缩放
            laserRenderer.material.SetTextureScale("_MainTex", new Vector2(length[0], length[1]));
            laserRenderer.material.SetTextureScale("_Noise", new Vector2(length[2], length[3]));

            // 设置激光的位置
            if (laserRenderer != null)
            {
                UpdateLaserPositionAndDirection(laserState);
            }

            // 处理激光的生命周期
            laserState.duration -= timePassed;
            laserState.timeElapsed += timePassed;

            // 如果生命周期结束，销毁激光
            if (laserState.duration <= 0)
            {
                laserState.model.onRemoved?.Invoke(laser);
                Destroy(laser);
            }
        }
    }

    public void UpdateLaserPositionAndDirection(LaserState laserState)
    {
        LineRenderer laserRenderer = laserState.Laser;
        Vector4 length = laserState.Length;
        ParticleSystem[] flasheffects = laserState.FlashEffects;
        ParticleSystem[] hitEffects = laserState.HitEffects;
        GameObject hitEffect = laserState.HitEffectGameObject;
        GameObject flashEffect = laserState.FlashEffectGameObject;
        foreach (var effect in hitEffects)
        {
            if (effect.isPlaying)
            {
                effect.Stop();
            }
        }
        foreach (var effect in flasheffects)
        {
            if (effect.isPlaying)
            {
                effect.Stop();
            }
        }
        float mainTextureLength = 1f;
        float noiseTextureLength = 1f;
        laserRenderer.SetPosition(0, laserState.firePositionTransform.position);
        // 如果激光未碰撞物体，设置激光的结束位置为最大长度
        Vector3 endPos = laserState.firePositionTransform.position + laserState.firePositionTransform.forward * laserState.model.MaxLength;

        RaycastHit[] hits = Physics.RaycastAll(laserState.firePositionTransform.position, laserState.firePositionTransform.TransformDirection(Vector3.forward), laserState.model.MaxLength);
        // 按照距离从近到远排序
        System.Array.Sort(hits, (hit1, hit2) => hit1.distance.CompareTo(hit2.distance));

        List<GameObject> hitCharacters = new List<GameObject>();
        if (hits.Length > 0)
        {
            int penetrationCount = laserState.model.penetrationCount;
            Vector3 endPoint = laserState.firePositionTransform.position + laserState.firePositionTransform.forward * laserState.model.MaxLength;
            foreach (var hit in hits)
            {
                if (penetrationCount <= 0)
                {
                    break;
                }

                ChaState characterState = hit.collider.GetComponent<ChaState>();
                if (characterState != null && !characterState.dead
                    && characterState.side != laserState.caster.GetComponent<ChaState>().side)
                {
                    penetrationCount--;

                    // 更新激光的终点为最后一个可命中敌人的位置
                    endPoint = hit.point;

                    // 处理激光命中逻辑
                    HandleLaserHit(laserState, hit.collider);
                    hitCharacters.Add(hit.collider.gameObject);
                }
                if (penetrationCount > 0)
                {
                    endPoint = endPos;
                }
            }

            // 设置激光的结束位置
            laserRenderer.SetPosition(1, endPoint);

            // 播放激光发射效果
            flashEffect.transform.position = laserState.firePositionTransform.position;
            flashEffect.transform.LookAt(endPoint);

            // 更新激光命中效果
            laserState.UpdateHitEffect(hitCharacters);
            foreach (GameObject hitTarget in hitCharacters)
            {
                laserState.HitEffectsDict[hitTarget].transform.LookAt(laserState.firePositionTransform.position);
                laserState.HitEffectsDict[hitTarget].transform.Rotate(0, 180, 0);
                foreach (var effect in laserState.HitEffectsDict[hitTarget].GetComponentsInChildren<ParticleSystem>())
                {
                    if (!effect.isPlaying)
                    {
                        effect.Play();
                    }
                }
            }

            foreach (var effect in flasheffects)
            {
                if (!effect.isPlaying)
                {
                    effect.Play();
                }
            }
            foreach (var effect in hitEffects)
            {
                if (!effect.isPlaying)
                {
                    effect.Play();
                }
            }
            // 更新纹理坐标
            length[0] = mainTextureLength * (Vector3.Distance(laserState.firePositionTransform.position, endPoint));
            length[2] = noiseTextureLength * (Vector3.Distance(laserState.firePositionTransform.position, endPoint));
        }
        else
        {
            // 如果激光未碰撞物体，设置激光的结束位置为最大长度
            laserRenderer.SetPosition(1, endPos);
            hitEffect.transform.position = endPos;
            foreach (var effect in flasheffects)
            {
                if (!effect.isPlaying)
                {
                    effect.Play();
                }
            }

            // 更新纹理坐标
            length[0] = mainTextureLength * (Vector3.Distance(laserState.firePositionTransform.position, endPos));
            length[2] = noiseTextureLength * (Vector3.Distance(laserState.firePositionTransform.position, endPos));
        }
    }


    /// <summary>
    /// 处理激光命中逻辑
    /// </summary>
    private void HandleLaserHit(LaserState laserState, Collider collider)
    {
        int laserSide = GetLaserSide(laserState.caster);
        ChaState characterState = collider.GetComponent<ChaState>();

        if (characterState != null && laserState.CanHit(collider.gameObject))
        {
            // 根据设定条件判断是否命中角色
            if ((laserState.model.hitAlly == false && laserSide == characterState.side) ||
                (laserState.model.hitFoe == false && laserSide != characterState.side))
            {
                return;
            }

            // 对命中的目标执行OnHit函数
            if (laserState.model.onHit != null)
            {
                laserState.model.onHit.Invoke(laserState.gameObject, characterState.gameObject);
            }

            // 记录命中目标
            laserState.AddHitRecord(collider.gameObject);
        }
    }

    /// <summary>
    /// 获取激光的立场
    /// </summary>
    private int GetLaserSide(GameObject caster)
    {
        int side = -1;
        if (caster != null)
        {
            ChaState characterState = caster.GetComponent<ChaState>();
            if (characterState != null)
            {
                side = characterState.side;
            }
        }
        return side;
    }
}