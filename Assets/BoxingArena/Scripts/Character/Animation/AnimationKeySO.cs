using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationKeySO", menuName = "BoxingArena/Animation/AnimationKeySO")]
public class AnimationKeySO : SerializableScriptableObject
{
    public string Idle = "Idle";
    public string Chasing = "Chasing";
    public string Attack = "Attack";
}
