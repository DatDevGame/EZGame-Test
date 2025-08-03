using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class BoxerAnimationEventReceiver : MonoBehaviour
{
    [ShowInInspector]private BoxerAttackingState m_AttackingState;

    public void SetAttackingState(BoxerAttackingState state)
    {
        m_AttackingState = state;
    }

    public void OnAttackHit()
    {
        m_AttackingState?.HandleAttackHit();
    }
}
