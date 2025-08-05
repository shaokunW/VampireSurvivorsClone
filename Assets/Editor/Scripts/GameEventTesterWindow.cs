using System;
using UnityEditor;
using UnityEngine;

namespace Vampire
{
    public class GameEventTesterWindow : EditorWindow
    {
        private ScriptableObject eventAsset;
        private SerializedObject serializedObjectForWindow;
        private SerializedProperty dataProperty;

        [SerializeReference] private object serializedData;

        [MenuItem("Tools/GameEvent Tester")]
        private static void Open()
        {
            // 获取或创建一个新的编辑器窗口实例
            GetWindow<GameEventTesterWindow>("GameEvent Tester");
        }

        public void OnGUI()
        {
            eventAsset =
                (ScriptableObject)EditorGUILayout.ObjectField("Event Asset", eventAsset, typeof(ScriptableObject),
                    false);
            // 如果没有拖入事件资产，则不继续绘制
            if (eventAsset == null)
            {
                // 清理旧数据，防止切换事件后UI残留
                serializedData = null;
                return;
            }

            if (ScriptableObjectExtensions.TryGetEventGenericArgs(eventAsset, out var dataType))
            {
                EnsureDataInstance(dataType);
                // 初始化或更新 SerializedObject
                // 这一步是为了让 Unity 的序列化系统能够识别和处理我们的窗口实例
                if (serializedObjectForWindow == null || serializedObjectForWindow.targetObject != this)
                {
                    serializedObjectForWindow = new SerializedObject(this);
                    dataProperty = serializedObjectForWindow.FindProperty(nameof(serializedData));
                }

                serializedObjectForWindow.Update();
                EditorGUILayout.PropertyField(dataProperty, new GUIContent($"Data {dataType.Name}"), true);
                serializedObjectForWindow.ApplyModifiedProperties();
                // 绘制触发按钮
                if (GUILayout.Button("Raise Event"))
                {
                    // 使用反射找到并调用事件资产的 Raise 方法
                    var raiseMethod = eventAsset.GetType().GetMethod("Raise");
                    if (raiseMethod != null)
                    {
                        // 将我们缓存和编辑好的数据 (serializedData) 作为参数传入
                        raiseMethod.Invoke(eventAsset, new[] { serializedData });
                        Debug.Log($"<color=lime>Event '{eventAsset.name}' raised successfully!</color>", eventAsset);
                    } 
                    else
                    {
                        // 如果拖入的不是一个有效的 GameEvent<T> 资产，显示错误提示
                        EditorGUILayout.HelpBox("The provided asset is not a valid GameEvent<T>.", MessageType.Error);
                    }
                }
                
            }
        }

        // [4] 辅助方法
        // --------------------------------------------------
        /// <summary>
        /// 确保 serializedData 字段的实例类型与期望的事件数据类型一致。
        /// 如果不一致（或者为 null），则创建一个新的实例。
        /// </summary>
        /// <param name="expectedType">期望的事件数据类型 (T)</param>
        private void EnsureDataInstance(Type expectedType)
        {
            if (serializedData == null || serializedData.GetType() != expectedType)
            {
                // 使用 Activator.CreateInstance 创建一个新实例
                // 注意：这要求类型 T 必须有一个无参数的构造函数
                serializedData = Activator.CreateInstance(expectedType);

                // 创建新实例后，需要重置 SerializedObject 以避免引用旧数据
                serializedObjectForWindow = null;
            }
        }
    }
}