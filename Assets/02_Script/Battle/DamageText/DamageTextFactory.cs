using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IdleBattle;

public class DamageTextFactory : ObjectPool<DamageTextFactory, DamageText>
{
    [SerializeField] private bool _useRandomPosition = true;

    public void ToggleRandomPosition(bool isOn)
    {
        _useRandomPosition = isOn;
    }

    public void Show(Vector3 position, Damage damage)
    {
var pooledDamageText = GetPooledObject();

        if (pooledDamageText == null)
        {
            return;
        }

        pooledDamageText.Show(position, damage, _useRandomPosition);
    }
}