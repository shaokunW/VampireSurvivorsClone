using System;
using UnityEngine;
using Vampire;

namespace Vampire
{
    public class ScriptableObjectExtensions
    {
        public static bool TryGetEventGenericArgs(ScriptableObject obj, out Type genericType)
        {
            genericType = null;
            if (obj == null) return false;   
            var baseType = obj.GetType();
            var target = typeof(GameEvent<>);
            var rootType = typeof(object);
            while (baseType != null && baseType != rootType)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == target)
                {
                    genericType = baseType.GetGenericArguments()[0];
                    return true;
                }
                baseType = baseType.BaseType;
            }

            return false;
        }
    }
}