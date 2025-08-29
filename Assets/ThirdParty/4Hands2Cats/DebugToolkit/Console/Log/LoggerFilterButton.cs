using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DebugToolkit.Console.Log
{
    public class LoggerFilterButton : MonoBehaviour, IPointerClickHandler
    {
        [Header("UI - Dependencies")]
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private Image image;

        [Header("Colors")]
        [SerializeField] private Color activeColor;
        [SerializeField] private Color inactiveColor;

        private bool active;

        public event Action<bool> OnActivation;

        private void Awake()
        {
            active = true;
        }

        public void UpdateText(string txt)
        {
            text.text = txt;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            active = !active;
            UpdateColor();
            OnActivation?.Invoke(active);
        }

        public void UpdateColor()
        {
            image.color = active ? activeColor : inactiveColor;
        }
    }
}

