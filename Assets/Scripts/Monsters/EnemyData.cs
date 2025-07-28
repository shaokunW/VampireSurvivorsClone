using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Vampire
{
    [CreateAssetMenu(fileName = "NewEnemyData", menuName = "GameData/Enemy")]
    public class EnemyData : ScriptableObject
    {
        [Header("基础信息")] [Tooltip("敌人的唯一ID，用于索引和查找。")]
        public string id;

        [Tooltip("敌人的外观预制体。使用AssetReference以兼容Addressables系统。")]
        public AssetReferenceSprite graphicsId;

        [Header("生命值 (HP)")] [Tooltip("怪物的基础生命值。")]
        public float baseHp;

        [Tooltip("每一波（Wave）到来时，生命值额外增加的量。最终HP = Floor(基础HP + 波数 * 每波HP提升)")]
        public float hpPerWave;

        [Header("移动速度")] [Tooltip("怪物的移动速度，单位：毫米/帧。")]
        public float moveSpeed;

        [Header("伤害力 (Damage)")] [Tooltip("怪物的基础碰撞伤害。")]
        public float baseDamage;

        [Tooltip("每一波到来时，伤害额外增加的量。最终伤害 = Floor(基础伤害 + 波数 * 每波伤害提升)")]
        public float damagePerWave;

        [Header("掉落物 (Drops)")] [Tooltip("击败时掉落的固定资源数量。")]
        public int resourceDropAmount;

        [Tooltip("掉落回血道具的概率，范围 0-100。")] [Range(0, 100)]
        public int healthDropChance;

        [Tooltip("掉落宝箱的概率，范围 0-100。")] [Range(0, 100)]
        public int chestDropChance;

        [Header("事件")] [Tooltip("击败后触发的事件函数名列表（用于反射调用）。")]
        public List<string> onDefeatEvents;
    }
}