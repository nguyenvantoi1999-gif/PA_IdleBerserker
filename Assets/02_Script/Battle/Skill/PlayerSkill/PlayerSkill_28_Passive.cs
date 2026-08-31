using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;

//사망에 이르는 피해를 입을 시 보호막이 발동하고 (3+0.1*SLV)초간 무적 상태가 된다.
public class PlayerSkill_28_Passive : PlayerPassiveSkill
{
    public override void OnDeath(ref Damage damage)
    {
        if (!IsSkillEnable())
        {
            return;
        }

        SafeSetActive(true);

        // value 가 무적 시간이기 때문에 시간이 추가된게 있으면 더해준다.
        // > 시간을 어디는 time이고 어디는 value 값을 사용해서 개별적으로 설정을 해줘야한다... ㅠ_ㅠ
        var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), Value + DurationBySpec)
            .AddStatusEffect(Enum_Good_Status_Effect.Invincible, Value + DurationBySpec);
        
        _owner.AddBuff(buff);
        
        ResetCoolDownTimer();
        
        //SoundManager.Instance.PlaySound("berserk_sfx_skill_immortal");

        StartCoroutine(SkillEffectCoroutine());
        
        // 불멸 지속 시간 동안 회심의 일격 피해량 100% 증가 > 펫 스킬
        BerserkerEffectManager.Trigger(_owner, Enum_EffectTrigger.UseSkill28Effect);
        
        damage.Value = 0;
    }

    IEnumerator SkillEffectCoroutine()
    {
        if (_skillParticle != null)
        {
            _skillParticle.gameObject.SetActive(true);
            _skillParticle.Play();
        }

        // value 가 무적 시간이기 때문에 시간이 추가된게 있으면 더해준다.
        var duration = Value + DurationBySpec;

        var endOfFrame = new WaitForEndOfFrame();
        
        while (duration >= 0)
        {
            if (_skillParticle != null && _owner != null)
            {
                transform.position = _owner.Position;
            }
            
            duration -= Time.deltaTime;
            yield return endOfFrame;
        }

        Hide();
    }
}
