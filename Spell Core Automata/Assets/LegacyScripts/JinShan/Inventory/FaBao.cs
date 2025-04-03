using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JinShan
{
    [CreateAssetMenu(fileName = "New Fabao Object", menuName = "Inventory System/Items/Fabao")]
    public class FaBao : ItemObject
    {
        private void Awake()
        {
            type = ItemType.法宝;
        }
        ///法宝槽位数量
        [OnValueChanged("UpdateFaBaoSlots")]
        public int SlotCount;
        ///法宝充能时间
        public float BaseChargeTime;
        ///法宝槽位
        public List<FaBaoSlot> Slots;
        ///法宝基础抽取数
        public int DrawTimes;
        ///法术基础延迟时间
        public float BaseDelayTime;
        ///最大法力值
        public int MaxMp;
        ///法力基础恢复速度（加上角色法力恢复值就是法宝最终回复值）
        public float BaseMpRecover;

        [FoldoutGroup("Visual"), InlineEditor(InlineEditorModes.LargePreview)]
        [ShowInInspector, ReadOnly]
        private GameObject prefabPreview;

        [FoldoutGroup("Visual")]
        [FilePath(ParentFolder = "Assets/Resources/Prefabs/Weapon", Extensions = "prefab")]
        [OnValueChanged("LoadPrefab")]
        public string Prefab;

        public string[] tags;

        public bool HasTag(string tag)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i] == tag) return true;
            }
            return false;
        }

        private void LoadPrefab()
        {
            if (!string.IsNullOrEmpty(Prefab))
            {
                prefabPreview = Resources.Load<GameObject>("Prefabs/Weapon/" + Prefab);
            }
        }

        private void OnEnable()
        {
            LoadPrefab();
        }

        private void UpdateFaBaoSlots()
        {
            Slots = new List<FaBaoSlot>();
            for (int i = 0; i < SlotCount; i++)
            {
                Slots.Add(new FaBaoSlot());
            }
        }
    }

    [Serializable]
    public class FaBaoSlot
    {
        public bool isLock;
        public ItemObject itemObject;
        public FaBaoSlot()
        {
            isLock = false;
            itemObject = null;
        }
    }
}