using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Vampire
{
    /// <summary>
    /// 定义子弹的移动模式
    /// </summary>
    public enum MovementPattern
    {
        Linear, // 线性（直线）
        Homing, // 轻微追踪目标
        Wave, // 正弦波轨迹
        Stationary, // 静止（例如爆炸、地雷）
        MeleeSwing, // 近战挥砍式轨迹
        Boomerang // 回旋镖式轨迹
    }

    [CreateAssetMenu(fileName = "NewBulletData", menuName = "GameData/Bullet", order = 2)]
    public class BulletData : ScriptableObject
    {
        [Header("基础信息")] [Tooltip("子弹的唯一ID，用于程序识别")]
        public string bulletId;

        [Tooltip("子弹的外观资源ID (用于从资源管理器或对象池加载)")]
        public AssetReferenceSprite graphicsId;

        [Header("物理属性")] [Tooltip("子弹的基础碰撞半径")]
        public float baseRadius = 0.5f;

        [Tooltip("子弹的基础耐久度 (可穿透目标的次数)")] public int baseDurability = 1;

        [Tooltip("子弹的基础生命周期 (秒)")] public float baseLifetime = 5f;

        [Header("战斗属性")] [Tooltip("子弹的基础伤害力，可以为负数表示治疗")]
        public int baseDamage = 10;

        [Tooltip("基础生命窃取概率 (0-100)")] [Range(0, 100)]
        public int baseLifestealChance = 0;

        // [Header("目标阵营")] [Tooltip("是否能对玩家造成伤害")]
        // public bool canHitPlayer = false;
        //
        // [Tooltip("是否能对敌人造成伤害")] public bool canHitEnemy = true;

        [Header("移动轨迹")] [Tooltip("子弹的移动模式")] public MovementPattern movementPattern;

        [Tooltip("子弹的基础移动速度")] public float baseSpeed = 10f;

        [Tooltip("仅用于某些移动模式的额外参数(如波浪频率、追踪强度、回旋时间等)")]
        public float movementParameter = 1f;

        public float startWidth;
        
        public float endWidth;

        // [Header("命中效果")]
        // [Tooltip("子弹命中目标时触发的特殊效果列表")]
        // public List<EffectData> hitEffects;
    }
}