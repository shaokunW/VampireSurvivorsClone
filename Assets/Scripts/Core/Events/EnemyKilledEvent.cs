using UnityEngine;

namespace Vampire
{
    [CreateAssetMenu(fileName = "EnemyKilledEvent", menuName = "Events/EnemyKilledEvent")]

    public class EnemyKilledEvent : GameEvent<EnemyDiedEventData> { }
    
}