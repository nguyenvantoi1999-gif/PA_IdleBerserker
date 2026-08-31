using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

public class CompanionSkill_3 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        // CE OPTION SKILL, 스킬 사용 동안 치명타 확률 증가
        if(PlayerDataManager.CompanionData.GetCECache(_specData.fieldID).TryGetValue(Enum_CompanionEquipmentType.CE, out var CEdata))
        {
            if (CEdata != null && CEdata.level >= 4)
            {
                var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
                var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, _specData.skillDuration)
                    .AddStatusEffect(Enum_Good_Status_Effect.IncreaseCriticalChance, specSubStat.value * 100);
                _owner.AddBuff(CESkillOption);
            }    
        }
        
        var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), _specData.skillDuration)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value_1 * 100) // 버프 경우에 적용하는 곳에서 나누기 100을 하기 때문에 예외적으로 * 100을 해서 사용
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseCriticalDamage, Value_1 * 100);
        
        _owner.AddBuff(buff);
    }
}