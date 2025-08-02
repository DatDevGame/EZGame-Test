using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HCore.Helpers;
using Premium;
using Premium.StateMachine;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

[Serializable]
public class BoxerChasingState : AIBotState
{
    [ShowInInspector] protected BoxerAIBotController m_BoxerAIBotController;
    protected Vector3 m_LastTargetPosition;
    protected float m_RepathThreshold = 0.2f;
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    protected override void OnStateEnable()
    {
        m_BoxerAIBotController.NavMeshAgent.isStopped = false;
    }

    protected override void OnStateDisable()
    {
        m_BoxerAIBotController.NavMeshAgent.isStopped = true;
    }

    protected override void OnStateUpdate()
    {
        base.OnStateUpdate();
        if (BotController.Target == null)
            return;
        MoveTarget(m_BoxerAIBotController.Target.GetSelfPoint());
    }

    public override void InitializeState(AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeState(botController);
        Debug.Log($"InitializeState -> BoxerChasingState");
    }

    protected virtual void MoveTarget(Vector3 targetPosition)
    {
        if (m_LastTargetPosition != targetPosition)
        {
            m_LastTargetPosition = targetPosition;
            m_BoxerAIBotController.transform.DOLookAt(targetPosition, 0.2f);
            m_BoxerAIBotController.NavMeshAgent.SetDestination(targetPosition);
        }
    }
}

[Serializable]
public class BoxerChasingToAttackTransition : AIBotStateTransition
{
    protected BoxerAIBotController m_BoxerAIBotController;
    private BoxerChasingState m_BoxerChasingState;

    protected override bool Decide()
    {
        return CheckAttackRange(m_BoxerAIBotController.Target.GetSelfPoint());
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeTransition(originState, botController);
        m_BoxerChasingState = GetOriginStateAsType<BoxerChasingState>();
        Debug.Log($"InitializeTransition -> BoxerChasingTargetToAttackTargetTransition");
    }

    protected bool CheckAttackRange(Vector3 vecTarget)
    {
        if (m_BoxerAIBotController == null) return false;
        float distanceToTarget = Vector3.Distance(m_BoxerAIBotController.transform.position, vecTarget);
        return distanceToTarget <= m_BoxerAIBotController.BoxerAIProfile.AttackRange;
    }

}