using System;
using DG.Tweening;
using UnityEngine;

public class DropItem : MonoBehaviour , IPoolable<DropItem> , IDisposable
{
    public bool isTriggerd = false;
    public float triggerRadius = 1f;
    
    private Transform destination;
    private Transform itemTransform;
    private float toward = 0;
    [SerializeField] int point = 5;
    private void Start()
    {
        itemTransform = transform;
        destination = Player.Instance.transform;
    }

    private void OnEnable()
    {
        toward = 0;
        isTriggerd = false;
        transform.localScale = Vector3.one;
        
    }

    public void OnTriggerDrop()
    {
        isTriggerd = true;
        
        var sequence = DOTween.Sequence();
        var tween = DOTween.To(
            () => toward,
            x => itemTransform.position = Vector3.Lerp(itemTransform.position, destination.position, x),
            1f,
            1
        );
        
        sequence.Join(tween);
        var scaleTween = transform.DOScale(Vector3.zero, 1);
        
        sequence.Join(scaleTween);
        sequence.OnComplete(Dispose).SetAutoKill();
    }

    private void Update()
    {
        if (isTriggerd) return;
        
        var offset = destination.position - itemTransform.position;
        if (offset.magnitude < triggerRadius)
        {
            OnTriggerDrop();
        }
    }

    public IPoolable<DropItem>.DisposePoolItem OnDispose { get; set; }

    public void Dispose()
    {
        OnDispose?.Invoke();
    }
}
