using UnityEngine;

namespace IdleBattle
{
    // Thanh máu boss trên đỉnh màn hình (parent theo camera).
    public class BossHealthBar : MonoBehaviour
    {
        private MonsterObject _boss;
        private Transform _fill;
        private float _width;
        private float _displayed = 1f;

        public static BossHealthBar Create(Camera cam, MonsterObject boss, string bossName)
        {
            GameObject anchor = new GameObject("BossHealthBar");
            anchor.transform.SetParent(cam.transform, false);
            float hh = cam.orthographicSize;
            float hw = cam.orthographicSize * cam.aspect;
            anchor.transform.localPosition = new Vector3(0f, hh * 0.8f, 10f);

            BossHealthBar b = anchor.AddComponent<BossHealthBar>();
            b._boss = boss;
            b._width = hw * 1.4f;

            GameObject bg = new GameObject("BG"); bg.transform.SetParent(anchor.transform, false);
            SpriteRenderer bgSr = bg.AddComponent<SpriteRenderer>();
            bgSr.sprite = BattleArt.WhiteSprite; bgSr.color = new Color(0f, 0f, 0f, 0.7f); bgSr.sortingOrder = 950;
            bg.transform.localScale = new Vector3(b._width + 0.12f, 0.32f, 1f);

            GameObject fill = new GameObject("Fill"); fill.transform.SetParent(anchor.transform, false);
            SpriteRenderer fSr = fill.AddComponent<SpriteRenderer>();
            fSr.sprite = BattleArt.WhiteSprite; fSr.color = new Color(0.85f, 0.15f, 0.15f); fSr.sortingOrder = 951;
            b._fill = fill.transform; b._fill.localScale = new Vector3(b._width, 0.24f, 1f);

            TextMesh t = BattleArt.CreateText("Name", anchor.transform, 50, cam.orthographicSize * 0.012f, Color.white, 952);
            t.text = bossName; t.transform.localPosition = new Vector3(0f, 0.33f, 0f);
            return b;
        }

        private void LateUpdate()
        {
            if (_boss == null || _boss.IsDeath) { Destroy(gameObject); return; }
            float r = Mathf.Clamp01(_boss.HealthPercent);
            _displayed = Mathf.MoveTowards(_displayed, r, Time.deltaTime * 2f);
            _fill.localScale = new Vector3(_width * _displayed, _fill.localScale.y, 1f);
            _fill.localPosition = new Vector3(-_width * 0.5f * (1f - _displayed), 0f, 0f);
        }
    }
}
