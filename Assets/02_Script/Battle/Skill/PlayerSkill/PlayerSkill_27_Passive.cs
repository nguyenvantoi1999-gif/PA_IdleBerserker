using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
//상태 이상에 걸린 적에게 가하는 피해량이 상태 이상 효과 1개당 (30+2*SLV)% 증가한다.
public class PlayerSkill_27_Passive : PlayerPassiveSkill
{
    public override void OnEquip()
    {
        _owner.AddBuff(new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(),-1).AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamageByEnemyDebuff, Value));
    }

    public override void OnUnEquip()
    {
        _owner.RemoveBuff(_specData.GetBuffID());
    }
}
