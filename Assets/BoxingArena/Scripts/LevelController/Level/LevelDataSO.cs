using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;


[System.Serializable]
public struct StatProbability
{
    [Range(0, 100)]
    public float probability;

    public StatsSO stats;
}
[CreateAssetMenu(fileName = "LevelDataSO", menuName = "BoxingArena/Level/LevelDataSO")]
public class LevelDataSO : SerializableScriptableObject
{
    [BoxGroup("General")]
    public BA_Map MapPrefab;

    [BoxGroup("Config One vs Many")]
    public int MinRangeEnemy, MaxRangeEnemy;
    public int RandomEnemy => UnityEngine.Random.Range(MinRangeEnemy, MaxRangeEnemy);

    [BoxGroup("Config Many vs Many")]
    public int PlayerTeamCount;
    [BoxGroup("Config Many vs Many")]
    public int EnemyTeamCount;

    [BoxGroup("Config")]
    public List<StatProbability> StatsProbabilities;

    public StatsSO GetRandomStatsByProbability()
    {
        float total = 0f;
        foreach (var entry in StatsProbabilities)
            total += entry.probability;

        float rand = Random.Range(0f, total);
        float cumulative = 0f;

        foreach (var entry in StatsProbabilities)
        {
            cumulative += entry.probability;
            if (rand <= cumulative)
                return entry.stats;
        }
        return StatsProbabilities.Count > 0 ? StatsProbabilities[0].stats : null;
    }

}
