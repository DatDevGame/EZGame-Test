using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "AIProfile", menuName = "DraftMaze/AI/AI Profile")]
public class AIProfile : ScriptableObject
{
    [Header("AI Behavior Settings")]
    [SerializeField, Range(0.1f, 10f)]
    private float m_DetectionRange = 5f;
    
    [SerializeField, Range(0.1f, 5f)]
    private float m_AttackRange = 1.5f;
    
    [SerializeField, Range(0.1f, 10f)]
    private float m_MoveSpeed = 3f;
    
    [SerializeField, Range(0.1f, 5f)]
    private float m_RotationSpeed = 2f;
    
    [Header("Target Priority Weights")]
    [SerializeField, Range(0f, 1f)]
    private float m_PlayerTargetWeight = 0.8f;
    
    [SerializeField, Range(0f, 1f)]
    private float m_CollectableTargetWeight = 0.6f;
    
    [SerializeField, Range(0f, 1f)]
    private float m_UtilityTargetWeight = 0.4f;
    
    [Header("State Transition Settings")]
    [SerializeField, Range(0.1f, 5f)]
    private float m_TargetLostTimeout = 2f;
    
    [SerializeField, Range(0.1f, 5f)]
    private float m_AttackCooldown = 1f;
    
    [Header("Navigation Settings")]
    [SerializeField, Range(0.1f, 2f)]
    private float m_ReachThreshold = 0.5f;
    
    [SerializeField]
    private LayerMask m_TargetLayerMask = -1;
    
    [SerializeField]
    private LayerMask m_ObstacleLayerMask = -1;


    public float DetectionRange => m_DetectionRange;
    public float AttackRange => m_AttackRange;
    public float MoveSpeed => m_MoveSpeed;
    public float RotationSpeed => m_RotationSpeed;
    public float PlayerTargetWeight => m_PlayerTargetWeight;
    public float CollectableTargetWeight => m_CollectableTargetWeight;
    public float UtilityTargetWeight => m_UtilityTargetWeight;
    public float TargetLostTimeout => m_TargetLostTimeout;
    public float AttackCooldown => m_AttackCooldown;
    public float ReachThreshold => m_ReachThreshold;
    public LayerMask TargetLayerMask => m_TargetLayerMask;
    public LayerMask ObstacleLayerMask => m_ObstacleLayerMask;

    [Header("Debug Settings")]
    [SerializeField]
    private bool m_ShowDebugGizmos = true;
    
    [SerializeField]
    private Color m_DebugColor = Color.red;
    
    public bool ShowDebugGizmos => m_ShowDebugGizmos;
    public Color DebugColor => m_DebugColor;

    private void OnValidate()
    {
        m_DetectionRange = Mathf.Max(0.1f, m_DetectionRange);
        m_AttackRange = Mathf.Clamp(m_AttackRange, 0.1f, m_DetectionRange);
        m_MoveSpeed = Mathf.Max(0.1f, m_MoveSpeed);
        m_RotationSpeed = Mathf.Max(0.1f, m_RotationSpeed);
        m_TargetLostTimeout = Mathf.Max(0.1f, m_TargetLostTimeout);
        m_AttackCooldown = Mathf.Max(0.1f, m_AttackCooldown);
        m_ReachThreshold = Mathf.Max(0.1f, m_ReachThreshold);
    }
} 