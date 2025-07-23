using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
        public List<Transform> WeaponSlots { get; set; } // 武器挂点
        [SerializeField] private CharacterStats ownerStats;
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
                // 3. 实例化武器预制体
                GameObject weaponObj = Instantiate(weaponPrefab, currentSlot.position, currentSlot.rotation, currentSlot);
                weaponObj.name = "Weapon_" + currentData.weaponID; 
                WeaponController wc = weaponObj.GetComponent<WeaponController>();
                if (wc != null)
                {
                    Debug.Log($"wc initialized {wc} {currentSlot.name}");
                    wc.Initialize(currentData);
                    equippedWeapons.Add(wc); // 将初始化好的武器控制器加入管理列表
                }
                else
                {
                    Debug.LogError($"错误: 武器预制体 {weaponPrefab.name} 上没有找到WeaponController脚本!", this);
                }
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
                        // 4. 【决策】在这里进行射程检查
                        float finalAttackRange = weapon.Data.baseAttackRange * (1 + ownerStats.Range);
                        if (Vector2.Distance(weapon.transform.position, currentTarget.position) <= finalAttackRange)
                        {
                            // 5. 【决策】在这里计算最终的冷却时间
                            float finalFireInterval = weapon.Data.baseFireInterval / (1 + ownerStats.AttackSpeed);

                            // 6. 【命令】命令武器开火，并把计算好的冷却时间传给它
                            weapon.Fire(directionToTarget, finalFireInterval);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 装备一把新武器
        /// </summary>
        public void EquipWeapon(WeaponData weaponData, GameObject weaponPrefab)
        {
            foreach (Transform slot in WeaponSlots)
            {
                if (slot.childCount == 0)
                {
                    GameObject weaponObj = Instantiate(weaponPrefab, slot.position, slot.rotation, slot);
                    WeaponController wc = weaponObj.GetComponent<WeaponController>();

                    if (wc != null)
                    {
                        wc.Initialize(weaponData); // 初始化时不再需要传入玩家数据
                        equippedWeapons.Add(wc);
                    }

                    return;
                }
            }
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