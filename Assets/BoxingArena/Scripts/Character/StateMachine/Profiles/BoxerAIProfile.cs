using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "AIProfile", menuName = "BoxingArena/StateMachine/AI Profile")]
public class BoxerAIProfile : AIProfile
{
    [Title("Looking State Config", "", TitleAlignments.Centered)]
    [MinMaxSlider(0f, 5f, true)]
    [SerializeField]
    protected Vector2 m_TimeRandomChasingTarget;
    public float TimeRandomChasingTarget => Random.Range(m_TimeRandomChasingTarget.x, m_TimeRandomChasingTarget.y);

    [Title("Chasing State Config", "", TitleAlignments.Centered)]
    public float RotationSpeed = 2f;
    public float ReachThreshold = 0.1f;
}
