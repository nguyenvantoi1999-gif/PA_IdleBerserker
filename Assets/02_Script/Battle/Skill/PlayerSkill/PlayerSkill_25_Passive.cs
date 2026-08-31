using IdleBattle;

// 남은 생명력이 절반 이하일 때 공격력과 스킬 피해량이 (30+3*SLV)% 증가한다.
public class PlayerSkill_25_Passive : PlayerPassiveSkill
{
    private bool isActivating = false;
    
    public override void OnHit(CharacterObject from)
    {
        var healthFactor = _owner.CurrentHealth;
        healthFactor *= 100;

        if (healthFactor <= SubValue)
        {
            if (isActivating)
            {
                return;
            }
            
            var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), -1)
                .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, Value)
                .AddStatusEffect(Enum_Good_Status_Effect.IncreaseSkillDamage, Value);
        
            _owner.AddBuff(buff);
            isActivating = true;
        }
        else
        {
            if (!isActivating)
            {
                return;
            }
            
            _owner.RemoveBuff(_specData.GetBuffID());
            isActivating = false;
        }
    }
}
