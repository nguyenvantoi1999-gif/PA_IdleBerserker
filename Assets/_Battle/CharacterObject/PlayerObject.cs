using UnityEngine;

namespace IdleBattle
{
    public class PlayerObject : BerserkerObject
    {
        public void Initialize(Stat stat)
        {
            if (stat != null) { Stat = stat.Copy(); }
            BattleManager.Instance.RegisterPlayer(this);
            Init();
        }

        public override void InitFSM()
        {
            _fsmAbility = GetAbility<FSMAbility>();
            StateMachine sm = _fsmAbility.StateMachine;

            _fsmAbility.Register(Enum_BerserkStateType.Init, new PlayerInitState(this, sm));
            _fsmAbility.Register(Enum_BerserkStateType.Idle, new BerserkerIdleState(this, sm));
            _fsmAbility.Register(Enum_BerserkStateType.Run, new BerserkerRunState(this, sm));
            _fsmAbility.Register(Enum_BerserkStateType.Attack, new BerserkerAttackState(this, sm));
            _fsmAbility.Register(Enum_BerserkStateType.Dash, new BerserkerDashState(this, sm));
            _fsmAbility.Register(Enum_BerserkStateType.Skill, new BerserkerSkillState(this, sm));
            _fsmAbility.Register(Enum_BerserkStateType.Berserk, new BerserkState(this, sm));
            _fsmAbility.Register(Enum_BerserkStateType.Death, new PlayerDeathState(this, sm));

            _fsmAbility.Initialize(Enum_BerserkStateType.Init);
        }
    }
}
