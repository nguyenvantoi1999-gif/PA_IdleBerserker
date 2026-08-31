using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 한명의 적에게 랜덤한 속성 공격(무작위 속성 공격력의 n% 피해)
public class CompanionSkill_17 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    [SerializeField] protected ParticleSystem _skillParticle2;
    [SerializeField] protected ParticleSystem _skillParticle3;
    
    private int ran = 0;
    private ParticleSystem _particleSystem;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null && targets.Count > 0)
        {
            var element = (Enum_Element)ran;
            var damage = GetPlayerElementDamage(element);
            damage.Value *= Value_1;

            switch (element)
            {
                case Enum_Element.Water: damage.DamageType = IdleBattle.Enum_DamageType.Water; break;
                case Enum_Element.Fire: damage.DamageType = IdleBattle.Enum_DamageType.Fire; break;
                case Enum_Element.Grass: damage.DamageType = IdleBattle.Enum_DamageType.Grass; break;
                default: damage.DamageType = IdleBattle.Enum_DamageType.Skill; break;
            }

            var characterObject = targets[0];
            
            if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
            {
                return;
            }

            characterObject.TryTakeHit(damage, _owner);
            
            // CE OPTION SKILL, m초 동안 단일 몬스터에게 무작위 속성 피해량 * n% 추가 데미지
            if (IsCEEquipped())
            {
                var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
                var elementDamage = GetPlayerElementDamage(element);
                elementDamage.Value *= specSubStat.value;
                switch (element)
                {
                    case Enum_Element.Water: elementDamage.DamageType = IdleBattle.Enum_DamageType.Water; break;
                    case Enum_Element.Fire: elementDamage.DamageType = IdleBattle.Enum_DamageType.Fire; break;
                    case Enum_Element.Grass: elementDamage.DamageType = IdleBattle.Enum_DamageType.Grass; break;
                    default: elementDamage.DamageType = IdleBattle.Enum_DamageType.Skill; break;
                }
                    
                var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, specSubStat.duration)
                    .AddStatusEffect(Enum_Bad_Status_Effect.Poison, elementDamage.Value);
                characterObject.AddBuff(CESkillOption);
                    
            }    
        }
    }
    
    protected override void PlaySkillVFX()
    {
        if (_skillParticle != null)
        {
            if (_particleSystem != null)
            {
                _particleSystem.gameObject.SetActive(false);
            }
            
            ran = Random.Range(0, 3);
            
            switch (ran)
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
