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
using Random = UnityEngine.Random;

[Serializable]
public class BoxerChasingState : AIBotState
{
    protected BoxerAIBotController m_BoxerAIBotController;
    protected Vector3 m_LastTargetPosition;
    protected float m_RepathThreshold = 0.2f;
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    protected override void OnStateEnable()
    {
        base.OnStateEnable();
    }

    protected override void OnStateUpdate()
    {
        base.OnStateUpdate();

        if (BotController.Target == null)
            return;
        MoveTarget((m_BoxerAIBotController.Target as MonoBehaviour).transform);
    }

    public override void InitializeState(AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeState(botController);

        Debug.Log($"InitializeState -> Chasing");
    }

    protected virtual void MoveTarget(Transform targetTransform)
    {
        Vector3 targetPosition = targetTransform.position;
        if (Vector3.Distance(m_LastTargetPosition, targetPosition) > m_RepathThreshold)
        {
            Debug.Log($"Chasing - B");
            m_LastTargetPosition = targetPosition;
            m_BoxerAIBotController.NavMeshAgent.SetDestination(targetPosition);
        }
    }
}

[Serializable]
public class BoxerChasingTargetToAttackTargetTransition : AIBotStateTransition
{
    protected BoxerAIBotController m_BoxerAIBotController;
    private BoxerChasingState m_BoxerChasingState;

    protected override bool Decide()
    {
        MonoBehaviour target = m_BoxerAIBotController.Target as MonoBehaviour;
        return CheckAttackRange((m_BoxerAIBotController.Target as MonoBehaviour).transform);
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeTransition(originState, botController);
        m_BoxerChasingState = GetOriginStateAsType<BoxerChasingState>();
    }

    protected bool CheckAttackRange(Transform targetTransform)
    {
        Vector3 targetPosition = targetTransform.position;
        float distanceToTarget = Vector3.Distance(m_BoxerAIBotController.transform.position, targetPosition);
        return distanceToTarget <= m_BoxerAIBotController.BoxerAIProfile.DistanceChansingToAttack;
    }

}