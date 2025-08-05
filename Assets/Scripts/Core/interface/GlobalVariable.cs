using System;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New[TYPE]Variable", menuName = "Vampire/Create/Variable/[TYPE]")]
public class GlobalVariable<T> : ScriptableObject
{
    public event Action OnValueChanged;

    [Tooltip("The value of this variable set in the editor. This is the value it will reset to on play.")]
    [SerializeField]
    private T initialValue;

    // 1. 定义一个公共事件

    [SerializeField] private T runtimeValue;

    public T Value
    {
        get => runtimeValue;
        set
        {
            // 2. 在 set 访问器中，检查值是否真的改变了
            if (!runtimeValue.Equals(value))
            {
                runtimeValue = value;
                // 3. 如果值改变了，就调用（触发）这个事件
                OnValueChanged?.Invoke();
            }
        }
    }

    // OnValidate 仍然可以用于编辑器内的逻辑
    private void OnValidate()
    {
        // 比如，你可以在编辑器里修改值时，也触发事件，让编辑器内的其他窗口响应
#if UNITY_EDITOR
        OnValueChanged?.Invoke();
#endif
    }
    
    private void ResetValue()
    {
        Value = initialValue;
    }

    // You can also add an implicit conversion to make using the variable more seamless.
    public static implicit operator T(GlobalVariable<T> variable)
    {
        return variable.Value;
    }
    
    [CreateAssetMenu(fileName = "NewFloatVariable", menuName = "Vampire/Create/Variable/Float")]
    public class FloatVariable : GlobalVariable<float> { }

    /// <summary>
    /// String 类型的全局变量。
    /// </summary>
    [CreateAssetMenu(fileName = "NewStringVariable", menuName = "Vampire/Create/Variable/String")]
    public class StringVariable : GlobalVariable<string> { }

    /// <summary>
    /// Int 类型的全局变量。
    /// </summary>
    [CreateAssetMenu(fileName = "NewIntVariable", menuName = "Vampire/Create/Variable/Int")]
    public class IntVariable : GlobalVariable<int> { }

    /// <summary>
    /// Bool 类型的全局变量。
    /// </summary>
    [CreateAssetMenu(fileName = "NewBoolVariable", menuName = "Vampire/Create/Variable/Bool")]
    public class BoolVariable : GlobalVariable<bool> { }
    
}