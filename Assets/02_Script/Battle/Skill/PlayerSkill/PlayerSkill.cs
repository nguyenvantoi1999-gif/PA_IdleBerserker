using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using IdleBattle;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSkill : GameBehaviour
{
    [Header("스킬 VFX")]
    [SerializeField]
    protected ParticleSystem _skillParticle;
    [SerializeField]
    protected SkeletonAnimation _skillSpine;
    [SerializeField]
    protected Animator _skillAnimator;

    [Space(20)]

    // 데이터 변수
    protected BerserkerObject _owner;

    protected SpecSkill _specData;

    protected float _skillCooldownTimer;

    protected float _skillCooldown;

    private int _level;

    public int FieldId => _specData.fieldID;

    public float RemainSkillCooldownTime => _skillCooldownTimer;
    public float RemainCoolTimeNormalized => 1 - (_skillCooldownTimer / _skillCooldown);

    // > 시간을 어디는 time이고 어디는 value 값을 사용해서 개별적으로 잘 확인하고 사용
    public float DurationBySpec => _specData.time + _extraDuratoin;
    public float DurationByValue => Value + _extraDuratoin;
    private float _extraDuratoin => PlayerDataManager.SkillData.petSubListValueList[_specData.fieldID].ExtraDuration;
    private float _extraCoolDown => PlayerDataManager.SkillData.petSubListValueList[_specData.fieldID].ExtraCoolDown;

    public virtual float Value
    {
        get
        {
            return GetSkillLevel() * _specData.increaseValue + _specData.value;
        }
    }

    public virtual float SubValue
    {
        get
        {
            return GetSkillLevel() * _specData.increaseSubValue + _specData.subValue;
        }
    }

    private int GetSkillLevel()
    {
        var level = 0;

        if (_isPlayer)
        {
            if (!BattleManager.Instance.IsEventWorld())
            {
                int skillLevel = PlayerDataManager.SkillData.Levels[_specData.fieldID] - 1;
                int maxLevel = PlayerDataManager.SkillData.GetSkillMaxLevel();
                level = Mathf.Clamp(skillLevel, 0, maxLevel - 1);
            }
            else
            {
                level = PlayerDataManager.EWPresetGrowthData.Levels[_specData.fieldID] - 1;
            }
        }
        else
        {
            level = _level;
        }

        return level;
    }

    [NonSerialized] public bool IsPassive;

    private bool _isToolScene;
    protected bool _isPlayer;

    public virtual void InitSkill(SpecSkill specData, BerserkerObject owner)
    {
        _specData = specData;
        _owner = owner;
        _skillCooldown = specData.cooldown;
        _skillCooldownTimer = 0;

        IsPassive = specData.isPassive;
        _isPlayer = owner is PlayerObject;

        _isToolScene = (SceneManager.GetActiveScene().name.Contains("Tool"));
    }

    //Todo: PVP용 레벨 데이터
    public void SetLevel(int level)
    {
        if (_isPlayer)
        {
            return;
        }

        _level = level;
    }

    public bool IsSkillEnable()
    {
#if UNITY_EDITOR
        if (PlayerSkillManager.Instance.ZeroCoolTime)
        {
            return true;
        }
#endif
        return _skillCooldownTimer <= 0;
    }

    public void UpdateCoolTime(float deltaTime)
    {
        _skillCooldownTimer -= deltaTime;
    }

    public virtual void ForceUseSkill()
    {
    }

    public virtual bool TryUseSkill()
    {
        return false;
    }

    public void ResetSkill()
    {
        ResetCoolDownTimer();
        Hide();
        StopAllCoroutines();
        _skillCooldownTimer = 0;
    }

    public Damage GetPlayerSkillDamage()
    {
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();

        damage.Value *= (1 + _owner.Stat[Enum_StatType.SkillDamage]);

        if (UtilCode.GetChance((float)(_owner.Stat[Enum_StatType.SkillCriticalChance] * 100)))
        {
            damage.Value *= (1 + _owner.Stat[Enum_StatType.SkillCriticalDamage]);
            damage.CriticalType = IdleBattle.Enum_CriticalType.Critical;
        }

        damage.DamageType = IdleBattle.Enum_DamageType.Skill;


        return damage;
    }

    public void ResetCoolDownTimer()
    {
        _skillCooldown = GetSkillCoolDown();

        _skillCooldownTimer = _isToolScene ? 0 : _skillCooldown;
    }

    public float GetSkillCoolDown()
    {
        return (_specData.cooldown - _extraCoolDown) * (1 - (float)_owner.Stat[Enum_StatType.CoolDownReduce]);
    }

    public virtual void Hide()
    {
        if (_skillParticle != null)
        {
            _skillParticle.gameObject.SetActive(false);
        }
        if (_skillAnimator != null)
        {
            _skillAnimator.gameObject.SetActive(false);
        }
        if (_skillSpine != null)
        {
            _skillSpine.gameObject.SetActive(false);
        }
    }

    protected void PlaySkillVFX()
    {
        if (_skillParticle != null)
        {
            _skillParticle.gameObject.SetActive(true);
            _skillParticle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            _skillParticle.Play();
            return;
        }

        if (_skillAnimator != null)
        {
            _skillAnimator.gameObject.SetActive(false);
            _skillAnimator.gameObject.SetActive(true);
            return;
        }

        if (_skillSpine != null)
        {
            _skillSpine.gameObject.SetActive(false);
            _skillSpine.gameObject.SetActive(true);
            _skillSpine.Initialize(true);
            return;
        }

        PCDebug.LogError($"{_specData.fieldID}번 스킬 VFX가 없습니다.");
    }

    protected void Attack(CharacterObject target)
    {
        var damage = GetPlayerSkillDamage();

        damage.Value *= (Value / 100f);

        target.TryTakeHit(damage, _owner);
    }

    public string GetSkillName()
    {
        return Localize.GetString($"Skill_{_specData.fieldID:D2}");
    }
}
