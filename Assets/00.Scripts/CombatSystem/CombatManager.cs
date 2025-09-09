using System.Collections.Generic;
using UnityEngine;

public class CombatManager : SingletonOnlyScene<CombatManager>
{
    Dictionary<Collider, ICombatable> registedCombators = new Dictionary<Collider, ICombatable>(); 
    
    protected override void InitializeSingleton()
    {
    }

    public static bool TryLookup(Collider collider, out ICombatable  combatable)
    {
        combatable = null;
        if (Instance is null)
        {
            return false;
        }
        var table = Instance.registedCombators;
        if (table.TryGetValue(collider, out combatable))
        {
            return true;
        }
        
        return false;
    }

    public static void Regist(Collider key, ICombatable register)
    {
        if (Instance is null)
        {
            return;
        }

        var table = Instance.registedCombators;
        if (table.ContainsKey(key))
        {
            Debug.LogError($"already registered for {key.name} / {register.CombatTransform.gameObject.name}");
        }
        else
        {
            table.Add(key, register);
        }
    }

    public static void Unregist(Collider key)
    {
        if (Instance is null)
        {
            return;
        }

        var table = Instance.registedCombators;

        if (table.ContainsKey(key))
        {
            table.Remove(key);
        }
        else
        {
            Debug.LogError($"not found for {key.name} / {key.gameObject.name}");
        }

    }
}
