using UnityEngine;

namespace Vampire
{


// [ExecuteInEditMode] 属性让这个脚本可以在编辑器模式下运行，
// 这样你调整参数时就能立即看到场景中的变化，非常方便！
    [ExecuteInEditMode]
    public class ArenaGenerator : MonoBehaviour
    {
        [Header("竞技场设置")] [Tooltip("竞技场的宽度")] public float arenaWidth = 30f;

        [Tooltip("竞技场的高度")] public float arenaHeight = 20f;

        [Tooltip("墙壁的厚度")] public float wallThickness = 1f;

        [Header("视觉效果")] [Tooltip("墙壁的颜色")] public Color wallColor = Color.white;

        // 私有变量，用来存储对四面墙的引用
        private Transform wallTop;
        private Transform wallBottom;
        private Transform wallLeft;
        private Transform wallRight;

        // 当脚本被加载或Inspector中的值被改变时调用
        // 这是实现编辑器内实时更新的关键
        private void OnValidate()
        {
            // 延迟调用GenerateWalls，以避免在OnValidate中直接修改场景可能引发的Unity编辑器问题
            // 这是更安全、更专业的做法
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (this != null) // 确保对象没有被销毁
                {
                    GenerateWalls();
                }
            };
#endif
        }

        private void GenerateWalls()
        {
            // 1. 清理旧的墙壁（如果存在）
            // 使用 transform.Find 来查找子对象
            if (transform.Find("Wall_Top")) DestroyImmediate(transform.Find("Wall_Top").gameObject);
            if (transform.Find("Wall_Bottom")) DestroyImmediate(transform.Find("Wall_Bottom").gameObject);
            if (transform.Find("Wall_Left")) DestroyImmediate(transform.Find("Wall_Left").gameObject);
            if (transform.Find("Wall_Right")) DestroyImmediate(transform.Find("Wall_Right").gameObject);

            // 2. 创建新的四面墙
            wallTop = CreateWall("Wall_Top");
            wallBottom = CreateWall("Wall_Bottom");
            wallLeft = CreateWall("Wall_Left");
            wallRight = CreateWall("Wall_Right");

            // 3. 设置墙壁的位置和尺寸
            // 上墙
            wallTop.localPosition = new Vector3(0, arenaHeight / 2, 0);
            wallTop.localScale = new Vector3(arenaWidth + wallThickness, wallThickness, 1);

            // 下墙
            wallBottom.localPosition = new Vector3(0, -arenaHeight / 2, 0);
            wallBottom.localScale = new Vector3(arenaWidth + wallThickness, wallThickness, 1);

            // 左墙
            wallLeft.localPosition = new Vector3(-arenaWidth / 2, 0, 0);
            wallLeft.localScale = new Vector3(wallThickness, arenaHeight, 1);

            // 右墙
            wallRight.localPosition = new Vector3(arenaWidth / 2, 0, 0);
            wallRight.localScale = new Vector3(wallThickness, arenaHeight, 1);
        }

        // 一个辅助方法，用于创建单个墙壁对象
        private Transform CreateWall(string wallName)
        {
            GameObject wall = new GameObject(wallName);
            wall.transform.parent = this.transform; // 设置为Arena对象的子对象

            // 添加SpriteRenderer让墙壁可见
            SpriteRenderer sr = wall.AddComponent<SpriteRenderer>();
            sr.sprite = CreateWhitePixelSprite(); // 使用一个纯白色的Sprite
            sr.color = wallColor; // 设置颜色
            sr.sortingOrder = -10; // 让墙壁在背景层，避免遮挡玩家

            // 添加BoxCollider2D用于物理碰撞
            wall.AddComponent<BoxCollider2D>();

            return wall.transform;
        }

        // 一个辅助方法，用于程序化创建一个1x1的纯白Sprite
        private Sprite CreateWhitePixelSprite()
        {
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f), 1);
        }
    }
}