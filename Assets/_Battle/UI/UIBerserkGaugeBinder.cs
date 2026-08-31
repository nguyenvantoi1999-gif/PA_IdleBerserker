using UnityEngine;
using UnityEngine.UI;

namespace IdleBattle
{
    // Điều khiển thanh fill Berserker (UI kéo-thả trong scene) bám theo BerserkAbility của player.
    // Bản rút gọn của UI_BerserkGauge game gốc: poll trạng thái thay cho hệ GameEvent.
    public class UIBerserkGaugeBinder : MonoBehaviour
    {
        [Header("Fill")]
        [SerializeField] private Image _fillImage;                 // BerserkGaugeImage
        [SerializeField] private string _fillMaterialProperty = "_FillLevel"; // để trống -> dùng fillAmount

        [Header("Interact")]
        [SerializeField] private Button _button;                   // BerserkGaugeFrame
        [SerializeField] private Toggle _autoToggle;               // ToggleAutoBerserk
        [SerializeField] private GameObject _readyGuide;           // ImageForBerserkGuide

        [Header("Animator (tùy chọn)")]
        [SerializeField] private Animator _animator;               // FX_UI_BerserkGauge

        private readonly int _idle = Animator.StringToHash("FX_UI_BerserkGauge_idle");
        private readonly int _ready = Animator.StringToHash("FX_UI_BerserkGauge_Ready");
        private readonly int _activate = Animator.StringToHash("FX_UI_BerserkGauge_Activate");
        private readonly int _over = Animator.StringToHash("FX_UI_BerserkGauge_Over");

        private BerserkerObject _player;
        private BerserkAbility _berserk;
        private Material _fillMaterialInstance;
        private int _fillPropertyId = -1;
        private bool _hasMaterialProp;

        private bool _wasReady;
        private bool _wasBerserk;

        private void Awake()
        {
            if (_button != null) { _button.onClick.AddListener(OnClickButton); }
            if (_autoToggle != null) { _autoToggle.onValueChanged.AddListener(OnToggleAuto); }

            if (_fillImage != null && !string.IsNullOrEmpty(_fillMaterialProperty) && _fillImage.material != null)
            {
                _fillPropertyId = Shader.PropertyToID(_fillMaterialProperty);
                _hasMaterialProp = _fillImage.material.HasProperty(_fillPropertyId);
                if (_hasMaterialProp)
                {
                    _fillMaterialInstance = Instantiate(_fillImage.material);
                    _fillImage.material = _fillMaterialInstance;
                }
            }
            if (_fillImage != null && !_hasMaterialProp)
            {
                _fillImage.type = Image.Type.Filled; // fallback dùng fillAmount
            }
        }

        private bool EnsureAbility()
        {
            if (_berserk != null) { return true; }
            if (BattleManager.Instance == null) { return false; }
            _player = BattleManager.Instance.PlayerObject as BerserkerObject;
            if (_player == null) { return false; }
            _berserk = _player.GetAbility<BerserkAbility>();
            if (_berserk == null) { return false; }
            if (_autoToggle != null) { _autoToggle.SetIsOnWithoutNotify(_berserk.IsAuto); }
            return true;
        }

        private void Update()
        {
            if (!EnsureAbility()) { return; }

            bool berserk = _berserk.IsBerserkMode;
            if (berserk)
            {
                SetFill(_berserk.RemainRatio);
                if (!_wasBerserk) { PlayState(_activate); }
                if (_readyGuide != null && _readyGuide.activeSelf) { _readyGuide.SetActive(false); }
            }
            else
            {
                SetFill(_berserk.GaugeRatio);
                if (_wasBerserk) { PlayState(_over); }

                bool ready = _berserk.IsReady;
                if (ready && !_wasReady) { PlayState(_ready); }
                if (_readyGuide != null && _readyGuide.activeSelf != ready) { _readyGuide.SetActive(ready); }
                _wasReady = ready;
            }
            _wasBerserk = berserk;
        }

        private void SetFill(float value)
        {
            if (_fillImage == null) { return; }
            if (_hasMaterialProp && _fillMaterialInstance != null)
            {
                _fillMaterialInstance.SetFloat(_fillPropertyId, value);
            }
            else
            {
                _fillImage.fillAmount = value;
            }
        }

        private void PlayState(int hash)
        {
            if (_animator != null && _animator.isActiveAndEnabled) { _animator.Play(hash, -1, 0f); }
        }

        public void OnClickButton()
        {
            if (_berserk == null) { return; }
            if (_berserk.IsReady) { _player.SetBerserkState(true); }
        }

        public void OnToggleAuto(bool isOn)
        {
            if (_berserk != null) { _berserk.IsAuto = isOn; }
        }
    }
}
