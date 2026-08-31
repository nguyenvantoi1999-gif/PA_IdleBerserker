using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 광폭화 상태에 돌입 후 4초 동안 치명타 피해량이 (30+3*SLV)% 증가한다
public class PlayerSkill_20_Passive : PlayerPassiveSkill
{
    public override void OnBerserkStart()
    {
        var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseCriticalDamage, Value);
        
        _owner.AddBuff(buff);
    }
}
