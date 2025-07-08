using UnityEngine;

namespace Vampire
{
    // ExecuteAlways 属性非常适合这种布局适配脚本，让你在编辑器里就能看到效果
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))] // 确保该脚本挂载的对象一定有RectTransform
    public class SafeAreaAdapter : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Rect _lastSafeArea; // 用于检测安全区是否真的改变了

        public RectTransform gameArea;
        public RectTransform joyStickArea;
        public float heightRatio = -1f;

        private void OnEnable()
        {
            _rectTransform = GetComponent<RectTransform>();
            _lastSafeArea = new Rect(0,0,0,0); // 初始化
            ApplySafeArea(); // 首次启用时立即应用一次
        }

        private void Update()
        {
            // 在编辑器模式下，我们需要在Update中检测变化
            // 因为OnRectTransformDimensionsChange可能不会在所有编辑器场景变化时都触发
            #if UNITY_EDITOR
            if (Application.isPlaying == false)
            {
                ApplySafeArea();
            }
            #endif
        }

        // 当RectTransform的尺寸改变时，此方法会被自动调用
        private void OnRectTransformDimensionsChange()
        {
            // 在运行模式下，这个回调是最高效和准确的
            if (Application.isPlaying)
            {
                ApplySafeArea();
            }
        }

        private void ApplySafeArea()
        {
            if (_rectTransform == null) return;

            Rect currentSafeArea = Screen.safeArea;

            // 优化：只有当安全区真正发生变化时才执行更新，避免不必要的计算
            if (_lastSafeArea == currentSafeArea)
            {
                return;
            }

            _lastSafeArea = currentSafeArea;

            // 1. 将根RectTransform适配到屏幕的安全区域
            Vector2 minAnchor = currentSafeArea.position;
            Vector2 maxAnchor = currentSafeArea.position + currentSafeArea.size;

            minAnchor.x /= Screen.width;
            minAnchor.y /= Screen.height;
            maxAnchor.x /= Screen.width;
            maxAnchor.y /= Screen.height;

            _rectTransform.anchorMin = minAnchor;
            _rectTransform.anchorMax = maxAnchor;
            _rectTransform.sizeDelta = Vector2.zero;
            _rectTransform.anchoredPosition = Vector2.zero;

            // 2. 在安全区内部，划分GameArea和JoystickArea
            // 注意：此时我们直接使用计算好的 safeArea.size，而不再依赖不稳定的 _rectTransform.rect.size
            // 但需要将像素尺寸转换为UI单位尺寸，最简单的方式是假设根Canvas的缩放是均匀的
            // 为了简化，我们直接使用_rectTransform.rect.size，但在一个延迟调用的方法里会更安全
            // 不过，由于我们现在在OnRectTransformDimensionsChange里，这个值现在是正确的了！
            UpdateChildLayouts();
        }

        private void UpdateChildLayouts()
        {
            // 确保引用的对象都存在
            if (gameArea == null || joyStickArea == null) return;
            
            // 现在，因为我们在OnRectTransformDimensionsChange里，rect.size是更新后的正确值
            Vector2 safeSize = _rectTransform.rect.size;

            float sideLength = Mathf.Min(safeSize.x, safeSize.y);
            if (heightRatio > 0)
            {
                sideLength = Mathf.Min(sideLength, heightRatio * safeSize.y);
            }

            float joyStickHeight = Mathf.Max(0, safeSize.y - sideLength);

            // 主视图区设置
            gameArea.anchorMin = new Vector2(0.5f, 1f);
            gameArea.anchorMax = new Vector2(0.5f, 1f);
            gameArea.pivot = new Vector2(0.5f, 1f);
            gameArea.sizeDelta = new Vector2(sideLength, sideLength);
            gameArea.anchoredPosition = Vector2.zero;

            // 操作区设置
            joyStickArea.anchorMin = new Vector2(0.5f, 1f);
            joyStickArea.anchorMax = new Vector2(0.5f, 1f);
            joyStickArea.pivot = new Vector2(0.5f, 1f);
            joyStickArea.sizeDelta = new Vector2(safeSize.x, joyStickHeight); // 摇杆区宽度应撑满安全区
            joyStickArea.anchoredPosition = new Vector2(0, -sideLength);
        }
    }
}