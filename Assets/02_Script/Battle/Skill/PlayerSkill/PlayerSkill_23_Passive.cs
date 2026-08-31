using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 적에게 피해를 입으면 2초 동안 공격력과 치명타 피해량이 (20+2*SLV)% 만큼 증가한다.
// ->  적에게 피해를 입으면 다음 공격의 공격력이 40% 증가한다.
public class PlayerSkill_23_Passive : PlayerPassiveSkill
{
    public override void OnHit(CharacterObject from)
    {
        if (!IsSkillEnable())
        {
            return;
        }
        
        // var buff = new Buff(_specData.fieldID, _specData.time)
        //     .AddStatusEffect(Enum_Good_Status_Effect.IncreaseAttackSpeed, Value)
        //     .AddStatusEffect(Enum_Good_Status_Effect.IncreaseCriticalDamage, Value);      

        var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time).AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamageOnce, Value);
        
        _owner.AddBuff(buff);

        ResetCoolDownTimer();
    }
}
