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
        [SerializeField] private LayerMask layerMask;
        public List<Transform> CurrentTargets = new List<Transform>();

        private PriorityQueue<Transform, float> _priorityQueue = new PriorityQueue<Transform, float>();

        void Update()
        {
            FindNearestTargetsInRadius();
        }

        private void FindNearestTargetsInRadius()
        {
            CurrentTargets.Clear();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, searchRadius, layerMask);
            // Debug.Log($"colliders.Length={colliders.Length}");
            if (colliders.Length > 0)
            {
                _priorityQueue.Clear();
                foreach (var col in colliders)
                {
                    float distSqr = (transform.position - col.transform.position).sqrMagnitude;
                    _priorityQueue.Enqueue(col.transform, distSqr);
                }
                // 获取当前图层对应的列表引用
                int count = Mathf.Min(queueSize, _priorityQueue.Count);
                for (int i = 0; i < count; i++)
                {
                    CurrentTargets.Add(_priorityQueue.Dequeue());
                }
            }
        }


        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, searchRadius);
        }

        public LayerMask GetLayerMask()
        {
            return layerMask;
        }
    }
}