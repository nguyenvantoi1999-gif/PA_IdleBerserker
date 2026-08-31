using System.Collections;
using System.Collections.Generic;
using IdleBattle;
using UnityEngine;
// 10초동안 공격력이 n%, 치명타 피해량이 m% 증가한다.
public class PlayerSkill_32_Active : PlayerActiveSkill
{ 
    protected override void Active(List<CharacterObject> targets)
    {
        Buff buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), DurationBySpec)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseCriticalDamage, SubValue);
        
        _owner.AddBuff(buff);

        StartCoroutine(FollowOwner());
    }
}
