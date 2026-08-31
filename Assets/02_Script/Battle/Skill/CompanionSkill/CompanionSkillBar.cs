using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace IdleBattle
{
    public class CompanionSkillBar : MonoBehaviour
    {
        private const float SlotSize = 92f;
        private static Font _font;

        public static void Create(IList<CompanionActiveSkill> skills, IList<Sprite> icons, Font font)
        {
            if (skills == null || skills.Count == 0)
            {
                return;
            }

            _font = font;

            EnsureEventSystem();

            GameObject canvasObject = new GameObject("CompanionSkillCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            Canvas canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            GameObject bar = CreateUIObject("BottomSkillBar", canvasObject.transform);
            RectTransform barRect = bar.GetComponent<RectTransform>();
            barRect.anchorMin = new Vector2(0.5f, 0f);
            barRect.anchorMax = new Vector2(0.5f, 0f);
            barRect.pivot = new Vector2(0.5f, 0f);
            barRect.anchoredPosition = new Vector2(0f, 42f);
            barRect.sizeDelta = new Vector2(skills.Count * (SlotSize + 18f), SlotSize);

            HorizontalLayoutGroup layout = bar.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 18f;
            layout.childAlignment = TextAnchor.MiddleCenter;
            layout.childControlWidth = false;
            layout.childControlHeight = false;
            layout.childForceExpandWidth = false;
            layout.childForceExpandHeight = false;

            for (int i = 0; i < skills.Count; i++)
            {
                Sprite icon = icons != null && i < icons.Count ? icons[i] : null;
                CreateSlot(bar.transform, skills[i], icon);
            }
        }

        private static void CreateSlot(Transform parent, CompanionActiveSkill skill, Sprite icon)
        {
            GameObject slot = CreateUIObject($"CompanionSkill_{skill.FieldId}_Button", parent);
            RectTransform slotRect = slot.GetComponent<RectTransform>();
            slotRect.sizeDelta = new Vector2(SlotSize, SlotSize);

            Image frame = slot.AddComponent<Image>();
            frame.sprite = icon;
            frame.color = icon == null ? GetFallbackColor(skill.FieldId) : Color.white;
            frame.preserveAspect = true;

            Button button = slot.AddComponent<Button>();
            button.targetGraphic = frame;
            button.onClick.AddListener(() => skill.TryUseSkill());

            GameObject cooldown = CreateUIObject("Cooldown", slot.transform);
            Stretch(cooldown.GetComponent<RectTransform>());
            Image cooldownImage = cooldown.AddComponent<Image>();
            cooldownImage.color = new Color(0f, 0f, 0f, 0.72f);
            cooldownImage.type = Image.Type.Filled;
            cooldownImage.fillMethod = Image.FillMethod.Radial360;
            cooldownImage.fillOrigin = 2; // Radial360: Top
            cooldownImage.fillClockwise = false;

            Text cooldownText = CreateText("CooldownText", slot.transform, 30, FontStyle.Bold);
            Stretch(cooldownText.rectTransform);
            cooldownText.alignment = TextAnchor.MiddleCenter;
            cooldownText.color = Color.white;

            if (icon == null)
            {
                Text fallbackIcon = CreateText("SkillIcon", slot.transform, 28, FontStyle.Bold);
                Stretch(fallbackIcon.rectTransform);
                fallbackIcon.alignment = TextAnchor.MiddleCenter;
                fallbackIcon.text = $"SKILL\n{skill.FieldId}";
                fallbackIcon.color = Color.white;
                fallbackIcon.transform.SetAsFirstSibling();
            }

            CompanionSkillButton view = slot.AddComponent<CompanionSkillButton>();
            view.Init(skill, button, cooldownImage, cooldownText);
        }

        private static Text CreateText(string name, Transform parent, int size, FontStyle style)
        {
            GameObject go = CreateUIObject(name, parent);
            Text text = go.AddComponent<Text>();
            text.font = _font;
            text.fontSize = size;
            text.fontStyle = style;
            text.raycastTarget = false;
            return text;
        }

        private static GameObject CreateUIObject(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        private static Color GetFallbackColor(int fieldId)
        {
            Color color = Color.HSVToRGB(Mathf.Repeat(fieldId * 0.17f, 1f), 0.72f, 0.88f);
            color.a = 0.95f;
            return color;
        }

        private static void EnsureEventSystem()
        {
            if (EventSystem.current == null)
            {
                new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
        }
    }

    public class CompanionSkillButton : MonoBehaviour
    {
        private CompanionActiveSkill _skill;
        private Button _button;
        private Image _cooldownImage;
        private Text _cooldownText;

        public void Init(CompanionActiveSkill skill, Button button, Image cooldownImage, Text cooldownText)
        {
            _skill = skill;
            _button = button;
            _cooldownImage = cooldownImage;
            _cooldownText = cooldownText;
        }

        private void Update()
        {
            if (_skill == null)
            {
                return;
            }

            float remaining = Mathf.Max(0f, _skill.RemainSkillCooldownTime);
            bool ready = _skill.IsSkillEnable();
            _button.interactable = ready;
            _cooldownImage.enabled = !ready;
            _cooldownText.enabled = !ready;

            if (!ready)
            {
                _cooldownImage.fillAmount = Mathf.Clamp01(1f - _skill.RemainCoolTimeNormalized);
                _cooldownText.text = remaining.ToString("0.0");
            }
        }
    }
}
