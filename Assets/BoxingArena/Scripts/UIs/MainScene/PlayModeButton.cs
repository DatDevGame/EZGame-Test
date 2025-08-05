using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using Premium.GameManagement;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class PlayModeButton : MonoBehaviour
{
    [SerializeField, BoxGroup("Config")] private GameMode m_GameMode;
    [SerializeField, BoxGroup("References")] private MultiImageButton m_Button;
    [SerializeField, BoxGroup("References")] private TMP_Text m_NameMode;
    [SerializeField, BoxGroup("References")] private LocalizationParamsManager m_LevelI2Text;
    [SerializeField, BoxGroup("Data")] private PlayModeEnum m_PlayModeVariable;
    [SerializeField, BoxGroup("Data")] private LevelManagerSO m_LevelManagerSO;

    private void Awake()
    {
        if (m_Button == null)
            m_Button = gameObject.GetComponentInChildren<MultiImageButton>();
        if (m_Button != null)
            m_Button.onClick.AddListener(OnClickButton);

        m_NameMode.SetText(m_LevelManagerSO.GetNameMode(m_GameMode));
        m_LevelI2Text.SetParameterValue("value", $"{m_LevelManagerSO.GetCurrentLevel(m_PlayModeVariable.value)}");
    }
    private void OnDestroy()
    {
        m_Button.onClick.RemoveListener(OnClickButton);
    }

    private void OnClickButton()
    {
        m_PlayModeVariable.value = m_GameMode;
        LoadingScreenUI.Load(SceneManager.LoadSceneAsync(SceneName.PlayScene, isPushToStack: false), 1f);
    }
}
