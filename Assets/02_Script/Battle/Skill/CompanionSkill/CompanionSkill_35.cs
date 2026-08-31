using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

public class CompanionSkill_35 : CompanionActiveSkill
{
    private Coroutine _coroutine;
    private WaitForSecondsRealtime _wfs;

    private class AttackBatch
    {
        public CharacterObject Target;
        public int RemainCount;
    }

    private readonly Queue<AttackBatch> _batches = new Queue<AttackBatch>();

    protected override void Awake()
    {
        base.Awake();
        _wfs = new WaitForSecondsRealtime(0.2f);
    }

    protected override void Active(List<CharacterObject> targets)
    {
        CharacterObject target = null;
        for (int i = 0; i < targets.Count; ++i)
        {
            if (!targets[i].isActiveAndEnabled || targets[i].IsDeath)
            {
                continue;
            }

            target = targets[i];
            break;
        }

        var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), _specData.skillDuration);
        buff.AddStatusEffect(Enum_Bad_Status_Effect.IncreaseHitDamage, Value_2);

        if (IsCEEquipped())
        {
            var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
            buff.AddStatusEffect(Enum_Bad_Status_Effect.IncreaseWaterHitDamage, specSubStat.value);
        }

        target?.AddBuff(buff);

        // 재발동 시 기존 배치를 덮어쓰지 않고 새 타겟을 별도 배치로 뒤에 쌓는다.
        // 기존 배치(이전 타겟)가 먼저 다 소진된 뒤에 이 배치가 처리된다.
        _batches.Enqueue(new AttackBatch { Target = target, RemainCount = 3 });

        if (_coroutine != null)
        {
            StopCoroutine(_coroutine);
        }

        _coroutine = StartCoroutine(CoAttack());
        //StartCoroutine(FollowOwner(_specData.skillDuration));
    }

    private IEnumerator CoAttack()
    {
        var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
        damage.Value *= Value_1;

        while (_batches.Count > 0)
        {
            var batch = _batches.Peek();

            if (batch.RemainCount <= 0)
            {
                _batches.Dequeue();
                continue;
            }

            var target = batch.Target;
            if (target != null)
            {
                if (!target.isActiveAndEnabled || target.IsDeath)
                {
                    _batches.Dequeue();
                    continue;
                }

                target.TryTakeHit(damage, _owner);
            }

            batch.RemainCount--;

            yield return _wfs;
        }
    }
}
