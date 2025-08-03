using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "StatsSO", menuName = "BoxingArena/StatsSO", order = 0)]
public class StatsSO : SerializableScriptableObject
{
    [SerializeField, BoxGroup("Combat Stats")] public float MaxHealth = 100f;
    [SerializeField, BoxGroup("Combat Stats")] public float BlockChance = 0.2f;
    [SerializeField, BoxGroup("Combat Stats")] public float CriticalChance = 0.1f;
    [SerializeField, BoxGroup("Combat Stats")] public float CriticalMultiplier = 2f;
    [SerializeField, BoxGroup("Combat Stats")] public LayerMask TeamLayerMask = -1;

    [Title("Move State Config", "", TitleAlignments.Centered)]
    [Range(0.1f, 5f)]
    public float MoveSpeed = 3.5f;

    [Title("Attack State Config", "", TitleAlignments.Centered)]
    public float AttackDamage = 15f;
    public float AttackRange = 0.5f;
    public float AttackCoolDown = 1f;

    [SerializeField, BoxGroup("AI Behavior")] private float m_Aggression = 0.5f;
    [SerializeField, BoxGroup("AI Behavior")] private float m_ReactionTime = 0.3f;

    public float Aggression => m_Aggression;
    public float ReactionTime => m_ReactionTime;
}
