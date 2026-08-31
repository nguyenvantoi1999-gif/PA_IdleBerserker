using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    // Gắn trên PLAYER: dò quái từ BattleManager theo trục X.
    public class MonsterDetectAbility : DetectAbility
    {
        public override bool Detect(double range)
        {
            List<MonsterObject> ms = BattleManager.Instance.GetMonsters();
            for (int i = 0; i < ms.Count; i++)
            {
                MonsterObject m = ms[i];
                if (m == null || m.IsDeath) { continue; }
                if (Mathf.Abs(_ownerObject.Position.x - m.PositionCenter.x) <= range) { return true; }
            }
            return false;
        }

        public override bool TryGetTargets(double range, out List<CharacterObject> targets)
        {
            targets = new List<CharacterObject>();
            List<MonsterObject> ms = BattleManager.Instance.GetMonsters();
            for (int i = 0; i < ms.Count; i++)
            {
                MonsterObject m = ms[i];
                if (m == null || m.IsDeath) { continue; }
                if (Mathf.Abs(_ownerObject.Position.x - m.PositionCenter.x) <= range) { targets.Add(m); }
            }
            return targets.Count > 0;
        }
    }
}
