using System.Collections;
using System.Collections.Generic;
using Premium;
using Sirenix.OdinInspector;
using UnityEngine;

public class PlayModePanel : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private CanvasGroupVisibility m_CanvasGroupVisibility;
    [SerializeField, BoxGroup("References")] private EZAnimSequence m_ButtonEZAnimSequence;
    [SerializeField, BoxGroup("References")] private MultiImageButton m_CloseButton;

    private void Awake()
    {
        m_CloseButton.onClick.AddListener(CloseButton);
    }

    private void OnDestroy()
    {
        m_CloseButton.onClick.RemoveListener(CloseButton);
    }

    public void Show()
    {
        m_CanvasGroupVisibility.Show();
        m_ButtonEZAnimSequence.Play();
    }

    public void Hide()
    {
        m_CanvasGroupVisibility.Hide();
        m_ButtonEZAnimSequence.InversePlay();
    }
    private void CloseButton()
    {
        Hide();
    }
}
