[System.Serializable]
public class BoxerStats
{
    public float Health;
    public float AttackDamage;
    public float AttackSpeed;
    public float BlockChance;
    public float MoveSpeed;
    public float CriticalChance;
    public float CriticalMultiplier;
    public float Aggression;

    public void LoadStats(StatsSO statsSO)
    {
        Health = statsSO.MaxHealth;
        AttackDamage = statsSO.AttackDamage;
        AttackSpeed = statsSO.AttackSpeed;
        BlockChance = statsSO.BlockChance;
        MoveSpeed = statsSO.MoveSpeed;
        CriticalChance = statsSO.CriticalChance;
        CriticalMultiplier = statsSO.CriticalMultiplier;
        Aggression = statsSO.Aggression;
    }
}
