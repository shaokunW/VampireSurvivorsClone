using UnityEngine;

namespace Vampire
{
    [CreateAssetMenu(fileName = "Effect_Drop", menuName = "Vampire/Effects/Drop/Effect")]
    public class DropEffect: GameplayEffect
    {
        public DropRule dropRule;
        public ItemDefinition item;
        public int amount;

        public override void Apply(object input, object context)
        {
            var dropPosition = (Vector3)input;     
            // 检查是否设置了必要的预制件，防止运行时出错。
            var collectablePrefab = dropRule.collectablePrefab;
            if (collectablePrefab == null)
            {
                Debug.LogError("DropEffect: Item Pickup Prefab 未设置！");
                return;
            }
            for (int i = 0; i < amount; i++)
            {
                // 2. 计算随机生成位置
                // 获取一个随机的2D方向
                Vector2 randomDirection = Random.insideUnitCircle.normalized;
                // 在最小和最大半径之间获取一个随机距离
                float randomDistance = Random.Range(dropRule.minRadius, dropRule.maxRadius);
                // 计算最终的生成位置
                Vector3 spawnPosition = dropPosition + (Vector3)(randomDirection * randomDistance);

                // 3. 实例化掉落物预制件
                //TODO pool + sprite
                GameObject pickupObject = Instantiate(collectablePrefab, spawnPosition, Quaternion.identity);
                pickupObject.GetComponent<CollectItem>().Initialize(item.id);
                SpritePool.GetSprite(item.graphicsId, sprite => pickupObject.GetComponent<SpriteRenderer>().sprite = sprite);
            }
        }
    }
}