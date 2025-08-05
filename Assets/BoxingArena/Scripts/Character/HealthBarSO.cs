using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HealthBarSO", menuName = "BoxingArena/Character/HealthBarSO")]
public class HealthBarSO : SerializableScriptableObject
{
    public Material PlayerHealthBarMaterial;
    public Material PlayerTeamHealthBarMaterial;
    public Material OpponentHealthBarMaterial;
}
