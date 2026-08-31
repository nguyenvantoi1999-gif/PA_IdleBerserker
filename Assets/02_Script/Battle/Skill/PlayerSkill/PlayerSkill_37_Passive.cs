using System;
using System.Collections;
using System.Collections.Generic;
using IdleBattle;
using UnityEngine;

public class PlayerSkill_37_Passive: PlayerPassiveSkill
{
    public SkillEffectPreset SkillEffectPreset;
    private int _passiveStack = 0;
    private int _attackStack = 0;
    
    [SerializeField] 
    protected ParticleSystem _skillParticle2;

    private WaitForSeconds VFXDelay;
    private WaitForSeconds DamageDelay;
    private WaitForSeconds HideDelay;

    private void Start()
    {
        VFXDelay = new WaitForSeconds(SkillEffectPreset.VFXDelay);
        DamageDelay = new WaitForSeconds(SkillEffectPreset.DamageDelay);
        HideDelay = new WaitForSeconds(SkillEffectPreset.HideDelay);
    }
    
    public override void OnUnEquip()
    {
        base.OnUnEquip();
        _passiveStack = 0;
        _attackStack = 0;
    }

    public override void OnEquip()
    {
        gameObject.SetActive(true);
    }

    protected CharacterObject GetTargetMonsters()
    {
        var range = _owner.Stat[Enum_StatType.DetectRange];
        _owner.GetAbility<DetectAbility>().TryGetTargets(range * 2, out var targetMonsterList);
        
        targetMonsterList.RemoveAll(x => x == null || x.IsDeath || !x.isActiveAndEnabled);

        if (targetMonsterList.Count <= 0)
        {
            return null;
        }

        targetMonsterList.Sort((x, y) =>
        {
            if (x == null || y == null)
            {
                return 0;
            }
            
            return x.Position.x > y.Position.x ? 1 : -1;
        });

        return targetMonsterList[0];
    }

    public override void OnCriticalAttack()
    {
        PassiveOn();
        ActiveOn();
    }

    private void PassiveOn()
    {
        if (_passiveStack < _specData.targetCount)
        {
            _passiveStack++;
        }

        var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time)
            .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value * _passiveStack);

        _owner.AddBuff(buff);
    }

    private void ActiveOn()
    {
        _attackStack++;

        if (_attackStack >= 10)
        {
            _attackStack = 0;
            StartCoroutine(SkillDelayCoroutine());
        }
    }
    
    protected virtual IEnumerator SkillDelayCoroutine()
    {
        yield return VFXDelay;
        
        var targets = GetTargetMonsters();
        if (targets != null && targets.isActiveAndEnabled)
        {
            bool isSecondVFX = false;
            SetSkillPosition(targets);
            if (_skillParticle.gameObject.activeInHierarchy)
            {
                _skillParticle2.gameObject.SetActive(true);
                _skillParticle2.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
                _skillParticle2.Play();
                isSecondVFX = true;
            }
            else
            {
                PlaySkillVFX();
            }

            // Delay
            yield return DamageDelay;

            Attack(targets);

            yield return new WaitForSeconds(0.3f);

            yield return HideDelay;

            if (isSecondVFX)
            {
                _skillParticle2.gameObject.SetActive(false);
            }
            else
            {
                Hide();
            }
        }
    }
    
    protected virtual void SetSkillPosition(CharacterObject targets)
    {
        if (SkillEffectPreset.TargetCenter)
        {
            transform.position = targets.transform.position;
        } 
        else if (SkillEffectPreset.TargetBottom)
        {
            transform.position = targets.transform.position;
        }
        else if (SkillEffectPreset.PlayerFront)
        {
            transform.position = _owner.transform.position + _owner.Model.right * SkillEffectPreset.PlayerFrontXOffset;
        }
        else if (SkillEffectPreset.PlayerBottom)
        {
            transform.position = _owner.transform.position;
        }
        else
        {
            transform.position = _owner.PositionCenter;
        }
    }
    
    private void Attack(CharacterObject target)
    {
        if (target == null || !target.isActiveAndEnabled)
        {
            return;
        }
        
        var damage = GetPlayerSkillDamage();
        
        damage.Value *= (SubValue / 100f);

        target.TryTakeHit(damage, _owner);
    }
    
    WaitForEndOfFrame endOfFrame = new WaitForEndOfFrame();
    

    
}
