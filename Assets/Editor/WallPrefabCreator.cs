// Assets/Editor/WallPrefabCreator.cs
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Vampire
{
    public static class WallPrefabCreator
    {
        private const string prefabPath = "Assets/Prefabs/Wall.prefab";

        [MenuItem("Tools/Create Wall Prefab")]
        public static void CreateWallPrefab()
        {
            // 1) 在临时场景里建一个空物体
            GameObject wall = new GameObject("Wall");
            wall.AddComponent<ArenaWall>();      // 自动附带 Rigidbody2D + BoxCollider2D via [Require]

            // 2) 保存 / 覆盖 prefab
            PrefabUtility.SaveAsPrefabAsset(wall, prefabPath, out bool success);

            // 3) 清理临时对象
            Object.DestroyImmediate(wall);

            EditorUtility.DisplayDialog(
                "Wall Prefab Creator",
                success ? $"已生成/更新 {prefabPath}" 
                    : "生成失败，请检查路径或权限",
                "OK");
        }
    }
}
