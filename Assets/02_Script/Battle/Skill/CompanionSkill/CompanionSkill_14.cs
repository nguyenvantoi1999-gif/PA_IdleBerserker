using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 치명타 공격력의 n%로 전체 범위 몬스터를 공격
public class CompanionSkill_14 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null)
        {
            var damage = GetPlayerCriticalDamage(IdleBattle.Enum_CriticalType.Critical);
            damage.Value *= Value_1;
         
            foreach (var characterObject in targets)
            {
                if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
                {
                    continue;
                }

                characterObject.TryTakeHit(damage, _owner);
            }
        }
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
