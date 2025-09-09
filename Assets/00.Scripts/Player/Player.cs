using UnityEngine;

public class Player : SingletonOnlyScene<Player>
{
    public float dropRadius = 1f;
    protected override void InitializeSingleton()
    {
    }
}
