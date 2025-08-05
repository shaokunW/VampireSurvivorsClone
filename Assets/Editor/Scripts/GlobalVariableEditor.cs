using UnityEditor;
using UnityEngine;

namespace Vampire
{
    [CustomEditor(typeof(GlobalVariable<>), true)]
    public class GlobalVariableEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            // 绘制默认的 Inspector 字段 (initialValue, runtimeValue)
            DrawDefaultInspector();

            // 获取当前正在检视的脚本对象
            var script = target;

            // 在 Inspector 中添加一些垂直间距
            EditorGUILayout.Space();

            // 绘制一个按钮，并检查它是否被点击
            if (GUILayout.Button("Reset"))
            {
                // 如果没有实现接口，作为备用方案，使用反射
                var method = script.GetType().GetMethod("ResetValue");
                if (method != null)
                {
                    method.Invoke(script, null);
                    // 标记对象为“脏”，以确保编辑器记录并保存此更改
                    EditorUtility.SetDirty(script);
                }
            }
        }
    }
}