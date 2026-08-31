using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    // Áp Buff lên nhân vật: Invincible, nhân chỉ số (Damage/AttackSpeed/Crit/Berserk...),
    // DoT (Poison/Burn), cờ Stun/Silence/Binding. Các buff meta (Gold/Exp...) lưu nhưng no-op.
    public class BuffAbility : CharacterAbility
    {
        private readonly List<Buff> _buffs = new List<Buff>();
        // undo cho từng buff: (Enum_StatType, factor đã nhân)
        private readonly Dictionary<Buff, List<KeyValuePair<Enum_StatType, double>>> _applied
            = new Dictionary<Buff, List<KeyValuePair<Enum_StatType, double>>>();
        private readonly Dictionary<Buff, List<KeyValuePair<Enum_StatType, double>>> _added
            = new Dictionary<Buff, List<KeyValuePair<Enum_StatType, double>>>();
        private readonly Dictionary<Buff, float> _dotTimer = new Dictionary<Buff, float>();

        public int DeBuffCount
        {
            get { int n = 0; for (int i = 0; i < _buffs.Count; i++) { if (_buffs[i].HaveDebuff) { n++; } } return n; }
        }

        public bool IsInvincible { get; private set; }
        public bool IsStunned { get; private set; }
        public bool IsSilenced { get; private set; }

        public void Add(Buff buff)
        {
            if (buff == null) { return; }
            for (int i = 0; i < _buffs.Count; i++)
            {
                if (_buffs[i].BuffID == buff.BuffID)
                {
                    _buffs[i].RefreshBuff(buff);
                    Recompute();
                    return;
                }
            }
            _buffs.Add(buff);
            ApplyStatEffects(buff);
            _dotTimer[buff] = 0f;
            Recompute();
        }

        public void Remove(int buffID)
        {
            for (int i = _buffs.Count - 1; i >= 0; i--)
            {
                if (_buffs[i].BuffID == buffID) { UnapplyStatEffects(_buffs[i]); _dotTimer.Remove(_buffs[i]); _buffs.RemoveAt(i); }
            }
            Recompute();
        }

        public void RemoveAllBad()
        {
            for (int i = _buffs.Count - 1; i >= 0; i--)
            {
                if (_buffs[i].HaveDebuff) { UnapplyStatEffects(_buffs[i]); _dotTimer.Remove(_buffs[i]); _buffs.RemoveAt(i); }
            }
            Recompute();
        }

        public bool IsActivateStatusEffect(Enum_Bad_Status_Effect e)
        {
            for (int i = 0; i < _buffs.Count; i++)
            {
                if (_buffs[i].BadEffects != null && _buffs[i].BadEffects.ContainsKey(e)) { return true; }
            }
            return false;
        }

        public override void ProcessAbility(float deltaTime)
        {
            for (int i = _buffs.Count - 1; i >= 0; i--)
            {
                Buff b = _buffs[i];
                TickDot(b, deltaTime);
                if (b.CheckBuffEnd(deltaTime)) { UnapplyStatEffects(b); _dotTimer.Remove(b); _buffs.RemoveAt(i); }
            }
            Recompute();
        }

        private void TickDot(Buff b, float dt)
        {
            if (b.BadEffects == null) { return; }
            double damagePerTick = 0;
            if (b.BadEffects.ContainsKey(Enum_Bad_Status_Effect.Poison)) { damagePerTick += b.BadEffects[Enum_Bad_Status_Effect.Poison]; }
            if (b.BadEffects.ContainsKey(Enum_Bad_Status_Effect.PoisonDamage)) { damagePerTick += b.BadEffects[Enum_Bad_Status_Effect.PoisonDamage]; }
            if (b.BadEffects.ContainsKey(Enum_Bad_Status_Effect.BurnDamage)) { damagePerTick += b.BadEffects[Enum_Bad_Status_Effect.BurnDamage]; }
            if (damagePerTick <= 0) { return; }
            float t = _dotTimer.ContainsKey(b) ? _dotTimer[b] : 0f;
            t += dt;
            if (t >= 1f)
            {
                t -= 1f;
                Damage damage = new Damage
                {
                    OriginValue = damagePerTick,
                    Value = damagePerTick,
                    DamageType = Enum_DamageType.Skill
                };
                _ownerObject.TryTakeHit(damage, null);
            }
            _dotTimer[b] = t;
        }

        private void Recompute()
        {
            IsInvincible = false; IsStunned = false; IsSilenced = false;
            for (int i = 0; i < _buffs.Count; i++)
            {
                Buff b = _buffs[i];
                if (b.GoodEffects != null && b.GoodEffects.ContainsKey(Enum_Good_Status_Effect.Invincible)) { IsInvincible = true; }
                if (b.BadEffects != null)
                {
                    if (b.BadEffects.ContainsKey(Enum_Bad_Status_Effect.Stun)) { IsStunned = true; }
                    if (b.BadEffects.ContainsKey(Enum_Bad_Status_Effect.Silence)) { IsSilenced = true; }
                    if (b.BadEffects.ContainsKey(Enum_Bad_Status_Effect.Binding)) { IsStunned = true; }
                }
            }
        }

        // map good-status -> Stat để nhân (1+value). Cái nào không có slot Stat thì bỏ qua (lưu vô hại).
        private static bool MapStat(Enum_Good_Status_Effect e, out Enum_StatType st)
        {
            switch (e)
            {
                case Enum_Good_Status_Effect.IncreaseDamage: case Enum_Good_Status_Effect.StatDamageUp: st = Enum_StatType.Damage; return true;
                case Enum_Good_Status_Effect.IncreaseAttackSpeed: case Enum_Good_Status_Effect.StatAttackSpeed: st = Enum_StatType.AttackSpeed; return true;
                case Enum_Good_Status_Effect.IncreaseCriticalChance: case Enum_Good_Status_Effect.StatSuperCriticalChance: st = Enum_StatType.CriticalChance; return true;
                case Enum_Good_Status_Effect.IncreaseCriticalDamage: st = Enum_StatType.CriticalDamage; return true;
                case Enum_Good_Status_Effect.StatSuperCriticalDamage: st = Enum_StatType.SuperCriticalDamage; return true;
                case Enum_Good_Status_Effect.IncreaseBerserkDamage: st = Enum_StatType.BerserkDamage; return true;
                case Enum_Good_Status_Effect.IncreaseMoveSpeed: st = Enum_StatType.MoveSpeed; return true;
            }
            st = Enum_StatType.Damage; return false;
        }

        private static bool MapAdditiveStat(Enum_Good_Status_Effect effect, out Enum_StatType stat)
        {
            switch (effect)
            {
                case Enum_Good_Status_Effect.StatFireDamage: stat = Enum_StatType.FireAttackDamage; return true;
                case Enum_Good_Status_Effect.StatWaterDamage: stat = Enum_StatType.WaterAttackDamage; return true;
                case Enum_Good_Status_Effect.StatGrassDamage: stat = Enum_StatType.GrassAttackDamage; return true;
                case Enum_Good_Status_Effect.StatFireDamageMultiply: stat = Enum_StatType.FireAttackDamageMultiply; return true;
                case Enum_Good_Status_Effect.StatWaterDamageMultiply: stat = Enum_StatType.WaterAttackDamageMultiply; return true;
                case Enum_Good_Status_Effect.StatGrassDamageMultiply: stat = Enum_StatType.GrassAttackDamageMultiply; return true;
            }

            stat = Enum_StatType.Damage;
            return false;
        }

        private void ApplyStatEffects(Buff buff)
        {
            if (buff.GoodEffects == null) { return; }
            List<KeyValuePair<Enum_StatType, double>> undo = new List<KeyValuePair<Enum_StatType, double>>();
            List<KeyValuePair<Enum_StatType, double>> added = new List<KeyValuePair<Enum_StatType, double>>();
            foreach (KeyValuePair<Enum_Good_Status_Effect, double> pair in buff.GoodEffects)
            {
                Enum_StatType st;
                if (pair.Key == Enum_Good_Status_Effect.IncreaseSkillDamage)
                {
                    double value = pair.Value / 100.0;
                    _ownerObject.Stat[Enum_StatType.SkillDamage] += value;
                    added.Add(new KeyValuePair<Enum_StatType, double>(Enum_StatType.SkillDamage, value));
                }
                else if (MapAdditiveStat(pair.Key, out st))
                {
                    _ownerObject.Stat[st] += pair.Value;
                    added.Add(new KeyValuePair<Enum_StatType, double>(st, pair.Value));
                }
                else if (MapStat(pair.Key, out st))
                {
                    double factor = 1.0 + pair.Value;
                    if (factor <= 0) { factor = 1.0; }
                    _ownerObject.Stat[st] = _ownerObject.Stat[st] * factor;
                    undo.Add(new KeyValuePair<Enum_StatType, double>(st, factor));
                }
            }
            _applied[buff] = undo;
            _added[buff] = added;
        }

        private void UnapplyStatEffects(Buff buff)
        {
            List<KeyValuePair<Enum_StatType, double>> undo;
            if (!_applied.TryGetValue(buff, out undo)) { return; }
            for (int i = 0; i < undo.Count; i++)
            {
                if (undo[i].Value != 0) { _ownerObject.Stat[undo[i].Key] = _ownerObject.Stat[undo[i].Key] / undo[i].Value; }
            }
            _applied.Remove(buff);

            List<KeyValuePair<Enum_StatType, double>> added;
            if (_added.TryGetValue(buff, out added))
            {
                for (int i = 0; i < added.Count; i++)
                {
                    _ownerObject.Stat[added[i].Key] -= added[i].Value;
                }
                _added.Remove(buff);
            }
        }
    }
}
