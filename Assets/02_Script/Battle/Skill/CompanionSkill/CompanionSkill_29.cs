using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;


using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// 이은혁 아저씨
// 체력을 회복할 때 마다 중첩이 1회 쌓인다. 중첩 12회 시 {1}초 동안 생명력 {0}%의 실드 발동 
// 최대 중첩에 필요한 횟수가 2회 감소한다. 
public class CompanionSkill_29 : CompanionActiveSkill
{
    private int _skillStack;

    public override void ResetSkill()
    {
        base.ResetSkill();
        _skillStack = 0;
    }

    public override void OnRecovery()
    {
        _skillStack++;
        
        if (IsSkillEnable())
        {
            var maxCount = IsCEEquipped() ? 10 : 12;

            if (_skillStack >= maxCount)
            {
                gameObject.SetActive(true);
                StartCoroutine(SkillDelayCoroutine());
            }
        }
    }

    public void UseSkill()
    {
        var time = _specData.skillDuration;
        
        _owner.TakeShield(_owner.Stat[Enum_StatType.Health] * Value_1, time);
        
        StartCoroutine(FollowOwner(time));
    }
    
    protected override IEnumerator SkillDelayCoroutine()
    {
        yield return new WaitUntil(()=> IsSkillEnable());

        _skillStack = 0;
        SetCoolTime(_skillCooldown);
        yield return GetDelay(SkillEffectPreset == null ? 0 : SkillEffectPreset.VFXDelay, true);
        
        var targets = GetTargetMonsters();
        
        SetSkillPosition(targets);
        PlaySkillVFX();
        PlaySkillSound();
        UseSkill();
        
        yield return new WaitUntil(() => _owner.CurrentShield <= 0 || _owner.ShieldTime <= 0);
        
        _owner.RemoveShield();
        Hide();
    }

    public override bool TryUseSkill()
    {
        return false;
    }
}

