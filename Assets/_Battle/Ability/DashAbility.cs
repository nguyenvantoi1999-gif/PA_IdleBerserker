using UnityEngine;

namespace IdleBattle
{
    // Lướt (dash) tới enemy khi enemy kế tiếp nằm NGOÀI attack range, có hồi chiêu.
    public class DashAbility : CharacterAbility
    {
        public float CoolTime = 2.5f;          // hồi chiêu (giây)
        public float DashTime = 0.22f;         // thời gian lướt
        public GameObject DashVfx;             // FX_Dash
        public float VfxYOffset = 0.15f;
        public float VfxLifetime = 0.8f;

        private float _timer;

        public bool IsDashReady { get { return _timer <= 0f; } }
        public float DashTimeValue { get { return DashTime; } }
        public float CooldownRatio { get { return Mathf.Clamp01(1f - _timer / Mathf.Max(0.01f, CoolTime)); } }

        public override void ProcessAbility(float deltaTime)
        {
            if (_timer > 0f) { _timer -= deltaTime; }
        }

        public void StartCooldown() { _timer = CoolTime; }

        public void PlayDashVfx(CharacterObject owner)
        {
            if (DashVfx == null) { return; }
            Vector3 pos = new Vector3(owner.transform.position.x, owner.transform.position.y + VfxYOffset, owner.transform.position.z - 0.2f);
            GameObject fx = Instantiate(DashVfx, pos, Quaternion.identity);
            fx.SetActive(true);
            Renderer[] rs = fx.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < rs.Length; i++)
            {
                rs[i].sortingLayerName = "Default";
                if (rs[i].sortingOrder < 200) { rs[i].sortingOrder += 200; }
            }
            ParticleSystem[] ps = fx.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < ps.Length; i++) { ps[i].Clear(); ps[i].Play(false); }
            Destroy(fx, VfxLifetime);
        }
    }
}
