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
public class BoxerAttackingState : AIBotState
{
    protected BoxerAIBotController m_BoxerAIBotController;
    protected float m_TriggerTimer;
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

        m_BoxerAIBotController.BotTransform.DOLookAt(m_BoxerAIBotController.Target.GetSelfPoint(), 0.2f);
        m_TriggerTimer -= Time.deltaTime;
        if (m_TriggerTimer <= 0)
            PerformAttack();
    }

    private void PerformAttack()
    {
        Debug.Log($"AI Bot attacking target at {BotController.Target.GetTargetPoint()}");
        m_TriggerTimer = m_BoxerAIBotController.BoxerAIProfile.AttackCoolDown;
    }

    public override void InitializeState(AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeState(botController);
        m_TriggerTimer = m_BoxerAIBotController.BoxerAIProfile.AttackCoolDown;
        Debug.Log($"InitializeState -> BoxerAttackingState");
    }
}

[Serializable]
public class BoxerAttackingToLookTransition : AIBotStateTransition
{
    protected BoxerAIBotController m_BoxerAIBotController;
    protected BoxerAttackingState attackingTargetState;
    protected override bool Decide()
    {
        //Condition Transition In Here
        if (botController.Target == null)
            return false;

        float distanceToTarget = Vector3.Distance(botController.BotTransform.position, botController.Target.GetSelfPoint());
        bool isOutRangeAttack = distanceToTarget > m_BoxerAIBotController.BoxerAIProfile?.AttackRange;
        Debug.Log($"{botController.BotTransform.name} Attack -> isOutRangeAttack: {isOutRangeAttack} | distanceToTarget: {distanceToTarget}");
        return isOutRangeAttack;
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeTransition(originState, botController);
        attackingTargetState = GetOriginStateAsType<BoxerAttackingState>();
        Debug.Log($"InitializeTransition -> BoxerAttackingToLookTransition");
    }
}