using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoxerAIBotController : AIBotController, INavigationPoint
{
    public Animator Animator => m_Animator;
    public BoxerAIProfile BoxerAIProfile => m_BoxerAIProfile;
    public AnimationKeySO AnimationKeySO => m_AnimationKeySO;
    [SerializeField, BoxGroup("Reference")] protected Animator m_Animator;
    [SerializeField, BoxGroup("Data")] protected AnimationKeySO m_AnimationKeySO;
    private BoxerAIProfile m_BoxerAIProfile;

    protected override void Awake()
    {
        base.Awake();
        if (m_AIProfile is BoxerAIProfile boxerAIProfile)
            m_BoxerAIProfile = boxerAIProfile;

        if (m_NavMeshAgent != null)
        {
            m_NavMeshAgent.speed = m_BoxerAIProfile.MoveSpeed;
            m_NavMeshAgent.angularSpeed = m_BoxerAIProfile.RotationSpeed * 100f;
            m_NavMeshAgent.stoppingDistance = m_BoxerAIProfile.ReachThreshold;
        }
    }
    public override void Initialize(AIBotController botController)
    {

    }
    public override List<INavigationPoint> FindTargetsInRange()
    {
        var targets = new List<INavigationPoint>();
        if (m_AIProfile == null)
            return targets;

        var colliders = Physics.OverlapSphere(m_BotTransform.position, m_BoxerAIProfile.DetectionRange, m_BoxerAIProfile.TargetLayerMask);
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

    public bool IsRobotReached(AIBotController botController)
    {
        return FindTargetsInRange().Count > 0;
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
        return m_Target.GetSelfPoint();
    }

    protected override void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        BoxerAIProfile m_GizMosBoxerAIProfile = AIProfile as BoxerAIProfile;
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

        // Đặt label ngay **rìa dưới vòng tròn detection**
        Vector3 detectionLabelPos = center + new Vector3(0, 0.01f, -detectionRadius + 0.2f);
        Handles.color = Color.white;
        Handles.Label(detectionLabelPos, $"Detection Range: {detectionRadius}", style);

        // === ATTACK RANGE ===
        float attackRadius = m_GizMosBoxerAIProfile.AttackRange;
        Gizmos.color = Color.yellow;
        DrawCircleXZ(center, attackRadius, 64);

        Color attackFill = new Color(1f, 1f, 0f, 0.1f);
        Handles.color = attackFill;
        Handles.DrawSolidDisc(center, Vector3.up, attackRadius);

        // Đặt label ngay **rìa dưới vòng tròn attack**
        Vector3 attackLabelPos = center + new Vector3(0, 0.01f, -attackRadius + 0.2f);
        Handles.color = Color.yellow;
        Handles.Label(attackLabelPos, $"Attack Range: {attackRadius}", style);
#endif
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
}
