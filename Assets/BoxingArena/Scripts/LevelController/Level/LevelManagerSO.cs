using System.Collections;
using System.Collections.Generic;
using HCore.Helpers;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelManagerSO", menuName = "BoxingArena/Level/LevelManagerSO")]
public class LevelManagerSO : SerializableScriptableObject
{
    [BoxGroup("Data")] public PPrefLevelSOVariable CurrentLevelSO;
    [BoxGroup("Data")] public PPrefIntVariable CurrentLevelPPrefIntVariable;
    [BoxGroup("Data")] public LevelListSOVariable m_LevelListSOVariable;
    [BoxGroup("Manager Data")] public StatsManagerSO StatsManagerSO;
    [BoxGroup("Manager Data")] public CharacterManagerSO CharacterManagerSO;

    public LevelDataSO GetCurrentLevelDataSO()
    {
        if (CurrentLevelPPrefIntVariable.value + 1 > m_LevelListSOVariable.value.Count)
            return m_LevelListSOVariable.value.GetRandom();
        return m_LevelListSOVariable.value[CurrentLevelPPrefIntVariable.value];
    }

    public int GetCurrentLevel()
    {
        return CurrentLevelPPrefIntVariable.value + 1;
    }
}
