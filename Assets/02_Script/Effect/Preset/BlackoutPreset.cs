using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlackoutPreset" , menuName = "Presets/BlackoutPreset", order = 2)]
public class BlackoutPreset : ScriptableObject
{
    public float BlackoutDelay = 0;
    
    public float FadeInTime = 0;
    public float BlackoutTime = 0;
    public float FadeOutTime = 0;
    
    public float BlackoutValue = 0.5f;

    public bool IsFront;

    public Color Color = Color.black;
    //
    // public bool IsLerpRecovery;
    //
    // public float RecoveryTime;
}
