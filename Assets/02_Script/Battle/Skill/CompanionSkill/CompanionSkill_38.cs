using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

public class CompanionSkill_38 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;

    protected override void Active(List<CharacterObject> targets)
    {
        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);

            if (UtilCode.GetChance(specSubStat.value * 100))
            {
                SetCoolTime(0);

                var equippedIndexList = _isPlayer
                    ? PlayerSkillManager.Instance.GetEquippedCompanionSkill()
                    : EnemySkillManager.Instance.GetEquippedCompanionSkill();

                var otherIndexList = equippedIndexList?.Where(index => index != _specData.fieldID).ToList();
                if (otherIndexList != null && otherIndexList.Count > 0)
                {
                    var randomIndex = otherIndexList[Random.Range(0, otherIndexList.Count)];
                    var randomCompanionSkill = _isPlayer
                        ? PlayerSkillManager.Instance.GetCompanionActiveSkill(randomIndex)
                        : EnemySkillManager.Instance.GetCompanionActiveSkill(randomIndex);

                    randomCompanionSkill?.SetCoolTime(0);
                }
            }
        }

        if (targets != null)
        {
            foreach (var characterObject in targets)
            {
                if (!characterObject.isActiveAndEnabled || characterObject.IsDeath)
                {
                    continue;
                }

                OnTargetHit(characterObject);
            }
        }
    }

    protected override void OnTargetHit(CharacterObject character)
    {
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
        var burnValue = Value_1 * damage.Value;

        var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), _specData.skillDuration)
            .AddStatusEffect(Enum_Bad_Status_Effect.BurnDamage, burnValue);
        character.AddBuff(buff);
    }
}
