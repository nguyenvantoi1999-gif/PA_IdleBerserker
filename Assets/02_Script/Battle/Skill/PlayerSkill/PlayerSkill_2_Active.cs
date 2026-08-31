
// 가장 가까운 적의 머리 위로 파멸의 검을 떨어뜨려 공격력의 (800+50*SLV)% 피해를 입히고 4초 동안 받는 피해량을 15% 증가시킨다.

using UnityEngine;
using IdleBattle;

public class PlayerSkill_2_Active : PlayerActiveSkill
{
    [SerializeField] private ParticleSystem _psHit;
    protected override void OnTargetHit(CharacterObject character)
    {
        var buff = new Buff(Enum_BuffFrom.Skill, _specData.GetBuffID(), _specData.time).AddStatusEffect(Enum_Bad_Status_Effect.IncreaseHitDamage, SubValue);
        character.AddBuff(buff);

        _psHit.transform.position = character.PositionCenter;
        _psHit.Stop();
        _psHit.Play();
    }
}
