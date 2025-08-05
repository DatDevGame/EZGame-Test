using System.Collections;
using System.Collections.Generic;
using HCore.Events;
using Premium;
using Sirenix.OdinInspector;
using UnityEngine;

public class SpectatorPanel : MonoBehaviour
{
    [SerializeField, BoxGroup("References")] private MultiImageButton m_PreviousPlayerButton, m_NextPlayerButton;
    [SerializeField, BoxGroup("References")] private BALevelController m_BALevelController;
    [SerializeField, BoxGroup("References")] private CanvasGroupVisibility m_CanvasGroupVisibility;
    [SerializeField, BoxGroup("Resource")] private Material m_DefaultMaterialCharacter;

    private CameraController m_CameraController;
    private BaseBoxer m_PreviousBoxer;
    private BaseBoxer m_NextBoxer;
    private int m_Index = 0;
    private void Awake()
    {
        if (m_BALevelController == null)
            m_BALevelController = FindAnyObjectByType<BALevelController>();
        if (m_CameraController == null)
            m_CameraController = FindAnyObjectByType<CameraController>();
        m_PreviousPlayerButton.onClick.AddListener(SelectPreviousSpectatedPlayer);
        m_NextPlayerButton.onClick.AddListener(SelectNextSpectatedPlayer);

        GameEventHandler.AddActionEvent(PVPEventCode.AnyCharacterDead, OnAnyCharacterDead);
        GameEventHandler.AddActionEvent(PVPEventCode.OnLevelEnd, OnLevelEnd);
    }
    private void OnDestroy()
    {
        m_PreviousPlayerButton.onClick.RemoveListener(SelectPreviousSpectatedPlayer);
        m_NextPlayerButton.onClick.RemoveListener(SelectNextSpectatedPlayer);

        GameEventHandler.RemoveActionEvent(PVPEventCode.AnyCharacterDead, OnAnyCharacterDead);
        GameEventHandler.RemoveActionEvent(PVPEventCode.OnLevelEnd, OnLevelEnd);
    }
    private void SelectPreviousSpectatedPlayer()
    {
        SetOutlineSpectorCharacter(0, m_PreviousBoxer);
        SetOutlineSpectorCharacter(5, m_NextBoxer);
        m_PreviousBoxer = m_NextBoxer != null ? m_NextBoxer : m_BALevelController.PlayerTeams[m_Index];
        m_CameraController.SetFollowing(m_NextBoxer != null ? m_NextBoxer.transform : m_BALevelController.PlayerTeams[m_Index].transform);
        m_Index--;
        if (m_Index < 0)
            m_Index = m_BALevelController.PlayerTeams.Count - 1;
        m_NextBoxer = m_BALevelController.PlayerTeams[m_Index];
    }

    private void SelectNextSpectatedPlayer()
    {
        SetOutlineSpectorCharacter(0, m_PreviousBoxer);
        SetOutlineSpectorCharacter(5, m_NextBoxer);
        m_PreviousBoxer = m_NextBoxer != null ? m_NextBoxer : m_BALevelController.PlayerTeams[m_Index];
        m_CameraController.SetFollowing(m_NextBoxer != null ? m_NextBoxer.transform : m_BALevelController.PlayerTeams[m_Index].transform);
        m_Index++;
        if (m_Index >= m_BALevelController.PlayerTeams.Count - 1)
            m_Index = 0;
        m_NextBoxer = m_BALevelController.PlayerTeams[m_Index];
    }

    private void SetOutlineSpectorCharacter(float outLineValue, BaseBoxer baseBoxer)
    {
        if (baseBoxer == null) return;
        SkinnedMeshRenderer skinnedMeshRenderer = baseBoxer.GetComponentInChildren<SkinnedMeshRenderer>();
        if (skinnedMeshRenderer == null) return;
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        skinnedMeshRenderer.GetPropertyBlock(mpb);
        mpb.SetFloat("_OutlineWidth", outLineValue);
        skinnedMeshRenderer.SetPropertyBlock(mpb);
    }

    private void OnAnyCharacterDead(object[] parrams)
    {
        if (parrams == null || parrams.Length <= 0) return;
        BaseBoxer baseBoxer = (BaseBoxer)parrams[0];
        if (baseBoxer != null && baseBoxer.IsLocal)
        {
            m_CanvasGroupVisibility.Show();
        }
    }
    private void OnLevelEnd()
    {
        m_CanvasGroupVisibility.Hide();
    }
}
