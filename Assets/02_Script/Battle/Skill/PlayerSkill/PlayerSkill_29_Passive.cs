using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
//자신에게 걸려 있는 상태 이상 효과 1개당 공격력과 공격속도, 생명력 흡수가 (15+2*SLV)% 증가한다.
// -> 광폭화 시, 공격력과 공격속도가 20% 증가한다.
public class PlayerSkill_29_Passive : PlayerPassiveSkill
{
    // public override void OnEquip()
    // {
    //     _playerObject.AddBuff(new Buff(_specData.fieldID,-1).AddStatusEffect(Enum_Good_Status_Effect.IncreaseStatByMyDebuff, Value));
    // }
    //
    // public override void OnUnEquip()
    // {
    //     _playerObject.RemoveBuff(_specData.fieldID);
    // }

    public override void OnBerserkStart()
    {
        _owner.AddBuff(new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), (float)_owner.Stat[Enum_StatType.BerserkDuration])
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseAttackSpeed, Value));
    }
}
