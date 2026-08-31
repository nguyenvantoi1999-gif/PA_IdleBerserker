using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    public class FSMAbility : CharacterAbility
    {
        private StateMachine _stateMachine;
        private readonly Dictionary<int, State> _states = new Dictionary<int, State>();

        public StateMachine StateMachine { get { return _stateMachine; } }

        public override void Init()
        {
            base.Init();
            _stateMachine = GetComponent<StateMachine>();
            if (_stateMachine == null)
            {
                _stateMachine = gameObject.AddComponent<StateMachine>();
            }
        }

        public void Register(Enum_MonsterStateType type, State state) { _states[(int)type] = state; }
        public void Register(Enum_BerserkStateType type, State state) { _states[(int)type] = state; }

        public void Initialize(Enum_MonsterStateType type) { _stateMachine.Initialize(_states[(int)type]); }
        public void Initialize(Enum_BerserkStateType type) { _stateMachine.Initialize(_states[(int)type]); }

        public void ChangeState(Enum_MonsterStateType type) { _stateMachine.ChangeState(_states[(int)type]); }
        public void ChangeState(Enum_BerserkStateType type) { _stateMachine.ChangeState(_states[(int)type]); }

        public bool CurrentStateEquals(Enum_MonsterStateType type)
        {
            return _states.TryGetValue((int)type, out State s) && _stateMachine.CurrentState == s;
        }
        public bool CurrentStateEquals(Enum_BerserkStateType type)
        {
            return _states.TryGetValue((int)type, out State s) && _stateMachine.CurrentState == s;
        }

        public override void ProcessAbility(float deltaTime)
        {
            _stateMachine.LogicUpdate(deltaTime);
        }
    }
}
