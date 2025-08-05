using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;

public class BoxerAnimationEventReceiver : MonoBehaviour
{
    protected BaseBoxer m_BaseBoxer;
    protected BoxerAttackingState m_AttackingState;
    protected List<ParticleSystem> m_PuncherVFXs = new List<ParticleSystem>();

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
        if (m_PuncherVFXs == null)
            m_PuncherVFXs = new List<ParticleSystem>();
        m_PuncherVFXs.Add(puncherVFX);
        puncherVFX.transform.SetParent(yourHand);
        puncherVFX.transform.localPosition = Vector3.zero;
        puncherVFX.transform.rotation = Quaternion.LookRotation(yourHand.forward, yourHand.up);
        puncherVFX.gameObject.SetActive(true);
        puncherVFX.Play();
        puncherVFX.Release(m_BaseBoxer.PuncherVFX, 0.2f);
    }

    private void OnDestroy()
    {
        try
        {
            if (m_PuncherVFXs.Count <= 0) return;
            for (int i = 0; i < m_PuncherVFXs.Count; i++)
                m_PuncherVFXs[i].transform.SetParent(PoolManager.Instance.transform);
        }
        catch { }
    }
}
