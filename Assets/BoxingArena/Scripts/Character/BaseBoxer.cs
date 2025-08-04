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
    public ParticleSystem PuncherVFX => m_PuncherVFX;
    public BoxerStats BoxerStats => m_BoxStats;
    public StatsSO StatsSOData => m_StatsSOData;
    public AnimationKeySO AnimationKeySO => m_AnimationKeySO;
    public Transform RightHandDeep => m_RightHandDeep;
    public Transform LeftHandDeep => m_LeftHandDeep;
    public bool IsAlive => m_IsAlive;

    [SerializeField, BoxGroup("Config")] protected LegsAnimator.PelvisImpulseSettings m_BlockHit;
    [SerializeField, BoxGroup("References")] protected BoxerAnimationEventReceiver m_BoxerAnimationEventReceiver;
    [SerializeField, BoxGroup("References")] protected CharacterController m_CharacterController;
    [SerializeField, BoxGroup("References")] protected HealthBar m_HealthBar;
    [SerializeField, BoxGroup("References")] protected Animator m_Animator;
    [SerializeField, BoxGroup("References")] protected LegsAnimator m_LegsAnimator;
    [SerializeField, BoxGroup("References")] protected ParticleSystem m_PuncherVFX;
    [SerializeField, BoxGroup("Data")] protected StatsSO m_StatsSOData;
    [SerializeField, BoxGroup("Data")] protected AnimationKeySO m_AnimationKeySO;
    [ShowInInspector, ReadOnly] protected BoxerStats m_BoxStats;
    protected bool m_IsAlive = true;
    protected Transform m_RightHandDeep;
    protected Transform m_LeftHandDeep;

    protected virtual void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        m_BoxerAnimationEventReceiver.Init(this);
        if (m_BoxerAnimationEventReceiver == null && m_Animator != null)
            m_BoxerAnimationEventReceiver = m_Animator.GetComponent<BoxerAnimationEventReceiver>();
        if (m_PuncherVFX != null && m_Animator != null)
        {
            Transform rightHand = m_Animator.GetBoneTransform(HumanBodyBones.RightHand);
            m_RightHandDeep = GetDeepestChild(rightHand);
            Transform leftHand = m_Animator.GetBoneTransform(HumanBodyBones.LeftHand);
            m_LeftHandDeep = GetDeepestChild(leftHand);
        }
        UpdateStatus();
    }

    protected virtual void UpdateStatus()
    {
        m_BoxStats = new BoxerStats();
        m_BoxStats.LoadStats(m_StatsSOData);

        if (m_HealthBar == null)
            m_HealthBar = gameObject.GetComponentInChildren<HealthBar>();

        RangeIntValue range = new RangeIntValue(0, m_BoxStats.Health);
        var progress = new RangeProgress<int>(range, 100);
        m_HealthBar.Init(progress);
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

        ApplyDamage((int)finalDamage);
        if (m_BoxStats.Health <= 0)
            Die();
        else
        {
            int roll = UnityEngine.Random.Range(0, 2);
            m_Animator?.SetTrigger(roll == 0 ? m_AnimationKeySO.Hit_1 : m_AnimationKeySO.Hit_2);
        }
    }

    protected virtual void ApplyDamage(int incomingDamage)
    {
        m_BoxStats.Health -= incomingDamage;
        m_HealthBar.SetValue(m_BoxStats.Health + incomingDamage, m_BoxStats.Health, 0.2f);
    }

    protected virtual void Die()
    {
        m_IsAlive = false;
        m_LegsAnimator.enabled = false;
        m_Animator?.SetTrigger(m_AnimationKeySO.Dead);
        OnDead?.Invoke();
    }

    protected Transform GetDeepestChild(Transform parent)
    {
        if (parent.childCount == 0)
            return parent;

        Transform deepest = parent;
        int maxDepth = 0;

        foreach (Transform child in parent)
        {
            Transform deepChild = GetDeepestChild(child);
            int depth = GetTransformDepth(deepChild);

            if (depth > maxDepth)
            {
                maxDepth = depth;
                deepest = deepChild;
            }
        }

        return deepest;
    }

    protected int GetTransformDepth(Transform t)
    {
        int depth = 0;
        while (t.parent != null)
        {
            depth++;
            t = t.parent;
        }
        return depth;
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
