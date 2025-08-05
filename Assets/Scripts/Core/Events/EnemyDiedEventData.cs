using System;
using UnityEngine;

namespace Vampire
{
    [Serializable]
    public class EnemyDiedEventData
    {
        public string EnemyID;
        public Vector3 EnemyPosition;
    }
}