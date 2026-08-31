using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    // Base nhân vật. Bỏ pooling/AntiCheat của game gốc; giữ pattern Ability + FSM.
    public abstract class CharacterObject : MonoBehaviour
    {
        public Transform Model;
        public Stat Stat = new Stat();

        protected double _currentHealth = 1;
        protected bool _alive;
        protected FSMAbility _fsmAbility;
        protected CharacterAbility[] _abilities;
        private bool _abilitiesCached;
        private AnimationAbility _animationAbility;

        public Vector3 Position { get { return transform.position; } }
        public bool IsDeath { get { return !_alive; } }
        public double CurrentHealth { get { return _currentHealth; } }

        public virtual float HealthPercent
        {
            get
            {
                double max = Stat[Enum_StatType.Health];
                return max > 0 ? (float)(_currentHealth / max) : 0f;
            }
        }

        protected AnimationAbility AnimationAbility
        {
            get
            {
                if (_animationAbility == null) { _animationAbility = GetAbility<AnimationAbility>(); }
                return _animationAbility;
            }
        }

        public Vector3 PositionCenter
        {
            get
            {
                AnimationAbility a = AnimationAbility;
                if (a != null && a.Animation != null) { return a.CenterWorld; }
                return Model != null ? Model.position : transform.position;
            }
        }

        public void SafeSetActive(bool value)
        {
            if (gameObject.activeSelf != value) { gameObject.SetActive(value); }
        }

        public void SetAlive(bool value) { _alive = value; }

        public bool IsAlive { get { return _alive; } }

        private double _shield;
        private float _shieldTime;
        public double CurrentShield { get { return _shield; } }
        public float ShieldTime { get { return _shieldTime; } }
        protected virtual double MaxHealth { get { return Stat[Enum_StatType.Health]; } }

        public void AddBuff(Buff buff) { BuffAbility ba = GetAbility<BuffAbility>(); if (ba != null) { ba.Add(buff); } }
        public void RemoveBuff(int buffID) { BuffAbility ba = GetAbility<BuffAbility>(); if (ba != null) { ba.Remove(buffID); } }
        public void RemoveAllBadBuff() { BuffAbility ba = GetAbility<BuffAbility>(); if (ba != null) { ba.RemoveAllBad(); } }
        public void TakeShield(double amount) { _shield += amount; _shieldTime = 9999f; }
        public void TakeShield(double amount, float time) { _shield += amount; _shieldTime = time; }
        public void RemoveShield() { _shield = 0; _shieldTime = 0; }
        public virtual void TakeRecovery(double amount)
        {
            _currentHealth += amount;
            double max = MaxHealth;
            if (_currentHealth > max) { _currentHealth = max; }
        }

        // Trả true nếu đòn bị chặn hoàn toàn (bất tử / khiên hấp thụ hết).
        protected bool AbsorbDamage(ref Damage damage)
        {
            BuffAbility ba = GetAbility<BuffAbility>();
            if (ba != null && ba.IsInvincible) { return true; }
            if (_shield > 0)
            {
                if (_shield >= damage.Value) { _shield -= damage.Value; return true; }
                damage.Value -= _shield; _shield = 0;
            }
            return false;
        }

        public void Init()
        {
            OnInit();
            _alive = true;
        }

        protected virtual void OnInit() { }

        private void CacheAbilities()
        {
            _abilities = GetComponents<CharacterAbility>();
            _abilitiesCached = true;
        }

        public void InitAbilities()
        {
            if (!_abilitiesCached) { CacheAbilities(); }
            for (int i = 0; i < _abilities.Length; i++) { _abilities[i].Init(); }
            for (int i = 0; i < _abilities.Length; i++) { _abilities[i].LateInit(); }
        }

        public T GetAbility<T>() where T : CharacterAbility
        {
            if (!_abilitiesCached) { CacheAbilities(); }
            for (int i = 0; i < _abilities.Length; i++)
            {
                if (_abilities[i] is T t) { return t; }
            }
            return null;
        }

        public abstract void InitFSM();

        public virtual bool TryTakeHit(Damage damage, CharacterObject from)
        {
            if (!_alive) { return false; }
            if (AbsorbDamage(ref damage)) { return true; }
            _currentHealth -= damage.Value;
            if (_currentHealth <= 0) { Death(); }
            OnTakeHit(damage, from);
            if (OnDamaged != null) { OnDamaged(damage); }
            return true;
        }

        public bool TryTakeHit(List<Damage> damages, CharacterObject from)
        {
            bool any = false;
            for (int i = 0; i < damages.Count; i++)
            {
                if (TryTakeHit(damages[i], from)) { any = true; }
            }
            return any;
        }

        protected virtual void OnTakeHit(Damage damage, CharacterObject from) { }

        public virtual void Death()
        {
            if (_alive)
            {
                _alive = false;
                _currentHealth = 0;
                OnDeath();
            }
        }

        protected virtual void OnDeath() { }

        public void Kill()
        {
            _alive = false;
            SafeSetActive(false);
        }

        // Hook cho UI (healthbar/damage number) — gọi mỗi khi trúng đòn.
        public System.Action<Damage> OnDamaged;

        public virtual void OnTargetAttack(CharacterObject target, Damage damage) { }
        public virtual void OnAttack() { }

        protected virtual void Update()
        {
            if (_abilitiesCached)
            {
                float dt = Time.deltaTime;
                for (int i = 0; i < _abilities.Length; i++) { _abilities[i].ProcessAbility(dt); }
                if (_shieldTime > 0f) { _shieldTime -= dt; if (_shieldTime <= 0f) { _shield = 0; } }
            }
        }
    }
}
