using UnityEngine;

namespace IdleBattle
{
    // Quái vực thẳm: lao theo vector tới player (tốc độ tăng dần theo Curve),
    // tới tầm thì dùng attack state bình thường.
    public class AbyssMonsterObject : MonsterObject
    {
        public AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 0.25f, 1f, 1f);

        private float _baseDistance = 10f;

        public override void InitStat()
        {
            MonsterType = Enum_MonsterType.AbyssMonster;
            Stat[Enum_StatType.MoveSpeed] = 7;
            Stat[Enum_StatType.AttackRange] = 8;
            Stat[Enum_StatType.DetectRange] = 8;
            if (Stat[Enum_StatType.AttackSpeed] <= 0) { Stat[Enum_StatType.AttackSpeed] = 1; }
        }

        public void SetStat(double damage, double health)
        {
            Stat[Enum_StatType.Damage] = damage;
            Stat[Enum_StatType.Health] = health;
            _currentHealth = health;
        }

        public void InitDistance(float distance)
        {
            _baseDistance = Mathf.Max(0.01f, distance);
        }

        public float GetSpeed(float distance)
        {
            float time = 1f - (Mathf.Abs(distance) / _baseDistance);
            return Curve.Evaluate(Mathf.Clamp01(time)) * (float)Stat[Enum_StatType.MoveSpeed];
        }

        public override void InitFSM()
        {
            _fsmAbility = GetAbility<FSMAbility>();
            StateMachine sm = _fsmAbility.StateMachine;

            _fsmAbility.Register(Enum_MonsterStateType.Init, new MonsterInitState(this, sm));
            _fsmAbility.Register(Enum_MonsterStateType.Idle, new MonsterIdleState(this, sm));
            _fsmAbility.Register(Enum_MonsterStateType.Run, new AbyssMonsterRunState(this, sm));
            _fsmAbility.Register(Enum_MonsterStateType.Attack, new MonsterAttackState(this, sm));
            _fsmAbility.Register(Enum_MonsterStateType.Hit, new MonsterHitState(this, sm));
            _fsmAbility.Register(Enum_MonsterStateType.Death, new AbyssMonsterDeathState(this, sm));

            _fsmAbility.Initialize(Enum_MonsterStateType.Idle);
        }
    }
}
