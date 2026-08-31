using UnityEngine;
using UnityEngine.UI;

namespace IdleBattle
{
    // Gắn vào từng UI_SkillBarIcon (nhóm Active). Bind slot -> PlayerActiveSkill của player
    // qua PlayerSkillDriver: icon, hồi chiêu radial, đếm ngược, bấm để dùng. Mirror UICompanionSkillIconBinder.
    public class UIPlayerSkillIconBinder : MonoBehaviour
    {
        [SerializeField] private int _slotIndex;
        [SerializeField] private Button _button;
        [SerializeField] private Image _iconImage;
        [SerializeField] private Image _cooldownImage;
        [SerializeField] private Text _cooldownText;
        [SerializeField] private GameObject _readyFx;

        private PlayerSkillDriver _driver;
        private PlayerActiveSkill _skill;
        private bool _bound;
        private bool _emptyHidden;

        private void Awake()
        {
            if (_cooldownImage != null)
            {
                _cooldownImage.type = Image.Type.Filled;
                _cooldownImage.fillMethod = Image.FillMethod.Radial360;
                _cooldownImage.fillOrigin = 2; // Radial360 Top (2) - tranh Image.Origin360 loi khi build Luna
                _cooldownImage.fillClockwise = false;
            }
        }

        private bool EnsureDriver()
        {
            if (_driver != null) { return true; }
            if (BattleManager.Instance == null) { return false; }
            var player = BattleManager.Instance.PlayerObject;
            if (player == null) { return false; }
            _driver = player.GetAbility<PlayerSkillDriver>();
            return _driver != null;
        }

        private void TryBind()
        {
            if (_bound || _emptyHidden) { return; }
            if (!EnsureDriver()) { return; }

            var skills = _driver.Skills;
            if (skills == null || skills.Count == 0) { return; }

            if (_slotIndex >= skills.Count)
            {
                _emptyHidden = true;
                gameObject.SetActive(false);
                return;
            }

            _skill = skills[_slotIndex];
            if (_iconImage != null)
            {
                var icons = _driver.Icons;
                Sprite icon = (icons != null && _slotIndex < icons.Count) ? icons[_slotIndex] : null;
                if (icon != null) { _iconImage.sprite = icon; }
                _iconImage.enabled = true;
                var c = _iconImage.color; c.a = 1f; _iconImage.color = c;
            }
            if (_button != null)
            {
                _button.onClick.RemoveListener(OnClick);
                _button.onClick.AddListener(OnClick);
            }
            _bound = true;
        }

        private void OnClick()
        {
            if (_skill != null) { _skill.TryUseSkill(); }
        }

        private void Update()
        {
            if (!_bound) { TryBind(); if (!_bound) { return; } }
            if (_skill == null) { return; }

            float remaining = Mathf.Max(0f, _skill.RemainSkillCooldownTime);
            bool ready = _skill.IsSkillEnable();

            if (_button != null) { _button.interactable = ready; }
            if (_cooldownImage != null) { _cooldownImage.enabled = !ready; }
            if (_cooldownText != null) { _cooldownText.enabled = !ready; }
            if (_readyFx != null && _readyFx.activeSelf != ready) { _readyFx.SetActive(ready); }

            if (!ready)
            {
                if (_cooldownImage != null) { _cooldownImage.fillAmount = Mathf.Clamp01(1f - _skill.RemainCoolTimeNormalized); }
                if (_cooldownText != null) { _cooldownText.text = remaining.ToString("0.0"); }
            }
        }
    }
}
