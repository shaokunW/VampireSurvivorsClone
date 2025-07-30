using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    /// <summary>
    /// 定义一个难度等级的完整进程数据，包括预算、节奏和怪物列表。
    /// </summary>
    [CreateAssetMenu(fileName = "LevelProgression_D0", menuName = "Vampire/Data/Level Progression Data")]
    public class LevelProgressionData : ScriptableObject
    {
        [Header("怪物池")]
        [Tooltip("在此关卡/难度中所有可能出现的怪物列表")]
        // --- 核心改动：更新了类型引用 ---
        public List<EnemyGenerationData> availableEnemies; 
        
        [Header("预算与节奏")]
        [Tooltip("每一波的基础“威胁点数”预算曲线。X轴=波次数, Y轴=预算值")]
        public AnimationCurve budgetPerWave = AnimationCurve.Linear(1, 10, 20, 200);

        [Tooltip("每一波的持续时间（秒）")]
        public float waveDuration = 60f;

        [Tooltip("在一波之内，刷怪速率随时间变化的曲线。X轴=波次时间进度(0到1), Y轴=速率乘数")]
        public AnimationCurve spawnRateOverTime = AnimationCurve.EaseInOut(0, 0.1f, 1, 1f);

        [Header("位置控制")] 
        [Tooltip("怪物生成的最小半径（确保在屏幕外）")]
        public float minSpawnRadius = 15f;
        [Tooltip("怪物生成的最大半径")]
        public float maxSpawnRadius = 20f;
    }
}