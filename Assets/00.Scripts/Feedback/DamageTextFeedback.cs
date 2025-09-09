using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class DamageTextFeedback : MonoBehaviour , IPoolable<DamageTextFeedback> , IDisposable
{
    
    [SerializeField] float duration;
    [ReadOnly] float startTime = 0f;
    
    private RectTransform parentRectTransform;
    private Transform target;
    private Camera mainCamera;
    
    [SerializeField, Required] private RectTransform rectTransform;
    [SerializeField, Required] private TMP_Text feedbackText;

    private void Awake()
    {
        parentRectTransform  = transform.parent.GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    public void Setup(Transform target,string message)
    {
        startTime = Time.time;
        this.target = target;
        feedbackText.text = message;
    }
    // Update is called once per frame
    void Update()
    {
        if (!target)
        {
            Dispose();
            return;
        }
        var worldPoint = target.position;
        var screenPoint = mainCamera.WorldToScreenPoint(worldPoint);
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTransform, screenPoint, null, out Vector2 localPoint))
        {
            rectTransform.anchoredPosition = localPoint;
        }
        else
        {
            Debug.LogError("Screen point is null");
            Dispose();
        }
        
        
        if (Time.time >= startTime + duration)
        {
            Dispose();
        }
    }

    public void Dispose()
    {
        OnDispose?.Invoke();
    }

    public IPoolable<DamageTextFeedback>.DisposePoolItem OnDispose { get; set; }
}
