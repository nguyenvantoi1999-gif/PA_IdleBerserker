using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 가장 가까운 적에게 죽음의 표식을 남겨 4초 동안 기절시키고 1초당 공격력의 (150+10*SLV)% 중독 피해를 매 초마다 입힌다.
public class PlayerSkill_7_Active : PlayerActiveSkill
{
    protected override void OnTargetHit(CharacterObject character)
    {
        var damage = GetPlayerSkillDamage();
        
        var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time)
            .AddStatusEffect(Enum_Bad_Status_Effect.Stun, SubValue)
            .AddStatusEffect(Enum_Bad_Status_Effect.Poison, (Value / 100f) * damage.Value);
        character.AddBuff(buff);
    }
}
