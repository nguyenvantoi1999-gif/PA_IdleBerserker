using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    // Skill chủ động cơ bản: cooldown; khi sẵn sàng & có target trong DetectRange*2
    // -> FSM chuyển sang Skill state, gây AoE damage (SkillMultiplier).
    public class PlayerSkillAbility : CharacterAbility
    {
        public float CoolTime = 6f;
        public double SkillMultiplier = 4.0;
        public int MaxTargets = 6;

        private float _timer;
        private BerserkerAttackAbility _attack;
        private MonsterDetectAbility _detect;

        public bool IsSkillReady { get { return _timer <= 0f; } }
        public float CooldownRatio { get { return Mathf.Clamp01(1f - _timer / Mathf.Max(0.01f, CoolTime)); } }

        public override void LateInit()
        {
            base.LateInit();
            _attack = _ownerObject.GetAbility<BerserkerAttackAbility>();
            _detect = _ownerObject.GetAbility<MonsterDetectAbility>();
            _timer = CoolTime;
        }

        public override void ProcessAbility(float deltaTime)
        {
            if (_timer > 0f) { _timer -= deltaTime; }
        }

        public bool HasTarget()
        {
            // Chỉ kích skill khi có quái trong TẦM ĐÁNH (không đứng tung skill vào quái còn ở xa;
            // để player chạy tới mục tiêu kế thay vì kẹt anim). Active vẫn quét AoE rộng hơn khi đã tung.
            double range = _ownerObject.Stat[Enum_StatType.AttackRange];
            return _detect != null && _detect.Detect(range);
        }

        public void ResetCooldown() { _timer = CoolTime; }

        // Gây damage vùng lên tất cả quái trong tầm.
        public void Active()
        {
            if (_detect == null || _attack == null) { return; }
            double range = _ownerObject.Stat[Enum_StatType.AttackRange] + 5.0;
            if (!_detect.TryGetTargets(range, out List<CharacterObject> targets)) { return; }
            targets.Sort((a, b) => a.Position.x.CompareTo(b.Position.x));
            int count = Mathf.Min(MaxTargets, targets.Count);
            for (int i = 0; i < count; i++)
            {
                _attack.Attack(targets[i], Enum_DamageType.Skill, SkillMultiplier);
            }
        }
    }
}
