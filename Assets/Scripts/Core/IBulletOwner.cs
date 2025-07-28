using UnityEngine;

namespace Vampire
{
    public interface IBulletOwner
    {
        /// <summary>
        /// 当子弹成功触发生命窃取时，会调用此方法。
        /// </summary>
        /// <param name="healAmount">恢复的生命值</param>
        void OnLifestealSuccess(float healAmount);

        /// <summary>
        /// 获取创建者的Transform组件，用于某些效果的定位（例如，爆炸效果的中心）。
        /// </summary>
        /// <returns>创建者的Transform</returns>
        Transform GetTransform();
    }
}