using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class UIPanelPrefabCreator
{
    [MenuItem("Tools/Create iPhone Vertical UI Panels")]
    public static void CreatePanels()
    {
        // 查找Canvas，没有就新建
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGO.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGO.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
        }

        // 上部内容区
        GameObject gameViewArea = new GameObject("GameViewArea", typeof(Image));
        gameViewArea.transform.SetParent(canvas.transform, false);
        RectTransform rect1 = gameViewArea.GetComponent<RectTransform>();
        rect1.anchorMin = new Vector2(0, 0.3f);
        rect1.anchorMax = new Vector2(1, 1);
        rect1.offsetMin = Vector2.zero;
        rect1.offsetMax = Vector2.zero;
        gameViewArea.GetComponent<Image>().color = new Color(0.2f, 0.25f, 0.35f, 0.8f); // 深色

        // 下部控制区
        GameObject controlArea = new GameObject("ControlArea", typeof(Image));
        controlArea.transform.SetParent(canvas.transform, false);
        RectTransform rect2 = controlArea.GetComponent<RectTransform>();
        rect2.anchorMin = new Vector2(0, 0);
        rect2.anchorMax = new Vector2(1, 0.3f);
        rect2.offsetMin = Vector2.zero;
        rect2.offsetMax = Vector2.zero;
        controlArea.GetComponent<Image>().color = new Color(0.85f, 0.85f, 0.85f, 0.6f); // 浅色

        // 自动保存为预制体
#if UNITY_EDITOR
        string path = "Assets/iPhoneVerticalUIPanels.prefab";
        PrefabUtility.SaveAsPrefabAsset(canvas.gameObject, path);
        Debug.Log("Panels and Canvas saved as prefab at: " + path);
#endif
    }
}
