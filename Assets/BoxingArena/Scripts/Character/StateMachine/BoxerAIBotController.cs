using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using JetBrains.Annotations;
using Premium.PoolManagement;




#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoxerAIBotController : AIBotController, INavigationPoint
{
    public bool IsRunning => m_IsRunning;
    public BaseBoxer Boxer => m_Boxer;
    public CharacterController CharacterController => m_CharacterController;
    public Animator Animator => m_Animator;
    public BoxerAIProfile BoxerAIProfile => m_BoxerAIProfile;
    public AnimationKeySO AnimationKeySO => m_AnimationKeySO;

    [SerializeField, BoxGroup("Reference")] protected BaseBoxer m_Boxer;
    [SerializeField, BoxGroup("Reference")] protected CharacterController m_CharacterController;

    private BoxerAIProfile m_BoxerAIProfile;
    private Animator m_Animator;
    private AnimationKeySO m_AnimationKeySO;

    protected override void Awake()
    {
        base.Awake();
        if (m_AIProfile is BoxerAIProfile boxerAIProfile)
            m_BoxerAIProfile = boxerAIProfile;
        if (m_Animator == null)
            m_Animator = m_Boxer.Animator;
        if (m_AnimationKeySO == null)
            m_AnimationKeySO = m_Boxer.AnimationKeySO;
        m_Boxer.OnDead += OnDead;
    }
    public override void InitializeStateMachine()
    {
        base.InitializeStateMachine();
        foreach (var state in m_States)
        {
            if (state is BoxerAttackingState boxerAttacking && m_Boxer.BoxerAnimationEventReceiver != null)
                m_Boxer.BoxerAnimationEventReceiver.SetAttackingState(boxerAttacking);
        }

    }
    public override List<INavigationPoint> FindTargetsInRange()
    {
        var targets = new List<INavigationPoint>();
        if (m_AIProfile == null)
            return targets;

        var colliders = Physics.OverlapSphere(m_BotTransform.position, m_Boxer.StatsSOData.DetectionRange, m_Boxer.BoxerStats.TeamLayerMask);
        foreach (var collider in colliders)
        {
            if (collider.gameObject.layer == gameObject.layer)
                continue;

            var navPoint = collider.GetComponent<INavigationPoint>();
            if (navPoint != null && navPoint.IsAvailable())
            {
                targets.Add(navPoint);
            }
        }
        return targets;
    }
    public bool IsAvailable()
    {
        return m_IsRunning;
    }
    public PointType GetPointType()
    {
        return PointType.OpponentPoint;
    }
    public Vector3 GetSelfPoint()
    {
        return transform.position;
    }
    public Vector3 GetTargetPoint()
    {
        return m_Target == null ? Vector3.zero : m_Target.GetSelfPoint();
    }
    public BaseBoxer GetBoxer()
    {
        return m_Boxer;
    }
    protected void OnDead()
    {
        StopStateMachine();
    }
#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        StatsSO m_GizMosBoxerAIProfile = m_Boxer.StatsSOData;
        if (m_GizMosBoxerAIProfile == null) return;

        Vector3 center = transform.position + (Vector3.up * 0.1f);

        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;

        // === DETECTION RANGE ===
        float detectionRadius = m_GizMosBoxerAIProfile.DetectionRange;
        Gizmos.color = Color.red;
        DrawCircleXZ(center, detectionRadius, 64);

        Color detectionFill = new Color(1f, 0f, 0f, 0.1f);
        Handles.color = detectionFill;
        Handles.DrawSolidDisc(center, Vector3.up, detectionRadius);

        Vector3 detectionLabelPos = center + new Vector3(0, 0.01f, -detectionRadius + 0.2f);
        Handles.color = Color.white;
        Handles.Label(detectionLabelPos, $"Detection Range: {detectionRadius}", style);

        // === ATTACK RANGE ===
        float attackRadius = Boxer.StatsSOData.AttackRange;
        Gizmos.color = Color.yellow;
        DrawCircleXZ(center, attackRadius, 64);

        Color attackFill = new Color(1f, 1f, 0f, 0.1f);
        Handles.color = attackFill;
        Handles.DrawSolidDisc(center, Vector3.up, attackRadius);

        Vector3 attackLabelPos = center + new Vector3(0, 0.01f, -attackRadius + 0.2f);
        Handles.color = Color.yellow;
        Handles.Label(attackLabelPos, $"Attack Range: {attackRadius}", style);
    }
    private void DrawCircleXZ(Vector3 center, float radius, int segments)
    {
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0f), 0f, Mathf.Sin(0f)) * radius;

        for (int i = 1; i <= segments; i++)
        {
            float rad = Mathf.Deg2Rad * angleStep * i;
            Vector3 nextPoint = center + new Vector3(Mathf.Cos(rad), 0f, Mathf.Sin(rad)) * radius;

            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
#endif
}
