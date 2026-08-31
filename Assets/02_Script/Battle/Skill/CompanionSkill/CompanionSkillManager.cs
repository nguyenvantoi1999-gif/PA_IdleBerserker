using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    // Driver: instantiate companion skill từ preset, cập nhật hồi chiêu và để UI kích hoạt.
    public class CompanionSkillManager : CharacterAbility
    {
        public GameObject[] SkillPrefabs;
        public Font SkillFont;
        public bool BuildRuntimeSkillBar = false;   // để false: dùng UI kéo-thả trong scene thay cho bar auto
        public bool AutoUse = false;                // bật/tắt tự dùng skill khi hồi xong (ToggleAutoSkill)
        public float AutoUseInterval = 1f;   // giay giua 2 lan tu dung skill khi Auto
        private float _autoTimer;

        private readonly List<CompanionActiveSkill> _skills = new List<CompanionActiveSkill>();
        private readonly List<Sprite> _icons = new List<Sprite>();

        public IReadOnlyList<CompanionActiveSkill> Skills { get { return _skills; } }
        public IReadOnlyList<Sprite> Icons { get { return _icons; } }

        public override void Init()
        {
            base.Init();
            BattleArt.Font = SkillFont;
        }

        public override void LateInit()
        {
            base.LateInit();

            BerserkerObject owner = _ownerObject as BerserkerObject;
            if (owner == null || SkillPrefabs == null)
            {
                return;
            }

            for (int i = 0; i < SkillPrefabs.Length; i++)
            {
                GameObject skillPrefab = SkillPrefabs[i];
                if (skillPrefab == null)
                {
                    continue;
                }

                GameObject instance = Instantiate(skillPrefab, owner.transform);
                CompanionActiveSkill skill = instance.GetComponent<CompanionActiveSkill>();
                if (skill == null)
                {
                    Debug.LogWarning($"Companion skill prefab '{skillPrefab.name}' has no CompanionActiveSkill.", skillPrefab);
                    Destroy(instance);
                    continue;
                }

                SkillEffectPreset preset = skill.SkillEffectPreset;
                if (preset == null)
                {
                    Debug.LogWarning($"Companion skill prefab '{skillPrefab.name}' has no SkillEffectPreset.", skillPrefab);
                    Destroy(instance);
                    continue;
                }

                skill.InitSkill(preset.CreateCompanionSpec(), owner);
                skill.Hide();
                _skills.Add(skill);
                _icons.Add(preset.Icon);
            }

            if (BuildRuntimeSkillBar)
            {
                CompanionSkillBar.Create(_skills, _icons, SkillFont);
            }
        }

        public override void ProcessAbility(float deltaTime)
        {
            if (BattleManager.Instance.State != Enum_BattleState.Start)
            {
                return;
            }

            for (int i = 0; i < _skills.Count; i++)
            {
                CompanionActiveSkill skill = _skills[i];
                if (skill == null)
                {
                    continue;
                }

                skill.UpdateCoolTime(deltaTime);
            }

            if (AutoUse)
            {
                _autoTimer -= deltaTime;
                if (_autoTimer <= 0f)
                {
                    for (int i = 0; i < _skills.Count; i++)
                    {
                        CompanionActiveSkill skill = _skills[i];
                        if (skill != null && skill.IsSkillEnable() && skill.TryUseSkill())
                        {
                            _autoTimer = AutoUseInterval;   // gian cach giua 2 lan
                            break;
                        }
                    }
                }
            }
        }
    }
}
