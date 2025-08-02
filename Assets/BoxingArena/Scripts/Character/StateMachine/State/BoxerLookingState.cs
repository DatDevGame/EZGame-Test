using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HCore.Helpers;
using Premium;
using Premium.StateMachine;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class BoxerLookingState : AIBotState
{
    protected BoxerAIBotController m_BoxerAIBotController;
    public INavigationPoint TargetSearch;
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
        if (m_BoxerAIBotController != null)
        {
            List<INavigationPoint> navigationPoints = m_BoxerAIBotController.FindTargetsInRange();
            if (navigationPoints.Count > 0)
            {
                m_BoxerAIBotController.Target = navigationPoints
                    .Where(v => v != null)
                    .OrderBy(point => Vector3.Distance(m_BoxerAIBotController.transform.position, point.GetSelfPoint()))
                    .FirstOrDefault();
            }
        }
    }

    public override void InitializeState(AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeState(botController);
        m_BoxerAIBotController.Animator.SetTrigger(m_BoxerAIBotController.AnimationKeySO.Idle);
        Debug.Log($"InitializeState -> BoxerLookingState");
    }
}

[Serializable]
public class BoxerLookToChasingTransition : AIBotStateTransition
{
    protected BoxerAIBotController m_BoxerAIBotController;
    protected BoxerLookingState m_LookingState;
    protected float m_TriggerTimer;
    protected float m_InitialTriggerTime;

    protected override bool Decide()
    {
        if (m_BoxerAIBotController.Target != null)
        {
            m_TriggerTimer -= Time.deltaTime;
            float timeLookAt = m_InitialTriggerTime * 0.5f;
            if (m_TriggerTimer <= timeLookAt)
                m_BoxerAIBotController.BotTransform.DOLookAt(m_BoxerAIBotController.Target.GetSelfPoint(), timeLookAt * 0.5f);

            if (m_TriggerTimer <= 0)
            {
                m_InitialTriggerTime = m_BoxerAIBotController.BoxerAIProfile.TimeRandomChasingTarget;
                m_TriggerTimer = m_InitialTriggerTime;
                return true;
            }
        }
        else
        {
            m_TriggerTimer = m_BoxerAIBotController.BoxerAIProfile.TimeRandomChasingTarget;
        }
        return false;
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeTransition(originState, botController);
        m_LookingState = GetOriginStateAsType<BoxerLookingState>();
        m_InitialTriggerTime = m_BoxerAIBotController.BoxerAIProfile.TimeRandomChasingTarget;
        m_TriggerTimer = m_InitialTriggerTime;
    }
}