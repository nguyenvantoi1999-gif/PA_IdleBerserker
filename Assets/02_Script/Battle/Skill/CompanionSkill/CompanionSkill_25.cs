using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 올리버
// 3초간 무적 상태로 만들며, 무적 시간 동안 회심의 일격 피해량 n% 증가
// [예외처리] 동료 무적 스킬은 1종만 사용할 수 있게끔

public class CompanionSkill_25 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    private float _extraValue = 0;
    
    protected override void Active(List<CharacterObject> targets)
    {
        // CE OPTION SKILL, 동료 스킬 시간 증가
        _extraValue = 0;
        // 액티브가 됐다면 장착했다는 뜻
        // 5레벨 특수 옵션이 해금됐으면
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            _extraValue = specSubStat.duration;
        }    
        
        var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), _specData.skillDuration + _extraValue)
            .AddStatusEffect(Enum_Good_Status_Effect.Invincible, Value_1)
            .AddStatusEffect(Enum_Good_Status_Effect.StatSuperCriticalDamage, Value_1);
        _owner.AddBuff(buff);

        StartCoroutine(FollowOwner());
    }
}
