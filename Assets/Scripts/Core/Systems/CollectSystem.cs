using System;
using UnityEngine;

namespace Vampire.Systems
{
    public class CollectSystem : MonoBehaviour
    {
        public GameObject player;
        public float radius;

        public void Update()
        {
            var collects = FindObjectsByType<CollectItem>(FindObjectsSortMode.None);
            foreach (var collectable in collects)
            {
                var id = collectable.Collect();
                float dist = Vector3.Distance(collectable.transform.position, player.transform.position);
                if (dist < radius)
                {
                    var item = ItemDatabase.Instance.GetItemByIDAsync(id);
                    foreach (var gameplayEffect in item.effects)
                    {
                        gameplayEffect.Apply(player, null);
                    }
                }
            }
        }
    }
}