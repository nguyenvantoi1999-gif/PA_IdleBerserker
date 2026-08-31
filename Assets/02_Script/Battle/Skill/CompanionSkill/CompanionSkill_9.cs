using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 모든 적에게 랜덤한 속성 공격 ( 무작위 속성 공격력의 n% 피해)
public class CompanionSkill_9 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    [SerializeField] protected ParticleSystem _skillParticle2;
    [SerializeField] protected ParticleSystem _skillParticle3;

    private int _ran = 0;
    private ParticleSystem _particleSystem;
    private float _extraValue;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null)
        {
            var element = (Enum_Element)_ran;
            var damage = GetPlayerElementDamage(element);
            damage.Value *= Value_1;

            switch (element)
            {
                case Enum_Element.Water: damage.DamageType = IdleBattle.Enum_DamageType.Water; break;
                case Enum_Element.Fire: damage.DamageType = IdleBattle.Enum_DamageType.Fire; break;
                case Enum_Element.Grass: damage.DamageType = IdleBattle.Enum_DamageType.Grass; break;
                default: damage.DamageType = IdleBattle.Enum_DamageType.Skill; break;
            }
            
            var beforeDeathCount = targets.Count(x => x != null && x.isActiveAndEnabled && x.IsDeath);
            
            foreach (var characterObject in targets)
            {
                if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
                {
                    continue;
                }

                characterObject.TryTakeHit(damage, _owner);
            }

            var afterDeathCount = targets.Count(x => x != null && x.isActiveAndEnabled && x.IsDeath);

            var diff = afterDeathCount - beforeDeathCount;

            _extraValue = 0;
            if (diff >= 1)
            {
                // CE OPTION SKILL, 처치한 적의 수 * n% 만큼 공격 속도 증가
                if (IsCEEquipped())
                {
                    var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                    _extraValue = diff * specSubStat.subValue;
                }    
            }
        }
    }
    
    public override float GetSkillCoolDown()
    {
        float skillCooldown = _specData.skillCooldown - _extraValue <= 0 ? _specData.skillCooldown : _specData.skillCooldown - _extraValue;
        return skillCooldown;
    }

    protected override void PlaySkillVFX()
    {
        if (_skillParticle != null)
        {
            if (_particleSystem != null)
            {
                _particleSystem.gameObject.SetActive(false);
            }
            
            _ran = Random.Range(0, 3);
            
            switch (_ran)
            {
                case 0:
                    _particleSystem = _skillParticle;
                    break;
                case 1:
                    _particleSystem = _skillParticle2;
                    break;
                case 2:
                    _particleSystem = _skillParticle3;
                    break;
            }
            
            _particleSystem.gameObject.SetActive(true);
            _particleSystem.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            _particleSystem.Play();
            return;
        } 
        
        if (_skillAnimator != null)
        {
            _skillAnimator.gameObject.SetActive(false);
            _skillAnimator.gameObject.SetActive(true);
            return;
        } 
        
        if (_skillSpine != null)
        {
            _skillSpine.gameObject.SetActive(false);
            _skillSpine.gameObject.SetActive(true);
            _skillSpine.Initialize(true);
            return;
        }
    }
    
    public override void Hide()
    {
        base.Hide();
        if (_skillParticle != null && _skillParticle2 != null && _skillParticle3 != null )
        {
            _skillParticle.gameObject.SetActive(false);
            _skillParticle2.gameObject.SetActive(false);
            _skillParticle3.gameObject.SetActive(false);
        }
    }
}
