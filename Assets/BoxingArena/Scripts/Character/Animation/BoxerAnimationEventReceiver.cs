using System.Collections;
using System.Collections.Generic;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using UnityEngine;

public class BoxerAnimationEventReceiver : MonoBehaviour
{
    protected BaseBoxer m_BaseBoxer;
    protected BoxerAttackingState m_AttackingState;

    public virtual void Init(BaseBoxer baseBoxer) => m_BaseBoxer = baseBoxer;

    public virtual void SetAttackingState(BoxerAttackingState state)
    {
        m_AttackingState = state;
    }

    public virtual void RightPunchVFX()
    {
        if (CanAttack())
            HandleAttackVFX(m_BaseBoxer.RightHandDeep);
    }
    public virtual void LeftPunchVFX()
    {
        if (CanAttack())
            HandleAttackVFX(m_BaseBoxer.LeftHandDeep);
    }

    public virtual void OnAttackHit()
    {
        m_AttackingState?.HandleAttackHit();
    }

    protected virtual bool CanAttack()
    {
        if (m_BaseBoxer == null) return false;
        INavigationPoint navigationPoint = m_BaseBoxer.GetComponent<INavigationPoint>();
        if (navigationPoint != null)
        {
            float distanceAttack = Vector3.Distance(navigationPoint.GetSelfPoint(), navigationPoint.GetTargetPoint());
            return distanceAttack <= m_BaseBoxer.BoxerStats.AttackRange;
        }
        return false;
    }
    protected virtual void HandleAttackVFX(Transform yourHand)
    {
        ParticleSystem puncherVFX = PoolManager.GetOrCreatePool(m_BaseBoxer.PuncherVFX, initialCapacity: 1).Get();
        Transform hand = yourHand;
        puncherVFX.transform.SetParent(hand);
        puncherVFX.transform.localPosition = Vector3.zero;
        puncherVFX.transform.rotation = Quaternion.LookRotation(hand.forward, hand.up);
        puncherVFX.gameObject.SetActive(true);
        puncherVFX.Play();
        puncherVFX.Release(m_BaseBoxer.PuncherVFX, 1f);
    }
}
