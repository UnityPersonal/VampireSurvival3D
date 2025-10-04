using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Player : SingletonOnlyScene<Player>
{
    [SerializeField] private GameValue<int> exp =new GameValue<int>();
    private int levelupExp = 10;
    [SerializeField] GameValue<int> level = new GameValue<int>();
    public GameValue<int> Level => level;
    
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
        exp.Value += obj.Point;
        if (exp.Value >= levelupExp)
        {
            exp.Value = levelupExp;
            level.Value += 1;
        }
    }

}
