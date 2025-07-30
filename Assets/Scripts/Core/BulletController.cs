using System;
using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    [RequireComponent(typeof(CircleCollider2D), typeof(SpriteRenderer))] // 确保有碰撞体
    public class BulletController : MonoBehaviour
    {
        public event Action<BulletController> OnDeactivated;
        private RaycastHit2D[] _hitsCache = new RaycastHit2D[16]; // 假设最多同时命中16个目标
        private CircleCollider2D circleCollider;
        private float _startWidth;
        private float _endWidth;
        
        private Vector2 fromPosition;
        private Vector2 _velocity;
        private float _initialMaxDistance;
        
        // --- 运行时数据 ---
        private float _traveledDistance;   // 记录已经飞行的距离
        private int currentDurability;
        private float currentLifetime;
        
        private LayerMask layerMask;
        private HashSet<Collider2D> hitSet;
        private BulletAbility ability;
        private IBulletOwner owner;


        public void Awake()
        {
            circleCollider = GetComponent<CircleCollider2D>();
            hitSet = new HashSet<Collider2D>();
        }

        /// <summary>
        /// 初始化子弹，由外部的子弹管理器在生成时调用。
        /// </summary>
        public void Initialize(BulletData data,
            IBulletOwner owner,
            Vector2 startPos,
            Vector2 velocity,
            LayerMask layerMask,
            float maxDistance,
            BulletAbility ability)
        {
            _velocity = velocity;
            // 初始化运行时数据
            this.currentDurability = data.baseDurability;
            this.currentLifetime = data.baseLifetime;
            this.fromPosition = startPos;
            this.owner = owner;
            this.layerMask = layerMask;
            
            
            // 根据创建者属性计算最终数值
            circleCollider.radius = data.baseRadius;
            this.ability = ability;
            this._initialMaxDistance = maxDistance;
            this._traveledDistance = 0;
            transform.position = startPos;
            transform.right = velocity.normalized; // 让子弹头朝向移动方向
            hitSet.Clear();
        }

        void FixedUpdate()
        {
            if (currentDurability <= 0)
            {
                DestroyBullet();
                return;
            }

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
            _traveledDistance += movement.magnitude;
            if (_initialMaxDistance <= _traveledDistance)
            {
                DestroyBullet();
                return;
            }

            // 2. 执行基于胶囊体的碰撞检测
            CheckForCollisions(newPosition);

            // 3. 更新位置和朝向
            transform.position = newPosition;
            transform.right = movement.normalized; // 让子弹头朝向移动方向
            fromPosition = newPosition;
            OnMove(owner, transform);
        }

        private void CheckForCollisions(Vector2 newPosition)
        {
            float distance = Vector2.Distance(fromPosition, newPosition);
            if (distance < 0.001f) return; // 如果没有移动，则不检测

            float radius = circleCollider.radius;
            Vector2 direction = (newPosition - fromPosition).normalized;
            // --- 修改：使用不会产生GC垃圾的 NonAlloc 版本 ---
            int hitCount = Physics2D.CapsuleCastNonAlloc(fromPosition, new Vector2(radius * 2, radius * 2),
                CapsuleDirection2D.Vertical, 0, direction, _hitsCache, distance, layerMask);

            // 只遍历实际检测到的 hitCount 个目标
            for (int i = 0; i < hitCount; i++)
            {
                var hit = _hitsCache[i]; // 从缓存中获取碰撞信息
                if (currentDurability > 0 && hitSet.Add(hit.collider))
                {
                    currentDurability -= 1;
                    OnHit(hit.collider, hit.transform);
                }
            }
        }

        private void OnHit(Collider2D hitTarget, Transform hitPosition)
        {
            ability?.OnHit(owner, hitTarget, hitPosition);
        }

        private void OnMove(IBulletOwner owner, Transform target)
        {
            ability?.OnMove(owner, target);
        }

        private void DestroyBullet()
        {
            ability?.OnHit(owner, null, null);
            OnDeactivated?.Invoke(this);
        }
    }
}