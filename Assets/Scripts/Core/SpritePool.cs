using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// 1. 改为静态类，不再继承 MonoBehaviour
public static class SpritePool
{
    // 2. 使用一个字典统一管理所有资源的句柄（无论是加载中还是已完成）
    //    键使用 object 类型来兼容 RuntimeKey
    private static Dictionary<object, AsyncOperationHandle<Sprite>> assetHandles = new Dictionary<object, AsyncOperationHandle<Sprite>>();

    /// <summary>
    /// 异步获取Sprite（优先缓存，否则异步加载）
    /// </summary>
    public static void GetSprite(AssetReferenceSprite assetRef, System.Action<Sprite> callback)
    {
        // 增加对 assetRef 本身的有效性检查
        if (assetRef == null || !assetRef.RuntimeKeyIsValid())
        {
            callback?.Invoke(null);
            return;
        }

        // 3. 使用 RuntimeKey 作为更健壮的键
        object key = assetRef.RuntimeKey;

        // 检查是否已有句柄（无论是加载中还是已完成）
        if (assetHandles.TryGetValue(key, out var handle))
        {
            // 如果句柄已存在，直接将回调添加到其 Completed 事件
            // 注意：如果操作已完成，此回调会被立即调用
            handle.Completed += (h) => { callback?.Invoke(h.Result); };
            return;
        }

        // 如果没有句柄，则开始新的加载操作
        var newHandle = Addressables.LoadAssetAsync<Sprite>(key);
        
        // 立即将新句柄存入字典，防止并发请求时重复加载
        assetHandles[key] = newHandle;

        newHandle.Completed += (h) =>
        {
            // 如果加载失败
            if (h.Status != AsyncOperationStatus.Succeeded)
            {
                // 打印错误日志，方便调试
                Debug.LogError($"[SpritePool] 资源加载失败, Key: {key}. Error: {h.OperationException}");
                
                // 关键：从字典中移除失败的句柄，以便下次可以重新尝试加载
                assetHandles.Remove(key);
                callback?.Invoke(null);
            }
            else // 加载成功
            {
                // 只需调用回调即可。句柄已保存在字典中用于后续的缓存和释放。
                callback?.Invoke(h.Result);
            }
        };
    }
    
    /// <summary>
    /// (新增) 释放单个缓存的Sprite资源
    /// </summary>
    public static void ReleaseSprite(AssetReferenceSprite assetRef)
    {
        if (assetRef == null || !assetRef.RuntimeKeyIsValid()) return;

        object key = assetRef.RuntimeKey;
        if (assetHandles.TryGetValue(key, out var handle))
        {
            // 从字典中移除句柄，并通知 Addressables 释放资源
            assetHandles.Remove(key);
            Addressables.Release(handle);
        }
    }


    /// <summary>
    /// (已修复) 释放全部缓存资源
    /// </summary>
    public static void Clear()
    {
        // 4. 正确的释放逻辑
        // 遍历所有缓存的句柄，并调用 Addressables.Release 释放它们
        foreach (var handle in assetHandles.Values)
        {
            Addressables.Release(handle);
        }
        
        // 最后清空字典
        assetHandles.Clear();
    }
}