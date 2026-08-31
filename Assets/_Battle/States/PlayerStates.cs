using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    // ===== Helpers =====
    internal static class PlayerAnim
    {
        public const string Prefix = "berserker_";
        public static string Resolve(AnimationAbility anim, bool berserk, string baseName)
        {
            if (berserk)
            {
                string b = Prefix + baseName;
                if (anim.IsAnimationExist(b)) { return b; }
            }
            return baseName;
        }
    }

    // ===== Init (state chờ) =====
    public class PlayerInitState : NormalState
    {
        public PlayerInitState(CharacterObject o, StateMachine sm) : base(o, sm) { }
        public override void Enter() { }
        public override void LogicUpdate(float dt) { }
    }

    // ===== Idle =====
    public class BerserkerIdleState : NormalState
    {
        private AnimationAbility _anim;
        private MonsterDetectAbility _detect;
        private BerserkAbility _berserk;
        private PlayerSkillAbility _skill;
        private FSMAbility _fsm;
        private BerserkerObject _b;

        public BerserkerIdleState(CharacterObject o, StateMachine sm) : base(o, sm) { }

        public override void Init()
        {
            base.Init();
            _anim = _owner.GetAbility<AnimationAbility>();
            _detect = _owner.GetAbility<MonsterDetectAbility>();
            _berserk = _owner.GetAbility<BerserkAbility>();
            _skill = _owner.GetAbility<PlayerSkillAbility>();
            _fsm = _owner.GetAbility<FSMAbility>();
            _b = _owner as BerserkerObject;
        }

        public override void Enter()
        {
            _anim.PlayAnimation(PlayerAnim.Resolve(_anim, _b.IsBerserkMode, "idle"), true);
        }

        public override void LogicUpdate(float dt) { TryExit(); }

        private bool TryExit()
        {
            if (BattleManager.Instance.State == Enum_BattleState.Ready) { return false; }
            if (_berserk != null && _berserk.IsEnableAutoBerserk()) { _b.SetBerserkState(true); return true; }
            if (_skill != null && _skill.IsSkillReady && _skill.HasTarget()) { _fsm.ChangeState(Enum_BerserkStateType.Skill); return true; }
            if (_detect.Detect(_owner.Stat[Enum_StatType.AttackRange])) { _fsm.ChangeState(Enum_BerserkStateType.Attack); return true; }
            _fsm.ChangeState(Enum_BerserkStateType.Run);
            return true;
        }
    }

    // ===== Run =====
    public class BerserkerRunState : NormalState
    {
        private AnimationAbility _anim;
        private MovementAbility _move;
        private MonsterDetectAbility _detect;
        private BerserkAbility _berserk;
        private PlayerSkillAbility _skill;
        private DashAbility _dash;
        private FSMAbility _fsm;
        private BerserkerObject _b;

        public BerserkerRunState(CharacterObject o, StateMachine sm) : base(o, sm) { }

        public override void Init()
        {
            base.Init();
            _anim = _owner.GetAbility<AnimationAbility>();
            _move = _owner.GetAbility<MovementAbility>();
            _detect = _owner.GetAbility<MonsterDetectAbility>();
            _berserk = _owner.GetAbility<BerserkAbility>();
            _skill = _owner.GetAbility<PlayerSkillAbility>();
            _dash = _owner.GetAbility<DashAbility>();
            _fsm = _owner.GetAbility<FSMAbility>();
            _b = _owner as BerserkerObject;
        }

        public override void Enter()
        {
            _anim.PlayAnimation(PlayerAnim.Resolve(_anim, _b.IsBerserkMode, "run"), true);
        }

        public override void LogicUpdate(float dt)
        {
            if (TryExit()) { return; }
            _move.MoveToRight(dt);
        }

        private bool TryExit()
        {
            if (BattleManager.Instance.State == Enum_BattleState.Ready) { _fsm.ChangeState(Enum_BerserkStateType.Idle); return true; }
            if (_berserk != null && _berserk.IsEnableAutoBerserk()) { _b.SetBerserkState(true); return true; }
            if (_skill != null && _skill.IsSkillReady && _skill.HasTarget()) { _fsm.ChangeState(Enum_BerserkStateType.Skill); return true; }
            if (_detect.Detect(_owner.Stat[Enum_StatType.AttackRange])) { _fsm.ChangeState(Enum_BerserkStateType.Attack); return true; }
            // enemy kế tiếp ngoài attack range nhưng trong detect range + dash sẵn sàng -> Dash
            if (_dash != null && _dash.IsDashReady && _detect.Detect(_owner.Stat[Enum_StatType.DetectRange]))
            {
                _fsm.ChangeState(Enum_BerserkStateType.Dash);
                return true;
            }
            return false;
        }
    }

    // ===== Dash (lướt tới enemy, có hồi chiêu) =====
    public class BerserkerDashState : CoroutineState
    {
        private AnimationAbility _anim;
        private MonsterDetectAbility _detect;
        private MovementAbility _move;
        private DashAbility _dash;
        private FSMAbility _fsm;
        private BerserkerObject _b;

        public BerserkerDashState(CharacterObject o, StateMachine sm) : base(o, sm) { }

        public override void Init()
        {
            base.Init();
            _anim = _owner.GetAbility<AnimationAbility>();
            _detect = _owner.GetAbility<MonsterDetectAbility>();
            _move = _owner.GetAbility<MovementAbility>();
            _dash = _owner.GetAbility<DashAbility>();
            _fsm = _owner.GetAbility<FSMAbility>();
            _b = _owner as BerserkerObject;
        }

        public override IEnumerator Enter_Coroutine()
        {
            double detectRange = _owner.Stat[Enum_StatType.DetectRange] + 2.0;
            if (!_detect.TryGetTargets(detectRange, out List<CharacterObject> targets) || targets.Count == 0)
            {
                _fsm.ChangeState(Enum_BerserkStateType.Run);
                yield break;
            }
            float ox = _owner.Position.x;
            targets.Sort((a, b) => Mathf.Abs(a.Position.x - ox).CompareTo(Mathf.Abs(b.Position.x - ox)));
            CharacterObject target = targets[0];

            _dash.StartCooldown();
            _dash.PlayDashVfx(_owner);

            string animName = PlayerAnim.Resolve(_anim, _b.IsBerserkMode, "dash");
            if (!_anim.IsAnimationExist(animName)) { animName = PlayerAnim.Resolve(_anim, _b.IsBerserkMode, "run"); }
            _anim.PlayAnimation(animName, false);

            float atk = (float)_owner.Stat[Enum_StatType.AttackRange];
            Vector3 start = _owner.transform.position;
            float dir = target.Position.x >= start.x ? 1f : -1f;
            if (_anim.Animation != null) { _anim.Skeleton.ScaleX = dir >= 0f ? 1f : -1f; }
            Vector3 end = new Vector3(target.PositionCenter.x - dir * atk * 0.7f, start.y, start.z);

            float dashTime = Mathf.Max(0.05f, _dash.DashTimeValue);
            float e = 0f;
            while (e < dashTime)
            {
                e += Time.deltaTime;
                _owner.transform.position = Vector3.Lerp(start, end, e / dashTime);
                if (target == null || target.IsDeath) { break; }
                yield return null;
            }
            if (_anim.Animation != null) { _anim.Skeleton.ScaleX = 1f; }

            if (_owner.IsDeath) { _fsm.ChangeState(Enum_BerserkStateType.Death); yield break; }
            if (_detect.Detect(_owner.Stat[Enum_StatType.AttackRange])) { _fsm.ChangeState(Enum_BerserkStateType.Attack); }
            else { _fsm.ChangeState(Enum_BerserkStateType.Run); }
        }
    }

    // ===== Attack (đánh thường, damage nổ ở Spine event "attack" hoặc fallback timing) =====
    public class BerserkerAttackState : CoroutineState
    {
        private AnimationAbility _anim;
        private MonsterDetectAbility _detect;
        private BerserkerAttackAbility _attack;
        private PlayerSkillAbility _skill;
        private FSMAbility _fsm;
        private BerserkerObject _b;
        private ComboAbility _combo;
        private CombatFeedback _feedback;
        private string _animName;
        private bool _hit;

        private static readonly string[] Combo = { "attack_A", "attack_B", "attack_C", "attack_D" };

        public BerserkerAttackState(CharacterObject o, StateMachine sm) : base(o, sm) { }

        public override void Init()
        {
            base.Init();
            _anim = _owner.GetAbility<AnimationAbility>();
            _detect = _owner.GetAbility<MonsterDetectAbility>();
            _attack = _owner.GetAbility<BerserkerAttackAbility>();
            _skill = _owner.GetAbility<PlayerSkillAbility>();
            _fsm = _owner.GetAbility<FSMAbility>();
            _combo = _owner.GetAbility<ComboAbility>();
            _feedback = _owner.GetAbility<CombatFeedback>();
            _b = _owner as BerserkerObject;
        }

        public override IEnumerator Enter_Coroutine()
        {
            double atkRange = _owner.Stat[Enum_StatType.AttackRange];
            if (!_detect.Detect(atkRange)) { _fsm.ChangeState(Enum_BerserkStateType.Run); yield break; }

            string baseAtk = _combo != null ? _combo.NextAttack() : Combo[Random.Range(0, Combo.Length)];
            string animName = PlayerAnim.Resolve(_anim, _b.IsBerserkMode, baseAtk);
            _animName = animName;
            if (_feedback != null) { _feedback.OnAttackStart(animName); }

            float atkSpeed = _attack.GetAttackSpeed();
            float duration;
            if (!_anim.TryGetAnimation(animName, atkSpeed, out duration) || duration <= 0f) { duration = 0.6f; }

            _hit = false;
            bool useEvent = _anim.HasEvent("attack");
            if (useEvent) { _anim.AnimationState.Event += OnSpineEvent; }

            _anim.PlayAnimation(animName, false, atkSpeed);

            if (useEvent)
            {
                yield return new WaitForSeconds(duration);
                _anim.AnimationState.Event -= OnSpineEvent;
                if (!_hit) { DoDamage(); }
            }
            else
            {
                yield return new WaitForSeconds(duration * 0.4f);
                DoDamage();
                yield return new WaitForSeconds(duration * 0.6f);
            }

            if (_owner.IsDeath) { _fsm.ChangeState(Enum_BerserkStateType.Death); yield break; }
            if (BattleManager.Instance.State == Enum_BattleState.Ready) { _fsm.ChangeState(Enum_BerserkStateType.Idle); yield break; }
            if (_skill != null && _skill.IsSkillReady && _skill.HasTarget()) { _fsm.ChangeState(Enum_BerserkStateType.Skill); yield break; }
            if (_detect.Detect(atkRange)) { _fsm.ChangeState(Enum_BerserkStateType.Attack); yield break; }
            _fsm.ChangeState(Enum_BerserkStateType.Run);
        }

        private void OnSpineEvent(Spine.TrackEntry entry, Spine.Event e)
        {
            if (e.Data.Name == "attack") { DoDamage(); }
        }

        private void DoDamage()
        {
            _hit = true;
            double range = _owner.Stat[Enum_StatType.AttackRange] + 5.0;
            if (_detect.TryGetTargets(range, out List<CharacterObject> targets))
            {
                targets.Sort((a, b) => a.Position.x.CompareTo(b.Position.x));
                AttackPreset ap = _feedback != null ? _feedback.GetForAnim(_animName) : null;
                int maxTargets = (ap != null && ap.AttackCount > 0) ? ap.AttackCount : 5;
                int count = Mathf.Min(maxTargets, targets.Count);
                Damage primary = default(Damage);
                for (int i = 0; i < count; i++)
                {
                    Damage d = _attack.Attack(targets[i]);
                    if (i == 0) { primary = d; }
                }
                if (_feedback != null && count > 0)
                {
                    _feedback.OnPlayerHit(primary, _combo != null ? _combo.ComboCount : 0, _animName);
                }
            }
            _owner.OnAttack();
        }
    }

    // ===== Skill (AoE) =====
    public class BerserkerSkillState : CoroutineState
    {
        private AnimationAbility _anim;
        private MonsterDetectAbility _detect;
        private BerserkerAttackAbility _attack;
        private PlayerSkillAbility _skill;
        private FSMAbility _fsm;
        private BerserkerObject _b;
        private CombatFeedback _feedback;

        public BerserkerSkillState(CharacterObject o, StateMachine sm) : base(o, sm) { }

        public override void Init()
        {
            base.Init();
            _anim = _owner.GetAbility<AnimationAbility>();
            _detect = _owner.GetAbility<MonsterDetectAbility>();
            _attack = _owner.GetAbility<BerserkerAttackAbility>();
            _skill = _owner.GetAbility<PlayerSkillAbility>();
            _fsm = _owner.GetAbility<FSMAbility>();
            _b = _owner as BerserkerObject;
            _feedback = _owner.GetAbility<CombatFeedback>();
        }

        public override IEnumerator Enter_Coroutine()
        {
            _skill.ResetCooldown();
            float atkSpeed = _attack.GetAttackSpeed();
            string animName = PlayerAnim.Resolve(_anim, _b.IsBerserkMode, "attack_C");
            float duration;
            if (!_anim.TryGetAnimation(animName, atkSpeed, out duration) || duration <= 0f) { duration = 0.8f; }

            _anim.PlayAnimation(animName, false, atkSpeed);
            yield return new WaitForSeconds(duration * 0.45f);
            if (_feedback != null) { _feedback.OnSkill(); }
            _skill.Active();
            yield return new WaitForSeconds(duration * 0.55f);

            if (_owner.IsDeath) { _fsm.ChangeState(Enum_BerserkStateType.Death); yield break; }
            _fsm.ChangeState(Enum_BerserkStateType.Idle);
        }
    }

    // ===== Berserk (biến hình + shockwave) =====
    public class BerserkState : CoroutineState
    {
        private AnimationAbility _anim;
        private MonsterDetectAbility _detect;
        private FSMAbility _fsm;

        public BerserkState(CharacterObject o, StateMachine sm) : base(o, sm) { }

        public override void Init()
        {
            base.Init();
            _anim = _owner.GetAbility<AnimationAbility>();
            _detect = _owner.GetAbility<MonsterDetectAbility>();
            _fsm = _owner.GetAbility<FSMAbility>();
        }

        public override IEnumerator Enter_Coroutine()
        {
            string animName = _anim.IsAnimationExist("berserker") ? "berserker" : "idle";
            float duration = _anim.GetDuration(animName);
            if (duration <= 0f) { duration = 0.5f; }

            _anim.PlayAnimation(animName, false);
            yield return new WaitForSeconds(duration * 0.4f);
            Shockwave();
            yield return new WaitForSeconds(duration * 0.6f);

            if (_detect.Detect(_owner.Stat[Enum_StatType.AttackRange])) { _fsm.ChangeState(Enum_BerserkStateType.Attack); }
            else { _fsm.ChangeState(Enum_BerserkStateType.Run); }
        }

        private void Shockwave()
        {
            double range = _owner.Stat[Enum_StatType.DetectRange];
            if (!_detect.TryGetTargets(range, out List<CharacterObject> targets)) { return; }
            double shock = _owner.Stat[Enum_StatType.BerserkShockWave];
            if (shock <= 0) { shock = 3; }
            double value = _owner.Stat[Enum_StatType.Damage] * shock;
            Damage dmg = new Damage
            {
                Value = value,
                OriginValue = value,
                DamageType = Enum_DamageType.BerserkNormal,
                PlayerState = Enum_PlayerState.Berserk
            };
            for (int i = 0; i < targets.Count; i++) { targets[i].TryTakeHit(dmg, _owner); }
        }
    }

    // ===== Death =====
    public class PlayerDeathState : CoroutineState
    {
        private AnimationAbility _anim;
        private BerserkerObject _b;

        public PlayerDeathState(CharacterObject o, StateMachine sm) : base(o, sm) { }

        public override void Init()
        {
            base.Init();
            _anim = _owner.GetAbility<AnimationAbility>();
            _b = _owner as BerserkerObject;
        }

        public override IEnumerator Enter_Coroutine()
        {
            _owner.SetAlive(false);
            string animName = PlayerAnim.Resolve(_anim, _b.IsBerserkMode, "death");
            float duration = _anim.GetDuration(animName);
            if (duration <= 0f) { duration = 1f; }
            _anim.PlayAnimation(animName, false);
            yield return new WaitForSeconds(duration + 0.2f);
            _owner.Kill();
            if (BattleManager.Instance != null) { BattleManager.Instance.PlayerDeath(); }
        }
    }
}
