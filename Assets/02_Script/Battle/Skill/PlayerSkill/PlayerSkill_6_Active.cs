using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 가장 가까운 적에게 검은 사슬을 날려 공격력의 (400+25*SLV)% 피해를 입히고 5초 동안 치명타 확률과 공격속도를 30% 감소시킨다.
public class PlayerSkill_6_Active : PlayerActiveSkill
{
    protected override void OnTargetHit(CharacterObject character)
    {
        var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time)
            .AddStatusEffect(Enum_Bad_Status_Effect.ReduceCriticalChance, SubValue)
            .AddStatusEffect(Enum_Bad_Status_Effect.ReduceAttackSpeed, SubValue);
        character.AddBuff(buff);
    }

}
