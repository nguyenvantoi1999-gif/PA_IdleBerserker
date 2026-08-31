using UnityEngine;

namespace IdleBattle
{
    public class MonsterAttackAbility : CharacterAbility
    {
        private PlayerObject _player;
        private float _coolTime;
        private Damage _damage;

        public bool IsAttackPossible { get { return _coolTime <= 0f; } }

        public override void LateInit()
        {
            base.LateInit();
            if (BattleManager.Instance != null) { _player = BattleManager.Instance.PlayerObject; }
        }

        public double GetDamage() { return _ownerObject.Stat[Enum_StatType.Damage]; }

        public bool Attack(float multiplier = 1f)
        {
            if (_player == null && BattleManager.Instance != null) { _player = BattleManager.Instance.PlayerObject; }
            if (_player == null || _player.IsDeath) { return false; }

            _damage.Value = GetDamage() * multiplier;
            _damage.DamageType = Enum_DamageType.PlayerHit;
            SetAttackCoolTime(GetCoolTime());

            if (_player.TryTakeHit(_damage, _ownerObject))
            {
                _ownerObject.OnTargetAttack(_player, _damage);
                return true;
            }
            return false;
        }

        private float GetCoolTime()
        {
            double atkSpeed = _ownerObject.Stat[Enum_StatType.AttackSpeed];
            return atkSpeed > 0 ? (float)(1.0 / atkSpeed) : 1f;
        }

        public void SetAttackCoolTime(float t) { _coolTime = t; }

        public override void ProcessAbility(float deltaTime)
        {
            if (_coolTime > 0f) { _coolTime -= deltaTime; }
        }
    }
}
