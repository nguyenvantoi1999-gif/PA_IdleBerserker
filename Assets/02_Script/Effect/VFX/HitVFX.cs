using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class HitVFX : MultiPoolItem
{
    public Enum_DamageType DamageType;

    private ParticleSystem _particle;

    [SerializeField] private float _duration = 1f;

    private ParticleSystem Particle
    {
        get
        {
            if (_particle == null)
            {
                _particle = GetComponentInChildren<ParticleSystem>();
            }

            return _particle;
        }
    }

    public void Show(Vector3 position)
    {
        transform.position = position + Random.insideUnitSphere;

        SafeSetActive(true);
        
        StartCoroutine(OnShow_Coroutine());
    }
    
    private IEnumerator OnShow_Coroutine()
    {
        Particle.Stop();
        Particle.Play();
        
        yield return new WaitForSecondsRealtime(_duration);
        
        Hide();
    }
    
    public void Hide()
    {
        SafeSetActive(false);
    }
}