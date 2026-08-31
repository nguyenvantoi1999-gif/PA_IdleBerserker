using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 		현재 생명력의 30%를 소모하고 8초 동안 생명력 흡수가 (15+2*SLV)% 증가한다.
 // - > 현재 생명력의 30%를 소모하고 8초 동안 치명타 피해량을 30% 증가한다.
public class PlayerSkill_15_Active : PlayerActiveSkill
{
    protected override void Active(List<CharacterObject> targets)
    {
        _owner.UseHealth(0.3f);
        
        _owner.AddBuff(new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(),_specData.time).AddStatusEffect(Enum_Good_Status_Effect.IncreaseCriticalDamage, Value));

        StartCoroutine(FollowOwner());
    }
}
