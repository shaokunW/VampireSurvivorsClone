using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    [CreateAssetMenu(fileName = "NewWeaponData", menuName = "GameData/Weapon", order = 1)]
    public class WeaponData : ScriptableObject
    {
        [Header("基础信息")]
        public string weaponID;
        public Sprite graphics; // 武器的精灵图
        public RuntimeAnimatorController animatorController; // 武器的动画控制器

        [Header("战斗逻辑")]
        public float baseAttackRange;  // 基础发射范围
        public float baseFireInterval; // 基础开火间隔(秒)

        [Header("发射物配置")]
        [Tooltip("一次开火会发射的所有子弹")]
        public List<BulletLauncher> bulletLaunchers;

        // 关于“附加效果 List<Function>”的说明：
        // 这是一个高级主题，通常会用一个“效果系统”来实现。
        // 这里我们可以用一个EffectData的ScriptableObject列表来代替。
        // public List<EffectData> additionalEffects; 
    }
}