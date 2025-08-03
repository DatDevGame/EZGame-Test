using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WeakPoint : MonoBehaviour, IDamageable
{
    public WeakPointType WeakPointType => m_WeakPointType;
    [SerializeField, BoxGroup("Config")] private WeakPointType m_WeakPointType;
    [SerializeField, BoxGroup("References")] private BaseBoxer m_Boxer;

    private void Awake()
    {
        if (m_Boxer == null)
            m_Boxer = GetComponentInParent<BaseBoxer>();
    }

    public void TakeDamage(float amount)
    {
        if (m_Boxer == null) return;
        //m_Boxer.TakeDamage(m_WeakPointType, amount);
    }

    public void Load(BaseBoxer baseBoxer)
    {
        if (baseBoxer != null)
            m_Boxer = baseBoxer;
    }
}
