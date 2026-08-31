using System.Collections;
using System.Collections.Generic;
using IdleBattle;
using UnityEngine;
// 동일한 적에게 일반 공격을 가할 때마다 5초 동안 공격력이 (1+0.1*SLV)% 증가한다. (최대 50회 중첩)
public class PlayerSkill_21_Passive : PlayerPassiveSkill
{
    private CharacterObject _lastAttackTarget;
    private int _attackCount = 0;

    public override void OnTargetAttack(CharacterObject characterObject)
    {
        if (_lastAttackTarget == characterObject)
        {
            if (_attackCount < SubValue)
            {
                _attackCount++;
            }
            
            var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time)
                .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value * _attackCount);
        
            _owner.AddBuff(buff);
        }
        else
        {
            _lastAttackTarget = characterObject;
            _attackCount = 0;
        }
    }
}
