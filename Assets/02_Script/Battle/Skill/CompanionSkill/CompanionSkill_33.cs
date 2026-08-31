using IdleBattle;
using System.Collections.Generic;
using System.Linq;

// 범위 마계 공격. 해당 스킬로 처치한 적의 수에 비례하여 n초동안 마계 공격력 M% 증가
public class CompanionSkill_33 : CompanionActiveSkill
{
    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null)
        {
            var damage = GetPlayerElementDamage(Enum_Element.Fire);
            damage.Value *= Value_1;
            
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
            if (diff >= 1)
            {
                var buffID = (int)Enum_BuffFrom.Companion * 1000 + _specData.fieldID;
                var CESkillOption = new Buff(Enum_BuffFrom.Companion, buffID, _specData.skillDuration)
                    .AddStatusEffect(Enum_Good_Status_Effect.StatFireDamage, (diff * Value_2 * 100));
                _owner.AddBuff(CESkillOption);
            
                // 전용 스킬로 처치한 적의 수에 비례하여 스킬 쿨타임도 0.5초씩 감소한다.
                if (IsCEEquipped())
                {
                    var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                    ReduceSkillCoolDown(specSubStat.subValue * diff);
                }    
            }
            
            StartCoroutine(FollowOwner());
        }
    }
}