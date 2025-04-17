using System.Collections.Generic;

namespace DesignerTables
{
    ///<summary>
    ///角色使用的动画信息
    ///</summary>
    public class UnitAnimInfo
    {
        public static Dictionary<string, Dictionary<string, AnimInfo>> data = new Dictionary<string, Dictionary<string, AnimInfo>>(){
            {"骷髅",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveForward", 1.667f),1)}, 1)},
                    {"MoveBack", new AnimInfo("MoveBack", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveForward",1.667f),1)}, 1)},
                    {"MoveLeft", new AnimInfo("MoveLeft", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveLeft", 1.667f),1)}, 1)},
                    {"MoveRight", new AnimInfo("MoveRight", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveRight", 1.667f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("DashForward", 0.833f),1)}, 1)},
                    {"BiteAttack", new AnimInfo("BiteAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("BiteAttack", 0.833f),1)}, 0)},
                    {"Slash Attack", new AnimInfo("Slash Attack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("LeftSlashAttack", 0.833f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("RightSlashAttack", 0.833f),1)}, 5)},
                    {"Spawn", new AnimInfo("Resurrect",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Resurrect", 1f),1)}, 10)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Hurt0", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("ProjectileAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"Jump", new AnimInfo("Jump",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Jump In Place", 0.667f),1) }, 1)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dead",1.667f),1)}, 100)}
                }
                },

                {"骷髅巨人",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveForward", 1.333f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("DashForward", 0.833f),5)}, 5)},
                    {"Slash Attack", new AnimInfo("Slash Attack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Slash Attack",0.667f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Swing Attack",0.833f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Kick Attack",0.833f),1)}, 3)},
                    {"BaseAttack", new AnimInfo("Slash Attack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Slash Attack",0.667f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Swing Attack",0.833f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Kick Attack",0.833f),1)}, 3)},
                    {"Jump Smash Attack In Place", new AnimInfo("Jump Smash Attack In Place",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Jump Smash Attack In Place",1.167f),1)}, 5)},
                    {"Spawn", new AnimInfo("Resurrect",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Resurrect", 1f),1)}, 10)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Hurt", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("ProjectileAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"Jump", new AnimInfo("Jump",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Jump", 0.667f),1) }, 1)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 100)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dead",1.667f),1)}, 100)}
                }
                },

                {"骷髅法师",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 0)},
                    {"Spawn",new AnimInfo("Spawn",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spawn", 0.833f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("FlyForward", 0.833f),1)}, 0)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Left Slash Attack",0.667f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Right Slash Attack",0.667f),1) }, 3)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Hurt0", 0.667f),1) }, 5)},
                    {"Fire", new AnimInfo("ProjectileAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"SpinAttack", new AnimInfo("SpinAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spin Attack", 0.333f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dead",1.667f),1)}, 10)}
                }
                },

                {"火灵",new Dictionary<string, AnimInfo>()
                    {
                        {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",0.833f),1)}, 0)},
                        {"Spawn",new AnimInfo("Spawn",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spawn", 0.833f),1)}, 0)},
                        {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveForward", 0.667f),1)}, 1)},
                        {"DashAttack", new AnimInfo("DashAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("DashAttack", 0.833f),1)}, 0)},
                        {"BiteAttack", new AnimInfo("BiteAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("BiteAttack", 0.833f),1)}, 3)},
                        {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Hurt0", 0.667f),1) }, 1)},
                        {"Fire", new AnimInfo("ProjectileAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                        {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                        {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dead",1.333f),1)}, 10)}
                    }
                },

                {"霜灵",new Dictionary<string, AnimInfo>()
                    {
                        {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",0.833f),1)}, 0)},
                        {"Spawn",new AnimInfo("Spawn",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spawn", 0.833f),1)}, 0)},
                        {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 0.667f),1)}, 1)},
                        {"DashAttack", new AnimInfo("DashAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Head Attack", 0.833f),1)}, 0)},
                        {"BiteAttack", new AnimInfo("BiteAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Bite Attack", 0.833f),1)}, 3)},
                        {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                        {"Fire", new AnimInfo("ProjectileAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                        {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                        {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                    }
                },

                {"火灵法师",new Dictionary<string, AnimInfo>()
                    {
                        {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 0)},
                        {"Spawn",new AnimInfo("Spawn",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spawn", 0.833f),1)}, 0)},
                        {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 0.667f),1)}, 0)},
                        {"MoveBack", new AnimInfo("MoveBack", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 0.667f),1)}, 0)},
                        {"MoveLeft", new AnimInfo("MoveLeft", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 0.667f),1)}, 0)},
                        {"MoveRight", new AnimInfo("MoveRight", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 0.667f),1)}, 0)},
                        {"DashForward", new AnimInfo("DashAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dash Forward In Place", 0.50f),1)}, 0)},
                        {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Clap Attack", 0.833f),1)}, 3)},
                        {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Slap Attack", 0.833f),1)}, 3)},
                        {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                        {"Fire", new AnimInfo("ProjectileAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack Low", 0.833f),1)}, 3)},
                        {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                        {"SpellAttack01", new AnimInfo("SpellAttack01",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spell Attack 01", 0.833f),1) }, 1)},
                        {"SpellAttack02", new AnimInfo("SpellAttack02",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spell Attack 02", 1.167f),1) }, 1)},
                        {"SummonAttack", new AnimInfo("SummonAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Summon Attack", 1f),1) }, 1)},
                        {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                    }
                },

                {"Default_Gunner", new Dictionary<string, AnimInfo>(){
                    {"Stand", new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Stand"),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveForward"),1)}, 0)},
                    {"MoveBack", new AnimInfo("MoveBack", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveBack"),1)}, 0)},
                    {"MoveLeft", new AnimInfo("MoveLeft", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveLeft"),1)}, 0)},
                    {"MoveRight", new AnimInfo("MoveRight", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MoveRight"),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Hurt0", 0.3f),5),new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Hurt1", 0.3f),2)}, 1)},
                    {"Happy", new AnimInfo("Happy",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Victory"),1)}, 2)},
                    {"Power", new AnimInfo("Power",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("PowerUp", 1.33f),1)}, 2)},

                    {"Fire", new AnimInfo("Fire", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fire",0.5f),1)}, 3)},

                    {"Reload", new AnimInfo("Reload", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Reload",1.33f),1)}, 3)},
                    {"JumpStart", new AnimInfo("JumpStart",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("JumpStart", 0.21f),1)}, 3)},
                    {"Flying", new AnimInfo("Flying",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("JumpAir"),1)}, 3)},
                    {"JumpEnd", new AnimInfo("JumpEnd",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("JumpEnd",0.33f),1)}, 3)},
                    {"RapidFire", new AnimInfo("RapidFire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("RapidFire"),1)}, 3)},
                    {"RollForward", new AnimInfo("RollForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("RollForward",1.0f),1)}, 3)},
                    {"RollBack", new AnimInfo("RollBack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("RollBack",1.0f),1)}, 3)},
                    {"RollLeft", new AnimInfo("RollLeft",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("RollLeft",1.0f),1)}, 3)},
                    {"RollRight", new AnimInfo("RollRight",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("RollRight",1.0f),1)}, 3)},
                    {"StepForward", new AnimInfo("StepForward", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("StepForward",0.66f),1)}, 3)},
                    {"StepBack", new AnimInfo("StepBack", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("StepBack",0.66f),1)}, 3)},
                    {"StepLeft", new AnimInfo("StepLeft",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("StepLeft",0.66f),1)}, 3)},
                    {"StepRight", new AnimInfo("StepRight", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("StepRight",0.66f),1)}, 3)},
                    {"Stun", new AnimInfo("Stun",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Stun"),1)}, 3)},
                    {"Slash Attack", new AnimInfo("Slash Attack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Slash Attack",1.3f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Swing Attack",1.3f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Kick Attack",1.3f),1)}, 5)},
                    {"Jump Smash Attack In Place", new AnimInfo("Jump Smash Attack In Place",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Jump Smash Attack In Place",2.2f),1)}, 5)},
                    {"RunForward", new AnimInfo("RunForward",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("RunForward",0.833f),1)}, 1)},

                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dead"),1)}, 10)}
                }
            },

            {"鼹鼠",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk Forward In Place", 0.833f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Run Forward In Place", 0.667f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Slash Attack", 0.667f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Head Attack W Root", 1.167f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 0.833f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.667f),1)}, 10)}
                }
            },

            {"仙人掌",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1.333f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk Forward In Place", 1.167f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Run Forward In Place", 0.667f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Bite Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Bite Attack", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"仙人掌巨兽",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk Forward In Place", 1.167f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Run Forward In Place", 0.833f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Slash Left Attack", 0.833f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Slash Right Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Jump Smack Attack", 1f),1)}, 0)},
                    {"BreathAttack", new AnimInfo("BreathAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Breath Attack", 1.167f),1)}, 0)},
                    {"DashAttack", new AnimInfo("DashAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dash Forward Attack In Place", 0.50f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 0.833f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"红猫",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk Forward In Place", 1.167f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Run Forward In Place", 0.667f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Bite Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Pounce Bite Attack In Place", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"雷猫",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk Forward In Place", 1.167f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Run Forward In Place", 0.667f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Bite Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Pounce Bite Attack In Place", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"喵喵",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk Forward In Place", 1.167f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Run Forward In Place", 0.667f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Bite Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Pounce Bite Attack In Place", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"死神",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1.333f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 0.833f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dash Forward In Place", 0.667f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Melee Attack", 0.667f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Tumble Attack", 0.333f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"死灵法师",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1.333f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 1.167f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dash Forward In Place", 0.667f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Left Slash Attack", 0.833f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Right Slash Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spin Attack", 0.833f),1)}, 3)},
                    {"KickAttack", new AnimInfo("KickAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Bite Attack", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"小狗",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",0.667f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk Forward In Place", 1f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Run Forward In Place", 0.4f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Bite Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Pounce Bite Attack In Place", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"巨犬",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",0.667f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk Forward In Place", 1f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Run Forward In Place", 0.5f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Left Slash Attack", 0.833f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Right Slash Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Pounce Smash Attack W Root", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"焰兽",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1.333f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 0.667f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 0.667f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Slash Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Slap Attack", 0.833f),1)}, 3)},
                    {"SpinAttack", new AnimInfo("KickAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spin Attack", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"SummonAttack", new AnimInfo("SummonAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Summon Attack", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"霜灵法师",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1.333f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 1.167f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dash Forward In Place", 0.667f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Icicle Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Icicle Attack Ground", 0.833f),1)}, 3)},
                    {"SpinAttack", new AnimInfo("KickAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spin Attack", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell 01", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"大鼹鼠",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk Forward In Place", 1f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Run Forward In Place", 0.5f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Slash Attack", 0.667f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Head Attack W Root", 1.167f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 0.833f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.667f),1)}, 10)}
                }
            },

            {"鼹鼠王",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk Forward In Place", 1.167f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Run Forward In Place", 0.667f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Left Slash Attack", 0.833f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Right Slash Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Head Attack W Root", 1.167f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 0.833f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.667f),1)}, 10)}
                }
            },

            {"夜魇",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",1.333f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 1.167f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dash Forward In Place", 0.667f),1)}, 1)},
                    {"BaseAttack", new AnimInfo("BaseAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Stab Attack", 1f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Head Attack", 1.333f),1)}, 3)},
                    {"SpinAttack", new AnimInfo("KickAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spin Attack", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Take Damage", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("Fire",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Die",1.333f),1)}, 10)}
                }
            },

            {"旋风",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",0.833f),1)}, 0)},
                    {"Spawn",new AnimInfo("Spawn",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spawn", 0.833f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 0.833f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dash Forward In Place", 0.50f),1)}, 0)},
                    {"BaseAttack", new AnimInfo("BiteAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Bite Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("BiteAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Wind Slash Attack", 0.833f),1)}, 3)},
                    {"SpinAttack", new AnimInfo("SpinAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Pushback Attack", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Hurt0", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("ProjectileAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dead",1.333f),1)}, 10)}
                }
            },

            {"风灵",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",0.833f),1)}, 0)},
                    {"Spawn",new AnimInfo("Spawn",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spawn", 0.833f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 0.833f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dash Forward In Place", 0.50f),1)}, 0)},
                    {"BaseAttack", new AnimInfo("BiteAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Bite Attack", 0.833f),1)}, 3)},
                    {"WindShield", new AnimInfo("WindShield",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Wind Shield", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("BiteAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Wind Slash Attack", 0.833f),1)}, 3)},
                    {"SpinAttack", new AnimInfo("SpinAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Pushback Attack", 0.833f),1)}, 0)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Hurt0", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("ProjectileAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dead",1.333f),1)}, 10)}
                }
            },

            {"狂风法师",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",0.833f),1)}, 0)},
                    {"Spawn",new AnimInfo("Spawn",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Spawn", 0.833f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Fly Forward In Place", 0.667f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dash Forward In Place", 0.667f),1)}, 0)},
                    {"BaseAttack", new AnimInfo("BiteAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Slash Attack", 0.833f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Clap Attack", 0.833f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Wind Slash Attack", 0.833f),1)}, 3)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Hurt0", 0.667f),1) }, 1)},
                    {"Fire", new AnimInfo("ProjectileAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Projectile Attack", 0.833f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Cast Spell", 1f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dead",1.333f),1)}, 10)}
                }
            },

             {"女法师",new Dictionary<string, AnimInfo>()
                {
                    {"Stand",new AnimInfo("Stand",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Idle",0.833f),1)}, 0)},
                    {"MoveForward", new AnimInfo("MoveForward",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk", 1.167f),1)}, 1)},
                    {"MoveBack", new AnimInfo("MoveBack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk", 1.167f),1)}, 1)},
                    {"MoveLeft", new AnimInfo("MoveLeft",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk", 1.167f),1)}, 1)},
                    {"MoveRight", new AnimInfo("MoveRight",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Walk", 1.167f),1)}, 1)},
                    {"DashForward", new AnimInfo("DashAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Run", 0.667f),1)}, 0)},
                    {"BaseAttack", new AnimInfo("BiteAttack",new KeyValuePair<SingleAnimInfo, int>[]{
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MagicHit01", 2.20f),1),
                        new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MagicHit02", 2.20f),1)}, 3)},
                    {"StrongAttack", new AnimInfo("StrongAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MagicHit03", 2.20f),1)}, 3)},
                    {"Hurt", new AnimInfo("Hurt",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Hurt", 1.20f),1) }, 1)},
                    {"Fire", new AnimInfo("ProjectileAttack",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MagicHit01", 2.20f),1)}, 3)},
                    {"CastSpell", new AnimInfo("CastSpell",new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("MagicHit01", 2.20f),1) }, 1)},
                    {"Dead", new AnimInfo("Dead", new KeyValuePair<SingleAnimInfo, int>[]{new KeyValuePair<SingleAnimInfo, int>(new SingleAnimInfo("Dead",3.363f),1)}, 10)}
                }
            }

        };
    }
}