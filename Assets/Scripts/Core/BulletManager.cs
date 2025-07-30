using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Vampire
{
    public class BulletManager : MonoBehaviour
    {
        public static BulletManager Instance { get; private set; }
        [SerializeField] public GameObject bulletPrefab;
        private Dictionary<string, AsyncOperationHandle<BulletData>> _bulletDataHandlesCache;
        private Dictionary<AssetReferenceSprite, AsyncOperationHandle<Sprite>> _bulletSpriteHandlesCache;
        private ObjectPool<BulletController> _bulletPool;

        void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                _bulletDataHandlesCache = new Dictionary<string, AsyncOperationHandle<BulletData>>();
                _bulletSpriteHandlesCache = new Dictionary<AssetReferenceSprite, AsyncOperationHandle<Sprite>>();
                _bulletPool = new ObjectPool<BulletController>(
                    createFunc: () =>
                    {
                        var o = Instantiate(bulletPrefab);
                        var controller = o.GetComponent<BulletController>();
                        controller.OnDeactivated += _bulletPool.Release;
                        return controller;
                    },
                    actionOnGet: (controller) => controller.gameObject.SetActive(true),
                    actionOnRelease: (controller) => controller.gameObject.SetActive(false),
                    actionOnDestroy: (controller) => Destroy(controller.gameObject),
                    collectionCheck: true,
                    defaultCapacity: 1000,
                    maxSize: 1000);
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(gameObject);
            }
        }

        public async void SpawnBullet(string bulletId,
            Vector2 startPos,
            Vector2 bulletDirection,
            LayerMask LayerMask,
            float maxDistance, BulletAbility abilitiy)
        {
            Debug.DrawRay(startPos, bulletDirection * 5, Color.cyan, 0.1f);
            var handle = GetOrLoadData(bulletId);
            await handle.Task; // 等待数据加载完成
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load BulletData for id: {bulletId}");
                return;
            }

            // INIT TRIGGER
            BulletData data = handle.Result;
            var bulletObject = GetFromPool(startPos, Quaternion.identity);
            BulletController controller = bulletObject.GetComponent<BulletController>();
#if UNITY_EDITOR
            controller.gameObject.name = $"Bullet_{data.bulletId}";
#endif
            controller.Initialize(data, null, startPos, bulletDirection, LayerMask, maxDistance,
                abilitiy);
            // INIT EFFECT
            // --- 2. 异步加载视觉资源 ---
            // 检查 data.graphics 是否是一个有效的引用
            var spriteRenderer = bulletObject.GetComponent<SpriteRenderer>();
            var spriteHandle = GetOrLoadSprite(data.graphicsId);
            await spriteHandle.Task;
            if (spriteHandle.Status == AsyncOperationStatus.Succeeded)
            {
                spriteRenderer.sprite = spriteHandle.Result;
            }
            else
            {
                // 如果没有指定图形，可以设置一个默认或清空
                spriteRenderer.sprite = null;
                Debug.LogWarning($"Bullet '{data.graphicsId}' 的 Graphics 未指定或无效。");
            }
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

        private AsyncOperationHandle<Sprite> GetOrLoadSprite(AssetReferenceSprite graphicId)
        {
            if (_bulletSpriteHandlesCache.TryGetValue(graphicId, out AsyncOperationHandle<Sprite> handle))
            {
                return handle;
            }

            // 如果没有，则开始异步加载，并将操作句柄存入缓存
            var newHandle = graphicId.LoadAssetAsync<Sprite>();
            _bulletSpriteHandlesCache[graphicId] = newHandle;
            return newHandle;
        }

        private BulletController GetFromPool(Vector2 pos, Quaternion quaternion)
        {
            var o = _bulletPool.Get();
            o.transform.rotation = quaternion;
            o.transform.position = pos;
            return o;
        }
    }
}