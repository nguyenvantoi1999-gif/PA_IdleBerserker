using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;


// 모든 적에게 공격력 n~m%의 피해를 랜덤으로 입힌다.
// Todo. 모두 랜덤? 전체 랜덤
public class CompanionSkill_8 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null)
        {
            var minFactor = 0.5f;
            var maxFactor = 1;
            var ranRatio = Random.Range(minFactor, maxFactor);
            
            var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
            damage.Value *= Value_1 * ranRatio;
            
            // CE OPTION SKILL, 몬스터에게 광폭화 충격 피해 * n%의 도트 데미지
            bool isCEEquipped = IsCEEquipped();
            
            foreach (var characterObject in targets)
            {
                if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
                {
                    continue;
                }

                characterObject.TryTakeHit(damage, _owner);
                
                if(isCEEquipped)
                {
                    var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                    var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
                    var basicDamage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
                    var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, specSubStat.duration)
                        .AddStatusEffect(Enum_Bad_Status_Effect.Poison, (basicDamage.Value * specSubStat.value)); 
                    characterObject.AddBuff(CESkillOption);
                }
            }
        }
    }
}
