using System.Collections;

namespace IdleBattle
{
    // Base state (giữ nguyên kiến trúc gốc).
    public abstract class State
    {
        protected CharacterObject _owner;
        protected StateMachine _stateMachine;
        private bool _inited;

        public bool Inited { get { return _inited; } }

        protected State(CharacterObject owner, StateMachine stateMachine)
        {
            _owner = owner;
            _stateMachine = stateMachine;
        }

        public virtual void Init() { _inited = true; }
        public abstract void Enter();
        public virtual void LogicUpdate(float deltaTime) { }
        public virtual void Exit() { }
    }

    // State đồng bộ: Enter chạy 1 lần, LogicUpdate mỗi frame.
    public abstract class NormalState : State
    {
        protected NormalState(CharacterObject owner, StateMachine sm) : base(owner, sm) { }
    }

    // State chạy tuần tự bằng coroutine.
    public abstract class CoroutineState : State
    {
        protected CoroutineState(CharacterObject owner, StateMachine sm) : base(owner, sm) { }
        public override void Enter() { }
        public override void Exit() { }
        public abstract IEnumerator Enter_Coroutine();
    }
}
