using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Player : SingletonOnlyScene<Player>
{
    private int exp = 0;
    private int levelupExp = 10;
    private int level = 1;
    public int Level => level;
    
    public UnityEvent<int> OnLevelUp = new UnityEvent<int>();
    
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
        if (exp >= levelupExp)
        {
            exp = levelupExp;
            level++;
            OnLevelUp?.Invoke(level);
        }
    }

}
