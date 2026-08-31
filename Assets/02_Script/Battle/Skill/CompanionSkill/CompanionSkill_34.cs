using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 범위 내 적들에게 1초마다 N% 피해를 8회 입힌다. 해당 스킬이 지속되는 동안 경험치 획득량 및 골드 획득량이 M% 증가한다. 
// [전용 장비] 해당 스킬이 지속되는 동안 정복자 경험치도 M% 증가한다. 
public class CompanionSkill_34 : CompanionActiveSkill
{
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
        
        var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), _specData.skillDuration)
            .AddStatusEffect(Enum_Good_Status_Effect.ExpUp, Value_2 * 100) // 버프 경우에 적용하는 곳에서 나누기 100을 하기 때문에 예외적으로 * 100을 해서 사용
            .AddStatusEffect(Enum_Good_Status_Effect.StatGoldUp, Value_2 * 100);
        _owner.AddBuff(buff);
        
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
            var CESkillOption = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, _specData.skillDuration)
                .AddStatusEffect(Enum_Good_Status_Effect.ParagonExpUp, specSubStat.value * 100);
            _owner.AddBuff(CESkillOption);
        }    
    }
    
    private IEnumerator CoAttack()
    {
        int count = (int)_specData.skillDuration;
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
        damage.Value *= Value_1;
        
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
