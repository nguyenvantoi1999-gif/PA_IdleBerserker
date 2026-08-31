using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using Spine.Unity;
using UnityEngine;

namespace IdleBattle
{
    public enum EventSpawnMode { Self, NearestEnemy }

    // 1 ánh xạ: tên Spine event -> particle prefab + sound.
    [System.Serializable]
    public class SpineEventMapping
    {
        
        [SpineEvent]public string EventName = "attack";
        public GameObject VfxPrefab;
        public AudioClip Sfx;
        public Vector3 Offset = new Vector3(0f, 1f, 0f);
        public float Scale = 1f;
        public EventSpawnMode SpawnMode = EventSpawnMode.Self;
        [Range(0f, 1f)] public float Volume = 1f;
        public float VfxLifetime = 2f;
        [Tooltip("Tùy chọn: rung camera khi event này nổ (vd boss FX_GroundBurst)")]
        public ShakePreset ShakeOnEvent;
    }

    // Gắn particle effect + âm thanh vào Spine animation event.
    // Nghe AnimationState.Event; khi event khớp tên -> spawn VFX + PlayOneShot.
    public class SpineEventEffect : CharacterAbility
    {
        public List<SpineEventMapping> Mappings = new List<SpineEventMapping>();
        [Range(0f, 1f)] public float MasterVolume = 1f;

        private AnimationAbility _anim;
        private MonsterDetectAbility _monsterDetect;
        private AudioSource _audio;
        private bool _subscribed;

        public void AddMapping(SpineEventMapping m) { Mappings.Add(m); }

        public override void LateInit()
        {
            base.LateInit();
            _anim = _ownerObject.GetAbility<AnimationAbility>();
            _monsterDetect = _ownerObject.GetAbility<MonsterDetectAbility>();
            _audio = GetComponent<AudioSource>();
            if (_audio == null) { _audio = gameObject.AddComponent<AudioSource>(); }
            _audio.playOnAwake = false;
            _audio.spatialBlend = 0f;
            Subscribe();
        }

        private void Subscribe()
        {
            if (_subscribed || _anim == null || _anim.Animation == null) { return; }
            _anim.AnimationState.Event += OnSpineEvent;
            _subscribed = true;
        }

        private void OnDisable() { Unsubscribe(); }
        private void OnDestroy() { Unsubscribe(); }
        private void OnEnable()
        {
            if (!_subscribed && _anim != null) { Subscribe(); }
        }

        private void Unsubscribe()
        {
            if (_subscribed && _anim != null && _anim.Animation != null)
            {
                _anim.AnimationState.Event -= OnSpineEvent;
            }
            _subscribed = false;
        }

        private void OnSpineEvent(Spine.TrackEntry entry, Spine.Event e)
        {
            string evName = e.Data.Name;
            for (int i = 0; i < Mappings.Count; i++)
            {
                if (Mappings[i].EventName == evName) { Fire(Mappings[i]); }
            }
        }

        private void Fire(SpineEventMapping m)
        {
            Vector3 pos = ResolvePosition(m);
            if (m.VfxPrefab != null)
            {
                GameObject fx = Instantiate(m.VfxPrefab, pos, Quaternion.identity);
                fx.transform.localScale = fx.transform.localScale * m.Scale;
                NormalizeRenderers(fx);
                PlayAllParticles(fx);
                Destroy(fx, Mathf.Max(0.3f, m.VfxLifetime));
            }
            if (m.Sfx != null && _audio != null)
            {
                _audio.PlayOneShot(m.Sfx, Mathf.Clamp01(m.Volume * MasterVolume));
            }
            if (m.ShakeOnEvent != null)
            {
                BattleCamera.Instance.Shake(m.ShakeOnEvent);
            }
        }

        private Vector3 ResolvePosition(SpineEventMapping m)
        {
            Vector3 basePos = _ownerObject.PositionCenter;
            if (m.SpawnMode == EventSpawnMode.NearestEnemy && _monsterDetect != null)
            {
                if (_monsterDetect.TryGetTargets(_ownerObject.Stat[Enum_StatType.AttackRange] + 5.0, out List<CharacterObject> targets) && targets.Count > 0)
                {
                    float ox = _ownerObject.Position.x;
                    targets.Sort((a, b) => Mathf.Abs(a.Position.x - ox).CompareTo(Mathf.Abs(b.Position.x - ox)));
                    basePos = targets[0].PositionCenter;
                }
            }
            // offset lật theo hướng nhìn (ScaleX của skeleton)
            float faceSign = (_anim != null && _anim.Animation != null && _anim.Skeleton.ScaleX < 0f) ? -1f : 1f;
            return basePos + new Vector3(m.Offset.x * faceSign, m.Offset.y, m.Offset.z);
        }

        private static void NormalizeRenderers(GameObject go)
        {
            Renderer[] rs = go.GetComponentsInChildren<Renderer>(true);
            for (int i = 0; i < rs.Length; i++)
            {
                rs[i].sortingLayerName = "Default";
                if (rs[i].sortingOrder < 200) { rs[i].sortingOrder += 200; }
            }
        }

        private static void PlayAllParticles(GameObject go)
        {
            ParticleSystem[] ps = go.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < ps.Length; i++)
            {
                ps[i].Clear();
                ps[i].Play(false);
            }
        }
    }
}
