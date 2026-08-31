using UnityEngine;

namespace IdleBattle
{
    // Đánh thường của berserker. Lược crit/element/boss phức tạp,
    // giữ: damage cơ bản, crit đơn giản, hệ số berserk, nạp gauge berserk.
    public class BerserkerAttackAbility : CharacterAbility
    {
        private BerserkAbility _berserk;
        private ComboAbility _combo;

        public override void LateInit()
        {
            base.LateInit();
            _berserk = _ownerObject.GetAbility<BerserkAbility>();
            _combo = _ownerObject.GetAbility<ComboAbility>();
        }

        public Damage Attack(CharacterObject target, Enum_DamageType type = Enum_DamageType.Normal, double multiplier = 1.0)
        {
            if (target == null || target.IsDeath) { return default(Damage); }
            Damage dmg = GetDamageData(type, multiplier);
            target.TryTakeHit(dmg, _ownerObject);
            _ownerObject.OnTargetAttack(target, dmg);
            if (_berserk != null && type != Enum_DamageType.Skill)
            {
                _berserk.OnPlayerAttack();
            }
            return dmg;
        }

        public Damage GetDamageData(Enum_DamageType type, double multiplier)
        {
            double baseDamage = _ownerObject.Stat[Enum_StatType.Damage];
            double damage = baseDamage * multiplier;
            if (_combo != null && type != Enum_DamageType.Skill) { damage *= _combo.DamageMultiplier; }
            bool berserk = _berserk != null && _berserk.IsBerserkMode;

            if (berserk)
            {
                double bMul = _ownerObject.Stat[Enum_StatType.BerserkDamage];
                if (bMul > 1.0) { damage *= bMul; }
                if (type == Enum_DamageType.Normal) { type = Enum_DamageType.BerserkNormal; }
            }

            Enum_CriticalType critType = Enum_CriticalType.None;
            double critChance = _ownerObject.Stat[Enum_StatType.CriticalChance]
                + (berserk ? _ownerObject.Stat[Enum_StatType.BerserkCriticalChance] : 0);
            if (Random.value < (float)critChance)
            {
                critType = Enum_CriticalType.Critical;
                double critDmg = _ownerObject.Stat[Enum_StatType.CriticalDamage]
                    + (berserk ? _ownerObject.Stat[Enum_StatType.BerserkCriticalDamage] : 0);
                damage *= (1.0 + critDmg);
            }

            return new Damage
            {
                OriginValue = baseDamage,
                Value = damage,
                DamageType = type,
                CriticalType = critType,
                PlayerState = berserk ? Enum_PlayerState.Berserk : Enum_PlayerState.None
            };
        }

        public Damage GetBasicDamageData()
        {
            double baseDmg = _ownerObject.Stat[Enum_StatType.Damage];
            bool berserk = _berserk != null && _berserk.IsBerserkMode;
            return new Damage
            {
                OriginValue = baseDmg, Value = baseDmg,
                DamageType = Enum_DamageType.Normal, CriticalType = Enum_CriticalType.None,
                PlayerState = berserk ? Enum_PlayerState.Berserk : Enum_PlayerState.None
            };
        }

        public Damage GetBasicDamageData(Enum_StatType statType)
        {
            double baseDmg = _ownerObject.Stat[statType];
            return new Damage { OriginValue = baseDmg, Value = baseDmg, DamageType = Enum_DamageType.Normal, CriticalType = Enum_CriticalType.None };
        }

        public Damage GetDefaultDamage(Enum_DamageType type, Enum_CriticalType crit)
        {
            double baseDamage = _ownerObject.Stat[Enum_StatType.Damage];
            bool berserk = _berserk != null && _berserk.IsBerserkMode;
            if (berserk)
            {
                baseDamage *= _ownerObject.Stat[Enum_StatType.BerserkDamage];
            }

            double value = baseDamage;
            if (crit == Enum_CriticalType.Critical || crit == Enum_CriticalType.SuperCritical)
            {
                value *= 1.0 + _ownerObject.Stat[Enum_StatType.CriticalDamage]
                    + (berserk ? _ownerObject.Stat[Enum_StatType.BerserkCriticalDamage] : 0);
            }
            if (crit == Enum_CriticalType.SuperCritical)
            {
                value *= 1.0 + _ownerObject.Stat[Enum_StatType.SuperCriticalDamage];
            }

            return new Damage
            {
                OriginValue = baseDamage,
                Value = value,
                DamageType = type,
                CriticalType = crit,
                PlayerState = berserk ? Enum_PlayerState.Berserk : Enum_PlayerState.None
            };
        }

        public Damage GetElementDamage(Enum_Element element, bool flag)
        {
            Damage damage = GetBasicDamageData();
            Enum_StatType damageStat;
            Enum_StatType multiplyStat;
            Enum_StatType criticalDamageStat;

            switch (element)
            {
                case Enum_Element.Fire:
                    damageStat = Enum_StatType.FireAttackDamage;
                    multiplyStat = Enum_StatType.FireAttackDamageMultiply;
                    criticalDamageStat = Enum_StatType.FireAttackCriticalDamage;
                    damage.DamageType = Enum_DamageType.Fire;
                    break;
                case Enum_Element.Water:
                    damageStat = Enum_StatType.WaterAttackDamage;
                    multiplyStat = Enum_StatType.WaterAttackDamageMultiply;
                    criticalDamageStat = Enum_StatType.WaterAttackCriticalDamage;
                    damage.DamageType = Enum_DamageType.Water;
                    break;
                case Enum_Element.Grass:
                    damageStat = Enum_StatType.GrassAttackDamage;
                    multiplyStat = Enum_StatType.GrassAttackDamageMultiply;
                    criticalDamageStat = Enum_StatType.GrassAttackCriticalDamage;
                    damage.DamageType = Enum_DamageType.Grass;
                    break;
                default:
                    return damage;
            }

            double elementValue = _ownerObject.Stat[damageStat]
                * (1.0 + _ownerObject.Stat[multiplyStat]);
            damage.Value *= 1.0 + elementValue;

            if (flag && _ownerObject.Stat[criticalDamageStat] > 0)
            {
                damage.Value *= 1.0 + _ownerObject.Stat[criticalDamageStat];
                damage.CriticalType = Enum_CriticalType.Critical;
            }

            return damage;
        }

        public float GetAttackSpeed()
        {
            double atkSpeed = _ownerObject.Stat[Enum_StatType.AttackSpeed];
            if (atkSpeed <= 0) { atkSpeed = 1; }
            if (_berserk != null && _berserk.IsBerserkMode)
            {
                double bMul = _ownerObject.Stat[Enum_StatType.BerserkAttackSpeed];
                if (bMul > 1.0) { atkSpeed *= bMul; }
            }
            float[] rand = { 1.0f, 1.1f, 1.2f };
            atkSpeed *= rand[Random.Range(0, 3)];
            return (float)atkSpeed;
        }
    }
}
