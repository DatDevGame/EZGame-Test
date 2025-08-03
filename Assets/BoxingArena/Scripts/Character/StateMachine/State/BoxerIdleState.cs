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
    public override void InitializeState(AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeState(botController);
        Debug.Log($"InitializeState -> BoxerIdleState");
    }
}

[Serializable]
public class BoxerIdleToLookingTransition : AIBotStateTransition
{
    protected BoxerAIBotController m_BoxerAIBotController;
    protected BoxerIdleState m_IdleTargetState;
    protected float m_SearchDelay;
    protected override bool Decide()
    {
        return true;
    }

    public override void InitializeTransition(AIBotState originState, AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeTransition(originState, botController);
        m_IdleTargetState = GetOriginStateAsType<BoxerIdleState>();
        Debug.Log($"InitializeTransition -> BoxerIdleToChasingTransition");
    }
}