using System.Collections;
using UnityEngine;

namespace IdleBattle
{
    // ===== Init =====
    public class MonsterInitState : NormalState
    {
        private AnimationAbility _anim;
        public MonsterInitState(CharacterObject o, StateMachine sm) : base(o, sm) { }
        public override void Init() { base.Init(); _anim = _owner.GetAbility<AnimationAbility>(); }
        public override void Enter() { _anim.PlayAnimation(_anim.ResolveName("idle", "Idle"), true); }
    }

    // ===== Idle =====
    public class MonsterIdleState : NormalState
    {
        private AnimationAbility _anim;
        private FSMAbility _fsm;
        public MonsterIdleState(CharacterObject o, StateMachine sm) : base(o, sm) { }
        public override void Init() { base.Init(); _anim = _owner.GetAbility<AnimationAbility>(); _fsm = _owner.GetAbility<FSMAbility>(); }
        public override void Enter() { _anim.PlayAnimation(_anim.ResolveName("idle", "Idle"), true); }
        public override void LogicUpdate(float dt)
        {
            if (BattleManager.Instance.State == Enum_BattleState.Ready) { return; }
            _fsm.ChangeState(Enum_MonsterStateType.Run);
        }
    }

    // ===== Run (đi thẳng trục X về phía player) =====
    public class MonsterRunState : NormalState
    {
        private AnimationAbility _anim;
        private MovementAbility _move;
        private PlayerDetectAbility _detect;
        private FSMAbility _fsm;
        public MonsterRunState(CharacterObject o, StateMachine sm) : base(o, sm) { }
        public override void Init()
        {
            base.Init();
            _anim = _owner.GetAbility<AnimationAbility>();
            _move = _owner.GetAbility<MovementAbility>();
            _detect = _owner.GetAbility<PlayerDetectAbility>();
            _fsm = _owner.GetAbility<FSMAbility>();
        }
        public override void Enter() { ((MonsterObject)_owner).FacePlayer(); _anim.PlayAnimation(_anim.ResolveName("walk", "idle"), true); }
        public override void LogicUpdate(float dt)
        {
            ((MonsterObject)_owner).FacePlayer();
            double attackRange = _owner.Stat[Enum_StatType.AttackRange];
            if (_detect.Detect(attackRange)) { _fsm.ChangeState(Enum_MonsterStateType.Attack); return; }
            _move.MoveToLeft(dt);
        }
    }

    // ===== Attack =====
    public class MonsterAttackState : CoroutineState
    {
        private AnimationAbility _anim;
        private MonsterAttackAbility _attack;
        private PlayerDetectAbility _detect;
        private FSMAbility _fsm;
        private bool _hit;
        public MonsterAttackState(CharacterObject o, StateMachine sm) : base(o, sm) { }
        public override void Init()
        {
            base.Init();
            _anim = _owner.GetAbility<AnimationAbility>();
            _attack = _owner.GetAbility<MonsterAttackAbility>();
            _detect = _owner.GetAbility<PlayerDetectAbility>();
            _fsm = _owner.GetAbility<FSMAbility>();
        }
        public override IEnumerator Enter_Coroutine()
        {
            if (_owner.IsDeath) { yield break; }
            while (!_attack.IsAttackPossible)
            {
                if (_owner.IsDeath) { yield break; }
                yield return null;
            }
            double attackRange = _owner.Stat[Enum_StatType.AttackRange];
            if (!_detect.Detect(attackRange)) { _fsm.ChangeState(Enum_MonsterStateType.Idle); yield break; }

            string animName = _anim.ResolveName("attack", "attack_A", "attack_B");
            float duration = _anim.GetDuration(animName);
            if (duration <= 0f) { duration = 1f; }

            _hit = false;
            bool useEvent = _anim.HasEvent("attack");
            if (useEvent) { _anim.AnimationState.Event += OnSpineEvent; }

            _anim.PlayAnimation(animName, false);

            if (useEvent)
            {
                yield return new WaitForSeconds(duration);
                _anim.AnimationState.Event -= OnSpineEvent;
                if (!_hit && !_owner.IsDeath) { _attack.Attack(); }
            }
            else
            {
                yield return new WaitForSeconds(duration / 3f);
                if (!_owner.IsDeath) { _attack.Attack(); }
                yield return new WaitForSeconds(duration / 3f * 2f);
            }

            if (_owner.IsDeath) { yield break; }
            _fsm.ChangeState(Enum_MonsterStateType.Idle);
        }
        private void OnSpineEvent(Spine.TrackEntry entry, Spine.Event e)
        {
            if (e.Data.Name == "attack" && !_owner.IsDeath) { _hit = true; _attack.Attack(); }
        }
    }

    // ===== Hit (giật lùi) =====
    public class MonsterHitState : CoroutineState
    {
        private AnimationAbility _anim;
        private FSMAbility _fsm;
        public MonsterHitState(CharacterObject o, StateMachine sm) : base(o, sm) { }
        public override void Init() { base.Init(); _anim = _owner.GetAbility<AnimationAbility>(); _fsm = _owner.GetAbility<FSMAbility>(); }
        public override IEnumerator Enter_Coroutine()
        {
            string animName = _anim.ResolveName("knockback", "Knockback", "idle");
            float duration = _anim.GetDuration(animName);
            if (duration <= 0f) { duration = 0.25f; }
            _anim.PlayAnimation(animName, false);
            yield return new WaitForSeconds(duration);
            if (_owner.IsDeath) { yield break; }
            _fsm.ChangeState(Enum_MonsterStateType.Idle);
        }
    }

    // ===== Death =====
    public class MonsterDeathState : CoroutineState
    {
        private AnimationAbility _anim;
        public MonsterDeathState(CharacterObject o, StateMachine sm) : base(o, sm) { }
        public override void Init() { base.Init(); _anim = _owner.GetAbility<AnimationAbility>(); }
        public override IEnumerator Enter_Coroutine()
        {
            _owner.SetAlive(false);
            string animName = _anim.ResolveName("death", "Kill", "knockback");
            float duration = _anim.GetDuration(animName);
            if (duration <= 0f) { duration = 0.6f; }
            _anim.PlayAnimation(animName, false);
            yield return new WaitForSeconds(duration + 0.6f);
            _owner.Kill();
        }
    }

    // ===== Abyss Run (lao theo vector, tăng tốc dần, chuyển sang attack khi tới player) =====
    public class AbyssMonsterRunState : NormalState
    {
        private AnimationAbility _anim;
        private MovementAbility _move;
        private PlayerDetectAbility _detect;
        private FSMAbility _fsm;
        private AbyssMonsterObject _abyss;
        private bool _isDirected;
        private Vector3 _targetDir = Vector3.left;

        public AbyssMonsterRunState(CharacterObject o, StateMachine sm) : base(o, sm) { }

        public override void Init()
        {
            base.Init();
            _anim = _owner.GetAbility<AnimationAbility>();
            _move = _owner.GetAbility<MovementAbility>();
            _detect = _owner.GetAbility<PlayerDetectAbility>();
            _fsm = _owner.GetAbility<FSMAbility>();
            _abyss = _owner as AbyssMonsterObject;
        }

        public override void Enter() { ((MonsterObject)_owner).FacePlayer(); _anim.PlayAnimation(_anim.ResolveName("walk", "idle"), true); }

        public override void LogicUpdate(float dt)
        {
            if (BattleManager.Instance.State == Enum_BattleState.Ready) { return; }
            ((MonsterObject)_owner).FacePlayer();
            PlayerObject player = BattleManager.Instance.PlayerObject;
            if (player == null) { return; }

            if (!_isDirected)
            {
                Vector3 targetPos = new Vector3(player.Position.x, _owner.Position.y, 0f);
                _targetDir = (targetPos - _owner.Position).normalized;
                _targetDir.y = 0f;
                _isDirected = true;
            }

            double attackRange = _owner.Stat[Enum_StatType.AttackRange];
            if (_detect.Detect(attackRange))
            {
                _fsm.ChangeState(Enum_MonsterStateType.Attack);
                return;
            }

            float speed = _abyss.GetSpeed(_detect.GetDistance());
            _move.MoveTargetDirection(speed, dt, _targetDir);
            Vector3 lockPos = _owner.transform.position;
            if (lockPos.y != 0f) { lockPos.y = 0f; _owner.transform.position = lockPos; }
        }
    }

    // ===== Abyss Death (nổ nhanh) =====
    public class AbyssMonsterDeathState : CoroutineState
    {
        private AnimationAbility _anim;
        public AbyssMonsterDeathState(CharacterObject o, StateMachine sm) : base(o, sm) { }
        public override void Init() { base.Init(); _anim = _owner.GetAbility<AnimationAbility>(); }
        public override IEnumerator Enter_Coroutine()
        {
            _owner.SetAlive(false);
            if (_anim.IsAnimationExist("death"))
            {
                float duration = _anim.GetDuration("death");
                if (duration <= 0f) { duration = 0.6f; }
                _anim.PlayAnimation("death", false);
                yield return new WaitForSeconds(duration);
            }
            else
            {
                if (_owner.Model != null) { _owner.Model.gameObject.SetActive(false); }
                yield return new WaitForSeconds(0.4f);
            }
            _owner.Kill();
        }
    }
}
