using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    public class LootManager: MonoBehaviour
    {
        public static LootManager Instance { get; private set; }
        [Header("系统引用")]
        [Tooltip("引用场景中的 RewardManager")]
        [SerializeField] private RewardManager rewardManager;

        [Tooltip("引用项目中的 ItemDatabase 资产")]
        [SerializeField] private ItemDatabase itemDatabase;

        [Header("掉落表配置")]
        [Tooltip("在此处配置每种敌人ID对应的掉落容器")]
        [SerializeField] private List<EnemyLootMapping> lootTable;
        private Dictionary<string, int> _lootDictionary;
        
        void Awake()
        {
            if (!Instance)
            {
                Instance = this;
              
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
            _lootDictionary = new Dictionary<string, int>();
            foreach (var mapping in lootTable)
            {
                _lootDictionary.Add(mapping.EnemyID, mapping.RewardContainerId);
            }
        }
        
        /// <summary>
        /// 当监听到敌人死亡事件时，此方法被调用。
        /// </summary>
        public void OnEnemyDied(EnemyDiedEventData eventData)
        {
            Debug.Log($"[LootManager] Receive {eventData.EnemyID} {eventData.EnemyPosition}");
            // 1. 根据敌人ID查找对应的掉落表 (RewardContainer)
            if (_lootDictionary.TryGetValue(eventData.EnemyID, out var container))
            {
                var itemIds = rewardManager.Select(container);

                // 3. 遍历ID列表，执行掉落
                foreach (int id in itemIds)
                {
                    // 4. 从物品数据库异步获取完整的物品定义
                    ItemDefinition itemDef = ItemDatabase.Instance.GetItemByIDAsync(id);
                    
                    if (itemDef == null)
                    {
                        Debug.LogWarning($"LootManager: 未能加载 ID 为 {id} 的物品。请检查Addressables配置。");
                        continue;
                    }

                    // 5. 遍历物品的效果列表，找到并执行 DropEffect
                    foreach (var effect in itemDef.effects)
                    {
                        if (effect is DropEffect dropEffect)
                        {
                            // 6. 执行掉落，传入死亡敌人的游戏对象
                            dropEffect.Apply(eventData.EnemyPosition, null);
                        }
                    }
                }
            }
        }

        // 用于在 Inspector 中方便配置的辅助类
        [Serializable]
        public class EnemyLootMapping
        {
            public string EnemyID;
            public int RewardContainerId;
        }
    }
}