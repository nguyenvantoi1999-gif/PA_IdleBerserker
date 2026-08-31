using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
// 전방으로 검은 구체를 날려 처음 맞는 적에게 공격력의 (300+25*SLV)% 피해를 입히고 5초 동안 스킬 사용을 금지시킨다.
public class PlayerSkill_4_Active : PlayerActiveSkill
{
    [SerializeField] private ParticleSystem _projectile;
    [SerializeField] private Transform _starPosition;
    
    [SerializeField] private  ParticleSystem _explosion;

    public float flightTime = 0.5f;

    protected override void Active(List<CharacterObject> targets)
    {
        _projectile.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        
        if (targets.Count > 0 && targets[0].isActiveAndEnabled )
        {
            _explosion.transform.position = targets[0].PositionCenter;
            _explosion.Play();

            if (!targets[0].IsDeath)
            {
                var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time).AddStatusEffect(Enum_Bad_Status_Effect.Silence, SubValue);
                targets[0].AddBuff(buff);
                Attack(targets[0]);
            }
        }
        
        SoundManager.Instance.PlaySound("PlayerSkill_4_Hit"); // 실제 타격 이펙트
    }

    protected override IEnumerator SkillDelayCoroutine()
    {
        var targets = GetTargetMonsters();
        
        transform.position = _owner.Position;
        var startPos = _starPosition.position;
        
        _skillParticle.gameObject.SetActive(true);
        _skillParticle.Play();

        yield return new WaitForSecondsRealtime(0.2f);
        
        _projectile.transform.position = startPos;
        _projectile.Play();
        
        SoundManager.Instance.PlaySound("PlayerSkill_4_Active"); // 스킬 발동 이펙트

        var timer = 0f;

        while (timer < flightTime)
        {
            timer += Time.deltaTime;
            
            Vector3 lerpPos = 
                Vector3.Lerp(startPos, targets[0].transform.position, timer / flightTime);

            lerpPos.y = startPos.y;

            _projectile.transform.position = lerpPos;

            yield return _eofDelay;
        }

        Active(targets);
        OnActive(targets);
        
        yield return GetDelay(SkillEffectPreset == null ? 3 : SkillEffectPreset.HideDelay);
        
        Hide();
    }
}
