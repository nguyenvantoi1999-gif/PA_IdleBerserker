using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 단일 적에게 n% 피해를 입힘, 상태 이상이 걸려 있는 적에게는 m%의 피해를 입힌다.
public class CompanionSkill_23 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetElementDamage(Enum_Element.Fire,false);
        damage.DamageType = IdleBattle.Enum_DamageType.Fire;

        if (targets != null && targets.Count > 0)
        {
            var characterObject = targets[0];

            if (characterObject.gameObject.GetComponent<BuffAbility>().DeBuffCount > 0)
            {
                damage.Value *= Value_2;
            }
            else
            {
                damage.Value *= Value_1;
            }

            if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
            {
                return;
            }

            characterObject.TryTakeHit(damage, _owner);


            // CE OPTION SKILL, m초 동안 모든 적에게 마계 n% 추가 데미지
            if (IsCEEquipped())
            {
                if (characterObject == null)
                {
                    return;
                }

                var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat()
                    .First(x => x.companionIndex == _specData.fieldID);
                var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
                var elementDamage = GetPlayerElementDamage(Enum_Element.Fire);
                elementDamage.Value *= specSubStat.value;
                elementDamage.DamageType = IdleBattle.Enum_DamageType.Fire;

                var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, specSubStat.duration)
                    .AddStatusEffect(Enum_Bad_Status_Effect.Poison, elementDamage.Value);
                characterObject.AddBuff(CESkillOption);
            }
        }
    }
}
