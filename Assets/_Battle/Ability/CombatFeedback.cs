using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    // Cầu nối giữa combat và hệ Preset diễn hoạt (shake/slow/blackout/sound).
    // - AttackPresets: bảng preset THEO TỪNG ANIMATION đòn đánh (attack_A/B/D/F,
    //   berserker_attack_A/B/D/E/F...). Mỗi đòn tự diễn shake/sound + quyết AttackCount.
    // - Crit/Finisher/Berserk/Skill preset: hiệu ứng phụ thêm ở các mốc đặc biệt.
    public class CombatFeedback : CharacterAbility
    {
        [Header("Preset theo animation đòn đánh")]
        [Tooltip("Mỗi AttackPreset khớp theo AnimtaionName của nó")]
        public AttackPreset[] AttackPresets;

        [Header("Hiệu ứng phụ ở mốc đặc biệt")]
        [Tooltip("Hit-stop thêm khi đòn CHÍ MẠNG (crit)")]
        public AttackPreset CritPreset;
        [Tooltip("Hit-stop thêm khi đòn KẾT COMBO (attack_D)")]
        public AttackPreset FinisherPreset;
        [Tooltip("attack_D rơi vào mỗi N đòn (chuỗi A/B/C/D = 4)")]
        public int FinisherEveryN = 4;

        [Header("Kỹ năng / Cuồng nộ")]
        public SkillEffectPreset BerserkStartPreset;
        public SkillEffectPreset SkillPreset;

        private Dictionary<string, AttackPreset> _byAnim;

        public override void LateInit()
        {
            base.LateInit();
            BuildTable();
        }

        private void BuildTable()
        {
            _byAnim = new Dictionary<string, AttackPreset>();
            if (AttackPresets == null) { return; }
            for (int i = 0; i < AttackPresets.Length; i++)
            {
                AttackPreset p = AttackPresets[i];
                if (p != null && !string.IsNullOrEmpty(p.AnimtaionName) && !_byAnim.ContainsKey(p.AnimtaionName))
                {
                    _byAnim.Add(p.AnimtaionName, p);
                }
            }
        }

        public AttackPreset GetForAnim(string animName)
        {
            if (_byAnim == null) { BuildTable(); }
            if (string.IsNullOrEmpty(animName)) { return null; }
            AttackPreset p;
            return _byAnim.TryGetValue(animName, out p) ? p : null;
        }

        // Gọi ngay khi BẮT ĐẦU đòn: preset có ShakeImmediately thì rung ngay.
        public void OnAttackStart(string animName)
        {
            AttackPreset ap = GetForAnim(animName);
            if (ap != null && ap.ShakeImmediately) { ap.PlayShake(); }
        }

        // Gọi ở đòn trúng mục tiêu chính.
        public void OnPlayerHit(Damage dmg, int comboCount, string animName)
        {
            AttackPreset ap = GetForAnim(animName);
            if (ap != null)
            {
                if (!ap.ShakeImmediately) { ap.PlayShake(); } // đòn không rung sẵn -> rung lúc chạm
                ap.PlaySlow();                                 // null-safe (đa số đòn thường không slow)
                ap.PlaySoundFXWithSoundName();
            }

            // Hit-stop phụ cho crit / đòn kết combo (dùng preset riêng, không nhân đôi shake).
            bool finisher = FinisherEveryN > 0 && (comboCount % FinisherEveryN) == (FinisherEveryN - 1);
            bool crit = dmg.CriticalType != Enum_CriticalType.None;
            if (finisher && FinisherPreset != null) { FinisherPreset.PlaySlow(); }
            else if (crit && CritPreset != null) { CritPreset.PlaySlow(); }
        }

        public void OnBerserkStart()
        {
            if (BerserkStartPreset == null) { return; }
            BerserkStartPreset.PlayShake();
            BerserkStartPreset.PlayBlackout();
            BerserkStartPreset.PlaySlow();
            BerserkStartPreset.PlaySoundFXWithSoundName();
        }

        public void OnSkill()
        {
            if (SkillPreset == null) { return; }
            SkillPreset.PlayShake();
            SkillPreset.PlaySlow();
            SkillPreset.PlayBlackout();
            SkillPreset.PlaySoundFXWithSoundName();
        }
    }
}
