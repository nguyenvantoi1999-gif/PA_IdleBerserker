using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;

[CreateAssetMenu(fileName = "SkillEffectPreset" , menuName = "Presets/SkillEffectPreset", order = 1)]
public class SkillEffectPreset : ScriptableObject
{    
    [Header("애니메이션")]
    [Space]
    public string AnimtaionName = string.Empty;
    public float AnimtaionSpeed = 1; 
    
    [Space]
    [Header("딜레이")]
    [Space]
    public float VFXDelay;
    public float DamageDelay;
    public float HideDelay = 3;

    [Space]
    [Header("연출이펙트 프리셋")]
    [Space]
    public ShakePreset ShakePreset;
    public SlowPreset SlowPreset;
    public BlackoutPreset BlackoutPreset;

    [Space]
    [Header("이펙트 위치")] 
    [Space] 
    public bool TargetCenter;
    public bool TargetBottom;
    public float TargetBottomYOffset;
    public bool PlayerBottom;
    public bool PlayerFront;
    public float PlayerFrontXOffset;

    [Header("효과음")]
    [Space]
    public string SoundName = string.Empty;
    public AudioClip SoundFX = null;

    [Header("대사")] 
    [Space] 
    public bool UseTextBalloon;
    public string BalloonKey;

    [Header("Companion Skill")]
    public int FieldId;
    public Sprite Icon;

    [Header("Companion Timing")]
    [Min(0f)] public float Cooldown = 8f;
    [Min(0f)] public float Duration = 3f;

    [Header("Companion Damage / Effect")]
    [Tooltip("Damage multiplier; for support skills this is the primary heal, shield or buff value.")]
    public float Value1 = 0.5f;
    [Tooltip("Value1 increase for each level after level 1.")]
    public float Value1PerLevel = 0.02f;
    [Tooltip("Secondary damage or effect value.")]
    public float Value2 = 0.3f;
    [Tooltip("Value2 increase for each level after level 1.")]
    public float Value2PerLevel = 0.01f;

    [Header("Companion Targets / Ticks")]
    [Min(0)] public int TargetCount = 5;
    [Min(0)] public int TickInterval = 1;
    [Min(0)] public int TickCount = 3;

    public SpecCompanionSkill CreateCompanionSpec()
    {
        return new SpecCompanionSkill
        {
            fieldID = FieldId,
            skillCooldown = Cooldown,
            skillDuration = Duration,
            targetCount_1 = TargetCount,
            effectValue_1 = Value1,
            effectValueIncrease_1 = Value1PerLevel,
            effectValue_2 = Value2,
            effectValueIncrease_2 = Value2PerLevel,
            tickTime_1 = TickInterval,
            tickCount_1 = TickCount
        };
    }

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
            
        }
    }

    public void PlayBlackout(float speed = 1f)
    {
        if (BlackoutPreset != null)
        {
            
        }
    }

    public void PlayBalloonText()
    {
        if (UseTextBalloon)
        {
           
        }
    }

    // 효과음을 프리셋내에 넣을지 사운드매니저에 몰아넣고 string으로 호출할지 논의 후 결정
    public void PlaySoundFXWithSoundName()
    {
        
    }
    
    // public void PlaySoundFXWithSoundClip()
    // {
    //     SoundManager.Instance.PlaySound(SoundFX);
    // }
}
