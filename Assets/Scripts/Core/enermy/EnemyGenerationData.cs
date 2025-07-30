using UnityEngine;

namespace Vampire
{
    /// <summary>
    /// 定义单个怪物在“生成”时所需的规则数据。
    /// </summary>
    [CreateAssetMenu(fileName = "NewEnemyGenData", menuName = "Vampire/Data/Enemy Generation Data")]
    public class EnemyGenerationData : ScriptableObject
    {
        [Header("核心属性")]
        [Tooltip("敌人的唯一ID，用于识别")]
        public string id;

        [Header("刷怪系统参数")]
        [Tooltip("这个怪物在计算预算时的“威胁成本”")]
        public int threatCost = 1;

        [Tooltip("决定此怪物在不同波次出现权重的曲线。X轴=波次数, Y轴=权重值")]
        public AnimationCurve spawnWeightCurve = AnimationCurve.Linear(0, 1, 20, 10);
    }
}