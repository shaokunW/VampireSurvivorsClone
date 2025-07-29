using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    [RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
    public class WeaponController : MonoBehaviour
    {
        // --- 引用 ---
        public WeaponData data;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        // --- 内部状态 ---
        private float fireCountdown;

        // --- 公共属性查询 ---
        /// <summary>
        /// 初始化武器，只关联静态数据。
        /// </summary>
        public void Initialize(WeaponData data)
        {
            this.data = data;
            this.animator = GetComponent<Animator>();
            this.spriteRenderer = GetComponent<SpriteRenderer>();

            // 应用数据
            if (this.spriteRenderer != null && data.graphics != null)
            {
                this.spriteRenderer.sprite = data.graphics;
                Debug.Log("configure sprite");
            }

            if (this.animator != null && data.animatorController != null)
            {
                this.animator.runtimeAnimatorController = data.animatorController;
                Debug.Log("configure animatorController");
            }
        }

        /// <summary>
        /// 由外部管理器每帧调用，用于更新内部冷却计时。
        /// </summary>
        public void TickCooldown(float deltaTime)
        {
            if (fireCountdown > 0)
            {
                fireCountdown -= deltaTime;
                fireCountdown = MathF.Max(0, fireCountdown);
            }
        }

        /// <summary>
        /// 查询武器当前是否可以开火。
        /// </summary>
        public bool CanFire()
        {
            return fireCountdown <= 0;
        }

        /// <summary>
        /// 执行开火动作，并根据外部传入的冷却时间重置计时器。
        /// </summary>
        /// <param name="fireDirection">由管理器计算好的发射方向</param>
        /// <param name="nextCooldown">由管理器计算好的下一次开火的冷却时间</param>
        public void Fire(Vector2 fireDirection, float nextCooldown, LayerMask layerMask, float maxDistance,
            DamageAbility ability)
        {
            // ability.AddDamage(weaponData.);
            if (animator != null) animator.SetTrigger("Fire");

            foreach (var launcher in data.bulletLaunchers)
            {
                float finalAngle = launcher.angleOffset +
                                   UnityEngine.Random.Range(-launcher.randomSpread, launcher.randomSpread);
                Vector2 bulletDirection = Quaternion.Euler(0, 0, finalAngle) * fireDirection;
                // 调用子弹管理器生成子弹
                BulletManager.Instance.SpawnBullet(launcher.bulletId, transform.position, bulletDirection,
                    layerMask, maxDistance, ability);
            }

            // 使用外部计算好的值来重置冷却
            this.fireCountdown = nextCooldown;
        }

        /// <summary>
        /// 调整武器的朝向。
        /// </summary>
        public void Aim(Vector2 direction)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        /// <summary>
        /// 允许外部通过函数灵活修改冷却时间。
        /// </summary>
        public void ModifyCooldown(Func<float, float> cooldownModifier)
        {
            if (cooldownModifier != null)
            {
                this.fireCountdown = cooldownModifier(this.fireCountdown);
            }
        }
    }
}