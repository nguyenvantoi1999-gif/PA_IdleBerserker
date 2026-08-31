using System.Collections;
using System.Collections.Generic;
using IdleBattle;
using UnityEngine;
// 가장 가까운 적에게 저주를 걸어 4초 동안 공격력의 (200+10*SLV)% 중독 피해를 매 초마다 입히고 지속시간 종료 시 해당 스킬로 입힌 데미지 만큼 내 생명력을 회복한다.
// -> 6명의적에게4초동안공격력의190%의지속피해를매초마다입힌다.
public class PlayerSkill_5_Active : PlayerActiveSkill
{
    private Coroutine _recoveryCoroutine;
    public ParticleSystem _hitVFX;
    
    protected override void OnTargetHit(CharacterObject character)
    {
        var damage = GetPlayerSkillDamage();
        
        var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time).AddStatusEffect(Enum_Bad_Status_Effect.Poison, (Value / 100f) * damage.Value);
        character.AddBuff(buff);
    }

    protected override void Active(List<CharacterObject> targets)
    {
        var targetCount = targets.Count;

        for (int i = 0; i < targetCount; ++i)
        {
            if (targets.Count > targetCount || !targets[i].isActiveAndEnabled || targets[i].IsDeath)
            {
                continue;
            }
            
            OnTargetHit(targets[i]);
            _hitVFX.transform.position = targets[i].PositionCenter;
            _hitVFX.Play();
            SoundManager.Instance.PlaySound("PlayerSkill_18_Hit");
        }
    }
}
