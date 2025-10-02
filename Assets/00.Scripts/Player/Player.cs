using System;
using Unity.VisualScripting;
using UnityEngine;

public class Player : SingletonOnlyScene<Player>
{
    private int exp = 0;
    private int level = 1;
    public int Level => level;
    public float dropRadius = 1f;
    protected override void InitializeSingleton()
    {
    }

    private void OnEnable()
    {
        GameEventManager.RegistListener<DropItemEventArgs>(OnDrop);
    }

    private void OnDrop(DropItemEventArgs obj)
    {
        exp += obj.Point;

    }

}
