using System.Collections.Generic;
using UnityEngine;

namespace IdleBattle
{
    // Driver: instantiate player skill từ prefab, cập nhật hồi chiêu và để UI/AutoUse kích hoạt.
    // Mirror của CompanionSkillManager. Không tự dựng canvas runtime (UI xử lý riêng).
    public class PlayerSkillDriver : CharacterAbility
    {
        public GameObject[] SkillPrefabs;
        public Font SkillFont;
        public bool AutoUse = false;
        public float AutoUseInterval = 1f;   // giay giua 2 lan tu dung skill khi Auto
        private float _autoTimer;

        private readonly List<PlayerActiveSkill> _skills = new List<PlayerActiveSkill>();
        private readonly List<Sprite> _icons = new List<Sprite>();

        public IReadOnlyList<PlayerActiveSkill> Skills { get { return _skills; } }
        public IReadOnlyList<Sprite> Icons { get { return _icons; } }

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
                PlayerActiveSkill skill = instance.GetComponent<PlayerActiveSkill>();
                if (skill == null)
                {
                    Debug.LogWarning($"Player skill prefab '{skillPrefab.name}' has no PlayerActiveSkill.", skillPrefab);
                    Destroy(instance);
                    continue;
                }

                SpecSkill spec = SpecDataManager.Instance.GetSpecSkill(i);
                skill.InitSkill(spec, owner);
                skill.Hide();
                _skills.Add(skill);

                SkillEffectPreset preset = skill.SkillEffectPreset;
                _icons.Add(preset != null ? preset.Icon : null);
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
                PlayerActiveSkill skill = _skills[i];
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
                        PlayerActiveSkill skill = _skills[i];
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
