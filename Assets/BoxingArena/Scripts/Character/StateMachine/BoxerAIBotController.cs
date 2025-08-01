using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BoxerAIBotController : AIBotController, INavigationPoint
{
    public Animator Animator => m_Animator;
    public BoxerAIProfile BoxerAIProfile => m_BoxerAIProfile;
    public AnimationKeySO AnimationKeySO => m_AnimationKeySO;
    [SerializeField, BoxGroup("Reference")] protected Animator m_Animator;
    [SerializeField, BoxGroup("Data")] protected BoxerAIProfile m_BoxerAIProfile;
    [SerializeField, BoxGroup("Data")] protected AnimationKeySO m_AnimationKeySO;

    public override void Initialize(AIBotController botController)
    {
        if (AIProfile is BoxerAIProfile boxerAIProfile)
        {
            m_BoxerAIProfile = boxerAIProfile;
            if (m_AIProfile != null && m_NavMeshAgent != null)
            {
                m_NavMeshAgent.speed = m_BoxerAIProfile.MoveSpeed;
                m_NavMeshAgent.angularSpeed = m_BoxerAIProfile.RotationSpeed * 100f;
                m_NavMeshAgent.stoppingDistance = m_BoxerAIProfile.ReachThreshold;
            }
        }
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
    public Vector3 GetTargetPoint()
    {
        return Vector3.zero;
    }

    protected override void OnDrawGizmosSelected()
    {
#if UNITY_EDITOR
        if (m_BoxerAIProfile == null) return;

        Vector3 center = transform.position + (Vector3.up * 0.2f);
        float radius = m_BoxerAIProfile.DetectionRange;

        Gizmos.color = Color.red;
        DrawCircleXZ(center, radius, 64);

        Color fillColor = new Color(1f, 0f, 0f, 0.1f); // đỏ nhạt, alpha 0.1
        Handles.color = fillColor;
        Handles.DrawSolidDisc(center, Vector3.up, radius);

        Handles.color = Color.white;
        GUIStyle style = new GUIStyle();
        style.normal.textColor = Color.white;
        style.alignment = TextAnchor.UpperCenter;
        style.fontStyle = FontStyle.Bold;
        Handles.Label(center + Vector3.up * 1.5f, $"Detection Range: {radius}", style);
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
