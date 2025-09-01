using System;
using UnityEngine;

public abstract class CombatEventArgs  : EventArgs
{
}

public class SpawnEventArgs : CombatEventArgs
{
}

public class DealEventArgs : CombatEventArgs
{
    public float DealDamage { get; set; }
    public DealEventArgs(float dealDamage)
    {
        DealDamage = dealDamage;
    }
}

public class TakeDamageEventArgs : CombatEventArgs
{
    public ICombatable Taker { get; set; }
    public int DamageTaken { get; set; }

    public TakeDamageEventArgs(ICombatable taker, int damageTaken)
    {
        Taker = taker;
        DamageTaken = damageTaken;
    }
}
