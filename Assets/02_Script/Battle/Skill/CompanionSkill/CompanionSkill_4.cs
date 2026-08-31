using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 체력의 n%로 단일 타겟을 공격
public class CompanionSkill_4 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    private Coroutine _coroutine;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null && targets.Count > 0)
        {
            var damage = GetDamagePerPlayerHealth();
            damage.Value *= Value_1;
            
            var characterObject = targets[0];
            
            if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
            {
                return;
            }
            
            targets[0].TryTakeHit(damage, _owner);
        }
        
        
        // CE OPTION SKILL, m초 동안 생명력 비례 공격력 n% 증가 시킨다.
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
            var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, specSubStat.duration)
                .AddStatusEffect(Enum_Good_Status_Effect.StatDamagePerHealthUp, specSubStat.value);
            _owner.AddBuff(CESkillOption);
                
        }    
    }
}
