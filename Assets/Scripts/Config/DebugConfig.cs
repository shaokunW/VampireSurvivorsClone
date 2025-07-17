using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vampire
{
    public class DebugConfig : MonoBehaviour
    {
        [SerializeField] protected CharacterBlueprint blueprint;

        void Awake()
        {
            if (CrossSceneData.CharacterBlueprint == null)
            {
                CrossSceneData.CharacterBlueprint = blueprint;
            }
        }

    }
}
