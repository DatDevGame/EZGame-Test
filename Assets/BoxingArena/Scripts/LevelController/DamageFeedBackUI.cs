using DG.Tweening;
using Premium;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class DamageFeedBackUI : MonoBehaviour
{
    [SerializeField, BoxGroup("Config")] private float m_MoveDistance = 2f;
    [SerializeField, BoxGroup("Config")] private float m_Duration = 1f;
    [SerializeField, BoxGroup("Config")] private Ease m_MoveEase = Ease.OutQuad;

    [SerializeField, BoxGroup("References")] private TMP_Text m_Value;
    private CanvasGroup m_CanvasGroup;

    private void Awake()
    {
        m_CanvasGroup = GetComponent<CanvasGroup>();
        if (m_CanvasGroup == null)
            m_CanvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Show(int value)
    {
        m_Value.text = $"-{value}";
        m_CanvasGroup.alpha = 1f;

        float maxAngle = 30f;
        Quaternion randomTilt = Quaternion.AngleAxis(Random.Range(-maxAngle, maxAngle), Vector3.right)
                                   * Quaternion.AngleAxis(Random.Range(-maxAngle, maxAngle), Vector3.forward);
        Vector3 moveDir = randomTilt * Vector3.up;
        transform.rotation = Quaternion.LookRotation(moveDir);

        Vector3 targetPos = transform.position + moveDir.normalized * m_MoveDistance;
        transform.DOMove(targetPos, m_Duration).SetEase(m_MoveEase);
        m_CanvasGroup.DOFade(0f, m_Duration).SetEase(Ease.Linear);
        m_Value.transform.LookAt(Camera.main.transform);
    }

}
