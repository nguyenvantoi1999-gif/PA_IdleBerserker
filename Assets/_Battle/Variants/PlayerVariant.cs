using UnityEngine;

namespace IdleBattle
{
    [CreateAssetMenu(fileName = "PlayerVariant", menuName = "IdleBattle/Player Variant")]
    public class PlayerVariant : CharacterVariant
    {
        [Header("Chỉ số Berserker")]
        public float CriticalChance = 0.25f;
        public float CriticalDamage = 1.0f;
        public float BerserkDamage = 1.8f;
        public float BerserkAttackSpeed = 1.5f;
        public float BerserkCriticalChance = 0.15f;
        public float BerserkCriticalDamage = 0.5f;
        public float BerserkDuration = 6f;
        public float BerserkShockWave = 5f;

        public override void WriteStats(Stat stat)
        {
            base.WriteStats(stat);
            stat[Enum_StatType.CriticalChance] = CriticalChance;
            stat[Enum_StatType.CriticalDamage] = CriticalDamage;
            stat[Enum_StatType.BerserkDamage] = BerserkDamage;
            stat[Enum_StatType.BerserkAttackSpeed] = BerserkAttackSpeed;
            stat[Enum_StatType.BerserkCriticalChance] = BerserkCriticalChance;
            stat[Enum_StatType.BerserkCriticalDamage] = BerserkCriticalDamage;
            stat[Enum_StatType.BerserkDuration] = BerserkDuration;
            stat[Enum_StatType.BerserkShockWave] = BerserkShockWave;
        }
    }
}
