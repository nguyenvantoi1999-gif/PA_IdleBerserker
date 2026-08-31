using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleFlowPreset" , menuName = "Presets/BattleFlowPreset", order = 3)]
public class BattleFlowPreset : ScriptableObject
{
    public float ZoomFactor;
    public float ZoomSpeed;

    public float MonsterSpawnDelay;
    public float BattleStartDelay;
}