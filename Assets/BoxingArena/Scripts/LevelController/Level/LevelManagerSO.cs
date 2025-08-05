using System.Collections;
using System.Collections.Generic;
using HCore.Helpers;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelManagerSO", menuName = "BoxingArena/Level/LevelManagerSO")]
public class LevelManagerSO : SerializableScriptableObject
{
    [BoxGroup("Data")] public PPrefLevelSOVariable CurrentLevelSO;
    [BoxGroup("Data")] public LevelListSOVariable m_LevelListSOVariable;
    [BoxGroup("Level Data")] public PPrefIntVariable OneVsOneCurrentLevel;
    [BoxGroup("Level Data")] public PPrefIntVariable OneVsManyCurrentLevel;
    [BoxGroup("Level Data")] public PPrefIntVariable ManyVsManyCurrentLevel;
    [BoxGroup("Manager Data")] public StatsManagerSO StatsManagerSO;
    [BoxGroup("Manager Data")] public CharacterManagerSO CharacterManagerSO;

    public LevelDataSO GetCurrentLevelDataSO(GameMode gameMode)
    {
        int currentLevel = gameMode switch
        {
            GameMode.OneVsOne => OneVsOneCurrentLevel.value,
            GameMode.OneVsMany => OneVsManyCurrentLevel.value,
            GameMode.ManyVsMany => ManyVsManyCurrentLevel.value,
            _ => 0
        };
        if (currentLevel > m_LevelListSOVariable.value.Count)
            return m_LevelListSOVariable.value.GetRandom();
        return m_LevelListSOVariable.value[currentLevel];
    }

    public int GetCurrentLevel(GameMode gameMode)
    {
        return gameMode switch
        {
            GameMode.OneVsOne => OneVsOneCurrentLevel.value + 1,
            GameMode.OneVsMany => OneVsManyCurrentLevel.value + 1,
            GameMode.ManyVsMany => ManyVsManyCurrentLevel.value + 1,
            _ => 0
        };
    }

    public string GetNameMode(GameMode gameMode)
    {
        return gameMode switch
        {
            GameMode.OneVsOne => "Duel",
            GameMode.OneVsMany => "Survivor",
            GameMode.ManyVsMany => "Team Battle",
            _ => "Duel"
        };
    }
}
