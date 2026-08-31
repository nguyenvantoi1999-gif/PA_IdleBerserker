using UnityEngine;
using UnityEngine.UI;

namespace IdleBattle
{
    // Gắn vào ToggleAutoSkill: bật/tắt tự dùng skill khi hồi xong
    // -> áp cho CẢ companion skill (CompanionSkillManager) LẪN player active skill (PlayerSkillDriver).
    public class UIAutoSkillToggleBinder : MonoBehaviour
    {
        [SerializeField] private Toggle _toggle;

        private CompanionSkillManager _companion;
        private PlayerSkillDriver _player;
        private bool _resolved;
        private bool _init;

        private void Awake()
        {
            if (_toggle == null) { _toggle = GetComponent<Toggle>(); }
            if (_toggle != null) { _toggle.onValueChanged.AddListener(OnToggle); }
        }

        private bool Resolve()
        {
            if (_resolved) { return _companion != null || _player != null; }
            if (BattleManager.Instance == null) { return false; }
            var owner = BattleManager.Instance.PlayerObject;
            if (owner == null) { return false; }
            _companion = owner.GetAbility<CompanionSkillManager>();
            _player = owner.GetAbility<PlayerSkillDriver>();
            _resolved = true;
            return _companion != null || _player != null;
        }

        private void Update()
        {
            if (_init) { return; }
            if (!Resolve()) { return; }
            bool on = (_companion != null && _companion.AutoUse) || (_player != null && _player.AutoUse);
            if (_toggle != null) { _toggle.SetIsOnWithoutNotify(on); }
            _init = true;
        }

        private void OnToggle(bool isOn)
        {
            if (!Resolve()) { return; }
            if (_companion != null) { _companion.AutoUse = isOn; }
            if (_player != null) { _player.AutoUse = isOn; }
        }
    }
}
