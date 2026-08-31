using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

// Port từ game gốc, bỏ asset-pack/network/SoundQueueManager để chạy độc lập trong PA.
public class SoundManager : SingletonBehaviour<SoundManager>
{
    private const int AudioSourceCount = 24;

    [SerializeField] private AudioMixer MainMixer;
    [SerializeField] private AudioSource BgmAudioSource;
    [SerializeField] private List<AudioClip> BackgroundClips = new List<AudioClip>();
    [SerializeField] private List<AudioClip> SfxClips = new List<AudioClip>();

    private readonly Dictionary<string, AudioClip> _backgroundByName = new Dictionary<string, AudioClip>();
    private readonly Dictionary<string, AudioClip> _sfxByName = new Dictionary<string, AudioClip>();
    private readonly List<AudioSource> _sfxSources = new List<AudioSource>(AudioSourceCount);
    private AudioMixerGroup _bgmGroup;
    private AudioMixerGroup _sfxGroup;
    private AudioClip _lastBgm;
    private bool _isFixed;

    public bool IsInited { get; private set; }
    public bool IsMusicOn { get { return GetOption() == null || GetOption().BGM; } }
    public bool IsSfxOn { get { return GetOption() == null || GetOption().SFX; } }

    protected override void Awake()
    {
        base.Awake();
        if (!m_Enabled) { return; }
        DontDestroyOnLoad(gameObject);
        Init();
    }

    private void Init()
    {
        if (IsInited) { return; }
        if (MainMixer != null)
        {
            AudioMixerGroup[] bgm = MainMixer.FindMatchingGroups("BGM");
            AudioMixerGroup[] sfx = MainMixer.FindMatchingGroups("SFX");
            if (bgm.Length > 0) { _bgmGroup = bgm[0]; }
            if (sfx.Length > 0) { _sfxGroup = sfx[0]; }
        }
        if (BgmAudioSource == null)
        {
            BgmAudioSource = CreateSource("BackgroundMusic", _bgmGroup);
            BgmAudioSource.loop = true;
        }
        for (int i = 0; i < AudioSourceCount; i++)
        {
            _sfxSources.Add(CreateSource("AudioSource_" + (i + 1), _sfxGroup));
        }
        RegisterClips(BackgroundClips, _backgroundByName, "BGM");
        RegisterClips(SfxClips, _sfxByName, "SFX");
        IsInited = true;
    }

    private AudioSource CreateSource(string sourceName, AudioMixerGroup group)
    {
        GameObject child = new GameObject(sourceName);
        child.transform.SetParent(transform, false);
        AudioSource source = child.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.spatialBlend = 0f;
        source.outputAudioMixerGroup = group;
        return source;
    }

    private static void RegisterClips(List<AudioClip> source, Dictionary<string, AudioClip> destination, string group)
    {
        for (int i = 0; i < source.Count; i++)
        {
            AudioClip clip = source[i];
            if (clip == null) { continue; }
            if (destination.ContainsKey(clip.name))
            {
                Debug.LogWarning("[" + group + "] Duplicate clip: " + clip.name);
                continue;
            }
            destination.Add(clip.name, clip);
        }
    }

    public void Register(string soundName, AudioClip clip)
    {
        if (!string.IsNullOrEmpty(soundName) && clip != null) { _sfxByName[soundName] = clip; }
    }

    public void RegisterBackground(string soundName, AudioClip clip)
    {
        if (!string.IsNullOrEmpty(soundName) && clip != null) { _backgroundByName[soundName] = clip; }
    }

    public void PlayBackgroundFix(string soundName, bool loop = true)
    {
        if (_isFixed) { return; }
        PlayBackground(soundName, loop);
        _isFixed = true;
    }

    public void StopBackgroundFix() { _isFixed = false; }

    public void PlayBackground(string soundName, bool loop = true)
    {
        if (_isFixed || string.IsNullOrEmpty(soundName)) { return; }
        AudioClip clip = GetClip(soundName, _backgroundByName, "BGM");
        if (clip == null || (BgmAudioSource.clip == clip && BgmAudioSource.isPlaying)) { return; }
        BgmAudioSource.Stop();
        BgmAudioSource.clip = clip;
        BgmAudioSource.volume = IsMusicOn ? GetBgmVolume() : 0f;
        BgmAudioSource.loop = loop;
        BgmAudioSource.Play();
        _lastBgm = clip;
    }

    public void StopBGM()
    {
        if (BgmAudioSource != null) { BgmAudioSource.Stop(); }
    }

    public void ResumeBGM()
    {
        if (BgmAudioSource == null || _lastBgm == null) { return; }
        BgmAudioSource.clip = _lastBgm;
        BgmAudioSource.volume = IsMusicOn ? GetBgmVolume() : 0f;
        BgmAudioSource.Play();
    }

    public void SetBGMVolume()
    {
        if (BgmAudioSource != null) { BgmAudioSource.volume = IsMusicOn ? GetBgmVolume() : 0f; }
    }

    public void PlaySoundDelay(float delayTime, string soundName, float volumeFactor = 1f)
    {
        if (IsSfxOn) { StartCoroutine(PlaySoundDelayCoroutine(delayTime, soundName, volumeFactor)); }
    }

    private IEnumerator PlaySoundDelayCoroutine(float delayTime, string soundName, float volumeFactor)
    {
        yield return new WaitForSeconds(delayTime);
        PlaySound(soundName, volumeFactor);
    }

    public void PlaySound(string soundName, float volumeFactor = 1f)
    {
        if (!IsSfxOn || string.IsNullOrEmpty(soundName)) { return; }
        AudioClip clip = GetClip(soundName, _sfxByName, "SFX");
        if (clip == null) { return; }
        for (int i = 0; i < _sfxSources.Count; i++)
        {
            AudioSource source = _sfxSources[i];
            if (source.isPlaying) { continue; }
            source.volume = GetSfxVolume() * Mathf.Max(0f, volumeFactor);
            source.loop = false;
            source.PlayOneShot(clip);
            return;
        }
    }

    private static AudioClip GetClip(string soundName, Dictionary<string, AudioClip> clips, string folder)
    {
        AudioClip clip;
        if (clips.TryGetValue(soundName, out clip)) { return clip; }
        clip = Resources.Load<AudioClip>(folder + "/" + soundName);
        if (clip != null) { clips[soundName] = clip; }
        else { Debug.LogWarning("[SoundManager] Missing " + folder + " clip: " + soundName); }
        return clip;
    }

    private static Option GetOption()
    {
        return OptionManager.Instance == null ? null : OptionManager.Instance.Option;
    }

    private static float GetBgmVolume()
    {
        Option option = GetOption();
        return option == null ? 1f : option.BGMVolume;
    }

    private static float GetSfxVolume()
    {
        Option option = GetOption();
        return option == null ? 1f : option.SFXVolume;
    }

    public void PlayPopupOpen() { PlaySound("berserk_sfx_popup"); }
    public void PlayClose() { PlaySound("berserk_sfx_close"); }
    public void PlayButtonClickSound() { PlaySound("berserk_sfx_click"); }

    public void OnBossMonsterDeath()
    {
        StopBGM();
        PlaySound("berserk_sfx_boss_death");
    }

    public void OnNormalMonsterDeath()
    {
        PlaySound(Random.value < 0.5f ? "berserk_sfx_monster_death_01" : "berserk_sfx_monster_death_02");
    }
}
