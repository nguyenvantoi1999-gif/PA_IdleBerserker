using System.Collections;
using UnityEngine;

namespace IdleBattle
{
    // Player base: HP dạng tỉ lệ 0..1, có chế độ berserk.
    public abstract class BerserkerObject : CharacterObject
    {
        protected BerserkAbility _berserkAbility;
        protected BerserkerAttackAbility _attackAbility;
        protected PlayerSkillAbility _skillAbility;

        private bool _settingBerserk;

        public bool IsBerserkMode { get { return _berserkAbility != null && _berserkAbility.IsBerserkMode; } }
        public override float HealthPercent { get { return (float)_currentHealth; } }

        protected override void OnInit()
        {
            SafeSetActive(true);
            InitAbilities();
            _berserkAbility = GetAbility<BerserkAbility>();
            _attackAbility = GetAbility<BerserkerAttackAbility>();
            _skillAbility = GetAbility<PlayerSkillAbility>();

            InitFSM();

            if (Model != null) { Model.localPosition = Vector3.zero; }
            _currentHealth = 1;
            _alive = true;

            _fsmAbility.ChangeState(Enum_BerserkStateType.Idle);
        }

        public override bool TryTakeHit(Damage damage, CharacterObject from)
        {
            if (!_alive) { return false; }
            if (AbsorbDamage(ref damage)) { return true; }
            double health = Stat[Enum_StatType.Health];
            double percent = health > 0 ? damage.Value / health : damage.Value;
            _currentHealth -= percent;
            if (_currentHealth <= 0) { Death(); }
            OnTakeHit(damage, from);
            if (OnDamaged != null) { OnDamaged(damage); }
            return true;
        }

        protected override void OnDeath()
        {
            _fsmAbility.ChangeState(Enum_BerserkStateType.Death);
        }

        // Thay UniTask.Delay của game gốc bằng coroutine.
        public void SetBerserkState(bool isOn)
        {
            if (isOn)
            {
                if (_settingBerserk || IsBerserkMode) { return; }
                StartCoroutine(BerserkOnRoutine());
            }
            else
            {
                _berserkAbility.ResetBerserk();
                OnBerserkFinish();
                BackgroundManager.Instance.SetActiveBerserkBackground(false);
                if (SoundManager.Instance != null) { SoundManager.Instance.PlayBackground("berserk_bgm_normal", true); }
            }
        }

        private IEnumerator BerserkOnRoutine()
        {
            _settingBerserk = true;
            _fsmAbility.ChangeState(Enum_BerserkStateType.Berserk);
            _berserkAbility.PlayTransformVFX();
            yield return new WaitForSeconds(0.3f);
            if (!IsDeath)
            {
                _berserkAbility.StartBerserk();
                OnBerserkStart();
                CombatFeedback fb = GetAbility<CombatFeedback>();
                if (fb != null) { fb.OnBerserkStart(); }
                BackgroundManager.Instance.SetActiveBerserkBackground(true);
                if (SoundManager.Instance != null) { SoundManager.Instance.PlayBackground("berserk_bgm_berserkmode", true); }
            }
            _settingBerserk = false;
        }

        protected virtual void OnBerserkStart() { }
        protected virtual void OnBerserkFinish() { }

        protected override double MaxHealth { get { return 1.0; } }
        public override void TakeRecovery(double amount)
        {
            double health = Stat[Enum_StatType.Health];
            _currentHealth += health > 0 ? amount / health : amount;
            if (_currentHealth > 1.0) { _currentHealth = 1.0; }
        }
    }
}
