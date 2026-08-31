namespace IdleBattle
{
    // Rút gọn từ game gốc, giữ tên & thứ tự các stat cốt lõi + berserk.
    public enum Enum_StatType
    {
        Damage = 0,
        Health,
        AttackSpeed,
        MoveSpeed,
        CriticalChance,
        CriticalDamage,
        BerserkDamage,
        BerserkAttackSpeed,
        BerserkCriticalChance,
        BerserkCriticalDamage,
        BerserkDuration,
        BerserkShockWave,
        DetectRange,
        AttackRange,
        SkillDamage,
        SkillCriticalChance,
        SkillCriticalDamage,
        CoolDownReduce,
        DamagePerHealth,
        SuperCriticalDamage,
        FireAttackDamage,
        WaterAttackDamage,
        GrassAttackDamage,
        FireAttackDamageMultiply,
        WaterAttackDamageMultiply,
        GrassAttackDamageMultiply,
        FireAttackCriticalDamage,
        WaterAttackCriticalDamage,
        GrassAttackCriticalDamage,
        Count
    }

    public enum Enum_MonsterStateType
    {
        Init = 0,
        Idle,
        Run,
        Death,
        Attack,
        Hit,
        Count
    }

    public enum Enum_BerserkStateType
    {
        Init = 0,
        Idle,
        Run,
        Skill,
        Death,
        Attack,
        Dash,
        Berserk,
        Count
    }

    public enum Enum_DamageType
    {
        Normal = 0,
        BerserkNormal,
        Skill,
        PlayerHit,
        HP,
        Water,
        Fire,
        Grass
    }

    public enum Enum_CriticalType
    {
        None = 0,
        Critical,
        SuperCritical,
        Reduced
    }

    public enum Enum_PlayerState
    {
        None = 0,
        Berserk
    }

    public enum Enum_MonsterType
    {
        StageMonster = 0,
        AbyssMonster,
        None
    }

    public enum Enum_BattleState
    {
        Ready = 0,
        Start,
        End
    }
}
