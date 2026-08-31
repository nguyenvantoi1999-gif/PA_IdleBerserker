using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    // Gắn trên MONSTER: dò player theo trục X.
    public class PlayerDetectAbility : DetectAbility
    {
        private PlayerObject _player;

        private PlayerObject Player
        {
            get
            {
                if (_player == null && BattleManager.Instance != null)
                {
                    _player = BattleManager.Instance.PlayerObject;
                }
                return _player;
            }
        }

        public override bool Detect(double range)
        {
            PlayerObject p = Player;
            if (p == null || p.IsDeath) { return false; }
            return Mathf.Abs(p.Position.x - _ownerObject.Position.x) <= range;
        }

        public override bool TryGetTargets(double range, out List<CharacterObject> targets)
        {
            targets = new List<CharacterObject>();
            if (Detect(range)) { targets.Add(Player); return true; }
            return false;
        }

        public override float GetDistance()
        {
            PlayerObject p = Player;
            if (p == null) { return 0f; }
            return _ownerObject.Position.x - p.Position.x;
        }
    }
}
