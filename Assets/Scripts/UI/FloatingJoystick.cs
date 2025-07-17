using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Vampire
{
    /// <summary>
    /// 浮动摇杆，可在屏幕任意位置点击生成
    /// </summary>
    public class FloatingJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("UI组件")] [Tooltip("摇杆的背景图，摇杆将在此区域内移动")] [SerializeField]
        private RectTransform joystickBG;

        [Tooltip("摇杆的可拖动滑块")] [SerializeField] private RectTransform joystickHandle;

        [Header("摇杆设置")] [Tooltip("摇杆可移动的最大半径（像素）")] [SerializeField]
        private float maxRadius = 100f;

        /// <summary>
        /// 摇杆的输出方向。X和Y的值在-1到1之间。
        /// 其他脚本可以通过这个属性获取摇杆输入。
        /// { get; private set; } 表示外部只能读取，只有这个脚本内部可以修改。
        /// </summary>
        public Vector2 Direction { get; private set; }

        // 私有变量
        private Vector2 _startTouchPos; // 摇杆中心（初始触摸点）的局部坐标
        [SerializeField] private UnityEvent<Vector2> onJoystickMoved;
        [SerializeField] private UnityEvent onStartTouch, onEndTouch;
        void Start()
        {
            // 游戏开始时，默认隐藏摇杆
            if (joystickBG != null)
            {
                joystickBG.gameObject.SetActive(false);
                onEndTouch?.Invoke();
            }
        }

        /// <summary>
        /// 当指针（手指/鼠标）在挂载此脚本的UI元素上按下时调用
        /// </summary>
        public void OnPointerDown(PointerEventData eventData)
        {
            
            var clickPosition = eventData.position;
            // 将屏幕坐标转换为RectTransform的局部坐标
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform, // 坐标要转换到的RectTransform（即此脚本所在的UI元素）
                clickPosition, // 当前指针的屏幕位置
                eventData.pressEventCamera, // 渲染此Canvas的摄像机
                out _startTouchPos); // 输出的局部坐标

            // 将摇杆背景移动到点击位置，并激活它
            // startTouchPos相对parent的pivot，由于pivot不是0.5,0.5所以有偏移，简单fix使用localPosition
            // TODO should use anchoredPosition
            joystickBG.localPosition = _startTouchPos;
            joystickBG.gameObject.SetActive(true);
            onStartTouch?.Invoke();
            // 立刻更新一次拖拽逻辑，让摇杆头在按下时就对齐
            OnDrag(eventData);
        }

        /// <summary>
        /// 当指针在UI元素上拖拽时持续调用
        /// </summary>
        public void OnDrag(PointerEventData eventData)
        {
            // 计算当前指针位置与摇杆中心的偏移
            Vector2 currentTouchPos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                (RectTransform)transform,
                eventData.position,
                eventData.pressEventCamera,
                out currentTouchPos);

            Vector2 offset = currentTouchPos - _startTouchPos;

            // 将偏移量限制在最大半径内
            // ClampMagnitude会保持向量的方向，但如果长度超过maxRadius，则将其长度限制为maxRadius
            Vector2 clampedOffset = Vector2.ClampMagnitude(offset, maxRadius);

            // 计算并设置标准化的输入方向 (-1 to 1)
            // Todo Direction is not normalized goback use normalized value
            Direction = clampedOffset / maxRadius;

            // 更新摇杆头的位置
            joystickHandle.anchoredPosition = clampedOffset;
            onJoystickMoved?.Invoke(Direction);
        }

        /// <summary>
        /// 当指针从UI元素上抬起时调用
        /// </summary>
        public void OnPointerUp(PointerEventData eventData)
        {
            // 重置所有状态
            Direction = Vector2.zero;
            joystickHandle.anchoredPosition = Vector2.zero;
            onJoystickMoved?.Invoke(Direction);
            onEndTouch?.Invoke();
            joystickBG.gameObject.SetActive(false);
        }
    }
}