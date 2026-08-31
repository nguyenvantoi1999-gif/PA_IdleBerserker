using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 	10초 동안 공격력이 (30+2*SLV)% 증가한다.
public class PlayerSkill_14_Active : PlayerActiveSkill
{
    protected override void Active(List<CharacterObject> targets)
    {
        _owner.AddBuff(new Buff(Enum_BuffFrom.Skill,_specData.GetBuffID(), DurationBySpec).AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value));

        StartCoroutine(FollowOwner());
    }
}
