using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    [Serializable]
    public class CharacterStats
    {
        // --- 核心生存属性 ---
        [Header("核心生存属性")] protected float CurrentHealth;

        [Tooltip("最大生命值")] public float MaxHealth;

        [Tooltip("生命再生 (点/秒)")] public float HealthRegen; // 设计上可以处理为每秒恢复的值

        // [Tooltip("生命窃取率 (0 to 1)")] [Range(0, 1)]
        // public float LifeStealChance; // 0.1f 代表 10%

        // --- 核心伤害属性 ---
        [Header("核心伤害属性 (这些值通常是百分比加成)")] [Tooltip("所有伤害的最终增伤百分比, 0.1f = +10%")]
        public float Damage;

        // [Tooltip("近战伤害增伤百分比")] public float MeleeDamage;
        //
        // [Tooltip("远程伤害增伤百分比")] public float RangedDamage;

        // [Tooltip("元素伤害增伤百分比")] public float ElementalDamage;
        //
        // [Tooltip("工程学伤害增伤百分比")] public float Engineering;
        //
        // [Tooltip("爆炸伤害增伤百分比")] public float ExplosionDamage;


        // --- 核心攻击属性 ---
        [Header("核心攻击属性")] [Tooltip("攻击速度增伤百分比, 0.1f = +10%")]
        public float AttackSpeed;

        [Tooltip("暴击率 (0 to 1), 超过1的部分可能用于计算超级暴击")]
        public float CritChance;

        [Tooltip("范围增伤百分比, 0.1f = +10%")] public float Range;

        [Tooltip("子弹穿透数量")] public int PiercingCount;


        // --- 核心防御属性 ---
        [Header("核心防御属性")] [Tooltip("护甲 (百分比减伤), 0.1f = 10%减伤")]
        public float Armor;

        [Tooltip("闪避率 (0 to 0.6, 最高60%), 0.1f = 10%闪避")] [Range(0, 0.9f)] // 某些角色可以到90%
        public float DodgeChance;

        [Tooltip("移动速度增伤百分比, 0.1f = +10%")] public float Speed;

        //
        // /// <summary>
        // /// 角色受到伤害的统一处理方法
        // /// </summary>
        // /// <param name="rawDamage">原始伤害值</param>
        // public void TakeDamage(float rawDamage)
        // {
        //     // 1. 计算闪避
        //     if (UnityEngine.Random.value < DodgeChance)
        //     {
        //         Debug.Log("伤害被闪避!");
        //         return; // 闪避成功，不承受伤害
        //     }
        //
        //     // 2. 计算护甲减伤
        //     // 伤害计算公式可以有很多种，这里用一种常见的： 伤害 = 原始伤害 * (1 - 护甲值)
        //     float finalDamage = rawDamage * (1 - Armor);
        //     if (finalDamage < 0) finalDamage = 0; // 避免护甲为负时反而加血
        //
        //     // 3. 扣除生命值
        //     CurrentHealth -= finalDamage;
        //     Debug.Log($"受到 {finalDamage} 点伤害, 剩余生命: {CurrentHealth}");
        //
        //     // 4. 触发事件，通知UI等其他系统
        //     OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        //
        //     // 5. 检查死亡
        //     if (CurrentHealth <= 0)
        //     {
        //         CurrentHealth = 0;
        //         Die();
        //     }
        // }
        //
        // /// <summary>
        // /// 角色恢复生命
        // /// </summary>
        // /// <param name="amount">恢复量</param>
        // public void Heal(float amount)
        // {
        //     CurrentHealth += amount;
        //     // 确保生命值不会超过上限
        //     if (CurrentHealth > MaxHealth)
        //     {
        //         CurrentHealth = MaxHealth;
        //     }
        //
        //     OnHealthChanged?.Invoke(CurrentHealth, MaxHealth);
        // }

    }
}