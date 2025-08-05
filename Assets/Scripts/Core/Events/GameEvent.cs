using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    // 这是一个抽象类，不能直接创建实例，需要被继承
    public abstract class GameEvent<T> : ScriptableObject
    {
        private readonly List<GameEventListener<T>> listeners = 
            new List<GameEventListener<T>>();

        public void Raise(T data)
        {
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                listeners[i].OnEventRaised(data);
            }
        }

        public void RegisterListener(GameEventListener<T> listener) => listeners.Add(listener);
        public void UnregisterListener(GameEventListener<T> listener) => listeners.Remove(listener);
    }

}
