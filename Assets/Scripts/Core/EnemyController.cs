using System;
using UnityEngine;

namespace Vampire
{
    public class EnemyController : MonoBehaviour
    {
        public StatsController statsController;
        public event Action<EnemyController> OnDeactivated;
        public EnemyData enemyData;
        public bool killedEventSent;
        public GameEvent<EnemyDiedEventData> DiedEvent;

        public void Awake()
        {
            statsController = GetComponent<StatsController>();
        }

        public void Initialize(EnemyData data)
        {
            this.enemyData = data;
            this.killedEventSent = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (statsController.Die())
            {
                if (!killedEventSent)
                {
                    var diedEvent = new EnemyDiedEventData();
                    diedEvent.EnemyID = enemyData.id;
                    diedEvent.EnemyPosition = transform.position;
                    DiedEvent.Raise(diedEvent);
                    killedEventSent = true;
                    OnDeactivated?.Invoke(this);
                }
            }
        }
    }
}