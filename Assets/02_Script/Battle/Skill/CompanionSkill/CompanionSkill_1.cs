using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 모든 상태 이상을 회복하고 3초간 속성 공격력 n% 증폭
// Todo. Clear 하면 한번에 다 사라지나?
public class CompanionSkill_1 : CompanionActiveSkill
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
        
        _owner.RemoveAllBadBuff();
        
        var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), _specData.skillDuration + extraValue)
            .AddStatusEffect(Enum_Good_Status_Effect.StatFireDamageMultiply, Value_1)
            .AddStatusEffect(Enum_Good_Status_Effect.StatWaterDamageMultiply, Value_1)
            .AddStatusEffect(Enum_Good_Status_Effect.StatGrassDamageMultiply, Value_1);
        
        _owner.AddBuff(buff);
        
        StartCoroutine(FollowOwner());
    }
}
