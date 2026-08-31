using UnityEngine;

namespace IdleBattle
{
    // Base cho mọi Ability (component pattern gốc).
    public abstract class CharacterAbility : MonoBehaviour
    {
        protected CharacterObject _ownerObject;

        public virtual void Init()
        {
            if (_ownerObject == null)
            {
                _ownerObject = GetComponent<CharacterObject>();
            }
        }

        public virtual void LateInit() { }
        public virtual void ProcessAbility(float deltaTime) { }
    }
}
