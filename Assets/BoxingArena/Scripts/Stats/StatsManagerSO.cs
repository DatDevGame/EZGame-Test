using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatsManagerSO", menuName = "BoxingArena/Stats/StatsManagerSO", order = 0)]
public class StatsManagerSO : SerializableScriptableObject
{
    public List<StatsSO> StatsSOs; 
}
