using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;

// 공격력이 (15+2*SLV)% 증가한다.
public class PlayerSkill_18_Passive : PlayerPassiveSkill
{
    public override void OnEquip()
    {
        _owner.AddBuff(
            new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), -1)
                .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value));
    }

    public override void OnUnEquip()
    {
        _owner.RemoveBuff(_specData.GetBuffID());
    }
}