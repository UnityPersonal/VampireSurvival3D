using UnityEditor.UIElements;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using DebugToolkit.ReportForm.Settings;

namespace UDT.Report.Inspector
{
    [CustomEditor(typeof(TrelloSettings))]
    public class ReportSettingsInspector : Editor
    {
        private TrelloSettings _trelloSettings;
        private VisualElement _root;
        private VisualTreeAsset _visualTree;

        private VisualElement _init;
        private VisualElement _api;
        private VisualElement _token;
        private VisualElement _trello;

        TextField tokenField;
        private bool _initializing;

        private void OnEnable()
        {
            _trelloSettings = (TrelloSettings)target;

            _root = new VisualElement();
            _visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/4Hands2Cats/DebugToolkit/Editor/Inspectors/TrelloInspectors/ReportSettingsInspector.uxml");

            StyleSheet ss = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/4Hands2Cats/DebugToolkit/Editor/Inspectors/InspectorUss.uss");
            _root.styleSheets.Add(ss);
        }

        public override VisualElement CreateInspectorGUI()
        {
            _root.Clear();
            _visualTree.CloneTree(_root);
            _init = _root.Q<VisualElement>("Init");
            _api = _root.Q<VisualElement>("API");
            _token = _root.Q<VisualElement>("TokenContainer");
            _trello = _root.Q<VisualElement>("Trello");

            var serializedObject = new SerializedObject(_trelloSettings);

            var apiField = _root.Q<TextField>("APIKey");
            apiField.Bind(serializedObject);
            apiField.bindingPath = "apiKey";
            apiField.RegisterValueChangedCallback(txt => HandleInitSection());

            tokenField = _root.Q<TextField>("Token");
            tokenField.Bind(serializedObject);
            tokenField.bindingPath = "token";
            tokenField.RegisterValueChangedCallback(txt => HandleInitSection());

            var workspaceField = _root.Q<TextField>("WorkspaceName");
            workspaceField.Bind(serializedObject);
            workspaceField.bindingPath = "workspaceName";

            var boardField = _root.Q<TextField>("BoardName");
            boardField.Bind(serializedObject);
            boardField.bindingPath = "boardName";

            var listField = _root.Q<TextField>("ListName");
            listField.Bind(serializedObject);
            listField.bindingPath = "listName";

            var initButton = _root.Q<Button>("btn_init");
            var apiButton = _root.Q<Button>("btn_goToApi");
            var tokenButton = _root.Q<Button>("btn_goToToken");
            var trelloButton = _root.Q<Button>("btn_goToTrello");
            var updateLabels = _root.Q<Button>("btn_updateLabels");

            initButton.clicked += async () =>
            {
                if (_initializing) return;
                _initializing = true;
                await _trelloSettings.Init();
                HandleInitSection();
                _initializing = false;
            };

            apiButton.clicked += () =>
            {
                _trelloSettings.OpenAPIKeyPage();
            };

            tokenButton.clicked += () =>
            {
                _trelloSettings.OpenTokenPage();
            };

            trelloButton.clicked += () =>
            {
                _trelloSettings.OpenBoardPage();
            };

            updateLabels.clicked += () =>
            {
                _trelloSettings.OnResetLabels();
            };

            HandleInitSection();

            return _root;
        }

        private void HandleInitSection()
        {
            HideAll();
            if (string.IsNullOrEmpty(_trelloSettings.ApiKey))
            {
                _api.style.display = DisplayStyle.Flex;
                tokenField.style.display = DisplayStyle.None;
                return;
            }

            if (string.IsNullOrEmpty(_trelloSettings.Token))
            {
                _token.style.display = DisplayStyle.Flex;
                tokenField.style.display = DisplayStyle.Flex;
                return;
            }

            if (string.IsNullOrEmpty(_trelloSettings.WorkspaceId) &&
                string.IsNullOrEmpty(_trelloSettings.BoardId) &&
                string.IsNullOrEmpty(_trelloSettings.ListId))
            {
                _init.style.display = DisplayStyle.Flex;
                return;
            }

            _trello.style.display = DisplayStyle.Flex;
        }

        private void HideAll()
        {
            _init.style.display = DisplayStyle.None;
            _token.style.display = DisplayStyle.None;
            _api.style.display = DisplayStyle.None;
            _trello.style.display = DisplayStyle.None;
        }
    }
}

