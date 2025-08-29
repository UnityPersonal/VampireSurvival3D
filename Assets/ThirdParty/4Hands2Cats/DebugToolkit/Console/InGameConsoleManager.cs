using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DebugToolkit.Console.Interaction
{
    public class InGameConsoleManager : MonoBehaviour
    {
        [Header("Inputs")]
        [SerializeField] private InputAction OpenClose;

        [Header("UI - Dependencies")]
        [SerializeField] private GameObject mainPanel;

        [Header("Dependcies")]
        [SerializeField] private InteractiveConsole consoleInteractive;
        [SerializeField] private DebugToolkit.Console.Log.Logger logger;

        //[Header("Params")]
        //[SerializeField] private int maxConsole = 100; // Unsused field

        [Header("Animation")]
        [SerializeField] private AnimationCurve openCurve;
        [SerializeField] private float animDuration;

        private bool isConsoleVisible = false;
        private RectTransform _mainPanelRT;
        private RectTransform _RT;
        private float targetSize;

        void Awake()
        {
            Application.logMessageReceived += logger.HandleApplicationLog;
            isConsoleVisible = false;
            mainPanel.SetActive(false);
            OpenClose.performed += i => Toggle();

            OpenClose.Enable();

            _mainPanelRT = mainPanel.transform as RectTransform;
            _RT = transform as RectTransform;

            Vector2 size = _mainPanelRT.sizeDelta;
            targetSize = size.y;
            if(targetSize <= 0)
                targetSize = 1080; // base res height

            size.y = 0;
            _mainPanelRT.sizeDelta = size;
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= logger.HandleApplicationLog;
            OpenClose.performed -= i => Toggle();
        }

        public void Toggle()
        {
            SetVisibility(!isConsoleVisible);
        }

        private void SetVisibility(bool visible)
        {
            isConsoleVisible = visible;
            AnimateOpenClose(visible);
        }

        private async void AnimateOpenClose(bool visible)
        {
            if (this == null) return;
            if (visible)
                mainPanel.SetActive(true);

            float time = 0;
            float advancement = 0;
            float eval;
            float startY = _mainPanelRT.sizeDelta.y;
            Vector2 currentSize = _mainPanelRT.sizeDelta;
            float targetY = visible ? targetSize : 0;
            
            while (advancement <= 1)
            {
                advancement = time / animDuration;
                eval = openCurve.Evaluate(advancement);

                if (targetY == 0)
                {
                    currentSize.y = startY - startY * eval;
                }
                else
                {
                    currentSize.y = targetY * eval;
                }

                _mainPanelRT.sizeDelta = currentSize;

                time += Time.deltaTime;
                await Awaitable.NextFrameAsync();
            }

            currentSize.y = targetY;
            _mainPanelRT.sizeDelta = currentSize;
            if (!visible)
                mainPanel.SetActive(false);
        }

        public async void ToggleConsole()
        {
            var mainPanelSizeFitter = mainPanel.GetComponent<ContentSizeFitter>();

            mainPanelSizeFitter.enabled = false;
            await Awaitable.NextFrameAsync();
            mainPanelSizeFitter.enabled = true;
        }
    }
}
