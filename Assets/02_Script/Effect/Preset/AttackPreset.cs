using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackPreset" , menuName = "Presets/AttackPreset", order = 1)]
public class AttackPreset : ScriptableObject
{    
    [Header("공격 연출")]
    [Space]
    public string AnimtaionName = string.Empty;
    [HideInInspector] public float DamageDelay = 0; // 스파인 애니메이션 이벤트 사용하기로 함
    public bool ShakeImmediately;

    [Header("연출이펙트 프리셋")]
    [Space]
    public ShakePreset ShakePreset;
    public SlowPreset SlowPreset;
    public BlackoutPreset BlackoutPreset;

    [Tooltip("플레이어 전용 : 해당 공격이 몇 명의 몬스터를 공격할 지")]
    public int AttackCount = 1;

    [Header("효과음")]
    [Space]
    public string SoundName = string.Empty;
    public AudioClip SoundFX = null;


    public void PlayShake()
    {
        if (ShakePreset != null)
        {
            BattleCamera.Instance.Shake(ShakePreset);
        }
    }

    public void PlaySlow(float speed = 1f)
    {
        if (SlowPreset != null)
        {
            //SlowManager.Instance.PlaySlowMotion(SlowPreset);
        }
    }

    public void PlayBlackout(float speed = 1f)
    {
        if (BlackoutPreset != null)
        {
            //BlackoutManager.Instance.PlayBlackOut(BlackoutPreset);
        }
    }

    // 효과음을 프리셋내에 넣을지 사운드매니저에 몰아넣고 string으로 호출할지 논의 후 결정
    public void PlaySoundFXWithSoundName()
    {
        //SoundManager.Instance.PlaySound(SoundName);
    }
    
    // public void PlaySoundFXWithSoundClip()
    // {
    //     SoundManager.Instance.PlaySound(SoundFX);
    // }
}
