using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaApplier : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _lastSafeArea = Rect.zero;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    void Update()
    {
        if (_lastSafeArea != Screen.safeArea)
        {
            ApplySafeArea();
        }
    }

    void ApplySafeArea()
    {
        _lastSafeArea = Screen.safeArea;

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 anchorMin = _lastSafeArea.position / screenSize;
        Vector2 anchorMax = (_lastSafeArea.position + _lastSafeArea.size) / screenSize;

        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
        _rectTransform.offsetMin = Vector2.zero;
        _rectTransform.offsetMax = Vector2.zero;
    }
}
