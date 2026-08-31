using UnityEngine;

namespace IdleBattle
{
    // Helper tĩnh: dựng phông nền demo (sky/ground quad) theo camera hiện tại.
    public static class BattleStage
    {
        public static void BuildEnvironment(Camera cam)
        {
            if (cam == null) { return; }
            float camX = cam.transform.position.x;
            float camY = cam.transform.position.y;
            float size = cam.orthographicSize;
            float halfW = size * cam.aspect;
            float w = halfW * 2f + 20f;
            Quad("Sky", camX, camY + size, w, size * 2f, new Color(0.16f, 0.19f, 0.32f), -100);
            Quad("Ground", camX, -3.0f, w, 6.0f, new Color(0.13f, 0.12f, 0.15f), -80);
            Quad("GroundTop", camX, 0.02f, w, 0.12f, new Color(0.32f, 0.28f, 0.22f), -70);
        }

        private static void Quad(string name, float x, float y, float w, float h, Color color, int order)
        {
            GameObject go = new GameObject(name);
            go.transform.position = new Vector3(x, y, 0f);
            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = MakeWhiteSprite();
            sr.color = color;
            sr.sortingOrder = order;
            go.transform.localScale = new Vector3(w, h, 1f);
        }

        private static Sprite _white;
        private static Sprite MakeWhiteSprite()
        {
            if (_white == null)
            {
                Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
                Color[] px = { Color.white, Color.white, Color.white, Color.white };
                tex.SetPixels(px); tex.Apply();
                _white = Sprite.Create(tex, new Rect(0, 0, 2, 2), new Vector2(0.5f, 0.5f), 2f);
            }
            return _white;
        }
    }
}
