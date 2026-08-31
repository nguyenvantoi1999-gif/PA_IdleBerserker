using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    // Bản tối giản thay cho BattleManager khổng lồ của game gốc:
    // chỉ giữ những gì các Ability/State cốt lõi cần.
    public class BattleManager : MonoBehaviour
    {
        private static BattleManager _instance;
        public static BattleManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("BattleManager");
                    _instance = go.AddComponent<BattleManager>();
                }
                return _instance;
            }
        }

        public Enum_BattleState State = Enum_BattleState.Ready;
        public Enum_BattleState BattleState { get { return State; } }
        public bool IsEventWorld() { return false; }
        public int KillCount;
        public System.Action OnMonsterKilled;
        public PlayerObject PlayerObject;

        private readonly List<MonsterObject> _monsters = new List<MonsterObject>();

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
        }

        private void OnDestroy()
        {
            if (_instance == this) { _instance = null; }
        }

        public void RegisterPlayer(PlayerObject player) { PlayerObject = player; }
        public void AddMonster(MonsterObject m) { if (!_monsters.Contains(m)) { _monsters.Add(m); } }
        public void RemoveMonster(MonsterObject m) { _monsters.Remove(m); }

        // Gọi khi quái chết (không tính boss) -> đếm kill + báo event.
        public void ReportKill(MonsterObject m)
        {
            if (m != null && !m.IsBoss) { KillCount++; }
            if (OnMonsterKilled != null) { OnMonsterKilled(); }
            RemoveMonster(m);
        }
        public List<MonsterObject> GetMonsters() { return _monsters; }
        public int AliveMonsterCount()
        {
            int c = 0;
            for (int i = 0; i < _monsters.Count; i++)
            {
                if (_monsters[i] != null && !_monsters[i].IsDeath) { c++; }
            }
            return c;
        }

        public virtual void PlayerDeath() { }
    }
}
