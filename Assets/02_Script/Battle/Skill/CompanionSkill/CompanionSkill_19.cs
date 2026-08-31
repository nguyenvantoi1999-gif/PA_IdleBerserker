using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 모든 적에게 치명타 n% 공격, 광폭화 상태라면 모든 적에 치명타 n+m% 공격
public class CompanionSkill_19 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null && targets.Count > 0)
        {
            var damage = GetPlayerCriticalDamage(IdleBattle.Enum_CriticalType.Critical);
            if (_owner.GetAbility<BerserkAbility>().IsBerserkMode)
            {
                damage.Value *= (Value_1 + Value_2);
            }
            else
            {
                damage.Value *= Value_1;
            }
            
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
