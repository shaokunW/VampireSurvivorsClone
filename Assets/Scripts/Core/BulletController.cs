using UnityEngine;

namespace Vampire
{

    [RequireComponent(typeof(CircleCollider2D))] // 确保有碰撞体
    public class BulletController : MonoBehaviour
    {
        // --- 引用 ---
        private BulletData data;
        private IBulletOwner owner;
        private Vector2 lastPosition;
        private Vector2 moveDirection;
        private float speed;

        // --- 运行时数据 ---
        private int currentDurability;
        private float currentLifetime;
        private float timeAlive; // 用于计算移动轨迹

        /// <summary>
        /// 初始化子弹，由外部的子弹管理器在生成时调用。
        /// </summary>
        public void Initialize(BulletData bulletData, IBulletOwner bulletOwner, Vector2 initialDirection)
        {
            this.data = bulletData;
            this.owner = bulletOwner;
            this.moveDirection = initialDirection.normalized;

            // 初始化运行时数据
            this.currentDurability = data.baseDurability;
            this.currentLifetime = data.baseLifetime;
            this.timeAlive = 0f;

            // 根据创建者属性计算最终数值
            CharacterStats ownerStats = owner.GetStats();
            this.speed = data.baseSpeed * (1 + ownerStats.Range); // 假设子弹速度受Range影响
            GetComponent<CircleCollider2D>().radius = data.baseRadius * (1 + ownerStats.Range); // 假设子弹大小受Range影响

            this.lastPosition = transform.position;
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
            timeAlive += Time.fixedDeltaTime;
            Vector2 movement = CalculateMovement(Time.fixedDeltaTime);
            Vector2 newPosition = lastPosition + movement;

            // 2. 执行基于胶囊体的碰撞检测
            CheckForCollisions(newPosition);

            // 3. 更新位置和朝向
            transform.position = newPosition;
            transform.right = movement.normalized; // 让子弹头朝向移动方向
            lastPosition = newPosition;
        }

        private Vector2 CalculateMovement(float deltaTime)
        {
            // 根据不同的移动模式计算位移，这里只实现线性模式作为示例
            switch (data.movementPattern)
            {
                case MovementPattern.Linear:
                default:
                    return moveDirection * speed * deltaTime;
                // case MovementPattern.Homing:
                // 在这里添加追踪逻辑
                // case MovementPattern.Wave:
                // 在这里添加正弦波逻辑
            }
        }

        private void CheckForCollisions(Vector2 newPosition)
        {
            float distance = Vector2.Distance(lastPosition, newPosition);
            if (distance < 0.001f) return; // 如果没有移动，则不检测

            float radius = GetComponent<CircleCollider2D>().radius;
            Vector2 direction = (newPosition - lastPosition).normalized;

            // 确定要检测的图层
            int layerMask = 0;
            if (data.canHitPlayer) layerMask |= (1 << LayerMask.NameToLayer("Player"));
            if (data.canHitEnemy) layerMask |= (1 << LayerMask.NameToLayer("Enemies"));

            // 执行胶囊体投射
            RaycastHit2D[] hits = Physics2D.CapsuleCastAll(lastPosition, new Vector2(radius * 2, radius * 2),
                CapsuleDirection2D.Vertical, 0, direction, distance, layerMask);

            foreach (var hit in hits)
            {
                OnHit(hit.collider);
                if (currentDurability <= 0) break; // 如果子弹在中途消失了，停止检测后续目标
            }
        }

        private void OnHit(Collider2D target)
        {
            Debug.Log($"子弹 {data.bulletId} 击中了 {target.name}");

            // 应用伤害
            if (target.TryGetComponent<CharacterStats>(out var targetStats))
            {
                // 这里可以加入更复杂的伤害计算，比如结合创建者的属性
                targetStats.TakeDamage(data.baseDamage);
            }

            // 处理生命窃取
            if (Random.Range(0, 100) < data.baseLifestealChance)
            {
                owner?.OnLifestealSuccess(1f); // 通知创建者回血
            }

            // 触发命中效果
            // data.hitEffects.ForEach(effect => effect.Execute(this, target));

            // 减少耐久度
            currentDurability--;
            if (currentDurability <= 0)
            {
                DestroyBullet();
            }
        }

        private void DestroyBullet()
        {
            // 在这里可以播放消失动画或特效，然后销毁对象
            // 理想情况下是回收到对象池
            Destroy(gameObject);
        }
    }
}