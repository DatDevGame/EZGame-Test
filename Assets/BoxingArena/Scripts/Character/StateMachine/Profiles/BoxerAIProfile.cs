using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "AIProfile", menuName = "BoxingArena/StateMachine/AI Profile")]
public class BoxerAIProfile : AIProfile
{
    [Title("AI Behavior Settings", "", TitleAlignments.Centered)]
    [Range(0.1f, 10f)]
    public float DetectionRange = 10f;
    public LayerMask TargetLayerMask = -1;
    public LayerMask ObstacleLayerMask = -1;

    [Title("Looking State Config", "", TitleAlignments.Centered)]
    [MinMaxSlider(0f, 5f, true)]
    [SerializeField]
    protected Vector2 m_TimeRandomChasingTarget;
    public float TimeRandomChasingTarget => Random.Range(m_TimeRandomChasingTarget.x, m_TimeRandomChasingTarget.y);

    [Title("Chasing State Config", "", TitleAlignments.Centered)]
    public float MoveSpeed = 3f;
    public float RotationSpeed = 2f;
    public float ReachThreshold = 0.1f;

    [Title("Attack State Config", "", TitleAlignments.Centered)]
    [Range(0.1f, 5f)]
    public float AttackRange = 0.5f;
    public float AttackCoolDown = 1f;

    [Title("Debug Settings", "", TitleAlignments.Centered)]
    [SerializeField]
    protected bool m_ShowDebugGizmos = true;

    [SerializeField]
    protected Color m_DebugColor = Color.red;

    public bool ShowDebugGizmos => m_ShowDebugGizmos;
    public Color DebugColor => m_DebugColor;
}
