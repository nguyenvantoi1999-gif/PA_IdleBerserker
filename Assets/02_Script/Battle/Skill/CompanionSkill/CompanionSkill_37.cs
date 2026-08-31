using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 단일 적에게 플레이어의 생명력 비례 공격력 (0)% 피해를 입힌다.
public class CompanionSkill_37 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    private Coroutine _coroutine;
    private float _extraValue = 0;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
            var CEBuff = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, specSubStat.duration) 
                .AddStatusEffect(Enum_Good_Status_Effect.StatDamagePerHealthUpMultiply, specSubStat.value);
                
            _owner.AddBuff(CEBuff);
        }
        
        CharacterObject target = null;
        for (int i = 0; i < targets.Count; ++i)
        {
            if (!targets[i].isActiveAndEnabled || targets[i].IsDeath)
            {
                continue;
            }

            target = targets[i];
            break;
        }
        
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData(Enum_StatType.DamagePerHealth);
        damage.Value *= Value_1;
        target?.TryTakeHit(damage, _owner);
    }
}
