using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 모든 범위의 적에게 13초 간 중독 지속 피해를 초당 {0}% 피해
// 중독의 걸린 적은 방어력이 {0}% 감소한다

public class CompanionSkill_36 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    private Coroutine _coroutine;
    private float _extraValue = 0;
    
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
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
            var elementDamage = GetPlayerElementDamage(Enum_Element.Grass);
            elementDamage.Value *= specSubStat.value;
            elementDamage.DamageType = IdleBattle.Enum_DamageType.Grass;
                        
            var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, specSubStat.duration)
                .AddStatusEffect(Enum_Bad_Status_Effect.Poison, elementDamage.Value);
            character.AddBuff(CESkillOption);
        }    
        
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
        var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), _specData.skillDuration)
            .AddStatusEffect(Enum_Bad_Status_Effect.PoisonDamage, (Value_1 * damage.Value));
        character.AddBuff(buff);
    }
}
