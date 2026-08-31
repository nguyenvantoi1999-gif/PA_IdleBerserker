using UnityEngine;

namespace IdleBattle
{
    // Thanh máu nổi trên đầu nhân vật (2 quad procedural).
    public class HealthBar : MonoBehaviour
    {
        private Transform _target;
        private Vector3 _offset;
        private Transform _fill;
        private float _width;
        private float _displayed = 1f;
        private float _wanted = 1f;

        public static HealthBar Create(Transform target, float yOffset, float width, Color color, int order)
        {
            GameObject root = new GameObject("HealthBar");
            HealthBar bar = root.AddComponent<HealthBar>();
            bar._target = target;
            bar._offset = new Vector3(0f, yOffset, 0f);
            bar._width = width;

            GameObject bg = new GameObject("BG");
            bg.transform.SetParent(root.transform, false);
            SpriteRenderer bgSr = bg.AddComponent<SpriteRenderer>();
            bgSr.sprite = BattleArt.WhiteSprite;
            bgSr.color = new Color(0f, 0f, 0f, 0.6f);
            bgSr.sortingOrder = order;
            bg.transform.localScale = new Vector3(width + 0.08f, 0.2f, 1f);

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(root.transform, false);
            SpriteRenderer fillSr = fill.AddComponent<SpriteRenderer>();
            fillSr.sprite = BattleArt.WhiteSprite;
            fillSr.color = color;
            fillSr.sortingOrder = order + 1;
            bar._fill = fill.transform;
            bar._fill.localScale = new Vector3(width, 0.14f, 1f);
            return bar;
        }

        public void SetRatio(float r) { _wanted = Mathf.Clamp01(r); }
        public void SetOffsetY(float y) { _offset.y = y; }

        private void LateUpdate()
        {
            if (_target == null || !_target.gameObject.activeInHierarchy)
            {
                Destroy(gameObject);
                return;
            }
            transform.position = _target.position + _offset;
            _displayed = Mathf.MoveTowards(_displayed, _wanted, Time.deltaTime * 2.5f);
            _fill.localScale = new Vector3(_width * _displayed, _fill.localScale.y, 1f);
            _fill.localPosition = new Vector3(-_width * 0.5f * (1f - _displayed), 0f, 0f);
        }
    }
}
