using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Sirenix.OdinInspector;
using UnityEngine;
using DG.Tweening;

public class PreviewCharacterHandle : MonoBehaviour
{
    public Action OnCompletedPreview = delegate { };

    [SerializeField, BoxGroup("Config")] private float m_PlayerPreviewDuration, m_OpponentPreviewDuration;
    [SerializeField, BoxGroup("References")] private Transform m_PlayerTeamStartPoint, m_PlayerTeamEndPoint;
    [SerializeField, BoxGroup("References")] private Transform m_OpponentTeamStartPoint, m_OpponentTeamEndPoint;
    [SerializeField, BoxGroup("References")] private Transform m_CinemachinePlayerTeam, m_CinemachineOpponentTeam;
    [SerializeField, BoxGroup("References")] private GameObject m_PanoramicCamera;

    public void PlayPreview()
    {
        StartCoroutine(DelayPlayView());
    }

    private IEnumerator DelayPlayView()
    {
        yield return new WaitForSeconds(1);
        m_PanoramicCamera.SetActive(false);
        m_CinemachinePlayerTeam.position = m_PlayerTeamStartPoint.position;
        m_CinemachineOpponentTeam.position = m_OpponentTeamStartPoint.position;

        yield return new WaitForSeconds(2f);
        m_CinemachinePlayerTeam
        .DOMove(m_PlayerTeamEndPoint.position, m_PlayerPreviewDuration)
        .OnComplete(() =>
        {
            //Play Preview Oppnent
            m_CinemachinePlayerTeam.gameObject.SetActive(false);
            m_CinemachineOpponentTeam
            .DOMove(m_OpponentTeamEndPoint.position, m_OpponentPreviewDuration)
            .SetDelay(2f)
            .OnComplete(() =>
            {
                OnCompletedPreview?.Invoke();
                m_CinemachineOpponentTeam.gameObject.SetActive(false);
            });
        });
    }
}
