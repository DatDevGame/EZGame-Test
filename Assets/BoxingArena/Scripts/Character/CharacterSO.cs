using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSO", menuName = "BoxingArena/Character/CharacterSO")]
public class CharacterSO : ItemSO
{
    [BoxGroup("Resource")] public BaseBoxer BaseBoxerPrefab;
}
