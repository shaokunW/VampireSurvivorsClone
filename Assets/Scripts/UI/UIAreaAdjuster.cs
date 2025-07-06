using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vampire;

public class UIAreaAdjuster : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _safeArea;
    private Vector2 _minAnchor;
    private Vector2 _maxAnchor;
    private void Awake(){
        _rectTransform = GetComponent<RectTransform>();
        _safeArea = Screen.safeArea;
        _minAnchor = _safeArea.position;
        _maxAnchor = _minAnchor + _safeArea.size;
        _minAnchor.x /= Screen.width;
        _minAnchor.y /= Screen.height;
        _maxAnchor.x /= Screen.width;
        _maxAnchor.y /= Screen.height;
        _rectTransform.anchorMin = _minAnchor;
        _rectTransform.anchorMax = _maxAnchor;
    }
    
    void LogRectTransformProperties(RectTransform rt)
    {
        Debug.Log($"[UI]----- RectTransform Properties of {rt.gameObject.name} -----");
        Debug.Log($"[UI]anchoredPosition: {rt.anchoredPosition}");
        Debug.Log($"[UI]anchoredPosition3D: {rt.anchoredPosition3D}");
        Debug.Log($"[UI]anchorMin: {rt.anchorMin}");
        Debug.Log($"[UI]anchorMax: {rt.anchorMax}");
        Debug.Log($"[UI]offsetMin: {rt.offsetMin}");
        Debug.Log($"[UI]offsetMax: {rt.offsetMax}");
        Debug.Log($"[UI]pivot: {rt.pivot}");
        Debug.Log($"[UI]sizeDelta: {rt.sizeDelta}");
        Debug.Log($"[UI]rect: {rt.rect}");
        Debug.Log($"[UI]localPosition: {rt.localPosition}");
        Debug.Log($"[UI]localScale: {rt.localScale}");
        Debug.Log($"[UI]localEulerAngles: {rt.localEulerAngles}");
        Debug.Log($"[UI]position (world): {rt.position}");
        Debug.Log($"[UI]rotation (world): {rt.rotation}");
        Debug.Log($"[UI]lossyScale (world): {rt.lossyScale}");
        Debug.Log($"[UI]hasChanged: {rt.hasChanged}");
        Debug.Log($"[UI]parent: {(rt.parent != null ? rt.parent.name : "None")}");
        Debug.Log("----------------------------------------");
    }
}