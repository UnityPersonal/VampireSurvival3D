using DebugToolkit.ReportForm.APIs.Discord;
using DebugToolkit.ReportForm.Settings;
using DebugToolkit.ReportForm.Trello;
using System;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace DebugToolkit.ReportForm
{
    public class ReportManager : MonoBehaviour
    {
        [SerializeField] EReportTarget reportTarget = EReportTarget.Trello;

        [Header("Settings")]
        [SerializeField] private TrelloSettings trelloSettings;
        private TrelloAPI _trelloAPI;
        [Space]
        private DiscordAPI _discordAPI;

        [Header("Ui -Ref")]
        [SerializeField] private GameObject formRoot;
        [Space]
        [SerializeField] private TMP_InputField title;
        [SerializeField] private TMP_InputField description;
        [SerializeField] private TMP_Dropdown reportType;
        [Space]
        [SerializeField] private Button sendForm;
        [SerializeField] private Button cancelForm;
        [SerializeField] private Button closeForm;
        [SerializeField] private Button openForm;

        [Header("Input")]
        [SerializeField] private InputAction toggleForm;

        private Vector2 _initialSize;
        private RectTransform _formRt;
        private bool _animating;

        [Header("Params")]
        [SerializeField] private float animDuration = 1.5f;

        public UnityEvent onFormClosed;
        public UnityEvent onFormOpened;

        private async void Awake()
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();

            _trelloAPI = new TrelloAPI(trelloSettings.ApiKey, trelloSettings.Token);
            _discordAPI = new DiscordAPI();
            toggleForm.performed += ToggleForm;
            cancelForm.onClick.AddListener(() => CloseForm());
            closeForm.onClick.AddListener(() => CloseForm());
            sendForm.onClick.AddListener(() => SendForm());
            openForm.onClick.AddListener(() => OpenForm());

            _formRt = (formRoot.transform as RectTransform);
            _initialSize = _formRt.sizeDelta;

            _formRt.sizeDelta = new Vector2(0, _initialSize.y);
            _formRt.gameObject.gameObject.SetActive(false);

            reportType.ClearOptions();
            reportType.AddOptions(Enum.GetNames(typeof(EReportType)).ToList());
            reportType.SetValueWithoutNotify(0);
        }

        private void OnEnable()
        {
            toggleForm.Enable();
        }

        private void OnDisable()
        {
            toggleForm.Disable();
        }

        private async void OpenForm()
        {
            if (_animating) return;
            _animating = true;

            onFormOpened?.Invoke();

            formRoot.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            float time = 0;
            while(time < animDuration)
            {
                time += Time.deltaTime;
                _formRt.sizeDelta = Vector2.Lerp(_formRt.sizeDelta, _initialSize, time / animDuration);
                await Awaitable.NextFrameAsync();
            }

            _formRt.sizeDelta = _initialSize;
            _animating = false;
        }

        private async void CloseForm()
        {
            if (_animating) return;
            _animating = true;

            onFormClosed?.Invoke();

            /// Pas spécialement necessaire et bloquant sur mobile
            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = false;

            float time = 0;
            while (time < animDuration)
            {
                time += Time.deltaTime;
                _formRt.sizeDelta = Vector2.Lerp(_formRt.sizeDelta, new Vector2(_initialSize.x, 0), time / animDuration);
                await Awaitable.NextFrameAsync();
            }

            formRoot.SetActive(false);
            _animating = false;
        }

        private void ToggleForm(InputAction.CallbackContext context)
        {
            if (formRoot.activeSelf)
            {
                CloseForm();
            }
            else
            {
                OpenForm();
            }
        }

        private void SendForm()
        {
            if (string.IsNullOrEmpty(title.text)) 
            {
                title.text = "BugReport";       
            }
            if (string.IsNullOrEmpty(description.text))
            {
                Debug.LogError("Description is empty!");
                description.placeholder.color = Color.red;
                description.text = "Please enter a description!";
                return;
            }

            CreateCard(title.text, description.text, (EReportType)reportType.value);
        }

        public async void CreateCard(string title, string description, EReportType typeValue)
        {
            string screenShotPath = await CaptureScreenshot();

            var labels = await _trelloAPI.CheckIfLabelsExist(trelloSettings.BoardId);
            if (labels != null)
            {
                trelloSettings.SetLabelIds(labels);
            }

            Debug.Log(trelloSettings.LabelsIDs);
            switch (reportTarget)
            {
                case EReportTarget.Trello:
                    await _trelloAPI.CreateCard(title, description, typeValue, trelloSettings.ListId, trelloSettings.LabelsIDs, screenShotPath);
                    break;
                case EReportTarget.Discord:
                    await _discordAPI.SendReport(title, description, screenShotPath);
                    break;
            }

        }

        public async Task<string> CaptureScreenshot()
        {
            string folderPath = System.IO.Path.Combine(Application.dataPath, "Screenshots");
            string screenshotPath = System.IO.Path.Combine(folderPath, "screenshot.png");

            CloseForm();

            await Awaitable.NextFrameAsync();

            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
                Debug.Log("Created folder: " + folderPath);
            }

            Debug.Log("Saving screenshot at: " + screenshotPath);
            ScreenCapture.CaptureScreenshot(screenshotPath);

            await Awaitable.WaitForSecondsAsync(1.5f);

            if (!System.IO.File.Exists(screenshotPath))
            {
                Debug.LogError("Screenshot file not found after waiting!");
                return null;
            }

            Debug.Log("Screenshot saved successfully at: " + screenshotPath);
            return screenshotPath;
        }

        private void OnValidate()
        {
            switch (reportTarget)
            {
                case EReportTarget.None:
                    Debug.Log("Current report target is NOne, please set up a report target");
                    break;
                case EReportTarget.Trello:
                    Debug.Log("Current report target is Trello, refer to the doc : " + "Link to the doc"
                        );
                    break;
                case EReportTarget.Discord:
                    Debug.Log("Report target : Discord, please refer to the doc for more information : " + "Link to the doc"
                        );
                    break;
            }
        }
    }
}
