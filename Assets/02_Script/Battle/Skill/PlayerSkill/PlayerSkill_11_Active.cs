using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 전방으로 심연의 파도를 일으켜 적중한 모든 적에게 공격력의 (350+20*SLV)% 피해를 입히고 5초 동안 이동속도를 50% 감소시킨다.
public class PlayerSkill_11_Active : PlayerActiveSkill
{

    protected override void Active(List<CharacterObject> targets)
    {
        for (int i = 0; i < targets.Count; ++i)
        {
            if (!targets[i].isActiveAndEnabled || targets[i].IsDeath)
            {
                continue;
            }

            var buff = new Buff(Enum_BuffFrom.Skill,_specData.GetBuffID(), _specData.time)
                .AddStatusEffect(Enum_Bad_Status_Effect.ReduceMoveSpeed, SubValue);
            targets[i].AddBuff(buff);
            Attack(targets[i]);
        }
    }
}
