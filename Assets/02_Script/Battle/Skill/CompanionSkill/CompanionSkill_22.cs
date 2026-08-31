using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 현재 생명력의 n%를 소모하여 m초동안 회심의 일격 확률과 회심의 일격 피해량을 n% 올려준다. 
public class CompanionSkill_22 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    private float _extraValue = 0;
    
    protected override void Active(List<CharacterObject> targets)
    {
        _extraValue = 0;
        // CE OPTION SKILL, 동료 스킬 시간 증가
        // 5레벨 특수 옵션이 해금됐으면
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            _extraValue = specSubStat.duration;
        }   
        
        var damage = new Damage()
        {
            Value = _owner.CurrentHealth * _owner.Stat[Enum_StatType.Health] * Value_2,
            DamageType = IdleBattle.Enum_DamageType.PlayerHit
        };
        
        _owner.TryTakeHit(damage , null);

        var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), _specData.skillDuration + _extraValue)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value_1 * 100)
            .AddStatusEffect(Enum_Good_Status_Effect.StatSuperCriticalDamage, Value_1);

        _owner.AddBuff(buff);

        StartCoroutine(FollowOwner());
    }
}
