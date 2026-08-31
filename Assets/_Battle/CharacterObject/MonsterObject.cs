using UnityEngine;

namespace IdleBattle
{
    public class MonsterObject : CharacterObject
    {
        public Enum_MonsterType MonsterType = Enum_MonsterType.StageMonster;
        public bool IsBoss;

        public override void InitFSM()
        {
            _fsmAbility = GetAbility<FSMAbility>();
            StateMachine sm = _fsmAbility.StateMachine;

            _fsmAbility.Register(Enum_MonsterStateType.Init, new MonsterInitState(this, sm));
            _fsmAbility.Register(Enum_MonsterStateType.Idle, new MonsterIdleState(this, sm));
            _fsmAbility.Register(Enum_MonsterStateType.Run, new MonsterRunState(this, sm));
            _fsmAbility.Register(Enum_MonsterStateType.Attack, new MonsterAttackState(this, sm));
            _fsmAbility.Register(Enum_MonsterStateType.Hit, new MonsterHitState(this, sm));
            _fsmAbility.Register(Enum_MonsterStateType.Death, new MonsterDeathState(this, sm));

            _fsmAbility.Initialize(Enum_MonsterStateType.Idle);
        }

        public virtual void InitStat() { }

        public void SetStats(double damage, double health, double moveSpeed, double attackRange, double detectRange, double attackSpeed)
        {
            Stat[Enum_StatType.Damage] = damage;
            Stat[Enum_StatType.Health] = health;
            Stat[Enum_StatType.MoveSpeed] = moveSpeed;
            Stat[Enum_StatType.AttackRange] = attackRange;
            Stat[Enum_StatType.DetectRange] = detectRange;
            Stat[Enum_StatType.AttackSpeed] = attackSpeed;
            _currentHealth = health;
        }

        // Gọi sau khi Model (SkeletonAnimation) đã sẵn sàng trên child.
        public virtual void InitCharacter()
        {
            InitStat();
            InitAbilities();
            InitFSM();
            _alive = true;
            FacePlayer();
            if (BattleManager.Instance != null) { BattleManager.Instance.AddMonster(this); }
            _fsmAbility.ChangeState(Enum_MonsterStateType.Idle);
        }

        // Art quái mặc định (ScaleX = +1) quay TRÁI; berserker quay PHẢI.
        // Hướng mặt về phía player: player bên trái -> +1, player bên phải -> -1.
        public void FacePlayer()
        {
            AnimationAbility anim = GetAbility<AnimationAbility>();
            if (anim == null || anim.Animation == null) { return; }
            PlayerObject player = BattleManager.Instance != null ? BattleManager.Instance.PlayerObject : null;
            bool playerLeft = player == null || player.Position.x <= Position.x;
            anim.SetScaleX(playerLeft ? 1f : -1f);
        }

        protected override void OnTakeHit(Damage damage, CharacterObject from)
        {
            KnockbackAbility kb = GetAbility<KnockbackAbility>();
            if (kb != null) { kb.Knockback(damage.DamageType); }
            // KHÔNG chuyển sang Hit state nữa (tránh cắt ngang đòn đánh).
            // Phản ứng trúng đòn do HitReactionAbility xử lý bằng cách hoà trộn anim ở track 1.
        }

        protected override void OnDeath()
        {
            _fsmAbility.ChangeState(Enum_MonsterStateType.Death);
            if (BattleManager.Instance != null) { BattleManager.Instance.ReportKill(this); }
        }

        public float GetHealthRate() { return HealthPercent; }
    }
}
