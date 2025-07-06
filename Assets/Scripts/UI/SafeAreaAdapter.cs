using System;
using UnityEngine;

namespace Vampire
{

    public class SafeAreaAdapter : MonoBehaviour
    {
        private RectTransform _rectTransform;
        private Rect _safeArea;
        private Vector2 _minAnchor;
        private Vector2 _maxAnchor;
        public RectTransform gameArea;
        public RectTransform joyStickArea;
        public float heightRatio = -1f;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _safeArea = Screen.safeArea;
           initSafeArea();
           // Screen.safeArea.size 给你的是屏幕像素坐标系下的“安全区域”宽高，单位是物理像素。
           //
           // _rectTransform.rect.size 给你的是UI空间（Canvas空间）下的宽高，单位是UI像素（受Canvas Scaler影响）。
           //
           // 这两者一般情况下不会完全相等，尤其是Canvas启用“Scale With Screen Size”适配的时候。


            Vector2 safeSize = _rectTransform.rect.size; // 这样才能获取锚点刷新后的实际宽高

            float sideLength = Mathf.Min(safeSize.x, safeSize.y);
            if (heightRatio > 0)
                sideLength = Mathf.Min(sideLength, heightRatio * safeSize.y);

            float joyStickHeight = Mathf.Max(0, safeSize.y - sideLength);

            // 主视图区：anchor、pivot都在顶部中间
            gameArea.anchorMin = new Vector2(0.5f, 1f);
            gameArea.anchorMax = new Vector2(0.5f, 1f);
            gameArea.pivot = new Vector2(0.5f, 1f);
            gameArea.sizeDelta = new Vector2(sideLength, sideLength);
            gameArea.anchoredPosition = new Vector2(0, 0);

            // 操作区：anchor、pivot都在顶部中间
            joyStickArea.anchorMin = new Vector2(0.5f, 1f);
            joyStickArea.anchorMax = new Vector2(0.5f, 1f);
            joyStickArea.pivot = new Vector2(0.5f, 1f);
            joyStickArea.sizeDelta = new Vector2(sideLength, joyStickHeight);
            joyStickArea.anchoredPosition = new Vector2(0, -sideLength);
        }

        public void OnValidate()
        {
            Awake();
        }

        public void updateSafeArea()
        {
            _safeArea = Screen.safeArea;

        }

        private void initGameArea(float height)
        {
            gameArea.sizeDelta = new Vector2(0, height);
        }

        private void initSafeArea()
        {
            _minAnchor = _safeArea.position;
            _maxAnchor = _minAnchor + _safeArea.size;
            _minAnchor.x /= Screen.width;
            _minAnchor.y /= Screen.height;
            _maxAnchor.x /= Screen.width;
            _maxAnchor.y /= Screen.height;
            _rectTransform.anchorMin = _minAnchor;
            _rectTransform.anchorMax = _maxAnchor;
            _rectTransform.sizeDelta = Vector2.zero;
            _rectTransform.anchoredPosition = Vector2.zero;
        }

        private void initJoystickArea(float newY, float newHeight)
        {
            var v = joyStickArea.anchoredPosition;
            joyStickArea.anchoredPosition = new Vector2(v.x, newY);
            joyStickArea.sizeDelta = new Vector2(0, newHeight);
        }
    }
}