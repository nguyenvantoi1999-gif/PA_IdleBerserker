using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;

// 가장 가까운 적 1명에게 칼날을 날려 공격력의
// (300+25*SLV)% 피해를 입힌다.
public class PlayerSkill_0_Active : PlayerActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    
    protected override void Active(List<CharacterObject> targets)
    {
        for (int i = 0; i < targets.Count; ++i)
        {
            if (!targets[i].isActiveAndEnabled || targets[i].IsDeath)
            {
                continue;
            }

            Attack(targets[i]);
        }
    }
}
