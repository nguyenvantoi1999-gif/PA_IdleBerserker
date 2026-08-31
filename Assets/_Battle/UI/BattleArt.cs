using UnityEngine;

namespace IdleBattle
{
    public static class BattleArt
    {
        private static Sprite _white;
        private static Font _font;

        public static Sprite WhiteSprite
        {
            get
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

        public static Font Font { get { return _font; } set { _font = value; } }

        public static TextMesh CreateText(string name, Transform parent, int fontSize, float charSize, Color color, int order)
        {
            GameObject go = new GameObject(name);
            if (parent != null) { go.transform.SetParent(parent, false); }
            TextMesh tm = go.AddComponent<TextMesh>();
            Font f = Font;
            if (f != null) { tm.font = f; go.GetComponent<MeshRenderer>().sharedMaterial = f.material; }
            tm.fontSize = fontSize;
            tm.characterSize = charSize;
            tm.color = color;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            go.GetComponent<MeshRenderer>().sortingOrder = order;
            return tm;
        }
    }
}
