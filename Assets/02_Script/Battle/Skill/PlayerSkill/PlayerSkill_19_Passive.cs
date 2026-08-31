using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 	일반 공격 시 20% 확률로 5초간 공격속도와 이동속도가 (15+SLV)% 증가한다.
// -> 치명타로피해를입힐경우, 2초간공격속도와공격력이14%증가한다.
public class PlayerSkill_19_Passive : PlayerPassiveSkill
{
    public override void OnCriticalAttack()
    {
        if (!IsSkillEnable())
        {
            return;
        }
        
        Active();
        ResetCoolDownTimer();
    }

    public override void ForceUseSkill()
    {
        Active();
    }

    private void Active()
    {
        var buff = new Buff(Enum_BuffFrom.Skill,_specData.GetBuffID(), _specData.time)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseAttackSpeed, Value)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value);
        
        _owner.AddBuff(buff);
    }
}
