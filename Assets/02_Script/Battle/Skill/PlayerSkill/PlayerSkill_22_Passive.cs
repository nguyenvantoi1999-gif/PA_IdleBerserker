using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 광폭화 상태가 해제된 후 4초 동안 공격속도가 (30+2*SLV)% 증가한다.
public class PlayerSkill_22_Passive : PlayerPassiveSkill
{
    public override void OnBerserkEnd()
    {
        var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseAttackSpeed, Value);
        
        _owner.AddBuff(buff);
    }
}
