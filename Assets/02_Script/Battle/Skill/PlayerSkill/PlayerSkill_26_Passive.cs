using IdleBattle;

// 남은 생명력이 20% 이하일 때 받는 모든 피해를 (15%+SLV)% 확률로 무시한다.
 // => 남은 생명력이 30% 이하일 때, 받는 모든 피해가 20% 감소한다.
public class PlayerSkill_26_Passive : PlayerPassiveSkill
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
                .AddStatusEffect(Enum_Good_Status_Effect.IgnoreDamagePercent, Value);
        
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
