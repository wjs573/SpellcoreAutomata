using UnityEngine;

namespace JinShan
{
    [CreateAssetMenu(fileName = "New ShenTong Object", menuName = "Inventory System/Items/ShenTong")]
    public class ShenTong : ItemObject
    {
        private void Awake()
        {
            type = ItemType.神通;
        }

        public string ShenTongEffects;

        public string[] tags;

        public bool HasTag(string tag)
        {
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i] == tag) return true;
            }
            return false;
        }
    }
}