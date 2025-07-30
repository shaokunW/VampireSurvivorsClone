using UnityEngine;

namespace Vampire.AI
{
    [CreateAssetMenu(fileName = "AI_Charge", menuName = "Vampire/AI Behavior/Charge")]
    public class ChargeBehavior : AIBehavior
    {
        [Header("类型5: 冲锋")]
        [Tooltip("距离玩家多远时可以触发此行为。")]
        public float EngageDistance = 8f;

        [Tooltip("冲锋时的移动速度倍率。")]
        public float SpeedMultiplier = 3.0f;
        
        [Tooltip("冲锋过程中的加速度。")]
        public float SpeedAcceleration = 0.1f;

        private Vector2 _chargeDirection;

        public override void OnEnter(EnemyAIController controller)
        {
            // 在行为开始时就确定好冲锋方向和目标点
            if (controller.Target == null) return;
            
            Vector2 targetPosition = controller.Target.position;
            Vector2 selfPosition = controller.transform.position;
            _chargeDirection = (targetPosition - selfPosition).normalized;

            // 冲锋目标点是一个“远方”的点，由持续时间和速度决定最终能冲多远
            Vector2 destination = selfPosition + _chargeDirection * 100f; // 100f 是一个足够远的值
            controller.SetMovementTarget(destination);
            controller.SetSpeedMultiplier(SpeedMultiplier);
            controller.SetSpeedAcceleration(SpeedAcceleration);
        }

        public override void UpdateBehavior(EnemyAIController controller)
        {
            // 在冲锋过程中，目标和方向是固定的，所以Update里可以什么都不做
            // 具体的移动由 EnemyAIController 的 FixedUpdate 完成
        }
    }
}