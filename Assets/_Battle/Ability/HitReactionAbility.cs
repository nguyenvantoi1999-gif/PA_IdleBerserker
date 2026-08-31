using UnityEngine;

namespace IdleBattle
{
    // Hoà trộn animation: khi trúng đòn, chớp phản ứng "knockback/hit" trên TRACK 1
    // với alpha thấp (blend) + flash đỏ, KHÔNG cắt ngang đòn đánh trên track 0.
    // => enemy/player vừa nhận damage vừa tiếp tục attack.
    public class HitReactionAbility : CharacterAbility
    {
        [Range(0f, 1f)] public float BlendAlpha = 0.5f;   // độ trộn của phản ứng
        public float HitDuration = 0.28f;                 // thời gian giữ flinch trước khi mix out
        public float FlashDuration = 0.12f;
        public Color FlashColor = new Color(1f, 0.4f, 0.4f);

        private AnimationAbility _anim;
        private float _flash;
        private float _clearTimer;
        private bool _hooked;

        private static readonly string[] HitAnims = { "knockback", "Knockback", "hit", "Hit", "stun" };

        public override void LateInit()
        {
            base.LateInit();
            _anim = _ownerObject.GetAbility<AnimationAbility>();
            _ownerObject.OnDamaged += OnDamaged;
            _hooked = true;
        }

        private void OnDamaged(Damage d)
        {
            if (_ownerObject.IsDeath || _anim == null || _anim.Animation == null) { return; }

            string hit = null;
            for (int i = 0; i < HitAnims.Length; i++)
            {
                if (_anim.IsAnimationExist(HitAnims[i])) { hit = HitAnims[i]; break; }
            }
            if (hit != null)
            {
                // Track 1 chồng lên track 0 với alpha thấp -> flinch nhẹ, KHÔNG thay thế đòn đánh.
                Spine.TrackEntry e = _anim.AnimationState.SetAnimation(1, hit, false);
                e.Alpha = BlendAlpha;
                e.MixBlend = Spine.MixBlend.Replace;
                e.MixDuration = 0.05f;
                _clearTimer = HitDuration;
            }
            _flash = FlashDuration;
        }

        public override void ProcessAbility(float deltaTime)
        {
            if (_clearTimer > 0f)
            {
                _clearTimer -= deltaTime;
                if (_clearTimer <= 0f && _anim != null && _anim.Animation != null)
                {
                    _anim.AnimationState.SetEmptyAnimation(1, 0.15f); // mix out track 1
                }
            }

            if (_flash > 0f && _anim != null && _anim.Animation != null && !_ownerObject.IsDeath)
            {
                _flash -= deltaTime;
                float f = Mathf.Clamp01(_flash / FlashDuration);
                Spine.Skeleton sk = _anim.Skeleton;
                sk.R = Mathf.Lerp(1f, FlashColor.r, f);
                sk.G = Mathf.Lerp(1f, FlashColor.g, f);
                sk.B = Mathf.Lerp(1f, FlashColor.b, f);
            }
        }

        private void OnDestroy()
        {
            if (_hooked && _ownerObject != null) { _ownerObject.OnDamaged -= OnDamaged; }
        }
    }
}
