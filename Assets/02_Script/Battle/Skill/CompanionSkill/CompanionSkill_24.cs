using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// n%의 범위 피해를 입히고 m초동안 속박한다.  
public class CompanionSkill_24 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null && targets.Count > 0)
        {
            var targetsList = targets;
            
            for (int i = 0; i < targetsList.Count; ++i)
            {
                if (!targetsList[i].isActiveAndEnabled || targetsList[i].IsDeath)
                {
                    continue;
                }

                var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(),_specData.skillDuration)
                    .AddStatusEffect(Enum_Bad_Status_Effect.Binding,Value_1);

                if (IsCEEquipped())
                {
                    var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                    var buffID = (int)Enum_BuffFrom.CompanionEquipment * 1000 + _specData.fieldID;
                    var CEBuff = new Buff(Enum_BuffFrom.CompanionEquipment, buffID, specSubStat.duration)
                        .AddStatusEffect(Enum_Bad_Status_Effect.ReduceAttackSpeed, specSubStat.value * 100)
                        .AddStatusEffect(Enum_Bad_Status_Effect.ReduceDamage, specSubStat.value * 100);

                    targets[i].AddBuff(CEBuff);
                }

                targetsList[i].AddBuff(buff);
                
                Attack(targets[i]);

                StartCoroutine(CoVFX(targets));
            }
        }
    }

    private IEnumerator CoVFX(List<CharacterObject> targets)
    {
        foreach (var target in targets)
        {
            if (!target.isActiveAndEnabled || target.IsDeath)
            {
                continue;
            }

            var pos = target.transform.position;
            _hitVFX.Stop();
            _hitVFX.transform.position = new Vector3(pos.x,pos.y+2f,pos.z);
            _hitVFX.Play();
            yield return new WaitForSeconds(0.05f);
        }
    }
}
