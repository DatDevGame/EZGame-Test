using System;
using System.Collections.Generic;
using System.Data;
using FIMSpace.FProceduralAnimation;
using HCore.Events;
using HCore.Helpers;
using Premium;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public abstract class BaseBoxer : MonoBehaviour, IAttackable, IDamageable
{
    public Action OnDead = delegate { };
    public BoxerAnimationEventReceiver BoxerAnimationEventReceiver => m_BoxerAnimationEventReceiver;
    public CharacterController CharacterController => m_CharacterController;
    public Animator Animator => m_Animator;
    public ParticleSystem PuncherVFX => m_PuncherVFX;
    public BoxerStats BoxerStats => m_BoxStats;
    public StatsSO StatsSOData => m_StatsSO;
    public AnimationKeySO AnimationKeySO => m_AnimationKeySO;
    public Transform RightHandDeep => m_RightHandDeep;
    public Transform LeftHandDeep => m_LeftHandDeep;
    public bool IsAlive => m_IsAlive;
    public bool IsActive => m_IsActive;
    public bool IsLocal => m_IsLocal;

    [SerializeField, BoxGroup("Config")] protected LegsAnimator.PelvisImpulseSettings m_BlockHit;
    [SerializeField, BoxGroup("References")] protected BoxerAnimationEventReceiver m_BoxerAnimationEventReceiver;
    [SerializeField, BoxGroup("References")] protected CharacterController m_CharacterController;
    [SerializeField, BoxGroup("References")] protected HealthBar m_HealthBar;
    [SerializeField, BoxGroup("References")] protected Animator m_Animator;
    [SerializeField, BoxGroup("References")] protected LegsAnimator m_LegsAnimator;
    [SerializeField, BoxGroup("References")] protected ParticleSystem m_PuncherVFX;
    [SerializeField, BoxGroup("References")] protected MeshRenderer m_HealthBarMesh;
    [SerializeField, BoxGroup("Data")] protected AnimationKeySO m_AnimationKeySO;
    [ShowInInspector, ReadOnly] protected BoxerStats m_BoxStats;
    private bool m_IsTryTakeDame = false;
    protected bool m_IsAlive = true;
    protected bool m_IsActive = false;
    protected bool m_IsLocal = false;
    protected Transform m_RightHandDeep;
    protected Transform m_LeftHandDeep;
    protected StatsSO m_StatsSO;

    public virtual void Init(StatsSO statsSO = null)
    {
        m_StatsSO = statsSO;
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
        m_BoxStats.LoadStats(m_StatsSO);

        if (m_HealthBar == null)
            m_HealthBar = gameObject.GetComponentInChildren<HealthBar>();

        RangeIntValue range = new RangeIntValue(0, m_BoxStats.Health);
        var progress = new RangeProgress<int>(range, 100);
        m_HealthBar.Init(progress);
        m_IsActive = true;
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
        if (m_IsTryTakeDame)
            return false;

        float roll = UnityEngine.Random.Range(0f, 1f);
        if (roll < m_BoxStats.BlockChance)
        {
            m_IsTryTakeDame = true;
            m_LegsAnimator.User_AddImpulse(m_BlockHit);
            StartCoroutine(CommonCoroutine.Delay(1f, false, () =>
            {
                m_IsTryTakeDame = false;
            }));
            return true;
        }
        return false;
    }

    [Button]
    public void TestPush()
    {
        m_LegsAnimator.User_AddImpulse(m_BlockHit);
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
        SoundManager.Instance.PlayLoopSFX(GetRandomHitSound(), volumn: 0.15f);
        GameEventHandler.Invoke(PVPEventCode.CharacterReceivedDamage, this, incomingDamage);
    }

    protected virtual void Die()
    {
        m_IsAlive = false;
        m_IsActive = false;
        m_LegsAnimator.enabled = false;
        m_HealthBarMesh.gameObject.SetActive(false);
        m_CharacterController.enabled = false;
        NavMeshAgent navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
        if (navMeshAgent != null)
            navMeshAgent.enabled = false;
        m_Animator?.SetBool(m_AnimationKeySO.Dead, true);
        m_Animator?.SetTrigger(m_AnimationKeySO.DeadTrigger);
        OnDead?.Invoke();
        GameEventHandler.Invoke(PVPEventCode.AnyCharacterDead, this);
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

    public virtual BASoundEnum GetRandomPunchSound()
    {
        int roll = UnityEngine.Random.Range(0, 4);
        if (roll == 0)
            return BASoundEnum.PunchSmall;
        else if (roll == 1)
            return BASoundEnum.PunchMedium;
        else
            return BASoundEnum.PunchHard;
    }
    public virtual BASoundEnum GetRandomHitSound()
    {
        int roll = UnityEngine.Random.Range(0, 4);
        if (roll == 0)
            return BASoundEnum.HitDame_1;
        else if (roll == 1)
            return BASoundEnum.HitDame_2;
        else
            return BASoundEnum.HitDame_3;
    }

    public void SetLocal(bool isLocal) => m_IsLocal = isLocal;
    public void SetHealthBarMaterials(Material material) => m_HealthBarMesh.material = material;

    #if UNITY_EDITOR
    public void DeadEditor()
    {
        HandleDamage(9999);
    }
    #endif
}
