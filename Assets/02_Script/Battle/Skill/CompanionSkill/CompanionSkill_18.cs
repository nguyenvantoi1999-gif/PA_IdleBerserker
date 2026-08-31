using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 자연계 공격력의 n%로 범위 공격
public class CompanionSkill_18 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null)
        {
            var damage = GetPlayerElementDamage(Enum_Element.Grass);
            damage.Value *= Value_1;
            foreach (var characterObject in targets)
            {
                if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
                {
                    continue;
                }

                characterObject.TryTakeHit(damage, _owner);
                
                // CE OPTION SKILL, m초 동안 모든 적에게 자연계 피혜량 n% 추가 데미지
                
                if (IsCEEquipped())
                {
                    var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                    var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
                    var elementDamage = GetPlayerElementDamage(Enum_Element.Grass);
                    elementDamage.Value *= specSubStat.value;
                    elementDamage.DamageType = IdleBattle.Enum_DamageType.Grass;
                        
                    var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, specSubStat.duration)
                        .AddStatusEffect(Enum_Bad_Status_Effect.Poison, elementDamage.Value);
                    characterObject.AddBuff(CESkillOption);
                        
                }    
            }
        }
    }
}
