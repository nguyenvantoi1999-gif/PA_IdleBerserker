using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 레츠
// 범위 내 적을 지정하여 7회 공격한다. 
// 내 공격력의 N%로 공격 (타겟이 1명일 때는 하나의 타겟에 7회 공격)
public class CompanionSkill_28 : CompanionActiveSkill
{
    [SerializeField] private ParticleSystem _hitVFX;
    private Coroutine _coroutine;
    private WaitForSecondsRealtime _wfs;

    private class AttackBatch
    {
        public List<CharacterObject> Targets;
        public int RemainCount;
    }

    private readonly Queue<AttackBatch> _batches = new Queue<AttackBatch>();

    protected override void Active(List<CharacterObject> targets)
    {
        if (targets != null)
        {
            _wfs =  new WaitForSecondsRealtime(0.1f);

            // CE OPTION SKILL, 동료 스킬 사용 후 m초 동안 공격력 n% 증가
            if (IsCEEquipped())
            {
                var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);

                var buff = new Buff(Enum_BuffFrom.Companion, _specData.GetBuffID(), specSubStat.duration)
                    .AddStatusEffect(Enum_Good_Status_Effect.IncreaseDamage, specSubStat.value);
                _owner.AddBuff(buff);
            }

            // 재발동 시 기존 배치를 덮어쓰지 않고 새 타겟 리스트를 별도 배치로 뒤에 쌓는다.
            // 기존 배치가 먼저 다 소진된 뒤에 이 배치가 처리된다.
            _batches.Enqueue(new AttackBatch { Targets = targets, RemainCount = _specData.tickCount_1 });

            if (_coroutine != null)
            {
                StopCoroutine(_coroutine);
            }

            _coroutine = StartCoroutine(CoAttack());
        }
    }

    private IEnumerator CoAttack()
    {
        while (_batches.Count > 0)
        {
            var batch = _batches.Peek();

            if (batch.RemainCount <= 0)
            {
                _batches.Dequeue();
                continue;
            }

            var targets = batch.Targets;
            if (targets != null && targets.Count > 0)
            {
                var target = targets[Random.Range(0, targets.Count)];

                var loopCounter = 0;

                while (target == null
                       || target.IsDeath
                       || target.isActiveAndEnabled)
                {
                    target = targets[Random.Range(0, targets.Count)];

                    loopCounter++;

                    if (loopCounter > 10)
                    {
                        break;
                    }
                }

                var damage = _owner.GetAbility<BerserkerAttackAbility>().GetBasicDamageData();
                damage.Value *= Value_1;
                target.TryTakeHit(damage, _owner);
            }

            batch.RemainCount--;

            yield return _wfs;
        }
    }
}
