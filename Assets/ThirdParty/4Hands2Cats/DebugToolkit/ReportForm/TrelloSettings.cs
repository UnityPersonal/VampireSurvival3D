using DebugToolkit.ReportForm.Trello;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DebugToolkit.ReportForm.Settings
{
    [CreateAssetMenu(fileName = "TrelloSettings", menuName = "DebugToolkit/Report/TrelloSettings")]
    public class TrelloSettings : ScriptableObject
    {
        [Header("Trello Settings")]
        [SerializeField] private string apiKey;
        [SerializeField] private string token;
        public string ApiKey => apiKey;
        public string Token => token;

        [Header("Trello Board Settings")]
        [SerializeField] private string workspaceName;
        [SerializeField] private string boardName;
        [SerializeField] private string listName;

        [SerializeField] private string _workspaceId;
        [SerializeField] private string _boardId;
        [SerializeField] private string _listId;
        [SerializeField] private List<ReportLabelEntry> _labelsIDs;

        public string WorkspaceId => _workspaceId;
        public string BoardId => _boardId;
        public string ListId => _listId;

        public Dictionary<EReportType, string> LabelsIDs
        {
            get
            {
                var d = new Dictionary<EReportType, string>();
                foreach (var label in _labelsIDs)
                {
                    d.Add(label.ReportType, label.LabelId);
                }
                return d;
            }
        }

        private TrelloAPI _trelloAPI;

        private const string TRELLO_API_KEY_URL = "https://trello.com/app-key";
        private const string TRELLO_TOKEN_URL = "https://trello.com/1/authorize?expiration=never&scope=read,write&response_type=token&key=";
        private const string TRELLO_BOARDS_URL = "https://trello.com/b/";

        public async Task Init()
        {
            _trelloAPI = new TrelloAPI(apiKey, token);

            if (string.IsNullOrEmpty(_workspaceId))
            {
                if (string.IsNullOrEmpty(workspaceName))
                {
                    workspaceName = "Default Workspace";
                }
                _workspaceId = await _trelloAPI.CreateWorkspace(workspaceName);
            }

            if (string.IsNullOrEmpty(_boardId))
            {
                if (string.IsNullOrEmpty(boardName))
                {
                    boardName = "Default Board";
                }
                var d = new Dictionary<EReportType, string>();
                _boardId = await _trelloAPI.CreateBoard(boardName, _workspaceId, d);
                SetLabelIds(d);
            }

            if (string.IsNullOrEmpty(_listId))
            {
                if (string.IsNullOrEmpty(listName))
                {
                    listName = "Default List";
                }
                _listId = await _trelloAPI.CreateList(listName, _boardId);
            }
        }

        public void OpenAPIKeyPage()
        {
            Application.OpenURL(TRELLO_API_KEY_URL);
        }

        public void OpenTokenPage()
        {
            Application.OpenURL(TRELLO_TOKEN_URL + apiKey);
        }

        public void OpenBoardPage()
        {
            Application.OpenURL(TRELLO_BOARDS_URL + _boardId);
        }

        [ContextMenu("Resetaeztze")]
        public void Lab()
        {
            OnResetLabels();
        }

        public async void OnResetLabels()
        {
            _trelloAPI = new TrelloAPI(apiKey, token);  
            List<string> labelIds = await _trelloAPI.GetAllLabelIds(_boardId);
            foreach (var labelId in labelIds)
            {
                await _trelloAPI.DeleteLabel(labelId);
            }

            var labels = await _trelloAPI.CreateLabels(_boardId);
            SetLabelIds(labels);

            Debug.Log("✅ Label Successfully reinitialized");
        }

        public void SetLabelIds(Dictionary<EReportType, string> labelsIDs)
        {
            _labelsIDs = new List<ReportLabelEntry>();
            foreach (var label in labelsIDs)
            {
                _labelsIDs.Add(new ReportLabelEntry
                {
                    ReportType = label.Key,
                    LabelId = label.Value
                });
            }
        }

        [Serializable]
        public class ReportLabelEntry
        {
            public EReportType ReportType;
            public string LabelId;
        }
    }
}
