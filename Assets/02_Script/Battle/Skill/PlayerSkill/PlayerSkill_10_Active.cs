using System.Collections;
using System.Collections.Generic;
using IdleBattle;
using UnityEngine;
// 	바닥에서 악의 손아귀가 나타나 6명의 적에게 공격력의 (230+15SLV)% 피해를 입히고 3초 동안 속박한다.
public class PlayerSkill_10_Active : PlayerActiveSkill
{

    protected override void Active(List<CharacterObject> targets)
    {
        for (int i = 0; i < targets.Count; ++i)
        {
            if (!targets[i].isActiveAndEnabled || targets[i].IsDeath)
            {
                continue;
            }

            var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time)
                .AddStatusEffect(Enum_Bad_Status_Effect.Binding, SubValue);
            targets[i].AddBuff(buff);
            Attack(targets[i]);
        }
    }
}
