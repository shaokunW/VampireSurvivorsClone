using UnityEngine;

namespace Vampire
{
    /// <summary>
    /// 将挂载对象（必须是 RectTransform）限制在当前设备的 Safe Area 内。
    /// - 运行时若 Safe Area 发生变化（例如旋转、分屏、折叠屏展开），会自动重新适配。
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        /// <summary>
        /// 目标 RectTransform（挂载节点）
        /// </summary>
        private RectTransform _rectTransform;

        /// <summary>
        /// 记录上一次使用的 Safe Area，避免每帧重复设置
        /// </summary>
        private Rect _lastSafeArea = Rect.zero;

        /// <summary>
        /// Awake 在脚本启用时最早执行 —— 确保一帧内就完成适配
        /// </summary>
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            ApplySafeArea();                   // 初次设置
        }

        /// <summary>
        /// 每帧检测 Safe Area 是否变化（方向切换、刘海机被遮挡等）
        /// </summary>
        private void Update()
        {
            // Safe Area 改变才重新适配；否则不做任何操作
            if (Screen.safeArea != _lastSafeArea)
                ApplySafeArea();
        }

        /// <summary>
        /// 把 Safe Area（屏幕像素坐标）转换成 0-1 归一化锚点，并写入 RectTransform
        /// </summary>
        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;   // 系统返回的“安全显示区域”
            _lastSafeArea = safeArea;          // 记录，供下次对比

            // ---------------- ① 像素坐标 ----------------
            // safeArea.position ＝ 左下角
            // + size ⇒ 右上角（仍是像素单位）
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            // ---------------- ② 归一化到 0-1 ----------------
            // 用屏幕宽高做分母，把像素值转成百分比
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            // ---------------- ③ 写回锚点 ----------------
            // 使 UI 元素的四个角对齐 Safe Area
            _rectTransform.anchorMin = anchorMin;
            _rectTransform.anchorMax = anchorMax;
        }
    }
}
