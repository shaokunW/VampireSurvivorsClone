using System;
using UnityEngine;

namespace Vampire
{
    [CreateAssetMenu(fileName = "Effect_Change_Exp", menuName = "Vampire/Effects/Change/Exp")]
    public class ChangeExpEffect : GameplayEffect
    {
        public int amount;

        public override void Apply(object input, object context)
        {
            Debug.Log($"ChangeExpEffect: {amount}");
        }
    }
}