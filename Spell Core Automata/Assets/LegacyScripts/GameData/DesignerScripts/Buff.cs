using System;
using System.Collections.Generic;
using System.Linq;
using MoreMountains.Tools;
using UnityEngine;

namespace DesignerScripts
{
    ///<summary>
    ///buff的效果
    ///</summary>
    public class Buff
    {
        public static Dictionary<string, BuffOnOccur> onOccurFunc = new Dictionary<string, BuffOnOccur>(){
            {"TheFallingHeartFlameOnCreate",TheFallingHeartFlameOnCreate },
            {"ColdOnOccur",ColdOnOccur },
            {"CreateAoEOnCreate",CreateAoEOnCreate},
            {"EnterInvisibilityState",EnterInvisibilityState },
            {"JumpUpOnCreate",JumpUpOnCreate},
            {"ResurrectOnCreate",ResurrectOnCreate }
        };

        public static Dictionary<string, BuffOnRemoved> onRemovedFunc = new Dictionary<string, BuffOnRemoved>(){
            {"TeleportCarrier", TeleportCarrier},
            {"AddBuff" ,AddBuff},
            {"SwordOnRemove",SwordOnRemove },
            {"TheFallingHeartFlameOnRemove",TheFallingHeartFlameOnRemove },
            {"DeadOnRemoved",DeadOnRemoved},
            {"ExitInvisibilityState",ExitInvisibilityState },
            {"ResurrectOnRemoved", ResurrectOnRemoved}
        };


        public static Dictionary<string, BuffOnTick> onTickFunc = new Dictionary<string, BuffOnTick>(){
            {"BarrelDurationLose", BarrelDurationLose},
            {"BaseRecover", BaseRecover},
            {"FlyingSwordTick",FlyingSwordTick },
            {"PoisoningDamageOnTick",PoisoningDamageOnTick },
            {"RecoverPercentMaxHp", RecoverPercentMaxHp},
            {"TurretSalvo",TurretSalvo },
            {"IgniteDamageOnTick",IgniteDamageOnTick },
            {"PercentDamageOnTick",PercentDamageOnTick },
            {"DoBleedingDamageOnTick",DoBleedingDamageOnTick }
        };

        public static Dictionary<string, BuffOnCast> onCastFunc = new Dictionary<string, BuffOnCast>(){
            {"FireTeleportBullet", FireTeleportBullet},
            {"EnduredDamageModified", EnduredDamageModified},
            {"PoisoningDamageOnCast",PoisoningDamageOnCast }
        };

        public static Dictionary<string, BuffOnHit> onHitFunc = new Dictionary<string, BuffOnHit>(){
            {"DamageModification",DamageModification },
            {"RecordHitTarget",RecordHitTarget }
        };

        public static Dictionary<string, BuffOnBeHurt> beHurtFunc = new Dictionary<string, BuffOnBeHurt>(){
            {"OnlyTakeOneDirectDamage", OnlyTakeOneDirectDamage},
            {"BurnDamageOnHurt",BurnDamageOnHurt },
            {"FreezeOnHurt",FreezeOnHurt },
            {"CalculateModifiedDamage",CalculateModifiedDamage },
            {"RemoveBuffOnPercentHp",RemoveBuffOnPercentHp }
        };

        public static Dictionary<string, BuffOnKill> onKillFunc = new Dictionary<string, BuffOnKill>()
        {

        };

        public static Dictionary<string, BuffOnBeKilled> beKilledFunc = new Dictionary<string, BuffOnBeKilled>(){
            {"BarrelExplosed", BarrelExplosed},
            {"ImmortalityOnDead", ImmortalityOnDead},
            {"ReturnMind",ReturnMind },
            {"GetHealOnDead",GetHealOnDead },
            {"AddMaxHPToCasterOnBeKilled",AddMaxHPToCasterOnBeKilled },
            {"CreateAoEOnDead", CreateAoEOnDead},
            {"RelayDebuffOnDeath",RelayDebuffOnDeath}
        };

        private static void RelayDebuffOnDeath(BuffObj buff, DamageInfo damageInfo, GameObject attacker)
        {
            ChaState buffCarrier = buff.carrier.GetComponent<ChaState>();
            List<BuffObj> debuffs = new List<BuffObj>();
            foreach (BuffObj buffObj in buffCarrier.buffs)
            {
                if (buffObj.model.tags.Contains("debuff") && buffObj.model.id != "DeathRelayDebuff")
                {
                    debuffs.Add(buffObj);
                }
            }
            GameObject closestAlly = null;
            float minDistance = float.MaxValue; // 初始化最小距离为一个很大的值
            foreach (GameObject character in GameManager.Instance.Characters)
            {
                ChaState charState = character.GetComponent<ChaState>();
                if (charState.side == buffCarrier.side && character != buff.carrier) // 确保同阵营且不是自身
                {
                    float distance = Vector3.Distance(character.transform.position, buffCarrier.transform.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        closestAlly = character; // 更新最近的友军
                    }
                }
            }

            if (debuffs.Count > 0 && closestAlly != null)
            {
                BuffObj relayedBuff = debuffs[UnityEngine.Random.Range(0, debuffs.Count)];
                AddBuffInfo addBuffInfo = new AddBuffInfo(relayedBuff.model, attacker, buff.carrier, relayedBuff.stack, relayedBuff.duration);
                closestAlly.GetComponent<ChaState>().AddBuff(addBuffInfo);
            }
        }

        /// <summary>
        /// 流血伤害，在三秒内持续受到伤害，每0.25秒结算一次
        /// </summary>
        /// <param name="buff"></param>
        private static void DoBleedingDamageOnTick(BuffObj buff)
        {
            int remainBleedingDamage = 0;
            if (buff.buffParam.ContainsKey("RemainBleedingDamage"))
            {
                remainBleedingDamage = (int)buff.buffParam["RemainBleedingDamage"];
                int remainTickTimes = (int)(buff.duration / 0.25f);
                remainTickTimes = remainTickTimes <= 0 ? 1 : remainTickTimes;
                int tickBleedingDamage = -remainBleedingDamage / remainTickTimes;
                tickBleedingDamage = tickBleedingDamage >= 0 ? -1 : tickBleedingDamage;
                remainBleedingDamage += tickBleedingDamage;
                buff.buffParam["RemainBleedingDamage"] = remainBleedingDamage;

                DamageInfo damageInfo = new DamageInfo(buff.caster, buff.carrier, new Damage(tickBleedingDamage),
                    0f, 0f, new DamageInfoTag[] { DamageInfoTag.periodHeal });

                SceneVariants.CreateDamage(damageInfo);
            }
            else
            {
                buff.buffParam.Add("RemainBleedingDamage", remainBleedingDamage);
            }

        }


        private static void ResurrectOnCreate(BuffObj buff, int modifyStack)
        {
            if (buff.carrier.GetComponent<CapsuleCollider>())
            {
                buff.carrier.GetComponent<CapsuleCollider>().enabled = false;
            }
            if (buff.carrier.GetComponentInChildren<AIBrain>())
            {
                buff.carrier.GetComponentInChildren<AIBrain>().BrainActive = false;
            }
            buff.carrier.GetComponent<UnitAnim>().BufferAnimation = "Spawn";
            buff.carrier.GetComponent<ChaState>().SetImmuneTime(0.80f);
        }

        private static void ResurrectOnRemoved(BuffObj buff)
        {
            if (buff.carrier.GetComponent<CapsuleCollider>())
            {
                buff.carrier.GetComponent<CapsuleCollider>().enabled = true;
            }
            if (buff.carrier.GetComponentInChildren<AIBrain>())
            {
                buff.carrier.GetComponentInChildren<AIBrain>().BrainActive = true;
            }
        }


        private static void RemoveBuffOnPercentHp(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker)
        {
            object[] Params = buff.model.onBeHurtParams;
            float hpPercent = (float)Params[0];
            ChaState chaState = buff.carrier.GetComponent<ChaState>();
            if (chaState && chaState.resource.hp <= (int)(chaState.property.hp * hpPercent))
            {
                buff.duration = 0f;
            }
        }

        private static void CreateAoEOnDead(BuffObj buff, DamageInfo damageInfo, GameObject attacker)
        {
            AoeLauncher aoeLauncher = new AoeLauncher(DesignerTables.AoE.data["ThunderStrikeField"],
                    null, buff.carrier.transform.position, 3f, 0.5f, 0f);
            aoeLauncher.caster = buff.caster;
            aoeLauncher.position = buff.carrier.transform.position;
            SceneVariants.CreateAoE(aoeLauncher);
        }

        private static void IgniteDamageOnTick(BuffObj buff)
        {
            if( buff.caster==null) return;
            ChaState casterChaState = buff.caster.GetComponent<ChaState>();
            ChaState carrierChaState = buff.carrier.GetComponent<ChaState>();
            if (casterChaState == null || carrierChaState == null)
            {
                return;
            }
            int igniteDamageValue = (int)(casterChaState.property.attack * 0.2f + 10) * buff.stack;
            DamageInfo igniteDamageInfo = new DamageInfo(buff.caster, buff.carrier,
                new Damage(igniteDamageValue), 0f, 0f,
                new DamageInfoTag[] { DamageInfoTag.periodDamage });
            SceneVariants.CreateDamage(igniteDamageInfo);
        }

        private static void PercentDamageOnTick(BuffObj buff)
        {
            ChaState casterChaState = buff.caster.GetComponent<ChaState>();
            ChaState carrierChaState = buff.carrier.GetComponent<ChaState>();
            if (casterChaState == null || carrierChaState == null)
            {
                return;
            }
            int Damagedalue = (int)(carrierChaState.property.hp * 0.01f);
            DamageInfo DamageInfo = new DamageInfo(buff.caster, buff.carrier,
                new Damage(Damagedalue), 0f, 0.2f,
                new DamageInfoTag[] { DamageInfoTag.periodDamage });
            SceneVariants.CreateDamage(DamageInfo);
        }

        private static void AddMaxHPToCasterOnBeKilled(BuffObj buff, DamageInfo damageInfo, GameObject attacker)
        {
            //拿到buff施加者和buff携带者的角色状态
            ChaState casterChaState = buff.caster.GetComponent<ChaState>();
            ChaState carrierChaState = buff.carrier.GetComponent<ChaState>();
            if (casterChaState == null || carrierChaState == null)
            {
                return;
            }

            //尝试在角色状态中找到吞噬属性记录buff
            List<BuffObj> buffObjs = casterChaState.GetBuffById("DevourChaPropertyRecord");
            if (buffObjs.Count <= 0)
            {
                //如果找不到，则直接给buff施加者添加一层永久的吞噬属性
                AddBuffInfo addBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["DevourChaPropertyRecord"],
                    buff.caster, buff.caster, 1, 10f, true, true, null);
                casterChaState.AddBuff(addBuffInfo);
                buffObjs = casterChaState.GetBuffById("DevourChaPropertyRecord");
            }

            //计算需要吞噬最大生命值的数值
            //后期可以从策划脚本中获取计算公式
            int MaxHpToAdd = Mathf.RoundToInt(carrierChaState.property.hp * 0.01f);

            //在吞噬属性记录中添加吞噬的最大生命值
            buffObjs[0].model.propMod[0].hp += MaxHpToAdd;

            //调用跳字系统，在吞噬者头顶播放吞噬信息

            //重新计算角色属性
            casterChaState.AttrRecheck();
        }

        private static void JumpUpOnCreate(BuffObj buff, int modifyStack)
        {
            buff.carrier.GetComponent<JumpingY>().JumpStart(buff.duration, 2f);
        }

        /// <summary>
        /// 创建一轮炮塔齐射
        /// </summary>
        /// <param name="buff"></param>
        private static void TurretSalvo(BuffObj buff)
        {
            if (!buff.buffParam.ContainsKey("turrets"))
            {
                return;
            }
            List<GameObject> turrets = (List<GameObject>)buff.buffParam["turrets"];
            if (turrets.Count <= 0)
            {
                return;
            }

            if (!buff.buffParam.ContainsKey("target"))
            {
                return;
            }

            GameObject target = (GameObject)buff.buffParam["target"];
            if (target == null)
            {
                return;
            }

            foreach (GameObject turret in turrets)
            {
                if (turret == null)
                {
                    continue;
                }
                ChaState turrectState = turret.GetComponent<ChaState>();
                turrectState.CastSkill("LaunchingHighSpeedShell");
            }
        }

        private static void TurretRotateToTarget(List<GameObject> turrets, GameObject target)
        {
            foreach (GameObject turret in turrets)
            {
                if (turret == null)
                {
                    continue;
                }
                ChaState turrectState = turret.GetComponent<ChaState>();
                turrectState.RotateToTarget(target);
            }
        }

        /// <summary>
        /// 记录当前攻击的目标
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="damageInfo"></param>
        /// <param name="target"></param>
        private static void RecordHitTarget(BuffObj buff, ref DamageInfo damageInfo, GameObject target)
        {
            if (damageInfo.attacker.GetComponent<ChaState>().side == target.GetComponent<ChaState>().side)
            {
                return;
            }

            if (buff.buffParam.ContainsKey("target"))
            {
                buff.buffParam["target"] = target;
            }
            else
            {
                buff.buffParam.Add("target", target);
            }
            if (!buff.buffParam.ContainsKey("turrets"))
            {
                return;
            }
            List<GameObject> turrets = (List<GameObject>)buff.buffParam["turrets"];
            TurretRotateToTarget(turrets, (GameObject)buff.buffParam["target"]);

            return;
        }

        private static void GetHealOnDead(BuffObj buff, DamageInfo damageInfo, GameObject attacker)
        {
            object[] Params = buff.model.onBeKilledParams;
            DamageInfo healInfo = (DamageInfo)Params[0];
            healInfo.attacker = buff.caster;
            healInfo.defender = buff.caster;

            //致死的伤害信息应该设置为0
            damageInfo.damage = new Damage(0);

            SceneVariants.CreateDamage(healInfo);
        }

        /// <summary>
        /// 离开隐身状态 美术效果的实现
        /// 角色会离开半透明化状态
        /// 原理是将unity standard shader的render mode设置为opague
        /// 同时 将color.a设置为255
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="modifyStack"></param>
        private static void ExitInvisibilityState(BuffObj buff)
        {
            GameObject cha = buff.carrier;
            foreach (Renderer renderer in cha.GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in renderer.materials)
                {
                    if (material.shader.name == "Standard")
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
                        material.SetInt("_ZWrite", 1);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = -1;

                        Color color = material.color;
                        color.a = 1f; // Reset alpha to 1
                        material.color = color;
                    }
                }
            }
        }

        /// <summary>
        /// 进入隐身状态 美术效果的实现
        /// 角色会进入半透明化状态
        /// 原理是将unity standard shader的render mode设置为transparent
        /// 同时 将color.a设置为100
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="modifyStack"></param>
        private static void EnterInvisibilityState(BuffObj buff, int modifyStack)
        {
            //如果不是添加一层buff
            if (modifyStack != 1)
            {
                return;
            }

            GameObject cha = buff.carrier;
            foreach (Renderer renderer in cha.GetComponentsInChildren<Renderer>())
            {
                foreach (Material material in renderer.materials)
                {
                    if (material.shader.name == "Standard")
                    {
                        material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
                        material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                        material.SetInt("_ZWrite", 0);
                        material.DisableKeyword("_ALPHATEST_ON");
                        material.DisableKeyword("_ALPHABLEND_ON");
                        material.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                        material.renderQueue = 3000;

                        Color color = material.color;
                        color.a = 100 / 255f; // Unity uses alpha values between 0 and 1
                        material.color = color;
                    }
                }
            }
        }

        /// <summary>
        /// 召唤物死亡后将神念返还给召唤者
        /// 召唤者、神念强度记录在BuffParam中
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="damageInfo"></param>
        /// <param name="attacker"></param>
        private static void ReturnMind(BuffObj buff, DamageInfo damageInfo, GameObject attacker)
        {
            if (buff.buffParam.ContainsKey("Summoner") && buff.buffParam.ContainsKey("Mind"))
            {
                GameObject Summoner = (GameObject)buff.buffParam["Summoner"];
                int Mind = (int)buff.buffParam["Mind"];

                ChaState chaState = Summoner.GetComponent<ChaState>();
                if (chaState)
                {
                    chaState.resource.mind += Mind;
                }
            }
        }

        /// <summary>
        /// 每次tick恢复百分比最大生命值
        /// </summary>
        /// <param name="buff"></param>
        private static void RecoverPercentMaxHp(BuffObj buff)
        {
            ChaState chaState = buff.carrier.GetComponent<ChaState>();
            float percent = (float)buff.model.onTickParams[0];
            int PercentMaxHp = Mathf.RoundToInt(chaState.property.hp * percent);
            DamageInfo damageInfo = new DamageInfo(buff.carrier, buff.carrier, new Damage(-PercentMaxHp), 0f, 0f, new DamageInfoTag[] { });
            SceneVariants.CreateDamage(damageInfo);
        }

        /// <summary>
        /// 不朽buff携带者hp归零时
        /// hp强制恢复至百分之10最大生命值
        /// 进入0.5秒的无敌时间
        /// 移除身上的debuff
        /// 获得持续2秒的重生buff
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="damageInfo"></param>
        /// <param name="attacker"></param>
        private static void ImmortalityOnDead(BuffObj buff, DamageInfo damageInfo, GameObject attacker)
        {
            buff.duration = 0f;

            ChaState chaState = buff.carrier.GetComponent<ChaState>();
            chaState.resource.hp = Mathf.RoundToInt(chaState.property.hp * 0.1f);
            chaState.immuneTime = 0.5f;

            foreach (BuffObj buffObj in chaState.buffs)
            {
                for (int i = 0; i < buffObj.model.tags.Length; i++)
                {
                    if (buffObj.model.tags[i] == "debuff")
                    {
                        buffObj.duration = 0f;
                    }
                }
            }
            AddBuffInfo addBuffInfo = new AddBuffInfo(DesignerTables.Buff.data["Resurgence"], chaState.gameObject, chaState.gameObject, 1, 2f);

            chaState.AddBuff(addBuffInfo);
        }

        private static void DeadOnRemoved(BuffObj buff)
        {
            ChaState chaState = buff.carrier.GetComponent<ChaState>();
            if (chaState != null)
            {
                chaState.SetImmuneTime(0f);
                chaState.Kill();
            }
        }

        /// <summary>
        /// buff创建时 创建一个aoe
        /// 参数0 AoeLauncher
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="modifyStack"></param>
        private static void CreateAoEOnCreate(BuffObj buff, int modifyStack)
        {
            //如果层数为1 且修改层数为1(说明是从0到1嘛)
            if (buff.stack == 1 && modifyStack == 1)
            {
                object[] p = buff.model.onOccurParams;

                AoeLauncher aoeLauncher;
                if (p.Length > 0)
                {
                    aoeLauncher = (AoeLauncher)p[0];
                }

                aoeLauncher = new AoeLauncher(DesignerTables.AoE.data["RockExplosion"],
                    null, Vector3.zero, 5f, 0.9f, 0f);
                aoeLauncher.caster = buff.caster;
                aoeLauncher.position = buff.carrier.transform.position;
                SceneVariants.CreateAoE(aoeLauncher);
            }
        }

        /// <summary>
        /// 通用型伤害修正函数 受伤回调点
        /// 例如 畏火 就是 火属性 直接伤害 翻倍
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="damageInfo"></param>
        /// <param name="attacker"></param>
        private static void CalculateModifiedDamage(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker)
        {
            object[] p = buff.model.onBeHurtParams;

            DamageInfoTag damageInfoTag = p.Length > 1 ? (DamageInfoTag)p[0] : DamageInfoTag.directDamage;

            string damageType = p.Length > 2 ? (string)p[1] : "";
            float damagedamageModifier = p.Length > 2 ? (float)p[2] : 1f;

            //例如 输入火属性 返回一个只有火属性为1的伤害，用它乘上原始伤害，再乘上伤害倍率
            Damage damage = Damage.GetDamageByString(damageType) * damageInfo.damage * damagedamageModifier;

            damageInfo.damage += damage;
        }

        /// <summary>
        /// 寒冷buff的层数更新回调函数
        /// 当寒冷达到5层时 移除寒冷buff
        /// 获得冻结buff
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="modifyStack"></param>
        private static void ColdOnOccur(BuffObj buff, int modifyStack)
        {
            if (buff.carrier.GetComponent<ChaState>() != null && buff.stack >= 5)
            {
                ChaState carrierChaState = buff.carrier.GetComponent<ChaState>();
                if (carrierChaState.GetBuffById("Freeze").Count != 0)
                {
                    return;
                }
                buff.duration = 0;

                AddBuffInfo info = new AddBuffInfo(DesignerTables.Buff.data["Freeze"], buff.caster, buff.carrier, 1, 1f, true, true, null);
                buff.carrier.GetComponent<ChaState>().AddBuff(info);
            }
        }

        /// <summary>
        /// 冻结 受到伤害回调点
        /// 受到直接伤害时，额外受到一次水属性伤害
        /// 将buff的时间设置为0
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="damageInfo"></param>
        /// <param name="attacker"></param>
        private static void FreezeOnHurt(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker)
        {
            ChaState chaState = buff.carrier.GetComponent<ChaState>();

            if (chaState == null)
            {
                return;
            }

            //如果这次伤害是一次直接伤害
            //额外受到一次水属性伤害
            for (int i = 0; i < damageInfo.tags.Length; i++)
            {
                if (damageInfo.tags[i] == DamageInfoTag.directDamage)
                {
                    DamageInfo freeze_damageInfo = new DamageInfo(buff.caster, buff.carrier,
               new Damage(100), 0f, 0f,
               new DamageInfoTag[] { DamageInfoTag.periodDamage });

                    SceneVariants.CreateDamage(freeze_damageInfo);
                }
            }

            return;
        }

        /// <summary>
        /// 烧伤状态下 受伤时回调函数
        /// 在烧伤状态下，每受到一次直接伤害
        /// 就会触发一次基于buff层数的火属性伤害
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="damageInfo"></param>
        /// <param name="attacker"></param>
        private static void BurnDamageOnHurt(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker)
        {
            ChaState chaState = buff.carrier.GetComponent<ChaState>();

            if (chaState == null)
            {
                return;
            }

            //如果这次伤害是一次直接伤害
            //额外受到一次火属性伤害
            for (int i = 0; i < damageInfo.tags.Length; i++)
            {
                if (damageInfo.tags[i] == DamageInfoTag.directDamage)
                {
                    DamageInfo burn_damageInfo = new DamageInfo(buff.caster, buff.carrier,
               new Damage(buff.stack * 10), 0f, 0f,
               new DamageInfoTag[] { DamageInfoTag.periodDamage });

                    SceneVariants.CreateDamage(burn_damageInfo);
                }
            }
        }

        /// <summary>
        /// 中毒状态下 施法回调点函数
        /// 施法时会造成一次伤害
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="skill"></param>
        /// <param name="timeline"></param>
        /// <returns></returns>
        private static TimelineObj PoisoningDamageOnCast(BuffObj buff, SkillObj skill, TimelineObj timeline)
        {
            PoisoningDamageOnTick(buff);
            return timeline;
        }

        /// <summary>
        /// 中毒秒伤buff
        /// 每秒造成伤害基于buff层数，木灵根加成的伤害
        /// </summary>
        /// <param name="buff"></param>
        private static void PoisoningDamageOnTick(BuffObj buff)
        {
            ChaState chaState = buff.carrier.GetComponent<ChaState>();

            //如果角色状态不存在 或者 角色死亡 则跳过
            if (chaState == null || chaState.dead)
            {
                return;
            }

            //创建一条伤害信息
            //中毒buff释放者 给 携带者 造成伤害
            DamageInfo damageInfo = DesignerScripts.CommonScripts.PoisoningDamage(chaState, buff);

            //中毒层数减半
            BuffObj buffObjs = chaState.GetBuffById("Poisoning") == null ? null : chaState.GetBuffById("Poisoning")[0];
            if (buffObjs.stack == 1)
            {
                buffObjs.stack = 0;
            }
            buffObjs.stack = Mathf.RoundToInt(buffObjs.stack - 1);

            //添加一条伤害信息
            SceneVariants.CreateDamage(damageInfo);
        }

        /// <summary>
        /// 陨落心炎 卸载时函数
        /// 读取参数记录的aoe 移除之
        /// </summary>
        /// <param name="buff"></param>
        private static void TheFallingHeartFlameOnRemove(BuffObj buff)
        {
            if (buff.buffParam.ContainsKey("Aoe"))
            {
                SceneVariants.RemoveAoE(buff.buffParam["Aoe"] as GameObject, true);
            }
        }

        /// <summary>
        /// 陨落心炎创建时方法
        /// 创建一个aoe 跟随着玩家移动
        /// 在param中记录aoe的应用
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="modifyStack"></param>
        private static void TheFallingHeartFlameOnCreate(BuffObj buff, int modifyStack)
        {
            if (buff.buffParam.ContainsKey("Aoe"))
            {
                return;
            }

            AoeLauncher aoeLauncher = new AoeLauncher(DesignerTables.AoE.data["BellRing"],
            buff.carrier, Vector3.zero, 5f, 9999f, 0f, DesignerScripts.AoE.aoeTweenFunc["AroundCaster_1"]);

            buff.buffParam.Add("Aoe", SceneVariants.CreateAoE(aoeLauncher));
        }

        /// <summary>
        /// 飞剑术buff
        /// 如果处于收剑状态 无效果
        /// 处于出剑状态，每秒消耗法力值，法力值低于使用每秒消耗值时，自动使用飞剑术技能，进行收剑
        /// 参数1
        /// </summary>
        /// <param name="buff"></param>
        private static void FlyingSwordTick(BuffObj buff)
        {
            //Debug.Log(0);

            if (!buff.buffParam.ContainsKey("IsOut"))
            {
                //Debug.Log(2);
                buff.buffParam["IsOut"] = false;
            }

            //是否处于出剑状态
            ChaState chaState = buff.carrier.GetComponent<ChaState>();

            //如果处于出剑状态 每秒消耗10点法力值
            if ((bool)buff.buffParam["IsOut"])
            {
                //Debug.Log(1);
                //如果灵力值小于10 收剑
                if (chaState.resource.mp <= 10)
                {
                    //法宝收剑逻辑
                    //移除飞剑子弹
                    SceneVariants.RemoveBullet(buff.buffParam["bullet"] as GameObject, true);
                    //重新显示飞剑武器
                    //Debug.Log(buff.model.id);

                    //切换状态
                    buff.buffParam["IsOut"] = false;
                }
                chaState.ModResource(new ChaResource(0, -20, 0));
            }
        }

        /// <summary>
        /// 装备buff 移除回调点添加事件：移除时读取buff的参数，如果处于出剑状态，则移除子弹。
        /// </summary>
        /// <param name="buff"></param>
        private static void SwordOnRemove(BuffObj buff)
        {
            if (buff.buffParam.ContainsKey("bullet"))
            {
                //移除飞剑子弹
                SceneVariants.RemoveBullet(buff.buffParam["bullet"] as GameObject, true);
                //切换状态
                buff.buffParam["IsOut"] = false;
            }
        }

        /////<summary>
        /////onCast
        /////如果子弹不够放技能，就会填装子弹
        /////no params
        /////</summary>
        //private static TimelineObj ReloadAmmo(BuffObj buff, SkillObj skill, TimelineObj timeline){
        //    ChaState cs = buff.carrier.GetComponent<ChaState>();
        //    return (cs.resource.Enough(skill.model.cost) == true) ? timeline :
        //        new TimelineObj(DesingerTables.Timeline.data["skill_reload"], buff.carrier, new object[0]);
        //}

        /// <summary>
        /// OnCast
        /// 释放 飞剑 时
        /// 把伤害修正系数调整为buff 存续时间乘百分之一的倍率提升
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="skill"></param>
        /// <param name="timeline"></param>
        /// <returns></returns>
        private static TimelineObj EnduredDamageModified(BuffObj buff, SkillObj skill, TimelineObj timeline)
        {
            object[] p = buff.model.onCastParams;

            //是否处于卧薪尝胆阶段
            bool IsEndured = p.Length > 0 ? (bool)p[0] : false;

            //如果处于卧薪尝胆阶段 且 释放技能为飞剑
            if (IsEndured && skill.model.id == "flyingSword")
            {
                //结束卧薪尝胆阶段
                buff.model.onCastParams[0] = false;

                //伤害修正系数调整为强化版本
                buff.model.onHitParams[0] = 1f + buff.timeElapsed / 100;
            }

            return timeline;
        }

        /// <summary>
        /// OnRemove
        /// 添加buff
        /// </summary>
        /// <param name="buff"></param>
        private static void AddBuff(BuffObj buff)
        {
            object[] p = buff.model.onRemovedParams;

            //buff施加的对象
            string buffId = p.Length > 0 ? (string)p[0] : "";
            if (!DesignerTables.Buff.data.ContainsKey(buffId))
            {
                return;
            }
            //buff施加的对象
            int stack = p.Length > 1 ? (int)p[1] : 1;
            float duration = p.Length > 2 ? (float)p[2] : 3f;
            bool isSetTo = p.Length > 3 ? (bool)p[3] : false;
            bool isPermanent = p.Length > 3 ? (bool)p[3] : false;
            //添加buff的信息
            AddBuffInfo buffInfo = new AddBuffInfo(DesignerTables.Buff.data[buffId], buff.carrier, buff.carrier, stack, duration, isSetTo, isPermanent);

            ChaState targetChastate = buff.carrier.GetComponent<ChaState>();
            if (targetChastate == null)
            {
                targetChastate.AddBuff(buffInfo);
            }
        }

        /// <summary>
        /// OnHit
        /// 伤害修正
        /// 参数0 修正比例
        /// </summary>
        /// <param name="buff"></param>
        /// <param name="damageInfo"></param>
        /// <param name="target"></param>
        private static void DamageModification(BuffObj buff, ref DamageInfo damageInfo, GameObject target)
        {
            object[] p = buff.model.onHitParams;
            //伤害修正系数
            float damageTimes = p.Length > 0 ? (float)p[0] : 1;
            //修正伤害信息
            if (damageInfo.tags[0] == DamageInfoTag.directDamage)
            {
                damageInfo.damage *= damageTimes;
            }
        }

        ///<summary>
        ///onCast
        ///判断自己的param的"firedBullet"，如果firedBullet已经不存在了，或者压根不存在，就发射子弹，否则，就传送过去，参数：
        ///["firedBullet"]GameObject：firedBullet，理论上也可以是别的东西
        ///</summary>
        private static TimelineObj FireTeleportBullet(BuffObj buff, SkillObj skill, TimelineObj timeline)
        {
            if (skill.model.id != "teleportBullet") return timeline;
            GameObject firedBullet = buff.buffParam.ContainsKey("firedBullet") ? (GameObject)buff.buffParam["firedBullet"] : null;
            ChaState cs = buff.carrier.GetComponent<ChaState>();

            if (firedBullet == null)
            {
                buff.buffParam["firedBullet"] = null;
                return timeline;
            }
            else
            {
                if (cs == null || SceneVariants.map.CanUnitPlacedHere(firedBullet.transform.position, cs.property.bodyRadius, cs.property.moveType) == false)
                {
                    SceneVariants.PopUpStringOnCharacter(buff.carrier, "<color=red>无法传送</color>");
                    return null;    //如果没有角色了，或者说飞弹的位置不能传送，那么就返回一个空，也就是不让放技能
                }
                return new TimelineObj(DesignerTables.Timeline.data["skill_teleportBullet_tele"], timeline.caster, null);
            }
        }

        ///<summary>
        ///onRemoved
        ///把buff的carrier传送到记录的子弹的世界坐标（非常危险，因为那个坐标未必能站立），并且移除掉那个子弹
        ///</summary>
        private static void TeleportCarrier(BuffObj buff)
        {
            ChaState cs = buff.carrier.GetComponent<ChaState>();
            if (cs.dead) return;
            List<BuffObj> fireRec = cs.GetBuffById("TeleportBulletPassive", new List<GameObject>() { buff.caster });
            if (fireRec.Count <= 0) return;
            GameObject bullet = fireRec[0].buffParam.ContainsKey("firedBullet") ? (GameObject)fireRec[0].buffParam["firedBullet"] : null;
            if (bullet == null) return;
            buff.carrier.transform.position = new Vector3(bullet.transform.position.x, 0, bullet.transform.position.z);
            SceneVariants.RemoveBullet(bullet);
        }

        ///<summary>
        ///beHurt
        ///buff的Carrier只能受到1点直接伤害，免疫其他一切，桶子就是这样的
        ///</summary>
        private static void OnlyTakeOneDirectDamage(BuffObj buff, ref DamageInfo damageInfo, GameObject attacker)
        {
            bool isDirectDamage = false;
            for (int i = 0; i < damageInfo.tags.Length; i++)
            {
                if (damageInfo.tags[i] == DamageInfoTag.directDamage)
                {
                    isDirectDamage = true;
                    break;
                }
            }
            if (isDirectDamage == true && damageInfo.DamageValue(false).result.damage > 0)
            {
                int finalDV = 1;
                if (attacker != null)
                {
                    ChaState cs = attacker.GetComponent<ChaState>();
                    //来自另外一个桶子（不包含自己）的伤害为9999，其他的都是1
                    if (cs != null && cs.HasTag("Barrel") == true && attacker.Equals(buff.carrier) == false)
                    {
                        finalDV = 9999;
                    }
                }
                damageInfo.damage = new Damage(0, finalDV);
            }
            else
            {
                damageInfo.damage = new Damage(0);
            }

            return;
        }

        ///<summary>
        ///onTick
        ///桶子每5秒对自己伤害，其实可以写一个公用的dot，不过这里表达的是，不公用也没问题
        ///</summary>
        private static void BarrelDurationLose(BuffObj buff)
        {
            SceneVariants.CreateDamage(buff.carrier, buff.carrier, new Damage(0, 1), 0, 0, new DamageInfoTag[] { DamageInfoTag.directDamage });
        }

        ///<summary>
        ///beKilled
        ///死亡后爆炸，对敌人造成伤害，其他桶子也是其他敌人，所以不必特殊处理，beHurt已经特殊处理了，当然还要立即清除掉这个桶子。
        ///</summary>
        private static void BarrelExplosed(BuffObj buff, DamageInfo damageInfo, GameObject attacker)
        {
            GameObject aoeCaster = buff.caster != null ? buff.caster : buff.carrier;
            //AoeModel是可以动态生成的
            SceneVariants.CreateAoE(new AoeLauncher(
                new AoeModel(
                    "BoomExplosive", "", new string[0], 0, false,
                    "CreateSightEffect", new object[] { "Effect/Explosion_A" },
                    "BarrelExplosed", new object[0],
                    "", new object[0],  //tick
                    "", new object[0],  //chaEnter
                    "", new object[0],  //chaLeave
                    "", new object[0],  //bulletEnter
                    "", new object[0]   //bulletLeave
                ),
                aoeCaster, buff.carrier.transform.position, 2.2f, 0.5f, 0,
                null, null, new Dictionary<string, object>(){
                    {"Barrel", buff.carrier}
                }
            ));
            //隐藏自己，反正后面会被Remover移走
            buff.carrier.transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// OnTick 每秒回复生命值和灵力值
        /// </summary>
        /// <param name="buff"></param>
        private static void BaseRecover(BuffObj buff)
        {
            ChaState carrier = buff.carrier.GetComponent<ChaState>();
            SceneVariants.CreateDamage(buff.carrier, buff.carrier, new Damage(-carrier.property.hp_recover), 0, 0, new DamageInfoTag[] { DamageInfoTag.periodHeal, DamageInfoTag.NoNeedPopText });
            //Debug.Log(carrier.property.mp_recover);
            carrier.ModResource(new ChaResource(0, carrier.property.mp_recover, 0));
        }
    }
}