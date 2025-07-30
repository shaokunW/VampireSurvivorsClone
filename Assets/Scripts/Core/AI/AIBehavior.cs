using Unity.VisualScripting;
using UnityEngine;

namespace Vampire.AI
{
    
    public abstract class AIBehavior : ScriptableObject
    {
        [Header("通用行为参数")]
        [Tooltip("该行为片段的持续时间（Tick），-1代表无限持续或由自身逻辑决定结束。")]
        public int TicksDuration = 200; // 对应您设计中的“持续时间”

        /// <summary>
        /// 当AI进入此行为状态时调用一次。
        /// </summary>
        /// <param name="controller">执行此行为的AI控制器</param>
        public virtual void OnEnter(EnemyAIController controller) { }

        /// <summary>
        /// AI处于此行为状态时，每Tick调用。
        /// </summary>
        /// <param name="controller">执行此行为的AI控制器</param>
        public abstract void UpdateBehavior(EnemyAIController controller);

        /// <summary>
        /// 当AI退出此行为状态时调用一次。
        /// </summary>
        /// <param name="controller">执行此行为的AI控制器</param>
        public virtual void OnExit(EnemyAIController controller) { }
    }
}