using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 하나야마
// 즉시 광폭화를 발동시키고, 광폭화 지속 시간 동안 광폭화 공격력 n% 상승 및 보스 피해량 m% 상승
public class CompanionSkill_27 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        // CE OPTION SKILL, 동료 스킬 시간 증가
        float extraValue = 0;
        // 액티브가 됐다면 장착했다는 뜻
        // 5레벨 특수 옵션이 해금됐으면
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            extraValue = specSubStat.duration;
        }    

        _owner.Stat[Enum_StatType.BerserkDuration] += extraValue;
      
        _owner.SetBerserkState(true);
        
        _owner.Stat[Enum_StatType.BerserkDuration] -= extraValue;
      
        var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), (float)_owner.Stat[Enum_StatType.BerserkDuration] + extraValue)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseBerserkDamage, Value_1)
            .AddStatusEffect(Enum_Good_Status_Effect.StatBossDamage, Value_2);
        _owner.AddBuff(buff);

        StartCoroutine(FollowOwner());
    }
}
