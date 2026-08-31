using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SlowPreset" , menuName = "Presets/SlowPreset", order = 1)]
public class SlowPreset : ScriptableObject
{
    public float SlowDelay = 0;
    public float SlowTime = 0;
    
    public float SlowValue = 0;

    public bool IsLerpRecovery;

    public float RecoveryTime;
}
