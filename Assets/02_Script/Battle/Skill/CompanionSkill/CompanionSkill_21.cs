using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 내 체력의 n% 실드를 발동한다.
// (예외처리: 실드 체력을 초과하는 피해를 입었을 시 1회 타격 무효화해줌)
public class CompanionSkill_21 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        var amount = _owner.Stat[Enum_StatType.Health] * Value_1;
        var time = _specData.skillDuration;
        
        _owner.TakeShield(amount,time);
        
        StartCoroutine(FollowOwner(time));
    }
    
    public override float GetSkillCoolDown()
    {
        // CE OPTION SKILL, 동료 스킬 쿨다운 감소
        float extraValue = 0;
        
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            extraValue = specSubStat.subValue;
        }    

        float skillCooldown = _specData.skillCooldown - extraValue <= 0 ? _specData.skillCooldown : _specData.skillCooldown - extraValue;
        
        return skillCooldown;
    }

    protected override IEnumerator SkillDelayCoroutine()
    { 
        yield return GetDelay(SkillEffectPreset == null ? 0 : SkillEffectPreset.VFXDelay, true);
        
        var targets = GetTargetMonsters();
        
        SetSkillPosition(targets);
        PlaySkillVFX();
        PlaySkillSound();
        
        // Delay
        yield return GetDelay(SkillEffectPreset == null ? 0 : SkillEffectPreset.DamageDelay, true);
        
        PlaySkillEffect();

        Active(targets);
        OnActive(targets);
        
        yield return new WaitUntil(() => _owner.CurrentShield <= 0 || _owner.ShieldTime <= 0);
        
        _owner.RemoveShield();
        Hide();
    }
}
