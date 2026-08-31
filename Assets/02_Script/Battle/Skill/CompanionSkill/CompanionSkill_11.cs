using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 한명의 적에게 n % 피해를 주고 적에게 준 피해량의 10% 회복
public class CompanionSkill_11 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;

    protected override void Active(List<CharacterObject> targets)
    {
        StartCoroutine(FollowOwner());

        if (targets != null && targets.Count > 0)
        {
            // var ranTarget = Random.Range(0, targets.Count);        
            // var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
            // damage.Value *= Value_1;
            // targets[ranTarget].TryTakeHit(damage, _owner);
            
            // 가장 앞의 적에게 피해
            var characterObject = targets[0];
            
            if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
            {
                return;
            }

            var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
            damage.Value *= Value_1;
            characterObject.TryTakeHit(damage, _owner);
            
            var healAmount = damage.Value * Value_2;
            _owner.TakeRecovery(healAmount);
            
            // CE OPTION SKILL, 공격력 n% 만큼 추가 데미지
            if (IsCEEquipped())
            {
                var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                var basicDamage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
                basicDamage.Value *= specSubStat.value;
                characterObject.TryTakeHit(basicDamage, _owner);
                    
            }    
        }
    }
}
