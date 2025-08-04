using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using HCore.Events;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField, BoxGroup("Referenes")] private CinemachineVirtualCamera m_EnvironmentCamera;
    [SerializeField, BoxGroup("Referenes")] private CinemachineVirtualCamera m_FollowingPlayerCamera;

    private PlayerBoxer m_PlayerBoxer;
    private void Awake()
    {
        GameEventHandler.AddActionEvent(PVPEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.AddActionEvent(PVPEventCode.OnLevelEnd, OnLevelEnd);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PVPEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.RemoveActionEvent(PVPEventCode.OnLevelEnd, OnLevelEnd);
    }

    private void OnLevelStart(object[] parrams)
    {
        if (parrams == null || parrams.Length <= 0)
            return;
        m_PlayerBoxer = (PlayerBoxer)parrams[1];
        m_EnvironmentCamera.gameObject.SetActive(false);

        if (m_PlayerBoxer != null)
            m_FollowingPlayerCamera.m_Follow = m_PlayerBoxer.transform;
    }

    private void OnLevelEnd()
    {

    }
}
