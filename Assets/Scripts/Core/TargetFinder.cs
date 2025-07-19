using UnityEngine;
using System.Collections.Generic;
using Utils;

namespace Vampire
{
    public class TargetFinder : MonoBehaviour
    {
        [Header("寻敌参数")]
        [Tooltip("最大索敌半径")]
        [SerializeField] private float searchRadius = 10f;
        [Tooltip("每个图层最多获取的目标数量")]
        [SerializeField] private int queueSize = 10;
        [Tooltip("需要搜索的目标图层列表")]
        [SerializeField] private List<LayerMask> layerMasks;

        // 【优化】使用Dictionary，并且设置为私有，通过公共属性对外暴露只读版本
        public Dictionary<LayerMask, List<Transform>> CurrentTargets { get; private set; }

        // 【优化】优先队列也只创建一次，避免GC
        private PriorityQueue<Transform, float> _priorityQueue;

        void Awake()
        {
            // --- 【优化】在Awake中初始化所有集合，只做一次 ---
            CurrentTargets = new Dictionary<LayerMask, List<Transform>>();
            _priorityQueue = new PriorityQueue<Transform, float>();

            // 为每个需要检测的图层预先创建好列表，避免后续在Update中new
            foreach (var mask in layerMasks)
            {
                CurrentTargets[mask] = new List<Transform>(queueSize);
            }
        }

        void Update()
        {
            // 【修正】在Update中调用寻敌逻辑
            FindNearestTargetsInRadius();
        }

        private void FindNearestTargetsInRadius()
        {
            // 【优化】在开始新一轮搜索前，清空所有列表的内容，而不是创建新列表
            foreach (var pair in CurrentTargets)
            {
                pair.Value.Clear();
            }

            foreach (var layer in layerMasks)
            {
                // 【修正】使用循环变量 layer, 而不是未定义的 enemyLayer
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius, layer);

                if (colliders.Length > 0)
                {
                    // 【优化】清空并复用优先队列
                    _priorityQueue.Clear();

                    foreach (var col in colliders)
                    {
                        float distSqr = (transform.position - col.transform.position).sqrMagnitude;
                        _priorityQueue.Enqueue(col.transform, distSqr);
                    }

                    // 获取当前图层对应的列表引用
                    var nearestTargetsList = CurrentTargets[layer];
                    int count = Mathf.Min(queueSize, _priorityQueue.Count);

                    for (int i = 0; i < count; i++)
                    {
                        nearestTargetsList.Add(_priorityQueue.Dequeue());
                    }
                }
            }
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, searchRadius);
        }
    }
}