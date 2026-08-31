using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;


// 천계 공격력의 n%로 범위 공격 (전방 몬스터 2,3마리)
public class CompanionSkill_15 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;

    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null)
        {
            var damage = GetPlayerElementDamage(Enum_Element.Water);
            damage.Value *= Value_1;
            
            var beforeDeathCount = targets.Count(x => x != null && x.isActiveAndEnabled && x.IsDeath);

            foreach (var characterObject in targets)
            {
                if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
                {
                    continue;
                }

                characterObject.TryTakeHit(damage, _owner);
            }

            var afterDeathCount = targets.Count(x => x != null && x.isActiveAndEnabled && x.IsDeath);

            var diff = afterDeathCount - beforeDeathCount;

            if (diff >= 1)
            {
                // CE OPTION SKILL, 처치한 적의 수 * n% 만큼 천계 피해 증가
                
                if (IsCEEquipped())
                {
                    var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                    var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
                    var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, _specData.skillDuration)
                        .AddStatusEffect(Enum_Good_Status_Effect.StatWaterDamageMultiply, (diff * specSubStat.value));
                    _owner.AddBuff(CESkillOption);
                        
                }    
            }
            
            StartCoroutine(FollowOwner());
        }
    }
}
