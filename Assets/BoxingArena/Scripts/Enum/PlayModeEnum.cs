using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModeVariable", menuName = "BoxingArena/GameMode/ModeVariable")]
public class PlayModeEnum : Variable<GameMode>
{
}

public enum GameMode
{
    OneVsOne,
    OneVsMany,
    ManyVsMany
}

