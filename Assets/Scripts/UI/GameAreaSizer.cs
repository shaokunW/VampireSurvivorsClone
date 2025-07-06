using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace Vampire
{
    /// GameArea ＝ 安全区全宽，高度 = inspector 给定；<=0 时用正方形
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class GameAreaSizer : MonoBehaviour
    {
        [Tooltip("≤0 使用正方形；>0 使用自定义高度(px)")] public float customHeight = -1f; // Inspector 字段

        [Header("Camera (填你的 Character Camera)")]
        public Camera gameplayCam; // 拖 Character Camera 进来

        RectTransform rt;
        RenderTexture rtTexture;
        RawImage raw;

        void Awake()
        {
            rt = GetComponent<RectTransform>();
            raw = GetComponent<RawImage>();
            ApplyAll();
        }

#if UNITY_EDITOR
        void OnValidate() => ApplyAll();
        void Update() => ApplyAll(); // 编辑器/运行时都实时刷新
#endif

        void ApplyAll()
        {
            if (rt == null) return;

            float width = ((RectTransform)rt.parent).rect.width;
            float height = customHeight > 0 ? customHeight : width;

            // Stretch 横向
            rt.sizeDelta = new Vector2(0, height); // 只改 Y
            // rt.anchoredPosition = Vector2.zero;

            // 生成 / 重建 RenderTexture
            // if (gameplayCam && (rtTexture == null || rtTexture.width != (int)width || rtTexture.height != (int)height))
            // {
            //     if (rtTexture) rtTexture.Release();
            //     rtTexture = new RenderTexture((int)width, (int)height, 24, RenderTextureFormat.ARGB32);
            //     gameplayCam.targetTexture = rtTexture;
            //     raw.texture = rtTexture;
            // }
            Vector2 size = rt.rect.size;
            Vector2 pivot = rt.pivot;
            Debug.Log($"[GameAreaSizer] {((RectTransform)rt.parent).name} {((RectTransform)rt.parent).anchoredPosition} {((RectTransform)rt.parent).anchorMin} {((RectTransform)rt.parent).anchorMax}");
            Debug.Log($"[GameAreaSizer] rt实际大小: width_source={width} width={size.x}, height={size.y}, pivot={pivot}");
        }
    }
}