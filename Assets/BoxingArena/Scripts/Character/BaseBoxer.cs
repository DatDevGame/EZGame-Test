using System.Collections.Generic;
using System.Data;
using HCore.Helpers;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using Premium.StateMachine;

public abstract class BaseBoxer : MonoBehaviour, IAttackable
{
    [SerializeField, BoxGroup("References")] protected Animator m_Animator;
    [SerializeField, BoxGroup("References")] protected Transform m_LeftHitBox;
    [SerializeField, BoxGroup("References")] protected Transform m_RightHitBox;
    [SerializeField, BoxGroup("References")] protected Transform m_FootstepPointL;
    [SerializeField, BoxGroup("References")] protected Transform m_FootstepPointR;
    [SerializeField, BoxGroup("Data")] protected StatsSO m_StatsSOData;
    [SerializeField, BoxGroup("Data")] protected WeakPointSO m_WeakPointSO;

    [ShowInInspector, ReadOnly] protected BoxerStats m_BoxStats;

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
            SetUpWeakPoint();
        }
        UpdateStatus();
    }

    private void SetUpWeakPoint()
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
        m_BoxStats = new BoxerStats();
        m_BoxStats.LoadStats(m_StatsSOData);
    }
    
    /// <summary>
    /// Get the current boxer stats
    /// </summary>
    public BoxerStats GetBoxerStats()
    {
        return m_BoxStats;
    }

    public virtual void Attack(IDamageable target)
    {
        if (!m_IsAlive || target == null) return;

        float finalDamage = m_BoxStats.AttackDamage;
        if (Random.value < m_BoxStats.CriticalChance)
            finalDamage *= m_BoxStats.CriticalMultiplier;

        target.TakeDamage(finalDamage);
    }

    public virtual void TakeDamage(WeakPointType weakPointType, float amount)
    {
        if (!m_IsAlive) return;
        if (TryTakeDamage()) return;

        var damageInfo = new DamageInfo(
            amount * m_WeakPointSO.GetMultiplier(weakPointType),
            weakPointType
        );
        HandleDamage(damageInfo);
    }


    protected virtual bool TryTakeDamage()
    {
        float roll = Random.Range(0f, 1f);

        if (roll < m_BoxStats.BlockChance)
        {
            Debug.Log("Blocked the punch!");
            return true;
        }
        return false;
    }
    protected virtual void HandleDamage(DamageInfo damageInfo)
    {
        float finalDamage = damageInfo.Amount;
        if (Random.value < m_BoxStats.CriticalChance)
            finalDamage *= m_BoxStats.CriticalMultiplier;

        ApplyDamage(finalDamage);
        if (m_BoxStats.Health <= 0)
            Die();
    }

    protected virtual void ApplyDamage(float incomingDamage)
    {
        m_BoxStats.Health -= incomingDamage;
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
