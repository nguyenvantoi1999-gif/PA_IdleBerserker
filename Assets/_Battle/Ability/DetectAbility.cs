using System.Collections.Generic;

namespace IdleBattle
{
    public abstract class DetectAbility : CharacterAbility
    {
        public virtual bool Detect(double range) { return false; }
        public virtual bool TryGetTargets(double range, out List<CharacterObject> targets)
        {
            targets = new List<CharacterObject>();
            return false;
        }
        public virtual float GetDistance() { return 0f; }
    }
}
