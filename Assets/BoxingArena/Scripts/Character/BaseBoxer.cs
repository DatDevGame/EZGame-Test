using System.Collections.Generic;
using System.Data;
using HCore.Helpers;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public abstract class BaseBoxer : MonoBehaviour, IAttackable
{
    [SerializeField, BoxGroup("References")] protected Animator m_Animator;
    [SerializeField, BoxGroup("References")] protected Transform m_LeftHitBox;
    [SerializeField, BoxGroup("References")] protected Transform m_RightHitBox;
    [SerializeField, BoxGroup("References")] protected Transform m_FootstepPointL;
    [SerializeField, BoxGroup("References")] protected Transform m_FootstepPointR;
    [SerializeField, BoxGroup("Data")] protected StatsSO m_Stats;
    [SerializeField, BoxGroup("Data")] protected WeakPointSO m_WeakPointSO;

    protected float m_CurrentHealth;
    protected float m_CurrentAttackDamage;
    protected float m_CurrentAttackSpeed;
    protected float m_CurrentBlockChance;
    protected float m_CurrentMoveSpeed;
    protected float m_CurrentCriticalChance;
    protected float m_CurrentCriticalMultiplier;
    protected float m_CurrentAggression;

    protected bool m_IsAlive = true;

    protected virtual void Start()
    {
        Init();
    }
    protected virtual void Init()
    {
        if (m_Animator == null)
            m_Animator = gameObject.GetOrAddComponent<Animator>();
        if (m_Animator != null)
        {
            m_LeftHitBox = m_Animator.GetBoneTransform(HumanBodyBones.LeftHand);
            m_RightHitBox = m_Animator.GetBoneTransform(HumanBodyBones.RightHand);
            m_FootstepPointL = m_Animator.GetBoneTransform(HumanBodyBones.LeftFoot);
            m_FootstepPointR = m_Animator.GetBoneTransform(HumanBodyBones.RightFoot);
            SetupWeakPoint();
        }
        UpdateStatus();
    }

    private void SetupWeakPoint()
    {
        WeakPoint[] weakPoints = GetComponentsInChildren<WeakPoint>();
        foreach (var weakPoint in weakPoints)
        {
            if (weakPoint == null) continue;

            weakPoint.Load(this);
            Transform parentBone = GetBoneTransformForWeakPoint(weakPoint.WeakPointType);
            if (parentBone != null)
            {
                weakPoint.transform.SetParent(parentBone);
                weakPoint.transform.localPosition = Vector3.zero;
                weakPoint.transform.eulerAngles = Vector3.zero;
            }
        }

        Transform GetBoneTransformForWeakPoint(WeakPointType type)
        {
            switch (type)
            {
                case WeakPointType.Head:
                    return m_Animator.GetBoneTransform(HumanBodyBones.Head);
                case WeakPointType.Body:
                    return m_Animator.GetBoneTransform(HumanBodyBones.Spine);
                case WeakPointType.Chest:
                    return m_Animator.GetBoneTransform(HumanBodyBones.Chest);
                default:
                    return null;
            }
        }
    }

    protected virtual void UpdateStatus()
    {
        m_CurrentHealth = m_Stats.MaxHealth;
        m_CurrentAttackDamage = m_Stats.AttackDamage;
        m_CurrentAttackSpeed = m_Stats.AttackSpeed;
        m_CurrentBlockChance = m_Stats.BlockChance;
        m_CurrentMoveSpeed = m_Stats.MoveSpeed;
        m_CurrentCriticalChance = m_Stats.CriticalChance;
        m_CurrentCriticalMultiplier = m_Stats.CriticalMultiplier;
        m_CurrentAggression = m_Stats.Aggression;
    }

    public virtual void Attack(IDamageable target)
    {
        if (!m_IsAlive || target == null) return;

        // float finalDamage = m_CurrentAttackDamage;
        // if (Random.value < m_CurrentCriticalChance)
        //             finalDamage *= m_CurrentCriticalMultiplier;
        // target.TakeDamage(WeakPointType.Head, finalDamage);
    }

    public virtual void TakeDamage(WeakPointType weakPointType, float amount)
    {
        if (!m_IsAlive) return;
        if (TryTakeDamage()) return;
        float dameHandle = amount * m_WeakPointSO.GetMultiplier(weakPointType);

        ApplyDamage(dameHandle);
        if (m_CurrentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual bool TryTakeDamage()
    {
        float roll = Random.Range(0f, 1f);

        if (roll < m_CurrentBlockChance)
        {
            Debug.Log("Blocked the punch!");
            return true;
        }
        return false;
    }

    protected virtual void ApplyDamage(float incomingDamage)
    {
        m_CurrentHealth -= incomingDamage;
        OnHitFeedback();
    }

    public virtual void OnHitFeedback()
    {
        m_Animator?.SetTrigger("Hit");
    }

    protected virtual void Die()
    {
        m_IsAlive = false;
        m_Animator?.SetTrigger("KO");
        Debug.Log($"{name} is KO!");
    }


#if UNITY_EDITOR
    [Button]
    private void Load()
    {
        Init();
    }
#endif
}
