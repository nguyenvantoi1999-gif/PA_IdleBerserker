using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 박유리씨
// {1}초동안 매초 생명력을 {0}% 회복한다. 
// 전용 스킬 지속 시간 {1}초 증가
public class CompanionSkill_31 : CompanionActiveSkill
{
    private Coroutine _coroutine;
    private float _extraValue = 0;
    
    protected override void Active(List<CharacterObject> targets)
    {
        _extraValue = 0;

        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            _extraValue = specSubStat.duration;
        }    

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }
        
        _coroutine = StartCoroutine(CoTakeRecovery());
        
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