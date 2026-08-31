using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 구조체 참조 안되면  
[Serializable]
public class Buff
{
    // 0~99 스킬
    // 100~999 장비스킬
    // 1000~9999 전투
    public Enum_BuffFrom From { get; }
    public int BuffID { get; }
    public bool HaveDebuff { get; private set; }
    public bool HaveBuff { get; private set; }

    public bool IsStatusEffect;


    public Dictionary<Enum_Good_Status_Effect, double> GoodEffects = new Dictionary<Enum_Good_Status_Effect, double>();
    public Dictionary<Enum_Bad_Status_Effect, double> BadEffects = new Dictionary<Enum_Bad_Status_Effect, double>();

    public bool IsStackable { get; private set; }
    public int CurrentStackCount { get; private set; }
    
    private int _maxStackCount;
    
    public bool IsInfiniteBuff { get; private set; }
    
    
    public float RemainTime { get; private set; }
    

    public Buff(Enum_BuffFrom from, int id, float remainTime, bool stackable = false, int maxStackCount = 1)
    {
        From = from;
        BuffID = id;
        
        RemainTime = remainTime;

        if (remainTime < 0)
        {
            IsInfiniteBuff = true;
        }

        IsStackable = stackable;
        CurrentStackCount = 1;
        _maxStackCount = maxStackCount;
        
//        PCDebug.LogError($"[Buff] Buff Created : {ToString()}");
    }

    public Buff AddStatusEffect(Enum_Good_Status_Effect goodStatus, double value)
    {
        if (value <= 0)
        {
            // value 총합이 0이상이어야 활성화됐다고 파악하기 때문에 0이하값이 들어오면 강제로 1으로 변경
            value = 1;
        }
        
        HaveBuff = true;

        if (GoodEffects == null)
        {
            GoodEffects = new Dictionary<Enum_Good_Status_Effect, double>();
        }

        GoodEffects.Add(goodStatus, value);
        
        return this;
    }

    public Buff AddStatusEffect(Enum_Bad_Status_Effect badStatus, double value)
    {
        if (value <= 0)
        {
            // value 총합이 0이상이어야 활성화됐다고 파악하기 때문에 0이하값이 들어오면 강제로 1으로 변경
            value = 1;
        }
        
        HaveDebuff = true;

        if (BadEffects == null)
        {
            BadEffects = new Dictionary<Enum_Bad_Status_Effect, double>();
        }

        if (BadEffects.ContainsKey(badStatus))
        {
            Debug.LogError("동일한 디버프 키값을 가지고 있음");
        }
        else
        {
            BadEffects.Add(badStatus, value);
        }
        
        switch (badStatus)
        {
            case Enum_Bad_Status_Effect.Silence:
            case Enum_Bad_Status_Effect.Binding:
            case Enum_Bad_Status_Effect.Stun:
                IsStatusEffect = true;
                break;
        }

        return this;
    }


    public bool CheckBuffEnd(float dt)
    {
        if(IsInfiniteBuff)
        {
            return false;
        }
        
        RemainTime -= dt;

        if (RemainTime <= 0)
        {
            BuffEnd();
            return true;
        }

        return false;
    }

    public void RefreshBuff(Buff newBuff)
    {
        RemainTime = Mathf.Max(RemainTime, newBuff.RemainTime);

        foreach (var pair in newBuff.GoodEffects)
        {
            if (GoodEffects.ContainsKey(pair.Key))
            {
                if (pair.Value > GoodEffects[pair.Key])
                {
                    GoodEffects[pair.Key] = pair.Value;
                }
            }
        }
        
        foreach (var pair in newBuff.BadEffects)
        {
            if (BadEffects.ContainsKey(pair.Key))
            {
                if (pair.Value > BadEffects[pair.Key])
                {
                    BadEffects[pair.Key] = pair.Value;
                }
            }
        }
        
        //Debug.Log($"{newBuff.BuffID}번 버프를 새로운 버프로 교체했습니다.");
    }

    public void StackBuff(Buff buff)
    {
        RemainTime = Mathf.Max(RemainTime, buff.RemainTime);

        if (CurrentStackCount >= _maxStackCount)
        {
            //Debug.LogErrorDev($"{BuffID}번 버프 최대 누적 횟수 {_maxStackCount}회 초과");
            return;
        }

        foreach (var pair in buff.GoodEffects)
        {
            if (GoodEffects.TryGetValue(pair.Key, out _))
            {
                GoodEffects[pair.Key] += pair.Value;
            }
        }
        
        foreach (var pair in buff.BadEffects)
        {
            if (BadEffects.TryGetValue(pair.Key, out _))
            {
                BadEffects[pair.Key] += pair.Value;
            }
        }

        CurrentStackCount++;
      //  Debug.LogErrorDev($"{buff.BuffID}번 버프가 {CurrentStackCount}/{_maxStackCount}회 누적되었습니다.");
    }

    public bool ConsumeBuffStack()
    {
        return CurrentStackCount-- > 0;
    }


    public virtual void BuffEnd()
    {
    }

    public Enum_Bad_Status_Effect GetStatusEffect()
    {
        if (IsStatusEffect)
        {
            if (BadEffects.ContainsKey(Enum_Bad_Status_Effect.Stun))
            {
                return Enum_Bad_Status_Effect.Stun;
            }
            if (BadEffects.ContainsKey(Enum_Bad_Status_Effect.Silence))
            {
                return Enum_Bad_Status_Effect.Silence;
            }
            if (BadEffects.ContainsKey(Enum_Bad_Status_Effect.Binding))
            {
                return Enum_Bad_Status_Effect.Binding;
            }
        }

        return Enum_Bad_Status_Effect.None;
    }

    public void ClearBuff()
    {
        RemainTime = 0;
        CurrentStackCount = 0;
    }

    public void ReduceRemain(float statValue)
    {
        if (statValue > 1)
        {
            RemainTime = 0;
            return;
        }
        
        RemainTime *= (1 - statValue);
    }

    public override string ToString()
    {
        return $"Buff_{From}_{BuffID}";
    }
}