using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Vampire
{
    [CreateAssetMenu(fileName = "Item_", menuName = "Vampire/ItemDefinition")]
    public class ItemDefinition: ScriptableObject
    {
        public int id;
        public string name;
        public string desc;
        public int basePrice; // 基础价格
        public int maxStack = 1; // 最大堆叠数量, 默认为1
        [Header("游戏逻辑")]
        // 这里将是连接到效果系统的关键
        public List<GameplayEffect> effects; // 物品拥有的效果列表
        public AssetReferenceSprite graphicsId;
        // if consumable apply effects once get the Items
        private bool consumable;


    }
}