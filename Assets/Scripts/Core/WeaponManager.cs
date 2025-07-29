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
        // --- From WeaponSlotsInitializer ---
        [Header("Slot Configuration")]
        public int slotsCount = 1;
        public float distributionRadius = 0.5f;
        private const string ParentName = "WeaponSlotParent";
        private int _previousSlotsCount = -1;
        private float _previousDistributionRadius = -1.0f;

        // --- Original WeaponManager fields ---
        [Header("配置")]
        [Tooltip("角色的初始武器列表")]
        [SerializeField]
        private List<WeaponData> debugWeapons;

        [Tooltip("所有武器共用的基础预制体，必须挂载有WeaponController脚本")]
        [SerializeField]
        private GameObject weaponPrefab;

        [SerializeField] public List<Transform> WeaponSlots; // 武器挂点

        [SerializeField] private TargetFinder targetFinder;

        public List<WeaponController> equippedWeapons = new List<WeaponController>();

        void OnValidate()
        {
#if UNITY_EDITOR
            // Check if slot configuration has changed
            if (slotsCount != _previousSlotsCount || !Mathf.Approximately(distributionRadius, _previousDistributionRadius))
            {
                // Use delayCall to avoid issues with modifying hierarchy during OnValidate
                UnityEditor.EditorApplication.delayCall += () =>
                {
                    if (this != null && this.gameObject != null)
                    {
                        GenerateSlots();
                        _previousSlotsCount = slotsCount;
                        _previousDistributionRadius = distributionRadius;
                        LoadWeaponsToSlots(); // Reload weapons after regenerating slots
                    }
                };
            }
            else
            {
                 LoadWeaponsToSlots();
            }
#else
            // Runtime behavior
            LoadWeaponsToSlots();
#endif
        }

        private void GenerateSlots()
        {
            var parentTransform = GetOrInitSlotParent();
            CleanupSlots(parentTransform);

            if (slotsCount < 1)
            {
                WeaponSlots = new List<Transform>();
                return;
            }

            var slots = new List<Transform>();
            float angleStep = 360f / slotsCount;

            for (int i = 0; i < slotsCount; i++)
            {
                float degrees = i * angleStep;
                float radians = degrees * Mathf.Deg2Rad;

                float x = Mathf.Cos(radians) * distributionRadius;
                float y = Mathf.Sin(radians) * distributionRadius;
                
                var slot = new GameObject("WeaponSlot_" + (i + 1));
                slot.transform.SetParent(parentTransform);
                slot.transform.localPosition = new Vector3(x, y, 0);
                slot.transform.localRotation = Quaternion.identity;
                slots.Add(slot.transform);
            }
            this.WeaponSlots = slots; // Directly assign to the local list
        }

        private Transform GetOrInitSlotParent()
        {
            var parentTransform = transform.Find(ParentName);
            if (parentTransform == null)
            {
                GameObject p = new GameObject(ParentName);
                p.transform.SetParent(transform);
                p.transform.localPosition = Vector3.zero;
                p.transform.localRotation = Quaternion.identity;
                parentTransform = p.transform;
            }
            return parentTransform;
        }

        private void CleanupSlots(Transform parentTransform)
        {
            for (int i = parentTransform.childCount - 1; i >= 0; i--)
            {
                var child = parentTransform.GetChild(i);
                if (child.name.StartsWith("WeaponSlot_"))
                {
#if UNITY_EDITOR
                    // In the editor, DestroyImmediate must be used for objects
                    DestroyImmediate(child.gameObject);
#endif
                }
            }
        }

        private void LoadWeaponsToSlots()
        {
            if (weaponPrefab == null)
            {
                Debug.LogError("weapon Prefab is null");
                return;
            }
            // Ensure WeaponSlots is not null before proceeding
            if (WeaponSlots == null) return;

            // Clean existing weapons from slots
            foreach (var slot in WeaponSlots)
            {
                if (slot != null) // The slot might have been destroyed
                   CleanWeaponSlotImmediate(slot);
            }

            equippedWeapons.Clear();
            int cnt = Mathf.Min(debugWeapons.Count, WeaponSlots.Count);

            for (int i = 0; i < cnt; i++)
            {
                Transform currentSlot = WeaponSlots[i];
                if (currentSlot == null) continue; // Skip if slot is somehow null

                WeaponData currentData = debugWeapons[i];
                if (currentData == null)
                {
                    Debug.Log("Weapon data is null");
                }
                else
                {
                    equippedWeapons.Add(addToSlot(currentData, currentSlot));
                }
            }
        }

        void Update()
        {
            // The rest of the Update method remains the same...
            List<Transform> currentTargets = targetFinder.CurrentTargets;
            Transform currentTarget = currentTargets.FirstOrDefault();

            foreach (var weapon in equippedWeapons)
            {
                weapon.TickCooldown(Time.deltaTime);

                if (currentTarget != null)
                {
                    Vector2 directionToTarget = (currentTarget.position - weapon.transform.position).normalized;
                    weapon.Aim(directionToTarget);

                    if (weapon.CanFire())
                    {
                        float finalAttackRange = attackRange(weapon);

                        if (Vector2.Distance(weapon.transform.position, currentTarget.position) <= finalAttackRange)
                        {
                            float finalFireInterval = attackSpeed(weapon);
                            DamageAbility damage = new DamageAbility(1, 1, 1);
                            weapon.Fire(directionToTarget, finalFireInterval, targetFinder.GetLayerMask(),
                                finalAttackRange, damage);
                        }
                    }
                }
            }
        }

        public float attackRange(WeaponController weapon)
        {
            return weapon.data.baseAttackRange;
        }

        public float attackSpeed(WeaponController weapon)
        {
            return weapon.data.baseFireInterval;
        }

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
            if (slot == null)
            {
                Debug.LogError("Error: 'slot' to attach weapon to is null!");
                return null;
            }

            if (weapon == null)
            {
                Debug.LogError("weapon data is null");
                return null;
            }

            if (weaponPrefab == null)
            {
                Debug.LogError("Error: 'weaponPrefab' not specified in Inspector!");
                return null;
            }

            GameObject weaponObj = Instantiate(weaponPrefab, slot.position, slot.rotation, slot);
            weaponObj.name = "Weapon_" + weapon.weaponID;

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
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}