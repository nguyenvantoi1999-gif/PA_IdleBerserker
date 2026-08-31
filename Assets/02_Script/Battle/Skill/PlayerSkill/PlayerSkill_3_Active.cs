using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 가장 가까운 적에게 공격력의 (300+25*SLV)% 피해를 입히고 5초 동안 공격력을 50% 감소시킨다.
public class PlayerSkill_3_Active : PlayerActiveSkill
{
    protected override void OnTargetHit(CharacterObject character)
    {
        var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time).AddStatusEffect(Enum_Bad_Status_Effect.ReduceDamage, SubValue);
        character.AddBuff(buff);
    }
}
