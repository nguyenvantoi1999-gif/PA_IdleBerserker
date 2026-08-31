using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 충격파 공격력의 n%로 단일 타겟을 공격
public class CompanionSkill_5 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null && targets.Count > 0)
        {
            var damage = GetPlayerShockWaveDamage();
            damage.Value *= Value_1;
            
            var characterObject = targets[0];
            
            if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
            {
                return;
            }
            
            characterObject.TryTakeHit(damage, _owner);
            
            // CE OPTION SKILL, 몬스터에게 광폭화 충격 피해 * n%의 도트 데미지
          
            if (IsCEEquipped())
            {
                var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
                var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, specSubStat.duration)
                    .AddStatusEffect(Enum_Bad_Status_Effect.Poison, (damage.Value * specSubStat.value)); 
                characterObject.AddBuff(CESkillOption);
            }
        }
    }
}
