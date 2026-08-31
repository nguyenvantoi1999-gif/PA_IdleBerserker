using UnityEngine;
using UnityEngine.UI;

namespace IdleBattle
{
    // Gắn vào từng UI_CompanionSkillBarIcon (UI kéo-thả trong scene).
    // Bind slot -> CompanionActiveSkill của player: icon, hồi chiêu radial, đếm ngược, bấm để dùng.
    public class UICompanionSkillIconBinder : MonoBehaviour
    {
        [SerializeField] private int _slotIndex;
        [SerializeField] private Button _button;              // trên chính icon
        [SerializeField] private Image _iconImage;            // ImageSkillIcon
        [SerializeField] private Image _cooldownImage;        // ImageSkillCooldown (radial fill)
        [SerializeField] private Text _cooldownText;          // TextSkillCooldown
        [SerializeField] private GameObject _readyFx;         // FX_SkillReady (tùy chọn)

        private CompanionSkillManager _manager;
        private CompanionActiveSkill _skill;
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

        private bool EnsureManager()
        {
            if (_manager != null) { return true; }
            if (BattleManager.Instance == null) { return false; }
            var player = BattleManager.Instance.PlayerObject;
            if (player == null) { return false; }
            _manager = player.GetAbility<CompanionSkillManager>();
            return _manager != null;
        }

        private void TryBind()
        {
            if (_bound || _emptyHidden) { return; }
            if (!EnsureManager()) { return; }

            var skills = _manager.Skills;
            if (skills == null || skills.Count == 0) { return; }

            if (_slotIndex >= skills.Count)
            {
                // không có skill cho slot này -> ẩn cả icon
                _emptyHidden = true;
                gameObject.SetActive(false);
                return;
            }

            _skill = skills[_slotIndex];
            if (_iconImage != null)
            {
                var icons = _manager.Icons;
                Sprite icon = (icons != null && _slotIndex < icons.Count) ? icons[_slotIndex] : null;
                if (icon != null) { _iconImage.sprite = icon; }
                _iconImage.enabled = true;                 // prefab ingame tắt sẵn ImageSkillIcon -> bật lên
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
