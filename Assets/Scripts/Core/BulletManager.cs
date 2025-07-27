using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Vampire
{
    public class BulletManager : MonoBehaviour
    {
        public static BulletManager Instance { get; private set; }
        [SerializeField] public GameObject bulletPrefab;
        private Dictionary<string, AsyncOperationHandle<BulletData>> _bulletDataHandlesCache;

        void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                _bulletDataHandlesCache = new Dictionary<string, AsyncOperationHandle<BulletData>>();
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        public async void SpawnBullet(string bulletId, Vector2 startPos, Vector2 bulletDirection, LayerMask LayerMask)
        {
            Debug.DrawRay(startPos, bulletDirection * 5, Color.cyan, 0.1f);
            var handle = GetOrLoadData(bulletId);
            await handle.Task; // 等待数据加载完成
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load BulletData for id: {bulletId}");
                return;
            }

            BulletData data = handle.Result;
            BulletController controller =
                Instantiate(bulletPrefab, Instance.transform).GetComponent<BulletController>();
            controller.Initialize(data, startPos, bulletDirection, LayerMask);
        }

        private AsyncOperationHandle<BulletData> GetOrLoadData(string bulletId)
        {
            if (_bulletDataHandlesCache.TryGetValue(bulletId, out AsyncOperationHandle<BulletData> handle))
            {
                return handle;
            }

            // 如果没有，则开始异步加载，并将操作句柄存入缓存
            AsyncOperationHandle<BulletData> newHandle = Addressables.LoadAssetAsync<BulletData>(bulletId);
            _bulletDataHandlesCache[bulletId] = newHandle;
            return newHandle;
        }
    }
}