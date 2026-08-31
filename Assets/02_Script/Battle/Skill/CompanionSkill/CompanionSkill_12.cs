using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;
using UnityEngine.TextCore.LowLevel;

// 모든 적에게 1초마다 n%의 피해를 8회 준다.
public class CompanionSkill_12 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    private Coroutine _coroutine;
    private WaitForSecondsRealtime _wfs;
    
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null)
        {
            _wfs =  new WaitForSecondsRealtime(1f);
            
            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }
            
            _coroutine = StartCoroutine(CoAttack());
            StartCoroutine(FollowOwner(_specData.skillDuration));
        }
    }

    private IEnumerator CoAttack()
    {
        int count = (int)_specData.skillDuration;
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
        damage.Value *= Value_1;
        
        // CE OPTION SKILL, 스킬 피해량 n% 증가
        float extraValue = 1;
        
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            extraValue = specSubStat.value;
        }    

        damage.Value *= extraValue;
        
        while (count > 0)
        {
            var targets = GetTargetMonsters();
            if (targets != null)
            {
                foreach (var characterObject in targets)
                {
                    if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
                    {
                        continue;
                    }

                    characterObject.TryTakeHit(damage, _owner);
                }
            }
            
            
            count--;
            yield return _wfs;
        }
    }
}
