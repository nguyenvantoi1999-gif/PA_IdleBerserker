using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using IdleBattle;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CompanionSkill : GameBehaviour
{
    [Header("스킬 VFX")]
    [SerializeField] protected ParticleSystem _skillParticle;
    [SerializeField] protected SkeletonAnimation _skillSpine;
    [SerializeField] protected Animator _skillAnimator;
    
    protected BerserkerObject _owner;
    protected SpecCompanionSkill _specData;
    protected float _skillCooldownTimer;
    protected float _skillCooldown;

    private bool _isToolScene;
    protected bool _isPlayer;
     
    // PVP 캐릭터용 스킬 데이터
    private int _level;
    private int _ceLevel;
    
    public int FieldId => _specData.fieldID;
    public float RemainSkillCooldownTime => _skillCooldownTimer;
    public float RemainCoolTimeNormalized => 1 - (_skillCooldownTimer / _skillCooldown);
    
    public virtual float Value_1
    {
        get
        {
            var level = GetCompanionLevel();
            // var starFactor = PlayerDataManager.CompanionData.GetCompanion(FieldId).GetStarFactor();
            return (level * _specData.effectValueIncrease_1 + _specData.effectValue_1);
        }
    }

    public virtual float Value_2
    {
        get
        {
            var level = GetCompanionLevel();
            // var starFactor = PlayerDataManager.CompanionData.GetCompanion(FieldId).GetStarFactor();
            return (level * _specData.effectValueIncrease_2 + _specData.effectValue_2);
        }
    }
    

    private int GetCompanionLevel()
    { 
        var level = 0;

        if (_isPlayer)
        {
            if (!BattleManager.Instance.IsEventWorld())
            {
                level = PlayerDataManager.CompanionData.GetCompanion(_specData.fieldID).Level - 1;
            }
            else
            {
                level = PlayerDataManager.EWPresetGrowthData.CompanionLevels[_specData.fieldID] - 1;
            }
        }
        else
        {
            level = _level;
        }

        return level;
    }

    public virtual void InitSkill(SpecCompanionSkill specData, BerserkerObject owner)
    {
        _specData = specData;
        _owner = owner;
        _skillCooldown = specData.skillCooldown;
        _skillCooldownTimer = 0;

        _isPlayer = owner is PlayerObject;
        
        _isToolScene = (SceneManager.GetActiveScene().name.Contains("Tool"));
    }

    public void SetSkillProfile(int level, int ceLevel)
    {
        _level = level;
        _ceLevel = ceLevel;
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

    public void SetCoolTime(float time)
    {
        _skillCooldownTimer = time;
    }
    
    public virtual void ResetSkill()
    {
        ResetCoolDownTimer();
        Hide();
        StopAllCoroutines();
        _skillCooldownTimer = 0;
    }

    public Damage GetPlayerSkillDamage()
    {
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
        damage.Value *= System.Math.Max(0, 1 + _owner.Stat[Enum_StatType.SkillDamage]);
        damage.DamageType = IdleBattle.Enum_DamageType.Skill;
        return damage;
    }

    protected Damage GetPlayerCriticalDamage(IdleBattle.Enum_CriticalType criticalType)
    {
        // var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
        // var value = _owner.GetAbility<BerserkerAttackAbility>().GetDamageValue(IdleBattle.Enum_DamageType.Normal, criticalType);
        // damage.Value = value;
        // return damage;
        var value = _owner.GetAbility<BerserkerAttackAbility>().GetDefaultDamage(IdleBattle.Enum_DamageType.Normal, criticalType);
        
        return value;
    }

    protected Damage GetPlayerElementDamage(Enum_Element element)
    {
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetElementDamage(element,false);
        
        return damage;
    }

    protected Damage GetPlayerShockWaveDamage()
    {
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
        damage.Value *= _owner.Stat[Enum_StatType.BerserkShockWave];

        return damage;
    }

    protected Damage GetDamagePerPlayerHealth()
    {
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData(Enum_StatType.Health);
        
        return damage;
    }

    protected void ResetCoolDownTimer()
    {
        _skillCooldown = GetSkillCoolDown();
        _skillCooldownTimer = _isToolScene ? 0 : _skillCooldown;
    }

    public void ReduceSkillCoolDown(float time)
    {
        _skillCooldownTimer -= time;
    }

    public virtual float GetSkillCoolDown()
    {
        // return _specData.skillCooldown * (1 - (float)_owner.Stat[Enum_StatType.CoolDownReduce]);
        return _specData.skillCooldown;
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

    protected virtual void PlaySkillVFX()
    {
        if (_skillParticle != null)
        {
            _skillParticle.gameObject.SetActive(true);
            _skillParticle.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
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
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
        damage.Value *= Value_1;
        DealSkillDamage(target, damage);
    }

    protected void DealSkillDamage(CharacterObject target, Damage damage)
    {
        if (target == null || target.IsDeath)
        {
            return;
        }

        target.TryTakeHit(damage, _owner);
        _owner.OnTargetAttack(target, damage);
    }

    public string GetSkillName()
    {
        return Localize.GetString($"Skill_{_specData.fieldID:D2}");
    }

    protected bool IsCEEquipped()
    {
        if (_isPlayer)
        {
            if (PlayerDataManager.CompanionData.GetCECache(_specData.fieldID).TryGetValue(Enum_CompanionEquipmentType.CE, out var CEdata))
            {
                if (CEdata != null && CEdata.level >= 4)
                {
                    return true;
                }
            }

            return false;
        }
        else
        {
            return _ceLevel >= 4;
        }
    }
    
    public virtual void OnRecovery()
    {
    }
}
