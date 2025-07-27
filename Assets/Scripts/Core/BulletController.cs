using System;
using UnityEngine;

namespace Vampire
{

    [RequireComponent(typeof(CircleCollider2D))] // 确保有碰撞体
    public class BulletController : MonoBehaviour
    {
        // --- 引用 ---
        private CircleCollider2D circleCollider;
        private Vector2 fromPosition;
        private float _startWidth;
        private float _endWidth;
        private float _maxDistance;

        private Vector2 _velocity;

        // --- 运行时数据 ---
        private int currentDurability;
        private float currentLifetime;
        private LayerMask layerMask;

        public void Awake()
        {
            circleCollider = GetComponent<CircleCollider2D>();
        }

        /// <summary>
        /// 初始化子弹，由外部的子弹管理器在生成时调用。
        /// </summary>
        public void Initialize(BulletData data, Vector2 startPos, Vector2 velocity, LayerMask layerMask)
        {
            _velocity = velocity;
            // 初始化运行时数据
            this.currentDurability = data.baseDurability;
            this.currentLifetime = data.baseLifetime;
            this.fromPosition = startPos;
            this.layerMask = layerMask;
            // 根据创建者属性计算最终数值
            circleCollider.radius = data.baseRadius;
        }

        void FixedUpdate()
        {
            // 更新生命周期
            currentLifetime -= Time.fixedDeltaTime;
            if (currentLifetime <= 0)
            {
                DestroyBullet();
                return;
            }

            // 1. 计算当前帧的移动
            Vector2 movement = _velocity * Time.fixedDeltaTime;
            Vector2 newPosition = fromPosition + movement;

            // 2. 执行基于胶囊体的碰撞检测
            CheckForCollisions(newPosition);

            // 3. 更新位置和朝向
            transform.position = newPosition;
            transform.right = movement.normalized; // 让子弹头朝向移动方向
            fromPosition = newPosition;
        }

        private void CheckForCollisions(Vector2 newPosition)
        {
            float distance = Vector2.Distance(fromPosition, newPosition);
            if (distance < 0.001f) return; // 如果没有移动，则不检测

            float radius = circleCollider.radius;
            Vector2 direction = (newPosition - fromPosition).normalized;
            // 执行胶囊体投射
            RaycastHit2D[] hits = Physics2D.CapsuleCastAll(fromPosition, new Vector2(radius * 2, radius * 2),
                CapsuleDirection2D.Vertical, 0, direction, distance, layerMask);

            foreach (var hit in hits)
            {
                OnHit(hit.collider, hit.transform);
                if (currentDurability <= 0) break; // 如果子弹在中途消失了，停止检测后续目标
            }
        }

        private void OnHit(Collider2D hitTarget, Transform hitPosition)
        {
            Debug.Log($"hit {hitTarget.name}");
        }

        private void DestroyBullet()
        {
        }

    }
}