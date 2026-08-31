using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

// Bản PA: giữ API BattleCamera.Instance.Shake(ShakePreset) mà hệ preset gốc gọi,
// dùng KIỂU ShakePreset của ProCamera2D, nhưng áp rung thủ công lên Camera.main
// (playable dùng follow-camera thủ công nên không để ProCamera2D core giành camera).
public class BattleCamera : MonoBehaviour
{
    private static BattleCamera _instance;
    public static BattleCamera Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("BattleCamera");
                _instance = go.AddComponent<BattleCamera>();
            }
            return _instance;
        }
    }

    public bool UseShake = true;
    public float ShakeStrengthMultiply = 1f;

    private Camera _cam;
    private ShakePreset _preset;
    private bool _active;
    private float _elapsed;
    private float _seedX, _seedY, _seedR;
    private Vector3 _prevOffset;
    private float _prevRot;

    private void Awake()
    {
        if (_instance == null) { _instance = this; }
    }

    public void Shake(ShakePreset preset)
    {
        if (!UseShake || preset == null) { return; }
        _cam = Camera.main;
        if (_cam == null) { return; }
        _preset = preset;
        _elapsed = 0f;
        _active = true;
        float baseAngle = preset.UseRandomInitialAngle ? Random.Range(0f, 360f) : preset.InitialAngle;
        _seedX = baseAngle * 0.0174533f + Random.value * preset.Randomness * 10f;
        _seedY = _seedX + 13.37f;
        _seedR = _seedX + 41.23f;
    }

    public void StopShaking()
    {
        _active = false;
        RemovePrev();
    }

    private void RemovePrev()
    {
        if (_cam == null) { return; }
        if (_prevOffset != Vector3.zero) { _cam.transform.position -= _prevOffset; _prevOffset = Vector3.zero; }
        if (_prevRot != 0f) { _cam.transform.Rotate(0f, 0f, -_prevRot); _prevRot = 0f; }
    }

    private void LateUpdate()
    {
        if (!_active || _preset == null || _cam == null) { return; }
        RemovePrev();

        float dt = _preset.IgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        _elapsed += dt;
        float dur = Mathf.Max(0.0001f, _preset.Duration);
        if (_elapsed >= dur) { _active = false; _preset = null; return; }

        float damper = 1f - (_elapsed / dur);
        float freq = _preset.Vibrato * (1f - _preset.Smoothness) + 1f;
        float t = _elapsed * freq;
        Vector3 str = _preset.Strength * ShakeStrengthMultiply * 0.01f;

        Vector3 offset = new Vector3(
            (Mathf.PerlinNoise(_seedX, t) * 2f - 1f) * str.x,
            (Mathf.PerlinNoise(_seedY, t) * 2f - 1f) * str.y,
            0f) * damper;
        float rot = (Mathf.PerlinNoise(_seedR, t) * 2f - 1f) * _preset.Rotation.z * damper;

        _cam.transform.position += offset;
        _cam.transform.Rotate(0f, 0f, rot);
        _prevOffset = offset;
        _prevRot = rot;
    }

    private void OnDisable() { RemovePrev(); }
}
