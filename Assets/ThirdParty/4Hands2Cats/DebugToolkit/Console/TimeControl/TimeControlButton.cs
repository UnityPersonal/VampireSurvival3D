using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DebugToolkit.Console
{
    public class TimeControlButton : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI - Dependencies")]
        [SerializeField] private Image image;

        [Header("Colors")]
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;

        private bool active;
        public bool Active { get { return active; } set { active = value; } }

        public UnityEvent OnActivation;

        public void OnPointerClick(PointerEventData eventData)
        {
            OnActivation?.Invoke();
            UpdateColor();
        }

        public void UpdateColor()
        {
            image.color = active ? activeColor : inactiveColor;
        }

        public async void UpdateColorAsync()
        {
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            await Awaitable.NextFrameAsync();
            active = !active;
            UpdateColor();
        }
    }
}
