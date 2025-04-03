using UnityEngine;

namespace JinShan
{
    [CreateAssetMenu(fileName = "New Default Object", menuName = "Inventory System/Items/Deafult")]
    public class DafultObject : ItemObject
    {
        public void Awake()
        {
            type = ItemType.Default;
        }
    }

}
