using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;


// 공격력 n%의 범위형 공격을 시전하고, 처치한 적 만큼 체력을 회복한다.
public class CompanionSkill_7 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        var beforeDeathCount = targets.Count(x => x != null && x.isActiveAndEnabled && x.IsDeath);

        for (int i = 0; i < targets.Count; i++)
        {
            if (!targets[i].isActiveAndEnabled || targets[i].IsDeath)
            {
                continue;
            }

            Attack(targets[i]);
        }

        var afterDeathCount = targets.Count(x => x != null && x.isActiveAndEnabled && x.IsDeath);

        var diff = afterDeathCount - beforeDeathCount;
        
        if (diff >= 1)
        {
            var healAmount = Value_2 * _owner.Stat[Enum_StatType.Health] * diff;
            _owner.TakeRecovery(healAmount);
        
            // CE OPTION SKILL, 처치한 적의 수 * n% 만큼 공격 속도 증가
            
            if (IsCEEquipped())
            {
                var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
                var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, specSubStat.duration)
                    .AddStatusEffect(Enum_Good_Status_Effect.IncreaseAttackSpeed, specSubStat.value * diff * 100);
                _owner.AddBuff(CESkillOption);
                    
            }    
        }
    }
}
