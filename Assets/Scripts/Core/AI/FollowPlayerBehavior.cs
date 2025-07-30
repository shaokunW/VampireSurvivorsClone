using UnityEngine;
using Vampire.AI;

namespace Vampire.Core.AI
{
    [CreateAssetMenu(fileName = "AI_FollowPlayer", menuName = "Vampire/AI Behavior/FollowPlayer")]
    public class FollowPlayerBehavior : AIBehavior
    {
        [Header("类型2: 跟踪玩家")]
        [Tooltip("与玩家保持的距离。<=0 则直接以玩家为目标点。")]
        public float KeepDistance = 0f;

        [Tooltip("基础移动速度的倍率。")]
        public float SpeedMultiplier = 1.0f;

        [Tooltip("每个Tick速度倍率的变化量（加速度）。")]
        public float SpeedAcceleration = 0f;

        public override void UpdateBehavior(EnemyAIController controller)
        {
            if (controller.Target == null) return;

            // 计算目标点
            Vector2 targetPosition = controller.Target.position;
            Vector2 selfPosition = controller.transform.position;
            Vector2 directionToTarget = (targetPosition - selfPosition).normalized;

            Vector2 destination;
            if (KeepDistance > 0)
            {
                destination = targetPosition - directionToTarget * KeepDistance;
            }
            else
            {
                destination = targetPosition;
            }

            // 更新AI控制器的运行时数据
            controller.SetMovementTarget(destination);
            controller.SetSpeedMultiplier(SpeedMultiplier);
            
            // 注意：您设计中的持续时间是控制“片段自增id”的，这里我们简化为控制行为切换。
            // 速度变化由EnemyAIController在FixedUpdate中处理。
        }
    }
}