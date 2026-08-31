using UnityEngine;

namespace IdleBattle
{
    [CreateAssetMenu(fileName = "EnemyVariant", menuName = "IdleBattle/Enemy Variant")]
    public class EnemyVariant : CharacterVariant
    {
        [Header("Hành vi quái")]
        [Tooltip("true = quái Abyss (lao vào rồi tự nổ khi tới player). false = quái thường đứng đánh")]
        public bool IsAbyss = true;

        [Tooltip("Sát thương theo % máu player mỗi đòn. > 0 sẽ ghi đè Damage tuyệt đối.")]
        public float DamagePercentOfPlayerHp = 0.06f;

        [Header("Ngẫu nhiên (tùy chọn)")]
        [Tooltip("Dao động scale ± giá trị này để cùng 1 biến thể trông đa dạng. 0 = tắt")]
        public float ScaleJitter = 0f;
    }
}
