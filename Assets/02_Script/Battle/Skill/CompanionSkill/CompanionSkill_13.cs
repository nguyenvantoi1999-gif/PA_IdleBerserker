using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 회심의 일격의 n%로 단일 타겟을 공격
public class CompanionSkill_13 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets == null || targets.Count <= 0)
        {
            return;
        }
        
        var damage = GetPlayerCriticalDamage(IdleBattle.Enum_CriticalType.SuperCritical);
        damage.Value *= Value_1;
        
        var characterObject = targets[0];
            
        if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
        {
            return;
        }

        characterObject.TryTakeHit(damage, _owner);
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
}
