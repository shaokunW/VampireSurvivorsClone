// FileName: Core/AI/WanderBehavior.cs
using UnityEngine;

namespace Vampire.AI
{
    [CreateAssetMenu(fileName = "AI_Wander", menuName = "Vampire/AI Behavior/Wander")]
    public class WanderBehavior : AIBehavior
    {
        [Header("类型4: 随机游荡")]
        [Tooltip("不会选择距离自己多近的范围内的点。")]
        public float MinWanderDistance = 2f;
        
        [Tooltip("随机选点的最大半径。")]
        public float MaxWanderDistance = 5f;

        [Tooltip("不会选择距离玩家多近的范围内的点。")]
        public float AvoidPlayerRadius = 3f;

        [Tooltip("移动速度倍率。")]
        public float SpeedMultiplier = 0.8f;
        
        [Tooltip("移动过程中的加速度。")]
        public float SpeedAcceleration = 0f;

        public override void OnEnter(EnemyAIController controller)
        {
            Vector2 selfPosition = controller.transform.position;
            Vector2 playerPosition = controller.Target != null ? (Vector2)controller.Target.position : new Vector2(float.MaxValue, float.MaxValue);

            // 简单实现一个随机点选择
            Vector2 randomPoint;
            int attempts = 0;
            do
            {
                randomPoint = selfPosition + Random.insideUnitCircle * MaxWanderDistance;
                attempts++;
                if (attempts > 10) // 防止死循环
                {
                    randomPoint = selfPosition; 
                    break;
                }
            }
            while (Vector2.Distance(selfPosition, randomPoint) < MinWanderDistance || Vector2.Distance(playerPosition, randomPoint) < AvoidPlayerRadius);
            
            controller.SetMovementTarget(randomPoint);
            controller.SetSpeedMultiplier(SpeedMultiplier);
            controller.SetSpeedAcceleration(SpeedAcceleration);
        }

        public override void UpdateBehavior(EnemyAIController controller)
        {
            // 目标点在OnEnter时已确定，此处无需更新
        }
    }
}