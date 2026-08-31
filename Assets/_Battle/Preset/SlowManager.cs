using System.Collections;
using UnityEngine;

// Manager global cho hệ preset gốc (SlowPreset ở global namespace). Hit-stop bằng Time.timeScale.
public class SlowManager : MonoBehaviour
{
    private static SlowManager _instance;
    public static SlowManager Instance
    {
        get
        {
            if (_instance == null) { _instance = new GameObject("SlowManager").AddComponent<SlowManager>(); }
            return _instance;
        }
    }

    private void Awake() { if (_instance == null) { _instance = this; } }

    public void PlaySlowMotion(SlowPreset preset, float speed = 1f)
    {
        if (preset == null) { return; }
        StopAllCoroutines();
        StartCoroutine(Co(preset, speed));
    }

    private IEnumerator Co(SlowPreset preset, float speed)
    {
        if (speed <= 0f) { speed = 1f; }
        yield return new WaitForSecondsRealtime(preset.SlowDelay / speed);
        Time.timeScale = preset.SlowValue;
        yield return new WaitForSecondsRealtime(preset.SlowTime / speed);
        if (preset.IsLerpRecovery)
        {
            float t = 0f;
            while (preset.RecoveryTime > t)
            {
                t += Time.unscaledDeltaTime * speed;
                Time.timeScale = Mathf.Lerp(preset.SlowValue, 1f, t / preset.RecoveryTime);
                yield return null;
            }
        }
        Time.timeScale = 1f;
    }

    private void OnDisable() { Time.timeScale = 1f; }
}
