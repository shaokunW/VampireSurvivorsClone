// Assets/Scripts/Environment/ArenaWall.cs
using UnityEngine;

namespace Vampire
{
    /// <summary>
    /// 挂在墙体上的组件：
    /// 1. 确保 Rigidbody2D 是 Static
    /// 2. 保证 BoxCollider2D 大小随 localScale 同步
    /// 3. 提供厚度/可见 Sprite 选项
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(Rigidbody2D))]
    public class ArenaWall : MonoBehaviour
    {
        [Header("视觉可选")]
        [Tooltip("可选：给墙体加个 SpriteRenderer 方便在 Scene 里看到")]
        [SerializeField] private Sprite wallSprite;
    
        private BoxCollider2D _collider;
    
        private void Reset()
        {
            // Editor里点“Reset”或首次挂脚本时调用 —— 自动加/配置组件
            var rb = GetComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;
            rb.simulated = true;
    
            _collider = GetComponent<BoxCollider2D>();
            _collider.size = Vector2.one;   // 初始 1×1，让 Scale 控真实尺寸
    
            if (wallSprite && GetComponent<SpriteRenderer>() == null)
            {
                var sr = gameObject.AddComponent<SpriteRenderer>();
                sr.sprite = wallSprite;
                sr.drawMode = SpriteDrawMode.Sliced;
                sr.size = Vector2.one;
            }
        }
    
        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            SyncColliderSize();
        }
    
        private void OnValidate()          // 在 Inspector 改 Scale 时即同步
        {
            if (_collider == null) _collider = GetComponent<BoxCollider2D>();
            SyncColliderSize();
        }
    
        private void SyncColliderSize()
        {
            // BoxCollider2D 真实覆盖范围 = size × lossyScale
            _collider.size = Vector2.one;
            _collider.offset = Vector2.zero;
        }
    }
}
