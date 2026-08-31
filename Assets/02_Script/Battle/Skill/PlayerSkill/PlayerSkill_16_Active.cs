using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 	8초 동안 적에게 피해를 입을 때마다 (20+2*SLV)% 확률로 해당 피해를 되돌려준다.
// -> 6초 동안, 적에게 {0}% 피해를 입히고, 해당 스킬로 입힌 데미지의 {0}%만큼 생명력을 회복한다. 
// -> 3초동안,공격력이 {0}% 증가하고,최대HP의5%를회복한다.
public class PlayerSkill_16_Active : PlayerActiveSkill
{
    protected override void Active(List<CharacterObject> targets)
    {
        _owner.AddBuff(new Buff(Enum_BuffFrom.Skill,_specData.GetBuffID(),_specData.time)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value));
        
        var healAmount = (SubValue / 100) * _owner.Stat[Enum_StatType.Health];
        _owner.TakeRecovery(healAmount);

        StartCoroutine(FollowOwner());
    }
    
}
