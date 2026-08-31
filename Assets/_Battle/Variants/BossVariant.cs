using UnityEngine;

namespace IdleBattle
{
    [CreateAssetMenu(fileName = "BossVariant", menuName = "IdleBattle/Boss Variant")]
    public class BossVariant : CharacterVariant
    {
        [Header("Boss")]
        public string BossName = "RAID BOSS";

        [Tooltip("Sát thương theo % máu player mỗi đòn (boss thường mạnh hơn quái).")]
        public float DamagePercentOfPlayerHp = 0.18f;
    }
}
