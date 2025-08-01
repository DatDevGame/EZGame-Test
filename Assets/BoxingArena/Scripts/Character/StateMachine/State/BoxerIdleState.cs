using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HCore.Helpers;
using Premium;
using Premium.StateMachine;
using UnityEngine;

[Serializable]
public class BoxerIdleState : AIBotState
{
    protected BoxerAIBotController m_BoxerAIBotController;
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
    }

    public override void InitializeState(AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeState(botController);
        m_BoxerAIBotController.Animator.SetTrigger(m_BoxerAIBotController.AnimationKeySO.Idle);
    }
}

[Serializable]
public class BoxerIdleToChasingTransition : AIBotStateTransition
{
    protected BoxerAIBotController m_BoxerAIBotController;
    protected BoxerIdleState m_IdleTargetState;
    protected float m_TimeFindEnemy;
    protected override bool Decide()
    {
        if (m_BoxerAIBotController != null)
        {
            List<INavigationPoint> navigationPoints = m_BoxerAIBotController.FindTargetsInRange();

            if (navigationPoints.Count > 0)
            {
                m_BoxerAIBotController.Target = navigationPoints
                    .Where(v => v != null)
                    .OrderBy(point => Vector3.Distance(m_BoxerAIBotController.transform.position, (point as MonoBehaviour).transform.position))
                    .FirstOrDefault();

                if (m_BoxerAIBotController.Target != null && m_BoxerAIBotController.Target.IsAvailable())
                    return true;
            }
        }

        m_TimeFindEnemy -= Time.deltaTime;
        if (m_TimeFindEnemy <= 0)
            return true;

        return false;
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeTransition(originState, botController);
        m_IdleTargetState = GetOriginStateAsType<BoxerIdleState>();
        m_TimeFindEnemy = m_BoxerAIBotController.BoxerAIProfile.TimeRandomChasingTarget;
    }
}