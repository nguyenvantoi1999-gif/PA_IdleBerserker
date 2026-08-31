using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 단일 적에게 n% 피해를 입힘, 광폭화 상태라면 n+m% 피해 공격
public class CompanionSkill_20 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null && targets.Count > 0)
        {
            var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
            if (_owner.GetAbility<BerserkAbility>().IsBerserkMode)
            {
                damage.Value *= (Value_1 + Value_2);
            }
            else
            {
                damage.Value *= Value_1;
            }
            
            var characterObject = targets[0];
            
            if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
            {
                return;
            }

            characterObject.TryTakeHit(damage, _owner);
            
            if (IsCEEquipped())
            {
                var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
                var basicDamage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
                basicDamage.Value *= specSubStat.value;
                var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, specSubStat.duration)
                    .AddStatusEffect(Enum_Bad_Status_Effect.Poison, basicDamage.Value);
                characterObject.AddBuff(CESkillOption);
            }    
        }
    }
}
