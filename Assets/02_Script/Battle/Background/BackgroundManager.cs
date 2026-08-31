using System.Collections.Generic;
using UnityEngine;
using IdleBattle;

// Port PA của BackgroundManager (bỏ addressables/CookApps.PAD + các mode dungeon/pvp/raid/eventworld).
// Giữ: follow player + parallax scroll + VFX nền (_backgroundVFX) + berserk background VFX + centerOffSet.
// Nguồn nền lấy từ mảng prefab serialize (thay AssetPackManager).
public class BackgroundManager : SingletonBehaviour<BackgroundManager>
{
    [Header("Nền stage (prefab BackgroundSet, thay addressables)")]
    [SerializeField] private GameObject[] _stageBackgrounds;   // World1..N
    [SerializeField] private int _autoStageIndex = 0;
    [SerializeField] private bool _autoStart = true;

    [Header("VFX nền (con của prefab)")]
    [SerializeField] private ParticleSystem _berserkBackgroundVFX;
    [SerializeField] private ParticleSystem _berserkBackgroundEventWorldVFX;
    [SerializeField] private ParticleSystem _backgroundVFX;

    [SerializeField] private Vector3 _centerOffSet;
    [SerializeField] private float _scrollSpeed = 0.5f;

    private BackgroundSet _bgSet;
    private bool _isBgSet;
    private bool _useScroll;
    private PlayerObject _playerObject;
    private List<Material> _layerMats;

    protected override void Awake()
    {
        base.Awake();

        Shader shader = Shader.Find("Mobile/Particles/Alpha Blended");
        if (shader == null) { shader = Shader.Find("Legacy Shaders/Particles/Alpha Blended"); }
        if (shader == null) { shader = Shader.Find("Sprites/Default"); }

        _layerMats = new List<Material>();
        for (int i = 0; i < 5; i++) { _layerMats.Add(new Material(shader)); }
    }

    private void Start()
    {
        _playerObject = BattleManager.Instance.PlayerObject;
        if (_playerObject == null) { _playerObject = FindObjectOfType<PlayerObject>(); }
        if (_autoStart) { SetStageBackground(_autoStageIndex); }
        SetActiveBerserkBackground(false); // berserk bg tắt tới khi player vào berserk
    }

    private void LateUpdate()
    {
        if (!_useScroll) { return; }
        if (_playerObject == null) { _playerObject = FindObjectOfType<PlayerObject>(); if (_playerObject == null) { return; } }

        Vector3 bgPos = _playerObject.Position;
        transform.position = bgPos + _centerOffSet;

        if (_isBgSet) { _bgSet.Refresh(bgPos, _scrollSpeed); }
    }

    private void SwapBgSet(BackgroundSet bgSet)
    {
        BackgroundSet last = _bgSet;
        _bgSet = bgSet;
        _isBgSet = true;
        _bgSet.Init(_layerMats);

        _bgSet.transform.localPosition = Vector3.zero;
        _bgSet.transform.localScale = new Vector3(3, 1, 1);
        _bgSet.transform.SetAsFirstSibling();

        if (last != null) { Destroy(last.gameObject); }
    }

    public void SetStageBackground(int backgroundIndex, bool useScroll = true)
    {
        if (_stageBackgrounds == null || backgroundIndex < 0 || backgroundIndex >= _stageBackgrounds.Length) { return; }
        GameObject prefab = _stageBackgrounds[backgroundIndex];
        if (prefab == null) { return; }

        _useScroll = useScroll;
        GameObject obj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
        SwapBgSet(obj.GetComponent<BackgroundSet>());

        if (_backgroundVFX != null) { _backgroundVFX.gameObject.SetActive(true); }
    }

    // Gọi khi player vào/thoát chế độ berserk (bật particle nền cuồng nộ).
    public void SetActiveBerserkBackground(bool isOn)
    {
        if (_berserkBackgroundEventWorldVFX != null) { _berserkBackgroundEventWorldVFX.gameObject.SetActive(false); }
        if (_berserkBackgroundVFX != null)
        {
            _berserkBackgroundVFX.gameObject.SetActive(isOn);
            if (isOn) { _berserkBackgroundVFX.Play(); }
            else { _berserkBackgroundVFX.Stop(); }
        }
    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos + _centerOffSet.y * Vector3.up;
    }
}
