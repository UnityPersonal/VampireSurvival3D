using System;
using UnityEngine;

public abstract class CombatEventArgs  : EventArgs
{
}

public class SpawnEventArgs : CombatEventArgs
{
}

public class DeathEventArgs : CombatEventArgs
{
    public Vector3 Position { get; set; }

    public DeathEventArgs(Vector3 position)
    {
        Position = position;
    }
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

public class DropItemEventArgs : CombatEventArgs
{
    public int Point {get; set;}
    public Vector3 Position {get; set;}

    public DropItemEventArgs(int point, Vector3 position)
    {
        Point = point;
        Position = position;
    }
}
