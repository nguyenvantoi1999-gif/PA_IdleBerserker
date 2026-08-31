using System.Collections;
using System.Collections.Generic;
using IdleBattle;
using UnityEngine;
// 머리 위로 생성된 악마의 검이 가장 가까운 적 1명을 타격하여 공격력의 (300+20*SLV)% 피해를 입히고 3초 동안 공격력의 (100+5*SLV)% 만큼 중독 피해를 매 초마다 입힌다.
public class PlayerSkill_1_Active : PlayerActiveSkill
{
    protected override void OnTargetHit(CharacterObject character)
    {
        var damage = GetPlayerSkillDamage();
        
        var buff = new Buff(Enum_BuffFrom.Skill,_specData.GetBuffID(), _specData.time).AddStatusEffect(Enum_Bad_Status_Effect.Poison, (SubValue / 100f) * damage.Value);
        character.AddBuff(buff);
    }
}
