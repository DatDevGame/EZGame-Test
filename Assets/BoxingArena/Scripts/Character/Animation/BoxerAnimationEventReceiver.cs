using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BoxerAnimationEventReceiver : MonoBehaviour
{
    protected BoxerAttackingState m_AttackingState;

    public virtual void SetAttackingState(BoxerAttackingState state)
    {
        m_AttackingState = state;
    }

    public virtual void OnAttackHit()
    {
        m_AttackingState?.HandleAttackHit();
    }
}
