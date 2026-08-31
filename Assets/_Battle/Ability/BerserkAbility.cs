using UnityEngine;

namespace IdleBattle
{
    // Chế độ cuồng nộ: gauge đầy ở 10 đòn đánh thường -> bật berserk theo BerserkDuration.
    public class BerserkAbility : CharacterAbility
    {
        public float MaxGauge = 10f;
        public bool IsAuto = true;

        private float _gauge;
        private float _remainTime;
        private float _duration;
        private BerserkerObject _berserker;
        private BerserkVFXContainer _vfx;

        public BerserkVFXContainer BerserkVFXContainer { get { return _vfx; } }

        public float CurrentGauge { get { return _gauge; } }
        public bool IsBerserkMode { get { return _remainTime > 0f; } }
        public bool IsReady { get { return _gauge >= MaxGauge && !IsBerserkMode; } }
        public float GaugeRatio { get { return Mathf.Clamp01(_gauge / MaxGauge); } }
        public float RemainRatio { get { return _duration > 0f ? Mathf.Clamp01(_remainTime / _duration) : 0f; } }

        public override void LateInit()
        {
            base.LateInit();
            _berserker = _ownerObject as BerserkerObject;
            _vfx = GetComponentInChildren<BerserkVFXContainer>();
        }

        // ===== VFX hooks (như game gốc) =====
        public void PlayTransformVFX() { if (_vfx != null) { _vfx.ChangeBerserkEffectActivation(true); } }
        public void StopTransformVFX() { if (_vfx != null) { _vfx.ChangeBerserkEffectActivation(false); } }
        public void PlayDashVfx() { if (_vfx != null) { _vfx.PlayDashEffect(); } }

        public void OnPlayerAttack()
        {
            if (IsBerserkMode) { return; }
            _gauge = Mathf.Min(_gauge + 1f, MaxGauge);
        }

        public bool IsEnableAutoBerserk()
        {
            return IsAuto && !IsBerserkMode && _gauge >= MaxGauge;
        }

        public void StartBerserk()
        {
            float dur = (float)_ownerObject.Stat[Enum_StatType.BerserkDuration];
            if (dur <= 0f) { dur = 5f; }
            _remainTime = dur;
            _duration = dur;
            _gauge = 0f;
        }

        public void ResetBerserk()
        {
            _remainTime = 0f;
            _gauge = 0f;
            StopTransformVFX();
        }

        public override void ProcessAbility(float deltaTime)
        {
            if (!IsBerserkMode) { return; }
            _remainTime -= deltaTime;
            if (_remainTime <= 0f)
            {
                _remainTime = 0f;
                if (_berserker != null) { _berserker.SetBerserkState(false); }
            }
        }
    }
}
