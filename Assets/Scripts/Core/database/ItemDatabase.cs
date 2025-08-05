using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Vampire
{
    public class ItemDatabase : MonoBehaviour
    {
        public static ItemDatabase Instance { get; private set; }
        
        private Dictionary<int, ItemDefinition> _cachedItems = new Dictionary<int, ItemDefinition>();
        
        
        [Tooltip("The Addressable label for all Item assets.")] [SerializeField]
        private string itemLabel = "Item";

        protected void Awake()
        {
            if (!Instance && this != Instance)
            {
                Destroy(gameObject);
            }
            Addressables.LoadAssetsAsync<ItemDefinition>(itemLabel, container =>
            {
                if (container != null && !_cachedItems.ContainsKey(container.id))
                {
                    _cachedItems.Add(container.id, container);
                }
            }).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    Debug.Log($"Successfully loaded {_cachedItems.Count} reward containers.");
                }
                else
                {
                    Debug.LogError("Failed to load reward containers.");
                }
            };
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 异步根据ID获取物品定义。
        /// </summary>
        /// <param name="id">物品ID。这个ID也必须是该物品在Addressables系统中的地址。</param>
        public ItemDefinition GetItemByIDAsync(int id)
        {
            return _cachedItems.TryGetValue(id, out ItemDefinition result) ? result : null;
        }

        /// <summary>
        /// 在游戏退出或场景切换时，释放所有已加载的物品资源。
        /// </summary>
        public void ClearCache()
        {
            foreach (var handle in _cachedItems.Values)
            {
                Addressables.Release(handle);
            }

            _cachedItems.Clear();
            Debug.Log("ItemDatabase cache cleared.");
        }

        private void OnDestroy()
        {
            ClearCache();
        }
    }
}