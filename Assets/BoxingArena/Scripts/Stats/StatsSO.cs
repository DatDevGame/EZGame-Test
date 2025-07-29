using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "StatsSO", menuName = "BoxingArena/StatsSO", order = 0)]
public class StatsSO : SerializableScriptableObject
{
    [SerializeField, BoxGroup("Combat Stats")] private float m_MaxHealth = 100f;
    [SerializeField, BoxGroup("Combat Stats")] private float m_AttackDamage = 15f;
    [SerializeField, BoxGroup("Combat Stats")] private float m_AttackSpeed = 1.2f;
    [SerializeField, BoxGroup("Combat Stats")] private float m_BlockChance = 0.2f;
    [SerializeField, BoxGroup("Combat Stats")] private float m_MoveSpeed = 3.5f;

    [SerializeField, BoxGroup("Special")] private float m_CriticalChance = 0.1f;
    [SerializeField, BoxGroup("Special")] private float m_CriticalMultiplier = 2f;

    [SerializeField, BoxGroup("AI Behavior")] private float m_Aggression = 0.5f;
    [SerializeField, BoxGroup("AI Behavior")] private float m_ReactionTime = 0.3f;

    // Public properties
    public float MaxHealth => m_MaxHealth;
    public float AttackDamage => m_AttackDamage;
    public float AttackSpeed => m_AttackSpeed;
    public float BlockChance => m_BlockChance;
    public float MoveSpeed => m_MoveSpeed;

    public float CriticalChance => m_CriticalChance;
    public float CriticalMultiplier => m_CriticalMultiplier;

    public float Aggression => m_Aggression;
    public float ReactionTime => m_ReactionTime;
}
