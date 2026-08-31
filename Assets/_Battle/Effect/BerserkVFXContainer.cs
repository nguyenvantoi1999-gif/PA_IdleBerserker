using UnityEngine;

namespace IdleBattle
{
    // Port của BerserkVFXContainer: child của PlayerObject, giữ ref VFX berserk & bật/tắt qua API.
    // Có thể gắn VFX THẬT (prefab FX_BerserkerAura / FX_BerserkerTransform...) qua SetupVfxPrefabs;
    // nếu không có thì tự sinh aura placeholder.
    public class BerserkVFXContainer : MonoBehaviour
    {
        [Header("VFX refs (gán trên prefab thật)")]
        [SerializeField] private GameObject _berserkIdleVfxs;
        [SerializeField] private ParticleSystem _berserkEnterEffect;
        [SerializeField] private Transform _berserkEyeEffect;
        [SerializeField] private ParticleSystem _berserkerDashEffect;
        [SerializeField] private ParticleSystem _berserkerGroggyEffect;

        [Header("VFX prefab thật (tùy chọn) — sẽ instantiate")]
        [SerializeField] private GameObject _auraPrefab;   // FX_BerserkerAura (loop khi berserk)
        [SerializeField] private GameObject _enterPrefab;  // FX_BerserkerTransform (burst lúc vào)
        [SerializeField] private float _vfxLocalY = 1.2f;
        [SerializeField] private float _vfxScale = 1f;

        [Header("Placeholder (khi không gán VFX thật)")]
        [SerializeField] private bool _autoPlaceholder = true;
        [SerializeField] private Vector2 _auraSize = new Vector2(2.6f, 3.4f);
        [SerializeField] private Color _auraColor = new Color(1f, 0.25f, 0.12f, 0.55f);
        [SerializeField] private int _auraSortingOrder = 45;

        private bool _built;
        private GameObject _enterInstance;
        private SpriteRenderer _auraRenderer; // chỉ dùng cho placeholder
        private int _auraIndex;
        private static Sprite _glow;

        // Gọi trước khi activate GameObject để nạp prefab VFX thật (bootstrap dùng).
        public void SetupVfxPrefabs(GameObject auraPrefab, GameObject enterPrefab, float localY, float scale)
        {
            _auraPrefab = auraPrefab;
            _enterPrefab = enterPrefab;
            _vfxLocalY = localY;
            _vfxScale = scale;
        }

        private void Awake()
        {
            EnsureBuilt();
        }

        private void EnsureBuilt()
        {
            if (_built) { return; }
            _built = true;

            if (_berserkIdleVfxs == null)
            {
                if (_auraPrefab != null)
                {
                    _berserkIdleVfxs = Instantiate(_auraPrefab, transform);
                    _berserkIdleVfxs.name = "BerserkAura";
                    _berserkIdleVfxs.transform.localPosition = new Vector3(0f, _vfxLocalY, 0f);
                    _berserkIdleVfxs.transform.localScale = Vector3.one * _vfxScale;
                    NormalizeRenderers(_berserkIdleVfxs);
                }
                else if (_autoPlaceholder)
                {
                    BuildPlaceholderAura();
                }
            }

            if (_enterPrefab != null)
            {
                _enterInstance = Instantiate(_enterPrefab, transform);
                _enterInstance.name = "BerserkEnter";
                _enterInstance.transform.localPosition = new Vector3(0f, _vfxLocalY, 0f);
                _enterInstance.transform.localScale = Vector3.one * _vfxScale;
                NormalizeRenderers(_enterInstance);
                _enterInstance.SetActive(false);
            }

            if (_berserkIdleVfxs != null) { _berserkIdleVfxs.SetActive(false); }
        }

        // ===== API giữ nguyên như game gốc =====
        public void ChangeBerserkEffectActivation(bool isOn)
        {
            EnsureBuilt();
            if (_berserkIdleVfxs != null)
            {
                _berserkIdleVfxs.SetActive(isOn);
                if (isOn) { PlayAllParticles(_berserkIdleVfxs); }
                else { StopAllParticles(_berserkIdleVfxs); }
            }
            if (isOn) { PlayEnterBurst(); }
        }

        private void PlayEnterBurst()
        {
            if (_enterInstance != null)
            {
                _enterInstance.SetActive(true);
                PlayAllParticles(_enterInstance);
            }
            if (_berserkEnterEffect != null)
            {
                _berserkEnterEffect.gameObject.SetActive(true);
                _berserkEnterEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _berserkEnterEffect.Play();
            }
        }

        public void PlayDashEffect()
        {
            if (_berserkerDashEffect != null)
            {
                _berserkerDashEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                _berserkerDashEffect.Play();
            }
        }

        public Transform GetBerserkEyeTrailEffect() { return _berserkEyeEffect; }

        public void SetAuraEffect(int index) { _auraIndex = index; }
        public int AuraIndex { get { return _auraIndex; } }

        public void SetGroggyEffect(bool isOn)
        {
            if (_berserkerGroggyEffect == null) { return; }
            _berserkerGroggyEffect.gameObject.SetActive(isOn);
            _berserkerGroggyEffect.Stop();
            if (isOn) { _berserkerGroggyEffect.Play(); }
        }

        // FX prefab game gốc có PlayOnAwake nhưng không tự chạy lại khi re-enable,
        // và tham chiếu sorting layer không tồn tại trong PA -> ép về Default + Play thủ công.
        private static void NormalizeRenderers(GameObject go)
        {
            ParticleSystemRenderer[] rs = go.GetComponentsInChildren<ParticleSystemRenderer>(true);
            for (int i = 0; i < rs.Length; i++)
            {
                rs[i].sortingLayerName = "Default";
            }
        }

        private static void PlayAllParticles(GameObject go)
        {
            ParticleSystem[] ps = go.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < ps.Length; i++)
            {
                //ps[i].Clear(true);
                ps[i].Play(false);
            }
        }

        private static void StopAllParticles(GameObject go)
        {
            ParticleSystem[] ps = go.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }

        private void Update()
        {
            if (_auraRenderer != null && _auraRenderer.gameObject.activeInHierarchy)
            {
                float pulse = 0.85f + Mathf.Sin(Time.time * 6f) * 0.15f;
                _auraRenderer.transform.localScale = new Vector3(_auraSize.x * pulse, _auraSize.y * pulse, 1f);
                Color c = _auraColor;
                c.a = _auraColor.a * (0.75f + Mathf.Sin(Time.time * 6f) * 0.25f);
                _auraRenderer.color = c;
            }
        }

        private void BuildPlaceholderAura()
        {
            GameObject aura = new GameObject("BerserkAuraPlaceholder");
            aura.transform.SetParent(transform, false);
            aura.transform.localPosition = new Vector3(0f, _vfxLocalY, 0.1f);
            aura.transform.localScale = new Vector3(_auraSize.x, _auraSize.y, 1f);
            _auraRenderer = aura.AddComponent<SpriteRenderer>();
            _auraRenderer.sprite = GlowSprite();
            _auraRenderer.color = _auraColor;
            _auraRenderer.sortingOrder = _auraSortingOrder;
            _berserkIdleVfxs = aura;
        }

        private static Sprite GlowSprite()
        {
            if (_glow == null)
            {
                int size = 64;
                Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
                Vector2 center = new Vector2(size * 0.5f, size * 0.5f);
                float maxR = size * 0.5f;
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        float d = Vector2.Distance(new Vector2(x, y), center) / maxR;
                        float a = Mathf.Clamp01(1f - d); a = a * a;
                        tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                    }
                }
                tex.Apply(); tex.filterMode = FilterMode.Bilinear;
                _glow = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            }
            return _glow;
        }
    }
}
