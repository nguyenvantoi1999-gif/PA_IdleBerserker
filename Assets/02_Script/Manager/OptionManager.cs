using System;
using UnityEngine;

[Serializable]
// 저장될 옵션
public class Option
{
    public bool AutoSleep = true;
    public bool VFX = true;
    
    public bool BGM = true;
    public bool SFX = true;
    
    public float BGMVolume = 1;
    public float SFXVolume = 1;
    
    public bool LowQuality = false;
    
    public bool AutoSkill = false;
    public bool AutoBerserk = true;

    public bool UseShake = true;
    public float ShakeStrength = 0.2f;
                
    public bool IsChatOn = true;
    public bool UsePush = true;
    public bool UsePushNightTime = false;
    public bool SkipPetEvolution = false;
    
    public bool UseNarrative = true;
    public bool UseCompanionActiveUI = true;
    public bool UseBerserkBGM = true;
    
    public bool UsePVPSpeed = false;
}

// 프로토타입  배경음, 효과음 , 자동전투(광폭화) , 보스 자동 ,
public class OptionManager : SingletonBehaviour<OptionManager>
{
    private Option _option;
    public Option Option => _option;

    protected override void Awake()
    {
        base.Awake();
        LoadOptionSetting();

        SetTargetFrame(false);
    }

    public void SetTargetFrame(bool forceLowFrame)
    {
        
#if UNITY_EDITOR
        Application.targetFrameRate = 60;
        Application.runInBackground = true;
#else
        if (forceLowFrame)
        {
            Application.targetFrameRate = 15;
        }
        else
        {
            Application.targetFrameRate = _option.LowQuality ? 30 : 45;
        }
#endif
    }

    public void ToggleAutoBerserkMode(bool isOn)
    {
        _option.AutoBerserk = isOn;
        SaveOption();
    }

    public void ToggleAutoSkill(bool isOn)
    {
        _option.AutoSkill = isOn;
        SaveOption();
    }

    public void ToggleEvolutionEffect(bool isOn)
    {
        _option.SkipPetEvolution = isOn;
        SaveOption();
    }

    public void TogglePVPSpeed()
    {
        _option.UsePVPSpeed = !_option.UsePVPSpeed;
        SaveOption();
    }

    private const string SaveKey = "Option";
    
    public void SaveOption()
    {
       
    }

    private void LoadOptionSetting()
    {
        
    }
}
