using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 모든 범위의 몬스터에게 n초간 지속 피해를 입힌다 ( 공격력의 n % )
public class CompanionSkill_10 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null)
        {
            foreach (var characterObject in targets)
            {
                if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
                {
                    continue;
                }

                OnTargetHit(characterObject);
            }
        }
    }

    protected override void OnTargetHit(CharacterObject character)
    {
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
        var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), _specData.skillDuration)
            .AddStatusEffect(Enum_Bad_Status_Effect.Poison, (Value_1 * damage.Value)); 
        character.AddBuff(buff);
        
        // CE OPTION SKILL, 모든 적의 공격력 및 공격속도 n% 감소
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
            var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, _specData.skillDuration)
                .AddStatusEffect(Enum_Bad_Status_Effect.ReduceDamage, specSubStat.value * 100)
                .AddStatusEffect(Enum_Bad_Status_Effect.ReduceAttackSpeed, specSubStat.value * 100);
            character.AddBuff(CESkillOption);
        }    
    }
}
