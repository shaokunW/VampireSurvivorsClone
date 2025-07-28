using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Vampire
{
    [ExecuteInEditMode]
    public class WeaponManager : MonoBehaviour
    {
        // --- 引用 ---
        [Header("配置")]
        [Tooltip("角色的初始武器列表")]
        [SerializeField] private List<WeaponData> debugWeapons;
        [Tooltip("所有武器共用的基础预制体，必须挂载有WeaponController脚本")]
        [SerializeField] private GameObject weaponPrefab;

        [SerializeField] public List<Transform> WeaponSlots; // 武器挂点
        // public CharacterStats ownerStats;
        [SerializeField] private TargetFinder targetFinder;

        // --- 运行时数据 ---
        public List<WeaponController> equippedWeapons = new List<WeaponController>();
        
        
        void OnValidate()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += LoadWeaponsToSlots;
#endif
        }

        private void LoadWeaponsToSlots()
        {
            if (weaponPrefab == null)
            {
                Debug.LogError("weapon Prefab is null");
                return;
            }
            foreach (var slot in WeaponSlots)
            {
                CleanWeaponSlotImmediate(slot);
            }
            equippedWeapons.Clear();
            int cnt = Mathf.Min(debugWeapons.Count, WeaponSlots.Count);
            Debug.Log($"LoadWeaponsToSlots {debugWeapons.Count} {WeaponSlots.Count}");

            for (int i = 0; i < cnt; i++)
            {
                Transform currentSlot = WeaponSlots[i];
                WeaponData currentData = debugWeapons[i];
                equippedWeapons.Add(addToSlot(currentData, currentSlot));
            }
        }

        void Update()
        {
            // 获取当前目标
            List<Transform> currentTargets = targetFinder.CurrentTargets;
            Transform currentTarget = currentTargets.FirstOrDefault();

            // --- 统一的武器驱动循环 ---
            foreach (var weapon in equippedWeapons)
            {
                // 1. 每帧更新武器的冷却
                weapon.TickCooldown(Time.deltaTime);

                // 2. 如果有目标，命令武器瞄准
                if (currentTarget != null)
                {
                    Vector2 directionToTarget = (currentTarget.position - weapon.transform.position).normalized;
                    weapon.Aim(directionToTarget);

                    // 3. 检查武器是否可以开火
                    if (weapon.CanFire())
                    {
                        Debug.Log(("start fire"));
                        // 4. 【决策】在这里进行射程检查
                        float finalAttackRange = attackRange(weapon); 
                        if (Vector2.Distance(weapon.transform.position, currentTarget.position) <= finalAttackRange)
                        {
                            // 5. 【决策】在这里计算最终的冷却时间
                            float finalFireInterval = attackSpeed(weapon);
                            Debug.Log(("start fire"));
                            // 6. 【命令】命令武器开火，并把计算好的冷却时间传给它
                            DamageAbility damage = new DamageAbility(1,1,1);
                            weapon.Fire(directionToTarget, finalFireInterval, targetFinder.GetLayerMask(), finalAttackRange, damage);
                        }
                    }
                }
            }
        }

        public float attackRange(WeaponController weapon)
        {
            return weapon.Data.baseAttackRange;
            // return weapon.Data.baseAttackRange * (1 + ownerStats.Range);
        }

        public float attackSpeed(WeaponController weapon)
        {
            return weapon.Data.baseFireInterval;
            // return weapon.Data.baseFireInterval / (1 + ownerStats.AttackSpeed);

        }

        /// <summary>
        /// 装备一把新武器
        /// </summary>
        public void EquipWeapon(WeaponData weaponData)
        {
            foreach (Transform slot in WeaponSlots)
            {
                if (slot.childCount == 0)
                {
                    equippedWeapons.Add(addToSlot(weaponData, slot));
                    return;
                }
            }
        }

        private WeaponController addToSlot(WeaponData weapon, Transform slot)
        {
            // --- 调试步骤 1: 检查参数 ---
            if (slot == null)
            {
                Debug.LogError("错误：尝试附加武器到的 'slot' 为空 (null)！");
                return null; // 或者做其他错误处理
            }
            if (weaponPrefab == null)
            {
                Debug.LogError("错误：'weaponPrefab' 未在 Inspector 中指定！");
                return null;
            }

            Debug.Log($"正在将武器附加到 '{slot.name}'...", slot.gameObject); // 点击这条日志可以在Hierarchy中高亮slot

            GameObject weaponObj = Instantiate(weaponPrefab, slot.position, slot.rotation, slot);
            weaponObj.name = "Weapon_" + weapon.weaponID;
            // --- 调试步骤 2: 验证父子关系 ---
            if (weaponObj.transform.parent == slot)
            {
                Debug.Log($"成功将 '{weaponObj.name}' 设置为 '{slot.name}' 的子物体。", weaponObj);
            }
            else
            {
                Debug.LogError($"未能将 '{weaponObj.name}' 设置为 '{slot.name}' 的子物体。当前的父物体是: " + (weaponObj.transform.parent != null ? weaponObj.transform.parent.name : "null"));
            }
    
            WeaponController wc = weaponObj.GetComponent<WeaponController>();
            wc.Initialize(weapon); 
            return wc;
        }

        private void CleanWeaponSlotImmediate(Transform slot)
        {
            for (int i = slot.childCount - 1; i >= 0; i--)
            {
                var child = slot.GetChild(i);
                if (child.name.StartsWith("Weapon"))
                {
                    // 在编辑器脚本中必须使用DestroyImmediate
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}