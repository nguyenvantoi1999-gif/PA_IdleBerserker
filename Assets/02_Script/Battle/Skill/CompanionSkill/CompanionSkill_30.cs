using System.Collections.Generic;
using UnityEngine;
using IdleBattle;

// 편상욱이
// {0}%의 피해를 입힌다. 4초동안 적이 받는 피해량이 {1}% 증가한다.
// 디버프 지속시간 동안 적이 받는 마계 피해량도 {1}% 증가한다.
public class CompanionSkill_30 : CompanionActiveSkill
{
    private float _extraValue = 0;
    
    protected override void Active(List<CharacterObject> targets)
    {
        var targetCount = targets.Count;

        for (int i = 0; i < targetCount; ++i)
        {
            if (targets.Count > targetCount || !targets[i].isActiveAndEnabled || targets[i].IsDeath)
            {
                continue;
            }

            var target = targets[i];
            
            Attack(target);
            
            OnTargetHit(target);
            
            var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(),_specData.skillDuration)
                .AddStatusEffect(Enum_Bad_Status_Effect.IncreaseHitDamage,Value_2);

            if (IsCEEquipped())
            {
                buff.AddStatusEffect(Enum_Bad_Status_Effect.IncreaseFireHitDamage,Value_2);
            }
            
            target.AddBuff(buff);
        }
    }
}
