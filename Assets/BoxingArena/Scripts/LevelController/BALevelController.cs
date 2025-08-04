using System.Collections;
using System.Collections.Generic;
using HCore.Events;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using UnityEngine;

public class BALevelController : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private RectTransform m_CanvasHolder;
    [SerializeField, BoxGroup("References")] private DamageFeedBackUI m_DamageFeedBackUI;

    private void Awake()
    {
        GameEventHandler.AddActionEvent(PVPEventCode.CharacterReceivedDamage, OnCharacterReceivedDamage);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PVPEventCode.CharacterReceivedDamage, OnCharacterReceivedDamage);
    }

    private void OnCharacterReceivedDamage(object[] parameters)
    {
        if (parameters == null || parameters.Length <= 0)
            return;
        BaseBoxer baseBoxer = (BaseBoxer)parameters[0];
        int damage = (int)parameters[1];

        DamageFeedBackUI damageFeedBackUI = PoolManager.GetOrCreatePool(m_DamageFeedBackUI, initialCapacity: 1).Get();
        damageFeedBackUI.transform.position = baseBoxer.transform.position + (Vector3.up * 1.5F);
        damageFeedBackUI.Show(damage);
    }
}
