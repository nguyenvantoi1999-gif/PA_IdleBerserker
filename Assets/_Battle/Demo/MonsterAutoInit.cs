using UnityEngine;

namespace IdleBattle
{
    // Cho phép kéo-thả prefab quái/abyss/boss vào scene và tự khởi tạo.
    // Nếu gán Variant (EnemyVariant/BossVariant SO) -> lấy hình ảnh + chỉ số từ asset (ưu tiên).
    // Bỏ trống Variant -> dùng field inline (tương thích ngược).
    [RequireComponent(typeof(MonsterObject))]
    public class MonsterAutoInit : MonoBehaviour
    {
        [Header("Biến thể (tùy chọn, ưu tiên nếu gán)")]
        public CharacterVariant Variant;

        [Header("Chỉ số (dùng khi không gán Variant)")]
        public float Damage = 6f;
        public float Health = 60f;
        public float MoveSpeed = 7f;
        public float AttackRange = 8f;
        public float DetectRange = 8f;
        public float AttackSpeed = 1f;

        [Header("Boss (dùng khi không gán Variant)")]
        public bool CreateBossBar = false;
        public string BossName = "RAID BOSS";

        private void Start()
        {
            BattleManager bm = BattleManager.Instance;
            MonsterObject m = GetComponent<MonsterObject>();

            float dmg = Damage, hp = Health, spd = MoveSpeed;
            float atkRange = AttackRange, detect = DetectRange, atkSpd = AttackSpeed;
            bool makeBossBar = CreateBossBar;
            string bossName = BossName;

            if (Variant != null)
            {
                Variant.ApplyVisual(m.Model);
                dmg = Variant.Damage; hp = Variant.Health; spd = Variant.MoveSpeed;
                atkRange = Variant.AttackRange; detect = Variant.DetectRange; atkSpd = Variant.AttackSpeed;
                BossVariant boss = Variant as BossVariant;
                if (boss != null) { makeBossBar = true; bossName = boss.BossName; }
            }

            AbyssMonsterObject abyss = m as AbyssMonsterObject;
            if (abyss != null)
            {
                abyss.SetStat(dmg, hp);
                abyss.InitCharacter();
                abyss.InitDistance(12f);
            }
            else
            {
                m.SetStats(dmg, hp, spd, atkRange, detect, atkSpd);
                m.InitCharacter();
            }

            if (makeBossBar && Camera.main != null)
            {
                BossHealthBar.Create(Camera.main, m, bossName);
            }
        }
    }
}
