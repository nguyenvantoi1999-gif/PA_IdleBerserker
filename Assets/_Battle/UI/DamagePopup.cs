using UnityEngine;

namespace IdleBattle
{
    // Số damage bay lên rồi mờ dần.
    public class DamagePopup : MonoBehaviour
    {
        private TextMesh _text;
        private float _life;
        private float _maxLife = 0.7f;
        private Vector3 _vel;
        private Color _color;

        public static void Spawn(Vector3 pos, string msg, Color color, float size)
        {
            GameObject go = new GameObject("DamagePopup");
            go.transform.position = pos;
            DamagePopup p = go.AddComponent<DamagePopup>();
            TextMesh tm = BattleArt.CreateText("txt", go.transform, 70, size * 0.14f, color, 600);
            tm.text = msg;
            p._text = tm;
            p._color = color;
            p._vel = new Vector3(Random.Range(-0.8f, 0.8f), 2.8f, 0f);
        }

        private void Update()
        {
            _life += Time.deltaTime;
            transform.position += _vel * Time.deltaTime;
            _vel.y -= 5f * Time.deltaTime;
            float t = _life / _maxLife;
            Color c = _color; c.a = Mathf.Clamp01(1f - t);
            _text.color = c;
            transform.localScale = Vector3.one * Mathf.Lerp(1.15f, 0.85f, t);
            if (_life >= _maxLife) { Destroy(gameObject); }
        }
    }
}
