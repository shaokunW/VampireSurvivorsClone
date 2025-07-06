// Assets/Scripts/Config/BoundsConfig.cs
using UnityEngine;

[CreateAssetMenu(fileName = "BoundsConfig", menuName = "GameConfig/Bounds")]
public class BoundsConfig : ScriptableObject
{
    [Min(1)]  public float halfWidth  = 25;     // → width  = 50
    [Min(1)]  public float halfHeight = 25;     // → length = 50

    // —— 要加载的资源 ——  
    public Sprite backgroundSprite;            // 纯 Sprite 贴 Quad；用 Tilemap 就换 TilemapAsset
    public GameObject wallPrefab;              // 带 Rigidbody2D+Collider 的墙体预制
    public GameObject  extraDecorPrefab;        

    #region 便捷 API
    public float MinX => -halfWidth;
    public float MaxX =>  halfWidth;
    public float MinY => -halfHeight;
    public float MaxY =>  halfHeight;

    public Vector2 RandomInside(float margin = 0) =>
        new(Random.Range(MinX + margin, MaxX - margin),
            Random.Range(MinY + margin, MaxY - margin));
    #endregion
}