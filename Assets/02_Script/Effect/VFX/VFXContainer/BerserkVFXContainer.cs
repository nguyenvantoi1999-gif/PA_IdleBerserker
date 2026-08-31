using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkVFXContainer : MonoBehaviour
{
    [Header("광전사 이펙트")] [SerializeField] private GameObject _berserkIdleVfxs;
    
    [Header("광전사 진입 이펙트")] 
    [SerializeField] private ParticleSystem _berserkEnterEffect;
    // ___SSSR
    [SerializeField] private ParticleSystem _berserkSSREnterEffect;

    [Header("광전사 눈 트레일")] [SerializeField] private Transform _berserkEyeEffect;
        
    [Header("광전사 진입 이펙트")] 
    [SerializeField] private ParticleSystem _berserkerDashEffect;
    
    [Header("광전사 아우라")] [SerializeField] private BerserkerAuraVFXContainer _berserkerAuraVFX;

    [Header("Groggy")] 
    [SerializeField] private ParticleSystem _berserkerGroggyEffect;
    
    public void ChangeBerserkEffectActivation(bool isOn)
    {
        // NOTE(PA port): lược nhánh IsEventWorld()/IsMaxPromotion() vì PA khong co
        // BattleManager/PlayerDataManager cua game goc. Giu toggle aura + enter effect.
        if (_berserkIdleVfxs != null)
        {
            _berserkIdleVfxs.SetActive(isOn);
        }
        if (_berserkEnterEffect != null)
        {
            _berserkEnterEffect.gameObject.SetActive(isOn);
            if (_berserkEnterEffect.gameObject.activeSelf && isOn)
            {
                _berserkEnterEffect.gameObject.SetActive(false);
                _berserkEnterEffect.gameObject.SetActive(true);
            }
        }
    }

    public Transform GetBerserkEyeTrailEffect()
    {
        return _berserkEyeEffect;
    }

    public void PlayDashEffect()
    {
        _berserkerDashEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        _berserkerDashEffect.Play();
    }

    public void SetAuraEffect(int index)
    {
        _berserkerAuraVFX.Init(index);
    }

    public void SetGroggyEffect(bool isOn)
    {
        _berserkerGroggyEffect.gameObject.SetActive(isOn);
        _berserkerGroggyEffect.Stop();
        if (isOn)
        {
            _berserkerGroggyEffect.Play();
        }
    }
}
