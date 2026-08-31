using UnityEngine;

namespace IdleBattle
{
    // Ability: tạo thanh máu nổi + bắn số damage khi trúng đòn.
    public class HealthUI : CharacterAbility
    {
        public Color BarColor = new Color(0.9f, 0.25f, 0.2f);
        public float BarWidth = 1.1f;
        public float HeadMargin = 0.25f;
        public bool ShowBar = true;

        private HealthBar _bar;
        private AnimationAbility _anim;
        private float _headY = 2f;
        private bool _headFinalized;
        private bool _hooked;
        private bool _barGone;

        public override void LateInit()
        {
            base.LateInit();
            _anim = _ownerObject.GetAbility<AnimationAbility>();
            _headY = ComputeHeadY();
            if (ShowBar)
            {
                _bar = HealthBar.Create(_ownerObject.transform, _headY, BarWidth, BarColor, 400);
                _bar.SetRatio(1f);
            }
            _ownerObject.OnDamaged += HandleDamaged;
            _hooked = true;
        }

        private float ComputeHeadY()
        {
            if (_anim != null && _anim.Animation != null)
            {
                MeshRenderer mr = _anim.Animation.GetComponent<MeshRenderer>();
                if (mr != null && mr.bounds.size.y > 0.4f)
                {
                    _headFinalized = true;
                    return (mr.bounds.max.y - _ownerObject.transform.position.y) + HeadMargin;
                }
            }
            return _headY;
        }

        private void HandleDamaged(Damage d)
        {
            bool crit = d.CriticalType == Enum_CriticalType.Critical || d.CriticalType == Enum_CriticalType.SuperCritical;
            Color c = crit ? new Color(1f, 0.85f, 0.2f) : Color.white;
            float size = crit ? 0.75f : 0.55f;
            Vector3 pos = _ownerObject.PositionCenter + new Vector3(Random.Range(-0.25f, 0.25f), 0.25f, -0.2f);
            DamagePopup.Spawn(pos, Mathf.CeilToInt((float)d.Value).ToString(), c, size);
            if (_bar != null) { _bar.SetRatio(_ownerObject.HealthPercent); }
        }

        public override void ProcessAbility(float deltaTime)
        {
            if (_bar != null)
            {
                if (_ownerObject.IsDeath) { Destroy(_bar.gameObject); _bar = null; _barGone = true; return; }
                if (!_headFinalized)
                {
                    float h = ComputeHeadY();
                    if (_headFinalized) { _headY = h; _bar.SetOffsetY(h); }
                }
                _bar.SetRatio(_ownerObject.HealthPercent);
            }
        }

        private void OnDestroy()
        {
            if (_hooked && _ownerObject != null) { _ownerObject.OnDamaged -= HandleDamaged; }
        }
    }
}
