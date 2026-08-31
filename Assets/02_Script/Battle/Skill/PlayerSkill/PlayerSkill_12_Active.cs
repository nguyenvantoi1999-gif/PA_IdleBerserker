using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;

// 	전장을 악령으로 뒤덮어 현재 생명력의 30%를 소모하고 모든 적에게 (700+50*SLV)% 피해를 입힌 뒤 4초 동안 받는 피해량을 15% 증가시킨다.
public class PlayerSkill_12_Active : PlayerActiveSkill
{
    protected override void OnSkillUse()
    {
        base.OnSkillUse();
        
        _owner.UseHealth(0.3f);
    }
    
    protected override void Active(List<CharacterObject> targets)
    {
        var targetCount = targets.Count;
    
        for (int i = 0; i < targetCount; ++i)
        {
            if (targets.Count > targetCount || !targets[i].isActiveAndEnabled || targets[i].IsDeath)
            {
                continue;
            }
    
            var buff = new Buff(Enum_BuffFrom.Skill,_specData.GetBuffID(), _specData.time)
                .AddStatusEffect(Enum_Bad_Status_Effect.IncreaseHitDamage, SubValue);
            targets[i].AddBuff(buff);
            Attack(targets[i]);
        }
    }
}