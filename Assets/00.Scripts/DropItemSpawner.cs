using System;
using UnityEngine;

public class DropItemSpawner : GameObjectPool<DropItem>
{
    protected override DropItem InstantiatePoolItem(DropItem sample)
    {
        return Instantiate(sample, transform);
    }

    private void Start()
    {
        GameEventManager.RegistListener<DropItemEventArgs>( OnDropItem );
    }

    private void OnDropItem(DropItemEventArgs args)
    {
        var item = PopPoolItem();
        item.transform.position = args.Position;
        item.transform.localScale = Vector3.one;
        Debug.Log($"Drop Itme {args.Position} " );
    }

}
