using System.Collections;
using System.Collections.Generic;
using HCore.Events;
using Premium;
using Premium.GameManagement;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private GameObject m_WinTitle;
    [SerializeField, BoxGroup("References")] private GameObject m_LoseTitle;
    [SerializeField, BoxGroup("References")] private MultiImageButton m_ContinueButton;
    [SerializeField, BoxGroup("References")] private EZAnimSequence m_OpenEZAnimSequence;
    [SerializeField, BoxGroup("References")] private CanvasGroupVisibility m_CanvasGroupVisibility;
    private void Awake()
    {
        m_ContinueButton.onClick.AddListener(ContinueButton);
        GameEventHandler.AddActionEvent(PVPEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.AddActionEvent(PVPEventCode.OnLevelEnd, OnLevelEnd);
    }

    private void OnDestroy()
    {
        m_ContinueButton.onClick.RemoveListener(ContinueButton);
        GameEventHandler.RemoveActionEvent(PVPEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.RemoveActionEvent(PVPEventCode.OnLevelEnd, OnLevelEnd);
    }

    private void ContinueButton()
    {
        LoadingScreenUI.Load(SceneManager.LoadSceneAsync(SceneName.MainScene, isPushToStack: false));
    }

    private void OnLevelStart()
    {

    }

    private void OnLevelEnd(object[] parrams)
    {
        if (parrams == null || parrams.Length <= 0)
            return;

        LevelDataSO levelDataSO = (LevelDataSO)parrams[0];
        bool isVictory = (bool)parrams[1];
        StartCoroutine(DelayShow(isVictory));
    }

    private IEnumerator DelayShow(bool isVictory)
    {
        yield return new WaitForSeconds(1.5f);
        m_WinTitle.SetActive(isVictory);
        m_LoseTitle.SetActive(!isVictory);
        m_CanvasGroupVisibility.Show();
        m_OpenEZAnimSequence.Play();
    }
}
