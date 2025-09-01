using System;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;

public abstract class GameObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField, Required, AssetsOnly] private T sample;
    ObjectPool<T> pool;

    protected T PopPoolItem()
    {
        return pool.Get();
    }
    
    protected virtual void Awake()
    {
        pool = new ObjectPool<T>(
            CreatePoolItem,
            GetPoolItem,
            ReleasePoolItem,
            DestroyPoolItem
        );
    }

    private void DestroyPoolItem(T obj)
    {
    }

    private void ReleasePoolItem(T obj)
    {
        obj.gameObject.SetActive(false);
    }

    private void GetPoolItem(T obj)
    {
        obj.gameObject.SetActive(true);
    }


    private T CreatePoolItem()
    {
        T instance = InstantiatePoolItem(sample);
        if (instance.TryGetComponent(out IPoolable<T> poolable))
        {
            poolable.OnDispose = () => { pool.Release(instance); };
        }
        else
        {
            Debug.LogError( $" Could not create instance of {sample.name}");
        }
        return instance;
    }

    private void DisposePoolItem(IPoolable<T> obj)
    {
        
        
    }

    protected abstract T InstantiatePoolItem(T sample);


    // Update is called once per frame
    void Update()
    {
        
    }
}

