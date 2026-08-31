using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Spine.Unity;
using IdleBattle;
using UnityEngine;


// todo: 각 스킬 효과별로 상속해서 Active메소드 수정해서 사용
public class PlayerActiveSkill : PlayerSkill
{
    protected Coroutine _skillCoroutine;
    protected WaitForEndOfFrame _eofDelay;

    private const string Berserk_Prefix = "berserker_";

    // 연출 관련 변수
    public SkillEffectPreset SkillEffectPreset;

    protected int _slotIndex;

    protected virtual void Awake()
    {
        _eofDelay = new WaitForEndOfFrame();
    }

    public void InitSlot(int i)
    {
        _slotIndex = i;
    }

    protected List<CharacterObject> GetTargetMonsters()
    {
        var range = _owner.Stat[Enum_StatType.DetectRange];
        _owner.GetAbility<DetectAbility>().TryGetTargets(range * 2, out var targetMonsterList);

        targetMonsterList.RemoveAll(x => x == null || x.IsDeath || !x.isActiveAndEnabled);

        if (targetMonsterList.Count <= _specData.targetCount)
        {
            return targetMonsterList;
        }

        targetMonsterList.Sort((x, y) =>
        {
            if (x == null || y == null)
            {
                return 0;
            }

            return x.Position.x > y.Position.x ? 1 : -1;
        });

        return targetMonsterList.GetRange(0, _specData.targetCount);
    }

    public override bool TryUseSkill()
    {
        if (!IsSkillEnable() || BattleManager.Instance.BattleState == Enum_BattleState.Ready)
        {
            return false;
        }

        if(_owner.GetAbility<FSMAbility>().CurrentStateEquals(Enum_BerserkStateType.Skill) ||
           _owner.GetAbility<FSMAbility>().CurrentStateEquals(Enum_BerserkStateType.Death))
        {
            return false;
        }

        if (_owner.GetAbility<BuffAbility>().IsActivateStatusEffect(Enum_Bad_Status_Effect.Silence)
            || _owner.GetAbility<BuffAbility>().IsActivateStatusEffect(Enum_Bad_Status_Effect.Stun))
        {
            return false;
        }

        if (_specData.targetCount > 0)
        {
            var targetMonsterList = GetTargetMonsters();
            var aliveTargetExist = targetMonsterList.Count(monster => monster.IsAlive) > 0;

            if (!aliveTargetExist)
            {
                return false;
            }
        }

        ResetCoolDownTimer();

        SafeSetActive(true);
        OnSkillUse();

        StopCoroutine();
        _skillCoroutine = StartCoroutine(SkillDelayCoroutine());
       // SpeechBubbleCanvas.Instance.Show(_playerObject, $"{GetSkillName()}!!!");

        return true;
    }

    public override void ForceUseSkill()
    {
        if (_owner.GetAbility<FSMAbility>().CurrentStateEquals(Enum_BerserkStateType.Skill))
        {
            return;
        }

        if (_specData.targetCount > 0)
        {
            var targetMonsterList = GetTargetMonsters();
            var aliveTargetExist = targetMonsterList.Count(monster => monster.IsAlive) > 0;

            if (!aliveTargetExist)
            {
                return;
            }
        }

        SafeSetActive(true);
        OnSkillUse();

        StopCoroutine();
        _skillCoroutine = StartCoroutine(SkillDelayCoroutine());

        return;
    }

    protected virtual void OnSkillUse()
    {
        if (_isPlayer)
        {
            PlayerDataManager.SkillData.SkillActiveCount[_specData.fieldID]++;
        }
        // _owner.ActionEvent.Skill.Action(); // PA: ActionEvent chưa port
    }

    protected void SetPlayerSkillState(bool isSkillState)
    {
        if (SkillEffectPreset == null || string.IsNullOrEmpty(SkillEffectPreset.AnimtaionName))
        {
            return;
        }

        if (_owner.IsDeath)
        {
            return;
        }

        if (isSkillState)
        {
            PlayAnimation();
            _owner.GetAbility<FSMAbility>().ChangeState(Enum_BerserkStateType.Skill);
        }
        else
        {
            _owner.GetAbility<FSMAbility>().ChangeState(Enum_BerserkStateType.Idle);
        }
    }

    private void PlayAnimation()
    {
        var animationName = string.Empty;
        var speed = (float)_owner.Stat[Enum_StatType.AttackSpeed];

        if (_owner.GetAbility<BerserkAbility>().IsBerserkMode && !SkillEffectPreset.AnimtaionName.Contains(Berserk_Prefix))
        {
            animationName = $"{Berserk_Prefix}{SkillEffectPreset.AnimtaionName}";
        }
        else
        {
            animationName = SkillEffectPreset.AnimtaionName;
        }

        _owner.GetAbility<AnimationAbility>().PlayAnimation(animationName, false, SkillEffectPreset.AnimtaionSpeed * speed);
    }

    protected virtual IEnumerator SkillDelayCoroutine()
    {
        SetPlayerSkillState(true);

        PlayBalloonText();

        // Delay
        yield return GetDelay(SkillEffectPreset == null ? 0 : SkillEffectPreset.VFXDelay, true);

        var targets = GetTargetMonsters();

        SetSkillPosition(targets);
        PlaySkillVFX();
        PlaySkillSound();

        // Delay
        yield return GetDelay(SkillEffectPreset == null ? 0 : SkillEffectPreset.DamageDelay, true);

        PlaySkillEffect();

        Active(targets);
        OnActive(targets);

        yield return new WaitForSeconds(0.3f);

        SetPlayerSkillState(false);

        yield return GetDelay(SkillEffectPreset.HideDelay);

        Hide();
    }

    protected virtual void PlaySkillSound()
    {
        if (SkillEffectPreset == null)
        {
            return;
        }

        SoundManager.Instance.PlaySound(SkillEffectPreset.SoundName);
    }

    protected void PlayBalloonText()
    {
        if (SkillEffectPreset == null)
        {
            return;
        }

        if (_isPlayer)
        {
            SkillEffectPreset.PlayBalloonText();
        }
    }

    protected virtual void SetSkillPosition(List<CharacterObject> targets)
    {
        if (SkillEffectPreset.TargetCenter)
        {
            int targetCount = 0;
            Vector3 targetPosition = Vector3.zero;

            targets.ForEach(target =>
            {
                if (target.isActiveAndEnabled && !target.IsDeath)
                {
                    targetPosition += target.PositionCenter;
                    targetCount++;
                }
            });

            if (targetCount > 0)
            {
                transform.position = targetPosition / targetCount;
            }
        }
        else if (SkillEffectPreset.TargetBottom)
        {
            int targetCount = 0;
            Vector3 targetPosition = Vector3.zero;

            targets.ForEach(target =>
            {
                if (target.isActiveAndEnabled && !target.IsDeath)
                {
                    targetPosition += target.Position;
                    targetCount++;
                }
            });

            if (targetCount > 0)
            {
                transform.position = targetPosition / targetCount;
            }
        }
        else if (SkillEffectPreset.PlayerFront)
        {
            transform.position = _owner.transform.position + _owner.Model.right * SkillEffectPreset.PlayerFrontXOffset;
        }
        else if (SkillEffectPreset.PlayerBottom)
        {
            transform.position = _owner.transform.position;
        }
        else
        {
            transform.position = _owner.PositionCenter;
        }
    }

    protected IEnumerator FollowOwner()
    {
        var timer = _specData.time;

        while (timer > 0)
        {
            timer -= Time.deltaTime;
            transform.position = _owner.transform.position;
            yield return _eofDelay;
        }
    }

    protected virtual void Active(List<CharacterObject> targets)
    {
        var targetCount = targets.Count;

        for (int i = 0; i < targetCount; ++i)
        {
            if (targets.Count > targetCount || !targets[i].isActiveAndEnabled || targets[i].IsDeath)
            {
                continue;
            }

            Attack(targets[i]);

            OnTargetHit(targets[i]);
        }
    }

    protected void OnActive(List<CharacterObject> targets)
    {
        // if (_specData != null)
        // {
        //     BerserkerEffectManager.Trigger(_owner, Enum_EffectTrigger.UseSkill, _specData.fieldID, targets:targets);
        // }
    }


    protected virtual void OnTargetHit(CharacterObject character)
    {
    }

    protected void PlaySkillEffect()
    {
        if (SkillEffectPreset == null)
        {
            return;
        }

        SkillEffectPreset.PlayBlackout();
        SkillEffectPreset.PlaySlow();
        SkillEffectPreset.PlayShake();
    }

    public override void Hide()
    {
        base.Hide();
        StopCoroutine();
        gameObject.SetActive(false);
    }

    private void StopCoroutine()
    {
        StopAllCoroutines();
    }

    protected WaitForSeconds GetDelay(float duration, bool useSpeed = false)
    {
        var speed = useSpeed ? (float)_owner.Stat[Enum_StatType.AttackSpeed] : 1;
        return new WaitForSeconds(duration / speed);
    }
}
