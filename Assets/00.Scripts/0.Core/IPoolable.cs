using UnityEngine;

public interface IPoolable<T> where T : MonoBehaviour
{
    public delegate void DisposePoolItem();
    
    public DisposePoolItem OnDispose{get;set;}
    
}
