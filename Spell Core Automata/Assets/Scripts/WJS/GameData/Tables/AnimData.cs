using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WJS
{
    ///<summary>
    ///角色使用的动画信息
    ///</summary>
    public class AnimData
    {
        public static Dictionary<string, Dictionary<string, AnimInfo>> data = new Dictionary<string, Dictionary<string, AnimInfo>>(){
            {"Skeleton",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 1)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveForward", 1.667f),1)}, 1)},
                    {"MoveBack", new AnimInfo("MoveBack", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveForward",1.667f),1)}, 1)},
                    {"MoveLeft", new AnimInfo("MoveLeft", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveForward", 1.667f),1)}, 1)},
                    {"MoveRight", new AnimInfo("MoveRight", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveForward", 1.667f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("DashForward", 0.833f),1)}, 1)},
                    {"BiteAttack", new AnimInfo("BiteAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("BiteAttack", 0.833f),1)}, 0)},
                    {"Slash Attack", new AnimInfo("Slash Attack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Left Slash Attack", 0.833f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Right Slash Attack", 0.833f),1)}, 5)},
                    {"Resurrect", new AnimInfo("Resurrect",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Resurrect", 1f),1)}, 10)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("ProjectileAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"Jump", new AnimInfo("Jump",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Jump In Place", 0.667f),1) }, 1)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dead",1.667f),1)}, 100)}
                }
            }
        };
    }
}
