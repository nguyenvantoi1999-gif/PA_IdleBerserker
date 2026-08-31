// Enum cho hệ CompanionSkill (port từ game gốc, global namespace).
public enum Enum_BuffFrom
{
    Skill, Pet, BlackKnight, EquipmentSet, Companion, CompanionEquipment, Paragon, SpiritEquipment,
}

public enum Enum_Element { None, Water, Fire, Grass }

public enum Enum_ItemGrade { Normal, Rare, Epic, Unique, Legend, Mythic }

public enum Enum_CompanionEquipmentType { CE, Weapon, Armor }

public enum Enum_Bad_Status_Effect
{
    Silence, Poison, PoisonDamage, BurnDamage, Binding, IncreaseHitDamage,
    IncreaseFireHitDamage, IncreaseGrassHitDamage, IncreaseWaterHitDamage,
    ReduceCriticalChance, ReduceAttackSpeed, ReduceDamage, ReduceMoveSpeed, Stun, None,
}

public enum Enum_Good_Status_Effect
{
    Invincible, IgnoreDamagePercent, ReduceHitDamage, ReduceFireHitDamage, ReduceGrassHitDamage,
    ReduceWaterHitDamage, IgnoreDeBuff, IncreaseDamage, IncreaseSkillDamage, IncreaseAttackSpeed,
    IncreaseCriticalChance, IncreaseCriticalDamage, IncreaseMoveSpeed, IncreaseHealthSteal, ReflectHitDamage,
    IncreaseStatByMyDebuff, IncreaseDamageByEnemyDebuff, CoolDownReduce, IncreaseDamageOnce,
    StatFireDamage, StatWaterDamage, StatGrassDamage,
    StatFireDamageMultiply, StatWaterDamageMultiply, StatGrassDamageMultiply,
    StatBossDamage, StatSuperCriticalDamage, StatAttackSpeed, StatHealthUp, StatGoldUp, StatDamageUp,
    EnchantStoneUp, ExpUp, EquipmentUp, StatDamagePerHealthUp, PetGoldUP, IncreaseBerserkDamage,
    FeastWaterAttackUP, FeastFireAttackUP, FeastGrassAttackUP, StatSuperCriticalChance, IncreaseNormalAttack,
    GuildBattleAttackDamageUP, GuildBattleHealthUP, BerserkModeHealthUp, BerserkModeDamagePerHealthUp,
    ParagonExpUp, PetExpUp, ElementAttackWater, ElementAttackFire, ElementAttackGrass,
    WaterAttackCriticalDamageUp, GrassAttackCriticalDamageUp, FireAttackCriticalDamageUp, SkillCriticalDamageUp,
    StageEssenceUP, StageCelestialReduce, StageMonsterDamageUp, StageEssenceExpUp, ShieldUp, StageNatureReduce,
    PetEnchantUP, IncreaseHealthRecover, StatDamagePerHealthUpMultiply,
}
