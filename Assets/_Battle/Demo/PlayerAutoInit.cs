using UnityEngine;

namespace IdleBattle
{
    // Cho phép kéo-thả prefab PlayerObject vào scene và tự khởi tạo.
    // Nếu gán Variant (PlayerVariant SO) -> lấy hình ảnh + chỉ số từ asset đó (ưu tiên).
    // Bỏ trống Variant -> dùng các field inline bên dưới (tương thích ngược).
    [RequireComponent(typeof(PlayerObject))]
    public class PlayerAutoInit : MonoBehaviour
    {
        [Header("Biến thể (tùy chọn, ưu tiên nếu gán)")]
        public PlayerVariant Variant;

        [Header("Chỉ số cơ bản (dùng khi không gán Variant)")]
        public float Damage = 20f;
        public float Health = 100f;
        public float AttackSpeed = 1.4f;
        public float MoveSpeed = 3.2f;
        public float CriticalChance = 0.25f;
        public float CriticalDamage = 1.0f;
        public float BerserkDamage = 1.8f;
        public float BerserkAttackSpeed = 1.5f;
        public float BerserkDuration = 6f;
        public float BerserkShockWave = 5f;
        public float DetectRange = 8f;
        public float AttackRange = 3.0f;

        [Header("Tự bắt đầu trận")]
        public bool AutoStartBattle = false;

        private void Start()
        {
            BattleManager bm = BattleManager.Instance;
            PlayerObject player = GetComponent<PlayerObject>();
            Stat stat = new Stat();

            if (Variant != null)
            {
                Variant.ApplyVisual(player.Model);
                Variant.WriteStats(stat);
            }
            else
            {
                stat[Enum_StatType.Damage] = Damage;
                stat[Enum_StatType.Health] = Health;
                stat[Enum_StatType.AttackSpeed] = AttackSpeed;
                stat[Enum_StatType.MoveSpeed] = MoveSpeed;
                stat[Enum_StatType.CriticalChance] = CriticalChance;
                stat[Enum_StatType.CriticalDamage] = CriticalDamage;
                stat[Enum_StatType.BerserkDamage] = BerserkDamage;
                stat[Enum_StatType.BerserkAttackSpeed] = BerserkAttackSpeed;
                stat[Enum_StatType.BerserkDuration] = BerserkDuration;
                stat[Enum_StatType.BerserkShockWave] = BerserkShockWave;
                stat[Enum_StatType.DetectRange] = DetectRange;
                stat[Enum_StatType.AttackRange] = AttackRange;
            }

            player.Initialize(stat);
            if (AutoStartBattle) { bm.State = Enum_BattleState.Start; }
        }
    }
}
