using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Manager global cho hệ preset gốc (BlackoutPreset ở global namespace). Overlay fade full-screen.
public class BlackoutManager : MonoBehaviour
{
    private static BlackoutManager _instance;
    public static BlackoutManager Instance
    {
        get
        {
            if (_instance == null) { _instance = new GameObject("BlackoutManager").AddComponent<BlackoutManager>(); }
            return _instance;
        }
    }

    private Image _overlay;
    private void Awake() { if (_instance == null) { _instance = this; } }

    private void EnsureOverlay(BlackoutPreset preset)
    {
        if (_overlay != null) { return; }
        GameObject cv = new GameObject("BlackoutCanvas");
        cv.transform.SetParent(transform, false);
        Canvas canvas = cv.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = preset.IsFront ? 32760 : -100;
        cv.AddComponent<CanvasScaler>();
        GameObject img = new GameObject("Overlay");
        img.transform.SetParent(cv.transform, false);
        _overlay = img.AddComponent<Image>();
        _overlay.raycastTarget = false;
        RectTransform rt = _overlay.rectTransform;
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    public void PlayBlackOut(BlackoutPreset preset)
    {
        if (preset == null) { return; }
        EnsureOverlay(preset);
        StopAllCoroutines();
        StartCoroutine(Co(preset));
    }

    private IEnumerator Co(BlackoutPreset preset)
    {
        _overlay.canvas.sortingOrder = preset.IsFront ? 32760 : -100;
        Color c = preset.Color;
        SetA(c, 0f);
        if (preset.BlackoutDelay > 0f) { yield return new WaitForSecondsRealtime(preset.BlackoutDelay); }
        yield return Fade(c, 0f, preset.BlackoutValue, preset.FadeInTime);
        SetA(c, preset.BlackoutValue);
        if (preset.BlackoutTime > 0f) { yield return new WaitForSecondsRealtime(preset.BlackoutTime); }
        yield return Fade(c, preset.BlackoutValue, 0f, preset.FadeOutTime);
        SetA(c, 0f);
    }

    private IEnumerator Fade(Color c, float from, float to, float time)
    {
        if (time <= 0f) { SetA(c, to); yield break; }
        float t = 0f;
        while (t < time) { t += Time.unscaledDeltaTime; SetA(c, Mathf.Lerp(from, to, t / time)); yield return null; }
    }

    private void SetA(Color c, float a) { c.a = a; _overlay.color = c; }
}
