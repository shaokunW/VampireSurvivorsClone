using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    public class JoystickAreaFitter : MonoBehaviour
    {
        public RectTransform gameArea;   // 拖 GameArea
        RectTransform rt;

        void Awake() => rt = (RectTransform)transform;

        void LateUpdate()
        {
            float h = gameArea.rect.height;

            // 旧：rt.offsetMin = new Vector2(0, h);   // Bottom = GameArea 高度
            //     rt.offsetMax = Vector2.zero;        // Top = 0 (Stretch到顶)

            rt.offsetMin = Vector2.zero;              // Bottom 贴 SafeAreaRoot 底
            rt.offsetMax = new Vector2(0, -h);        // Top 离 SafeAreaRoot 顶 h 像素
        }
    }

}
