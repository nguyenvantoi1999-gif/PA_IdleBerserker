using UnityEngine;

namespace IdleBattle
{
    public class StateMachine : MonoBehaviour
    {
        private State _currentState;
        private Coroutine _enterCoroutine;
        private bool _useLogicUpdate;

        public State CurrentState { get { return _currentState; } }

        public void Initialize(State startingState)
        {
            _currentState = startingState;
            if (!startingState.Inited)
            {
                startingState.Init();
            }

            if (startingState is CoroutineState coroutineState && gameObject.activeInHierarchy)
            {
                _useLogicUpdate = false;
                _enterCoroutine = StartCoroutine(coroutineState.Enter_Coroutine());
            }
            else
            {
                _useLogicUpdate = true;
                startingState.Enter();
            }
        }

        public void ChangeState(State newState)
        {
            if (_enterCoroutine != null)
            {
                StopCoroutine(_enterCoroutine);
                _enterCoroutine = null;
            }
            if (_currentState != null)
            {
                _currentState.Exit();
            }
            Initialize(newState);
        }

        public void LogicUpdate(float deltaTime)
        {
            if (_useLogicUpdate && _currentState != null)
            {
                _currentState.LogicUpdate(deltaTime);
            }
        }
    }
}
