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
using Unity.VisualScripting;
using Premium.PoolManagement;

[Serializable]
public class BoxerAttackingState : AIBotState
{
    protected BoxerAIBotController m_BoxerAIBotController;
    protected float m_TriggerTimer;
    protected float m_ForwardDistance = 0.8f;
    protected const float LOOK_AT_DURATION = 0.2F;
    protected IDamageable m_Target;

    protected override void OnStateDisable()
    {
        m_TriggerTimer = 0;
    }
    protected override void OnStateUpdate()
    {
        if (m_BoxerAIBotController.Target == null)
            return;

        float distanceTarget = Vector3.Distance(m_BoxerAIBotController.BotTransform.position, m_BoxerAIBotController.Target.GetSelfPoint());
        bool isTooClose = distanceTarget < m_BoxerAIBotController.Boxer.BoxerStats?.AttackRange * 0.95f;

        if (isTooClose)
        {
            Vector3 dirAway = (m_BoxerAIBotController.BotTransform.position - m_BoxerAIBotController.Target.GetSelfPoint()).normalized;
            float retreatSpeed = 0.8f;
            Vector3 retreatVelocity = dirAway * retreatSpeed;
            m_BoxerAIBotController.CharacterController.Move(retreatVelocity * Time.deltaTime);
        }

        m_BoxerAIBotController.BotTransform.DOLookAt(m_BoxerAIBotController.Target.GetSelfPoint(), m_BoxerAIBotController.Boxer.StatsSOData.LookAtDuration);
        m_TriggerTimer -= Time.deltaTime;
        if (m_TriggerTimer <= 0)
            PerformAttack();
    }

    private void PerformAttack()
    {
        string keyAttackType = UnityEngine.Random.Range(0, 3) <= 0 ? m_BoxerAIBotController.AnimationKeySO.HeadAttack : m_BoxerAIBotController.AnimationKeySO.BodyAttack;
        m_BoxerAIBotController.Animator.SetTrigger(keyAttackType);

        float animationLength = 0f;
        AnimatorStateInfo stateInfo = m_BoxerAIBotController.Animator.GetCurrentAnimatorStateInfo(0);
        animationLength = stateInfo.length / m_BoxerAIBotController.Boxer.AnimationKeySO.DivineAnimSpeedAttack;

        m_TriggerTimer = m_BoxerAIBotController.Boxer.BoxerStats.AttackCoolDown + animationLength;
        LayerMask targetLayer = m_BoxerAIBotController.Boxer.BoxerStats.TeamLayerMask;

        Vector3 origin = m_BoxerAIBotController.transform.position + Vector3.up * 0.5f;
        Vector3 direction = m_BoxerAIBotController.transform.forward * m_ForwardDistance;
        float attackRange = m_ForwardDistance;

#if UNITY_EDITOR
        Debug.DrawLine(origin, origin + direction * attackRange, Color.cyan, 1.0f);
#endif

        if (Physics.Raycast(origin, direction, out RaycastHit hit, attackRange, targetLayer))
        {
            IDamageable target = hit.collider.GetComponent<IDamageable>();
            if (target != null && hit.collider.gameObject.layer != m_BoxerAIBotController.gameObject.layer)
                m_Target = target;
        }
    }

    //Call In Animation
    public void HandleAttackHit()
    {
        float distanceAttack = Vector3.Distance(m_BoxerAIBotController.transform.position, m_BoxerAIBotController.Target.GetSelfPoint());
        if (distanceAttack <= m_BoxerAIBotController.Boxer.BoxerStats.AttackRange && m_Target != null)
        {
            m_Target.TakeDamage(m_BoxerAIBotController.Boxer.BoxerStats.AttackDamage);
            SoundManager.Instance.PlayLoopSFX(m_BoxerAIBotController.Boxer.GetRandomPunchSound(), volumn: 0.5f);
        }

    }

    public override void InitializeState(AIBotController botController)
    {
        if (botController is BoxerAIBotController boxerAIBotController)
            m_BoxerAIBotController = boxerAIBotController;
        base.InitializeState(botController);
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
        if (m_BoxerAIBotController.Target == null)
            return false;
        if (!m_BoxerAIBotController.Target.GetBoxer().IsAlive)
            return true;

        float distanceToTarget = Vector3.Distance(m_BoxerAIBotController.BotTransform.position, m_BoxerAIBotController.Target.GetSelfPoint());
        bool isOutRangeAttack = distanceToTarget > m_BoxerAIBotController.Boxer.BoxerStats.AttackRange;
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