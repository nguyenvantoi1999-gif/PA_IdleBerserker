using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 윤지수님
// {1}초간 경험치 획득량과 공격력을 {0}%증가시킨다.
// 전용 스킬의 지속시간 동안 회심의 일격 확률이 {0}% 증가한다.
public class CompanionSkill_32 : CompanionActiveSkill
{
    protected override void Active(List<CharacterObject> targets)
    {
        var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), _specData.skillDuration)
            .AddStatusEffect(Enum_Good_Status_Effect.ExpUp, Value_1 * 100) // 버프 경우에 적용하는 곳에서 나누기 100을 하기 때문에 예외적으로 * 100을 해서 사용
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value_1 * 100);
        
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
            var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, _specData.skillDuration)
                .AddStatusEffect(Enum_Good_Status_Effect.StatSuperCriticalChance, specSubStat.value);
            _owner.AddBuff(CESkillOption);
        }    
        
        _owner.AddBuff(buff);
    }
}