using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    public class EnemyController : MonoBehaviour
    {
        private float maxHp;
        private float currentHp;
        private float currentSpeed;
        public  event Action<EnemyController> OnDeactivated;
        
        public void Initialize(EnemyData data)
        {
            
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}