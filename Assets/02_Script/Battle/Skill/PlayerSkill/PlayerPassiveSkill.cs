using IdleBattle;

public class PlayerPassiveSkill : PlayerSkill
{
    public virtual void OnEquip()
    {
    }

    public virtual void OnUnEquip()
    {
        _owner.RemoveBuff(_specData.GetBuffID());
    }

    public virtual void OnTargetAttack(CharacterObject characterObject)
    {
    }

    public virtual void OntAttack()
    {
    }

    public virtual void OnBerserkStart()
    {
    }

    public virtual void OnBerserkEnd()
    {
    }

    public virtual void OnHit(CharacterObject from)
    {
    }

    public virtual void OnEnemyKill()
    {
    }

    public virtual void OnDeath(ref Damage damage)
    {
    }

    public virtual void OnHealthChange()
    {
    }

    public virtual void OnSkillLevelUp()
    {
        OnUnEquip();
        OnEquip();
    }

    public virtual void OnCriticalAttack()
    {
    }
}
