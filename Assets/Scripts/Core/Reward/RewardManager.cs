using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Random = UnityEngine.Random;

namespace Vampire
{
    public class RewardManager : MonoBehaviour
    {
        public static RewardManager Instance { get; private set; }
        
        [Tooltip("The Addressable label for all RewardContainer assets.")]
        [SerializeField] private string rewardContainerLabel = "RewardContainer";

        private Dictionary<int, RewardContainer> _rewardContainers = new Dictionary<int, RewardContainer>();
        private bool _isReady = false;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            LoadAllRewardContainers();
        }

        private void LoadAllRewardContainers()
        {
            Addressables.LoadAssetsAsync<RewardContainer>(rewardContainerLabel, container =>
            {
                if (container != null && !_rewardContainers.ContainsKey(container.id))
                {
                    _rewardContainers.Add(container.id, container);
                }
            }).Completed += handle =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    _isReady = true;
                    Debug.Log($"Successfully loaded {_rewardContainers.Count} reward containers.");
                }
                else
                {
                    Debug.LogError("Failed to load reward containers.");
                }
            };
        }
        
        public List<int> Select(int id)
        {
            _rewardContainers.TryGetValue(id, out var value);
            return Select(value);
        }
        
        public List<int> Select(RewardContainer rewardContainer)
        {
            var ret = new List<int>();
            if (rewardContainer == null)
            {
                return ret;
            }
            var rewardSlots = rewardContainer.slots;
            float sum = 0;
            foreach (var rewardSlot in rewardSlots)
            {
                if (MustSelect(rewardSlot))
                {
                    HandleSelected(ret, rewardSlot);
                }
                else
                {
                    sum += rewardSlot.Weight;
                }
            }
            var r = Random.Range(0, sum);
            var cur = 0f;
            for (var i = 0; i < rewardSlots.Count; i++)
            {
                var rewardSlot = rewardSlots[i];
                if (MustSelect(rewardSlot))
                {
                    continue;
                }

                cur += rewardSlot.Weight;
                if (cur > r)
                {
                    HandleSelected(ret, rewardSlot);
                    break;
                }
            }
            return ret;
        }

        private void HandleSelected(List<int> rewards, RewardSlot unit)
        {
            if (unit.isContainer)
            {
                _rewardContainers.TryGetValue(unit.ItemId, out var value);
                for (int i = 0; i < unit.Cnt; i++)
                {
                    
                    rewards.AddRange(Select(value));
                }
            }
            else
            {
                for (int i = 0; i < unit.Cnt; i++)
                {
                    rewards.Add(unit.ItemId);
                }
            }
        }

        private bool MustSelect(RewardSlot unit)
        {
            return unit.Weight < -0.001;
        }
    }
}