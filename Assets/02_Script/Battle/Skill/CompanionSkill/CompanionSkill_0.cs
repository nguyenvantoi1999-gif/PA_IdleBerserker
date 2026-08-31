using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 3초간 무적 상태로 만들며, 1초 마다 체력을 +N% 회복
public class CompanionSkill_0 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    private Coroutine _coroutine;
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
            .AddStatusEffect(Enum_Good_Status_Effect.Invincible, Value_1);
        _owner.AddBuff(buff);

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        _coroutine = StartCoroutine(CoTakeRecovery());

        // StartCoroutine(SkillEffectCoroutine());
        StartCoroutine(FollowOwner());
    }

    private IEnumerator CoTakeRecovery()
    {
        var duration = _specData.skillDuration + _extraValue;
        while (duration > 0)
        {
            var healAmount = Value_1 * _owner.Stat[Enum_StatType.Health];
            _owner.TakeRecovery(healAmount);
            
            duration -= _specData.tickTime_1;
            yield return new WaitForSecondsRealtime(_specData.tickTime_1);
        }
    }
}
