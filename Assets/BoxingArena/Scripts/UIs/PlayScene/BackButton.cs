using System.Collections;
using System.Collections.Generic;
using HCore.Events;
using Premium;
using Premium.GameManagement;
using Premium.PoolManagement;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private MultiImageButton m_BackButton;
    [SerializeField, BoxGroup("References")] private CanvasGroupVisibility m_CanvasGroupVisibility;

    private void Awake()
    {
        m_BackButton.onClick.AddListener(OnBackButton);
        GameEventHandler.AddActionEvent(PVPEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.AddActionEvent(PVPEventCode.OnLevelEnd, OnLevelEnd);
    }

    private void OnDestroy()
    {
        m_BackButton.onClick.RemoveListener(OnBackButton);
    }
    private void OnLevelStart()
    {
        m_CanvasGroupVisibility.Show();
    }
    private void OnLevelEnd()
    {
        m_CanvasGroupVisibility.Hide();
    }

    private void OnBackButton()
    {
        LoadingScreenUI.Load(SceneManager.LoadSceneAsync(SceneName.MainScene, isPushToStack: false));
    }
}
