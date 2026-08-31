using UnityEngine;

namespace IdleBattle
{
    public class MovementAbility : CharacterAbility
    {
        private Transform _model;

        public override void Init()
        {
            base.Init();
            _model = _ownerObject.Model != null ? _ownerObject.Model : transform;
        }

        private float Speed { get { return (float)_ownerObject.Stat[Enum_StatType.MoveSpeed]; } }

        public void MoveToRight(float deltaTime)
        {
            transform.Translate(deltaTime * Speed * _model.right, Space.World);
        }

        public void MoveToLeft(float deltaTime)
        {
            transform.Translate(-deltaTime * Speed * _model.right, Space.World);
        }

        public void MoveTargetDirection(float speed, float deltaTime, Vector3 dir)
        {
            transform.Translate(deltaTime * speed * dir, Space.World);
        }

        public void SetPosition(Vector3 p) { transform.position = p; }
    }
}
