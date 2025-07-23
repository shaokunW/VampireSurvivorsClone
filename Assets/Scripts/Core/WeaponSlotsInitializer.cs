using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Vampire
{
    [ExecuteInEditMode]
    public class WeaponSlotsInitializer : MonoBehaviour
    {
        private const string ParentName = "WeaponSlotParent"; 
        public int slotsCount = 1;
        public float distributionRadius = 0.5f;


        public void OnValidate()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null && this.gameObject != null)
                {
                    GenerateSlots();
                }
            };
#endif
        }

        private void GenerateSlots()
        {
            var parentTransform = GetOrInitSlotParent();
            CleanupSlots(parentTransform);

            if (slotsCount < 1)
            {
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
            var weaponManager = GetComponent<WeaponManager>();
            weaponManager.WeaponSlots = slots;
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
                    // 在编辑器脚本中必须使用DestroyImmediate
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}