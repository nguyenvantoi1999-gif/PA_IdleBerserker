using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DamageText : GameBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private Text _damageText;

    private readonly int _normalDamage = Animator.StringToHash("Normal");
    private readonly int _criticalDamage = Animator.StringToHash("Critical");
    private readonly int _superCriticalDamage = Animator.StringToHash("SuperCritical");
    private readonly int _criticalReduce = Animator.StringToHash("CriticalReduce");
    private readonly int _playerHitDamage = Animator.StringToHash("PlayerHit");
    private readonly int _playerHeal = Animator.StringToHash("HP");
    private readonly int _evasion = Animator.StringToHash("Evasion");
    
    private readonly int _waterDamage = Animator.StringToHash("Blue");
    private readonly int _fireDamage = Animator.StringToHash("Red");
    private readonly int _grassDamage = Animator.StringToHash("Green");
    private readonly int _waterCriticalDamage = Animator.StringToHash("BlueCritical");
    private readonly int _fireCriticalDamage = Animator.StringToHash("RedCritical");
    private readonly int _grassCriticalDamage = Animator.StringToHash("GreenCritical");
    
    private readonly int _poisonDamage = Animator.StringToHash("Poison");
    private readonly int _poisonCriticalDamage = Animator.StringToHash("PoisonCritical");

    private readonly int _burnDamage = Animator.StringToHash("Burn");
    private readonly int _burnCriticalDamage = Animator.StringToHash("BurnCritical");

    private void Awake()
    {
        InitComponent();
    }

    private void InitComponent()
    {
        if (!_animator)
        {
            _animator = GetComponent<Animator>();
        }

        if (!_damageText)
        {
            _damageText = GetComponentInChildren<Text>();
        }
    }

    public void Show(Vector2 startPosition, Damage damage, bool useRandomPosition)
    {
        //if (damage.DamageType != IdleBattle.Enum_DamageType.Evasion)
        //{
        //    return;
        //}

        if (damage.Value <= 0)
        {
            damage.Value = 0;
        }
        
        startPosition.y *= 1.5f;
        
        if (useRandomPosition)
        {
            startPosition += Random.insideUnitCircle * 1.5f;
        }
    
        InitComponent();

        transform.position = startPosition;
        transform.SetAsLastSibling();

        _damageText.text = damage.Value.ToUnitString();

        SafeSetActive(true);
        _animator.SetTrigger(GetAnimationID(damage));
        //_animator.Play(GetAnimationID(damage), -1, 0f);
    }

    //todo: 프로토타입용 임시
    private int GetAnimationID(Damage damage)
    {
        switch (damage.DamageType)
        {
            case IdleBattle.Enum_DamageType.Water:
                if (damage.CriticalType == IdleBattle.Enum_CriticalType.Critical)
                {
                    _damageText.fontSize = 40;
                    return _waterCriticalDamage;
                }
                _damageText.fontSize = 28;
                return _waterDamage;
            case IdleBattle.Enum_DamageType.Fire:
                if (damage.CriticalType == IdleBattle.Enum_CriticalType.Critical)
                {
                    _damageText.fontSize = 40;
                    return _fireCriticalDamage;
                }
                _damageText.fontSize = 28;
                return _fireDamage;
            case IdleBattle.Enum_DamageType.Grass:
                if (damage.CriticalType == IdleBattle.Enum_CriticalType.Critical)
                {
                    _damageText.fontSize = 40;
                    return _grassCriticalDamage;
                }
                _damageText.fontSize = 28;
                return _grassDamage;
            
            case IdleBattle.Enum_DamageType.PlayerHit:
                _damageText.fontSize = 30;
                return _playerHitDamage;
            
            case IdleBattle.Enum_DamageType.Evasion:
                _damageText.fontSize = 20;
                _damageText.text = "Miss!";
                return _evasion;
            
            case IdleBattle.Enum_DamageType.HP:
                _damageText.fontSize = 24;
                return _playerHeal;

            case IdleBattle.Enum_DamageType.PoisonDamage:
                if (damage.CriticalType == IdleBattle.Enum_CriticalType.Critical)
                {
                    _damageText.fontSize = 40;
                    return _poisonCriticalDamage;
                }
                _damageText.fontSize = 28;
                return _poisonDamage;

            case IdleBattle.Enum_DamageType.BurnDamage:
                if (damage.CriticalType == IdleBattle.Enum_CriticalType.Critical)
                {
                    _damageText.fontSize = 40;
                    return _burnCriticalDamage;
                }
                _damageText.fontSize = 28;
                return _burnDamage;

            default:
            {
                switch (damage.CriticalType)
                {
                    case IdleBattle.Enum_CriticalType.Critical:
                        _damageText.fontSize = 40;
                        return _criticalDamage;   
                    case IdleBattle.Enum_CriticalType.SuperCritical:
                        _damageText.fontSize = 28;
                        return _superCriticalDamage;
                    case IdleBattle.Enum_CriticalType.Reduced:
                        _damageText.fontSize = 32;
                        return _criticalReduce;
                    
                    default:
                        _damageText.fontSize = 32;
                        return _normalDamage;
                }
            }
        }
    }

    public void Hide()
    {
        SafeSetActive(false);
    }
}