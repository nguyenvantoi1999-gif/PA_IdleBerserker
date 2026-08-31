using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 적이 사망할 때마다 3초 동안 치명타 피해량과 이동속도가 (15+SLV)% 증가한다.
// -->  적이사망할때마다3초동안치명타피해량이14%증가하고치명타확률이10%증가한다.
public class PlayerSkill_24_Passive : PlayerPassiveSkill
{
    public override void OnEnemyKill()
    {
        var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseCriticalDamage, Value)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseCriticalChance, SubValue);
        
        _owner.AddBuff(buff);
    }
}
