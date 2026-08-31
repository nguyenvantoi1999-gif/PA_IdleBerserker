using UnityEngine;

namespace IdleBattle
{
    // Hiện hit VFX đúng loại khi TARGET trúng đòn của player:
    // NormalHit / CriticalHit / BerserkNormalHit / BerserkCriticalHit (theo crit + berserk).
    public class HitVfxAbility : CharacterAbility
    {
        public GameObject NormalHit;
        public GameObject CriticalHit;
        public GameObject BerserkNormalHit;
        public GameObject BerserkCriticalHit;
        public float Scale = 1f;
        public float Lifetime = 1.1f;

        private bool _hooked;

        public override void LateInit()
        {
            base.LateInit();
            _ownerObject.OnDamaged += OnDamaged;
            _hooked = true;
        }

        private void OnDamaged(Damage d)
        {
            // chỉ hiện cho đòn của player (không hiện khi quái đánh player)
            if (d.DamageType == Enum_DamageType.PlayerHit || d.DamageType == Enum_DamageType.HP) { return; }

            bool berserk = d.PlayerState == Enum_PlayerState.Berserk;
            bool crit = d.CriticalType == Enum_CriticalType.Critical || d.CriticalType == Enum_CriticalType.SuperCritical;
            GameObject prefab = berserk ? (crit ? BerserkCriticalHit : BerserkNormalHit)
                                        : (crit ? CriticalHit : NormalHit);
            if (prefab == null) { return; }

            Vector3 pos = _ownerObject.PositionCenter + new Vector3(Random.Range(-0.25f, 0.25f), Random.Range(-0.15f, 0.15f), -0.3f);
            GameObject fx = Instantiate(prefab, pos, Quaternion.identity);
            fx.SetActive(true);
            fx.transform.localScale = fx.transform.localScale * Scale;
            Renderer[] rs = fx.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < rs.Length; i++)
            {
                rs[i].sortingLayerName = "Default";
                if (rs[i].sortingOrder < 250) { rs[i].sortingOrder += 250; }
            }
            ParticleSystem[] ps = fx.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < ps.Length; i++) { ps[i].Clear(); ps[i].Play(false); }
            Destroy(fx, Lifetime);
        }

        private void OnDestroy()
        {
            if (_hooked && _ownerObject != null) { _ownerObject.OnDamaged -= OnDamaged; }
        }
    }
}
