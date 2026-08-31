using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 5초 동안 받는 피해가 (20+SLV)% 감소하고 자신에게 적용되는 상태이상 효과를 50% 확률로 무시한다.
public class PlayerSkill_17_Active : PlayerActiveSkill
{
    protected override void Active(List<CharacterObject> targets)
    {
        var buff = new Buff(Enum_BuffFrom.Skill,_specData.GetBuffID(), _specData.time)
            .AddStatusEffect(Enum_Good_Status_Effect.ReduceHitDamage, Value)
            .AddStatusEffect(Enum_Good_Status_Effect.IgnoreDeBuff, 100);
        
        _owner.AddBuff(buff);
        
        StartCoroutine(FollowOwner());
    }
}
