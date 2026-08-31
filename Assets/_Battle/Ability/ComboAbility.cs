using UnityEngine;

namespace IdleBattle
{
    // Combo theo GIỚI HẠN THỜI GIAN (không phải tuần tự cứng):
    // đánh liên tục trong ComboWindow -> chuỗi A→B→C→D tiến lên + tăng damage.
    // Ngưng đánh quá ComboWindow -> combo reset về đòn đầu.
    public class ComboAbility : CharacterAbility
    {
        public float ComboWindow = 1.2f;      // giây; quá thì reset
        public float DamagePerCombo = 0.05f;  // +5% damage mỗi bậc combo
        public int MaxComboForDamage = 20;

        private static readonly string[] Chain = { "attack_A", "attack_B", "attack_C", "attack_D" };

        private int _combo;
        private float _lastAttackTime = -999f;

        public int ComboCount { get { return _combo; } }
        public float DamageMultiplier { get { return 1f + Mathf.Min(_combo, MaxComboForDamage) * DamagePerCombo; } }

        // Gọi khi BẮT ĐẦU 1 đòn -> quyết định anim theo combo hiện tại.
        public string NextAttack()
        {
            float now = Time.time;
            if (now - _lastAttackTime > ComboWindow) { _combo = 0; }  // ngoài cửa sổ -> reset
            else { _combo++; }
            _lastAttackTime = now;
            return Chain[_combo % Chain.Length];
        }

        public override void ProcessAbility(float deltaTime)
        {
            // reset khi hết cửa sổ (để UI cập nhật đúng dù không đánh nữa)
            if (_combo > 0 && Time.time - _lastAttackTime > ComboWindow) { _combo = 0; }
        }
    }
}
