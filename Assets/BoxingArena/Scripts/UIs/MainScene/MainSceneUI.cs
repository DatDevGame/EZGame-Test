using System.Collections;
using System.Collections.Generic;
using Premium;
using Sirenix.OdinInspector;
using UnityEngine;

public class MainSceneUI : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private PlayModePanel m_PlayModePanel;
    [SerializeField, BoxGroup("References")] private MultiImageButton m_PlayButton;

    private void Awake()
    {
        m_PlayButton.onClick.AddListener(PlayModeButton);
    }

    private void OnDestroy()
    {
        m_PlayButton.onClick.RemoveListener(PlayModeButton);
    }

    private void PlayModeButton()
    {
        m_PlayModePanel.Show();
    }
}
