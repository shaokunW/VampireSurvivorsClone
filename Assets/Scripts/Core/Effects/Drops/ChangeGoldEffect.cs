using System;
using UnityEngine;

namespace Vampire
{
    [CreateAssetMenu(fileName = "Effect_Change_Gold", menuName = "Vampire/Effects/Change/Gold")]
    public class ChangeGoldEffect : GameplayEffect
    {
        public int amount;

        public override void Apply(object input, object context)
        {
            Debug.Log($"ChangeGoldEffect: {amount}");
        }
    }
}