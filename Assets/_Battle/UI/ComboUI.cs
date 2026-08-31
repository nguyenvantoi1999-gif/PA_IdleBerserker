using UnityEngine;

namespace IdleBattle
{
    // Hiện bộ đếm combo nổi gần player khi combo >= 2.
    public class ComboUI : CharacterAbility
    {
        public int MinShow = 2;
        public float HeadY = 2.6f;

        private ComboAbility _combo;
        private TextMesh _text;

        public override void LateInit()
        {
            base.LateInit();
            _combo = _ownerObject.GetAbility<ComboAbility>();
        }

        public override void ProcessAbility(float deltaTime)
        {
            if (_combo == null) { return; }
            int c = _combo.ComboCount;
            if (c >= MinShow)
            {
                if (_text == null)
                {
                    _text = BattleArt.CreateText("ComboUI", null, 64, 0.3f, new Color(1f, 0.82f, 0.2f), 700);
                }
                if (!_text.gameObject.activeSelf) { _text.gameObject.SetActive(true); }
                _text.text = c.ToString() + " COMBO";
                _text.transform.position = _ownerObject.PositionCenter + new Vector3(0f, HeadY, -0.3f);
                float pulse = 1f + Mathf.Sin(Time.time * 22f) * 0.06f;
                _text.transform.localScale = Vector3.one * pulse;
            }
            else if (_text != null && _text.gameObject.activeSelf)
            {
                _text.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if (_text != null) { Destroy(_text.gameObject); }
        }
    }
}
