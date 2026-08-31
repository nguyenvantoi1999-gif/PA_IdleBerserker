using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using IdleBattle;

// 범위 내 무작위 타켓을 지정하여 5회 공격한다. 
public class CompanionSkill_6 : CompanionActiveSkill
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
            _wfs = new WaitForSecondsRealtime(0.1f);

            // CE OPTION SKILL, 스킬 발동 회수 증가
            float extraValue = 0;
            if (IsCEEquipped())
            {
                var specSubStat = SpecDataManager.Instance.GetSpecCE_SubStat().First(x => x.companionIndex == _specData.fieldID);
                extraValue = specSubStat.subValue;
            }

            // 재발동 시 기존 배치를 덮어쓰지 않고 새 타겟 리스트를 별도 배치로 뒤에 쌓는다.
            // 기존 배치(이전 타겟)가 먼저 다 소진된 뒤에 이 배치가 처리된다.
            _batches.Enqueue(new AttackBatch { Targets = targets, RemainCount = _specData.tickCount_1 + (int)extraValue });

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

                Attack(target);
            }

            batch.RemainCount--;

            yield return _wfs;
        }
    }
}
