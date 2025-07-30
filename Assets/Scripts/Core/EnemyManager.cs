using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace Vampire
{

    public class EnemyManager : MonoBehaviour
    {
        public static EnemyManager Instance { get; private set; }

        
        [Header("核心配置")]
        [SerializeField] public GameObject enemyPrefab;
        [SerializeField] private LevelProgressionData levelProgression;
        [SerializeField] private Transform playerTransform;

        #if UNITY_EDITOR
        [Header("调试选项")]
        [Tooltip("勾选此项以启用按键测试指定波次的功能")]
        [SerializeField] private bool enableDebugMode = true;
        [Tooltip("要测试的波次数")]
        [SerializeField] private int debug_testWaveNumber = 1;
        [Tooltip("按下此键来触发测试波次")]
        [SerializeField] private KeyCode debug_triggerKey = KeyCode.T;
        #endif

        // --- 游戏状态 ---
        private int _currentWave = 0;
        private float _waveTimer;
        private Coroutine _spawnCoroutine;       
        private ObjectPool<EnemyAIController> _enemyPool;
        private Dictionary<string, AsyncOperationHandle<EnemyData>> _modelHandlesCache;
        private Dictionary<AssetReferenceSprite, AsyncOperationHandle<Sprite>> _spriteHandlesCache;

        void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                _enemyPool = new ObjectPool<EnemyAIController>(
                    createFunc: () =>
                    {
                        var o = Instantiate(enemyPrefab);
                        var controller = o.GetComponent<EnemyAIController>();
                        controller.OnDeactivated += _enemyPool.Release;
                        return controller;
                    },
                    actionOnGet: (controller) => controller.gameObject.SetActive(true),
                    actionOnRelease: (controller) => controller.gameObject.SetActive(false),
                    actionOnDestroy: (controller) => Destroy(controller.gameObject),
                    collectionCheck: true,
                    defaultCapacity: 1000,
                    maxSize: 1000);
                _modelHandlesCache = new Dictionary<string, AsyncOperationHandle<EnemyData>>();
                _spriteHandlesCache = new Dictionary<AssetReferenceSprite, AsyncOperationHandle<Sprite>>();
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        void Update()
        {
            // --- 常规游戏逻辑 ---
            if (_waveTimer > 0)
            {
                _waveTimer -= Time.deltaTime;
                if (_waveTimer <= 0)
                {
                    Debug.Log($"第 {_currentWave} 波结束！准备下一波...");
                    // 在此可以加入商店、休息、结算等逻辑
                    StartNextWave();
                }
            }
        }
        
        /// <summary>
        /// 启动下一波。
        /// </summary>
        private void StartNextWave()
        {
            StartWave(_currentWave + 1);
        }

        /// <summary>
        /// 启动一个指定的波次。
        /// </summary>
        /// <param name="waveNumber">要启动的波次数</param>
        private void StartWave(int waveNumber)
        {
            _currentWave = waveNumber;
            _waveTimer = levelProgression.waveDuration;
            
            Debug.Log($"GameManager: 准备第 {_currentWave} 波的数据。");

            // 使用固定的种子来保证刷怪顺序的可复现性
            Random.InitState(_currentWave);
            
            // 1. 生成怪物列表
            List<EnemyGenerationData> spawnList = GenerateSpawnListForCurrentWave();
            
            // 2. 将所有数据打包成“任务单”
            WaveData waveData = new WaveData(
                spawnList,
                levelProgression.spawnRateOverTime,
                levelProgression.minSpawnRadius,
                levelProgression.maxSpawnRadius
            );

            // 3. 命令执行者开始工作
            ExecuteSpawnWave(waveData);
        }

        private List<EnemyGenerationData> GenerateSpawnListForCurrentWave()
        {
            var list = new List<EnemyGenerationData>();
            float budget = levelProgression.budgetPerWave.Evaluate(_currentWave);

            while (budget > 0)
            {
                float totalWeight = 0;
                foreach (var enemy in levelProgression.availableEnemies)
                {
                    totalWeight += enemy.spawnWeightCurve.Evaluate(_currentWave);
                }
                if (totalWeight <= 0) break;

                float randomValue = Random.Range(0, totalWeight);
                float weightSum = 0;
                EnemyGenerationData chosenEnemy = null;
                
                foreach (var enemy in levelProgression.availableEnemies)
                {
                    weightSum += enemy.spawnWeightCurve.Evaluate(_currentWave);
                    if (randomValue <= weightSum)
                    {
                        chosenEnemy = enemy;
                        break;
                    }
                }
                
                if (chosenEnemy != null && chosenEnemy.threatCost > 0)
                {
                    list.Add(chosenEnemy);
                    budget -= chosenEnemy.threatCost;
                }
                else
                {
                    break;
                }
            }
            return list;
        }
        
        public void ExecuteSpawnWave(Transform target, WaveData waveData)
        {
            if (_spawnCoroutine != null)
            {
                StopCoroutine(_spawnCoroutine);
            }
            _spawnCoroutine = StartCoroutine(SpawnEnemiesCoroutine(target, waveData));
        }

        private IEnumerator SpawnEnemiesCoroutine(Transform t, WaveData waveData)
        {
            int spawnedCount = 0;
            var spawnList = waveData.EnemiesToSpawn;
            float waveStartTime = Time.time;

            while (spawnedCount < spawnList.Count)
            {
                // 1. 计算刷怪速率和延迟
                // 注意：这里需要GameManager来传递波次的总时长，或者我们在这里假设一个值
                // 为了让Spawner更纯粹，我们先简化处理
                float timeProgress = Mathf.Clamp01((Time.time - waveStartTime) / 60f); // 假设波长60秒
                float currentSpawnRate = waveData.SpawnRateOverTime.Evaluate(timeProgress);
                float delay = 0.5f / (currentSpawnRate + 0.1f); 
                yield return new WaitForSeconds(delay);

        
                
                // 3. 生成敌人
                EnemyGenerationData enemyToSpawn = spawnList[spawnedCount];
                Vector2 spawnPosition = GetSpawnPosition(t, waveData.MinSpawnRadius, waveData.MaxSpawnRadius);
                // _spawnCallback?.Invoke(enemyToSpawn.id, spawnPosition, Quaternion.identity);
                spawnedCount++;
            }
            _spawnCoroutine = null;
        }

        //TODO should be a strategy passed from upper class[Relies on Map]
        private Vector2 GetSpawnPosition(Transform t, float minRadius, float maxRadius)
        {
            if (t == null) return Vector2.zero;
            
            Vector2 playerPos = t.position;
            float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
            float radius = Random.Range(minRadius, maxRadius);
            
            return playerPos + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }
    }
}