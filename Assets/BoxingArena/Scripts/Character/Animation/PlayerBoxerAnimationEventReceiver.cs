using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoxerAnimationEventReceiver : BoxerAnimationEventReceiver
{
    protected PlayerBoxer m_PlayerBoxer;
    public void SetPlayerBoxer(PlayerBoxer playerBoxer)
    {
        m_PlayerBoxer = playerBoxer;
    }
    public override void OnAttackHit()
    {
        m_PlayerBoxer.HandleAttackHit();
    }
}
