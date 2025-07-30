using System;
using Unity.VisualScripting;
using UnityEngine;
using Vampire.AI;

namespace Vampire
{
    [RequireComponent(typeof(Rigidbody2D), typeof(TargetFinder), typeof(StatsController))]
    public class EnemyAIController : MonoBehaviour
    {
        [Header("AI配置")]
        [Tooltip("怪物的AI行为配置方案")]
        [SerializeField] private AIProfile aiProfile;
        
        [Header("基础属性")]
        [Tooltip("怪物的基础移动速度")]
        [SerializeField] private float baseSpeed = 3.0f;

        // --- 运行时数据 ---
        public Transform Target { get; private set; }
        private Vector2 _movementTarget;
        private float _currentSpeedMultiplier;
        private float _currentSpeedAcceleration;
        
        private int _currentBehaviorIndex;
        private float _tickCounter;

        // --- 组件引用 ---
        private Rigidbody2D _rb;
        private TargetFinder _targetFinder;
        private StatsController _statsController;
        public event Action<EnemyAIController> OnDeactivated;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _targetFinder = GetComponent<TargetFinder>();
            _statsController = GetComponent<StatsController>();
            // controlled by script
            _rb.isKinematic = true;

        }

        void Start()
        {
            InitializeAI();
        }

        private void InitializeAI()
        {
            if (aiProfile == null || aiProfile.Behaviors == null || aiProfile.Behaviors.Count == 0)
            {
                Debug.LogWarning($"{gameObject.name} 的AI Profile未配置或为空，AI将不会运行。");
                this.enabled = false;
                return;
            }
            
            _currentBehaviorIndex = 0;
            _tickCounter = 0;
            GetCurrentBehavior()?.OnEnter(this);
        }

        void FixedUpdate()
        {
            if (GetCurrentBehavior() == null)
            {
                _rb.velocity = Vector2.zero;
                return;
            }

            // 1. 更新目标
            UpdateTarget();

            // 2. 执行当前行为逻辑
            GetCurrentBehavior().UpdateBehavior(this);
            
            // 3. 执行移动
            MoveTowardsTarget();

            // 4. 更新速度和计时器
            _currentSpeedMultiplier += _currentSpeedAcceleration * Time.fixedDeltaTime;
            _tickCounter += Time.fixedDeltaTime;

            // 5. 检查是否需要切换到下一个行为
            int duration = GetCurrentBehavior().TicksDuration;
            if (duration > 0 && _tickCounter >= duration)
            {
                TransitionToNextBehavior();
            }
        }

        private void UpdateTarget()
        {
            // 从 TargetFinder 获取最近的目标
            if (_targetFinder.CurrentTargets.Count > 0)
            {
                Target = _targetFinder.CurrentTargets[0];
            }
            else
            {
                Target = null;
            }
        }

        private void MoveTowardsTarget()
        {
            if (_movementTarget != null)
            {
                float finalSpeed = baseSpeed * _currentSpeedMultiplier;
                // 计算当前帧能移动的最大距离
                float step = finalSpeed * Time.fixedDeltaTime; 
                // 使用 MoveTowards 来更新位置
                transform.position = Vector2.MoveTowards(transform.position, _movementTarget, step);
            }
        }

        private void TransitionToNextBehavior()
        {
            // 退出当前行为
            GetCurrentBehavior()?.OnExit(this);

            // 切换到下一个行为
            _currentBehaviorIndex = (_currentBehaviorIndex + 1) % aiProfile.Behaviors.Count;
            _tickCounter = 0;
            
            // 进入新行为
            GetCurrentBehavior()?.OnEnter(this);
        }
        
        private AIBehavior GetCurrentBehavior()
        {
            return aiProfile.Behaviors[_currentBehaviorIndex];
        }

        #region Public Setters for Behaviors
        // 这些方法供 Behavior ScriptableObjects 回调，以更新AI的运行时状态

        public void SetMovementTarget(Vector2 target)
        {
            _movementTarget = target;
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            _currentSpeedMultiplier = multiplier;
        }
        
        public void SetSpeedAcceleration(float acceleration)
        {
            _currentSpeedAcceleration = acceleration;
        }
        #endregion
    }
}