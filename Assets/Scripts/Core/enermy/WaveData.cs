using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    /// <summary>
    /// 一个简单的数据载体，用于封装单次刷怪波次所需的所有信息。
    /// </summary>
    public class WaveData
    {
        public List<EnemyGenerationData> EnemiesToSpawn { get; private set; }
        public AnimationCurve SpawnRateOverTime { get; private set; }
        public float MinSpawnRadius { get; private set; }
        public float MaxSpawnRadius { get; private set; }

        public WaveData(List<EnemyGenerationData> enemies, AnimationCurve rateCurve, float minRadius, float maxRadius)
        {
            EnemiesToSpawn = enemies;
            SpawnRateOverTime = rateCurve;
            MinSpawnRadius = minRadius;
            MaxSpawnRadius = maxRadius;
        }
    }
}