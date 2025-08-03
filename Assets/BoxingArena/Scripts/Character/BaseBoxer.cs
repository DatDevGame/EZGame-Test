using System;
using System.Collections.Generic;
using System.Data;
using FIMSpace.FProceduralAnimation;
using HCore.Helpers;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseBoxer : MonoBehaviour, IAttackable, IDamageable
{
    public Action OnDead = delegate { };
    public BoxerAnimationEventReceiver BoxerAnimationEventReceiver => m_BoxerAnimationEventReceiver;
    public CharacterController CharacterController => m_CharacterController;
    public Animator Animator => m_Animator;
    public BoxerStats BoxerStats => m_BoxStats;
    public StatsSO StatsSOData => m_StatsSOData;
    public AnimationKeySO AnimationKeySO => m_AnimationKeySO;
    [ShowInInspector] public bool IsAlive => m_IsAlive;

    [SerializeField, BoxGroup("Config")] protected LegsAnimator.PelvisImpulseSettings m_BlockHit;
    [SerializeField, BoxGroup("References")] protected BoxerAnimationEventReceiver m_BoxerAnimationEventReceiver;
    [SerializeField, BoxGroup("References")] protected CharacterController m_CharacterController;
    [SerializeField, BoxGroup("References")] protected Animator m_Animator;
    [SerializeField, BoxGroup("References")] protected LegsAnimator m_LegsAnimator;
    [SerializeField, BoxGroup("Data")] protected StatsSO m_StatsSOData;
    [SerializeField, BoxGroup("Data")] protected AnimationKeySO m_AnimationKeySO;
    [ShowInInspector, ReadOnly] protected BoxerStats m_BoxStats;
    protected bool m_IsAlive = true;

    protected virtual void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        if (m_BoxerAnimationEventReceiver == null && m_Animator != null)
            m_BoxerAnimationEventReceiver = m_Animator.GetComponent<BoxerAnimationEventReceiver>();

        UpdateStatus();
    }

    protected virtual void UpdateStatus()
    {
        m_BoxStats = new BoxerStats();
        m_BoxStats.LoadStats(m_StatsSOData);
    }

    public virtual void Attack(IDamageable target)
    {
        if (!m_IsAlive || target == null) return;

        float finalDamage = m_BoxStats.AttackDamage;
        if (UnityEngine.Random.value < m_BoxStats.CriticalChance)
            finalDamage *= m_BoxStats.CriticalMultiplier;

        target.TakeDamage(finalDamage);
    }

    public virtual void TakeDamage(float amount)
    {
        if (!m_IsAlive) return;
        if (TryTakeDamage()) return;
        HandleDamage(amount);
    }
    protected virtual bool TryTakeDamage()
    {
        float roll = UnityEngine.Random.Range(0f, 1f);

        if (roll < m_BoxStats.BlockChance)
        {
            m_LegsAnimator.User_AddImpulse(m_BlockHit);
            return true;
        }
        return false;
    }
    protected virtual void HandleDamage(float damage)
    {
        float finalDamage = damage;
        if (UnityEngine.Random.value < m_BoxStats.CriticalChance)
            finalDamage *= m_BoxStats.CriticalMultiplier;

        ApplyDamage(finalDamage);
        if (m_BoxStats.Health <= 0)
            Die();
        else
        {
            int roll = UnityEngine.Random.Range(0, 2);
            m_Animator?.SetTrigger(roll == 0 ? m_AnimationKeySO.Hit_1 : m_AnimationKeySO.Hit_2);
        }
    }

    protected virtual void ApplyDamage(float incomingDamage)
    {
        m_BoxStats.Health -= incomingDamage;
    }

    protected virtual void Die()
    {
        m_IsAlive = false;
        m_LegsAnimator.enabled = false;
        m_Animator?.SetTrigger(m_AnimationKeySO.Dead);
        OnDead?.Invoke();
    }

#if UNITY_EDITOR
    [Button]
    private void Load()
    {
        Init();
    }
    [Button]
    public void PushTest()
    {
        m_LegsAnimator.User_AddImpulse(m_BlockHit);
    }
#endif
}
