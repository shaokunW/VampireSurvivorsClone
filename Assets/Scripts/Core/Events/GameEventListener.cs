using UnityEngine;
using UnityEngine.Events;

namespace Vampire
{
    public abstract class GameEventListener<T> : MonoBehaviour
    {
        [Tooltip("Event to register with.")] public GameEvent<T> gameEvent;

        [Tooltip("Response to invoke when Event is raised.")]
        public UnityEvent<T> response;

        private void OnEnable() => gameEvent.RegisterListener(this);
        private void OnDisable() => gameEvent.UnregisterListener(this);
        public void OnEventRaised(T data) => response.Invoke(data);
    }
}