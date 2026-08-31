using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 	6초 동안 광전사 주위를 파멸의 땅으로 만들어 모든 적에게 1초마다 공격력의 (200+20*SLV)% 피해를 입히고 이동속도와 공격속도를 30% 감소시킨다.
public class PlayerSkill_13_Active : PlayerActiveSkill
{
    private Coroutine _activeCoroutine;
    private WaitForSeconds _waitForSeconds;

    private int _count;
    
    public override void InitSkill(SpecSkill specData, BerserkerObject owner)
    {
        base.InitSkill(specData, owner);
        
        _waitForSeconds = new WaitForSeconds(1);
    }

    protected override void Active(List<CharacterObject> targets)
    {
        StartCoroutine(FollowOwner());
        _activeCoroutine = StartCoroutine(SkillActiveCoroutine());
    }

    public override void Hide()
    {
        if (_activeCoroutine != null)
        {
            StopCoroutine(_activeCoroutine);
        }

        _activeCoroutine = null;
        
        base.Hide();
    }

    private IEnumerator SkillActiveCoroutine()
    {
        float count = _specData.time;
        
        while (count > 0)
        {
            yield return _waitForSeconds;
            
            var targets = GetTargetMonsters();
            
            for (int i = 0; i < targets.Count; ++i)
            {
                if (!targets[i].isActiveAndEnabled || targets[i].IsDeath)
                {
                    continue;
                }

                var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time)
                    .AddStatusEffect(Enum_Bad_Status_Effect.ReduceAttackSpeed, SubValue)
                    .AddStatusEffect(Enum_Bad_Status_Effect.ReduceMoveSpeed, SubValue);
                
                targets[i].AddBuff(buff);
                Attack(targets[i]);
            }

            count--;
        }
    }
}
