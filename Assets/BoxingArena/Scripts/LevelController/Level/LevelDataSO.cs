using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "BoxingArena/Level/LevelDataSO")]
public class LevelDataSO : SerializableScriptableObject
{
    [BoxGroup("General")]
    public BA_Map MapPrefab;

    [BoxGroup("Config")]
    public int MinRangeEnemy, MaxRangeEnemy;
    public int RandomEnemy => UnityEngine.Random.Range(MinRangeEnemy, MaxRangeEnemy);
}
