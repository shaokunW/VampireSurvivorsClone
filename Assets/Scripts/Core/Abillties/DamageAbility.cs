using UnityEngine;

namespace Vampire
{
    public class DamageAbility : BulletAbility
    {
        private float _damageRatio;
        private float _currentDamage;
        private float _damageFalloffRatio;

        /// <summary>
        /// 构造函数，用于在创建时初始化伤害能力。
        /// </summary>
        /// <param name="initialDamage">子弹第一次命中时的基础伤害。</param>
        /// <param name="damageFalloffRatio">每次贯穿后，伤害的衰减系数（0到1之间）。</param>
        public DamageAbility(float initialDamage, float damageRatio, float damageFalloffRatio)
        {
            _currentDamage = initialDamage;
            // 确保衰减率在合理范围内
            _damageRatio = damageRatio;
            _damageFalloffRatio = Mathf.Clamp01(damageFalloffRatio);
        }

        public void AddDamage(float damage)
        {
            _currentDamage += damage;
        }

        public void OnMove(IBulletOwner owner, Transform pos)
        {
        }

        public void OnHit(IBulletOwner owner, Collider2D hitTarget, Transform hitPoint)
        {
            if (hitTarget == null)
            {
                // no hit and destroy
                return;
            }

            // 尝试从命中目标上获取IDamageable组件。
            IMyDamageable damageable = hitTarget.GetComponent<IMyDamageable>();
            if (damageable != null)
            {
                // MISS OR damage reduction processed in target
                var finalDamage = _damageFalloffRatio * _currentDamage;
                damageable.TakeDamage(finalDamage);
                Debug.Log($"对 {hitTarget.name} 造成了 {finalDamage} 点伤害。");

                // 2. 实现【贯通伤害减少】
                // 在造成伤害后，立即更新下一次命中的伤害值。
                _currentDamage *= _damageFalloffRatio;
            }
            else
            {
                Debug.Log("No Damageable");
            }
        }
    }
}