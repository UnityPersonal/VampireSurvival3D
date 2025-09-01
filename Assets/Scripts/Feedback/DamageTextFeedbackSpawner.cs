using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Pool;

public class DamageTextFeedbackSpawner : GameObjectPool<DamageTextFeedback>
{
    protected override DamageTextFeedback InstantiatePoolItem(DamageTextFeedback sample)
    {
        var instance = Instantiate(sample, transform);
        return sample;
    }

    private void Start()
    {
        GameEventManager.RegistListener<TakeDamageEventArgs>(OnTakeDamage);
    }

    private void OnTakeDamage(TakeDamageEventArgs obj)
    {
        var feedback = PopPoolItem();
        feedback.Setup(obj.Taker.CombatTransform, obj.DamageTaken.ToString());
    }
}
