using System.Collections.Generic;
using UnityEngine;
using HCore.DataStructure;
using HCore.SerializedDataStructure;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "WeakPointConfig", menuName = "BoxingArena/WeakPointConfig")]
public class WeakPointSO : ScriptableObject
{
    [SerializeField, BoxGroup("Weak Point Multipliers")] private SerializedDictionary<WeakPointType, float> m_WeakPointMultipliers;

    public float GetMultiplier(WeakPointType type)
    {
        if (m_WeakPointMultipliers.TryGetValue(type, out float multiplier))
            return multiplier;

        return 1.0f;
    }
}
