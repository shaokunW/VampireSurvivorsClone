using System;
using UnityEngine;

namespace Vampire
{
    [Serializable]
    public class StatsController: MonoBehaviour, IMyDamageable, IMyHealable
    {
        private static float _zeroThreshold = 0.01f;
        public float _maxHp;
        public float _currentHp;
        public float _damageReduction;
        
        public void TakeDamage(float damage)
        {
            damage = Mathf.Max(0f, damage * (1 - _damageReduction));
            ChangeHp(-damage);
        }

        public void Heal(float healAmount)
        {
            healAmount = Mathf.Max(0f, healAmount);
            ChangeHp(healAmount);
        }

        private void ChangeHp(float delta)
        {
            var newHp = Mathf.Clamp(0, _currentHp + delta, _maxHp);
            _currentHp = newHp;
        }
        
        public bool Die() => _currentHp <= _zeroThreshold;
    }
}