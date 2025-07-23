using UnityEngine;

namespace Vampire
{
    [System.Serializable]
    public class BulletLauncher
    {
        [Tooltip("要发射的子弹的ID，将用于从资源管理器中查找对应的子弹数据")]
        public string bulletId;
        [Tooltip("发射角度相对于枪口方向的偏移")]
        public float angleOffset;
        [Tooltip("在最终发射前，会在此值范围内随机增减一个角度，实现散射效果")]
        public float randomSpread;
    }
}