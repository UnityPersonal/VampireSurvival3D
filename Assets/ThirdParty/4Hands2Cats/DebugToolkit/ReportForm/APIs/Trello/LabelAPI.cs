using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.Networking;

namespace DebugToolkit.ReportForm.Trello
{
    public class LabelAPI
    {
        public LabelAPI(string apiKey, string token)
        {
            API_KEY = apiKey;
            TOKEN = token;
        }

        private string API_KEY;
        private string TOKEN;

        //green, yellow, orange, red, purple, blue, sky, lime, pink, black -> to change to allow customization V1.1
        private Dictionary<EReportType, string> _labelColors = new Dictionary<EReportType, string>()
        {
            { EReportType.Bug, "blue" },
            { EReportType.SoftLock, "yellow" },
            { EReportType.HardLock, "orange" },
            { EReportType.Critical, "red" },
            { EReportType.Graphical, "green" }
        };

        public async Task<Dictionary<EReportType, string>> CheckIfLabelsExist(string boardId)
        {
            string url = $"https://api.trello.com/1/boards/{boardId}/labels?key={API_KEY}&token={TOKEN}&limit=100";

            UnityWebRequest request = UnityWebRequest.Get(url);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                List<TrelloLabel> labels = JsonHelper.FromJson<TrelloLabel>(json);

                if (labels.Count < Enum.GetNames(typeof(EReportType)).Length)
                {
                    return await CreateLabels(boardId);
                }
            }
            else
            {
                Debug.LogError("❌ Failed to fetch labels: " + request.error);
            }

            return null;
        }

        public async Task<List<string>> GetAllLabelIds(string boardId)
        {
            List<string> ids = new List<string>();
            string url = $"https://api.trello.com/1/boards/{boardId}/labels?key={API_KEY}&token={TOKEN}";

            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    var json = request.downloadHandler.text;
                    var labels = JsonUtilityWrapper.FromJson<LabelList>("{\"labels\":" + json + "}");
                    foreach (var label in labels.labels)
                    {
                        ids.Add(label.id);
                    }
                }
                else
                {
                    Debug.LogError($"❌ Failed to fetch labels: {request.error}");
                }
            }

            return ids;
        }

        public async Task DeleteLabel(string labelId)
        {
            string url = $"https://api.trello.com/1/labels/{labelId}?key={API_KEY}&token={TOKEN}";

            using (UnityWebRequest request = UnityWebRequest.Delete(url))
            {
                await request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"✅ Deleted label {labelId}");
                }
                else
                {
                    Debug.LogError($"❌ Failed to delete label {labelId}: {request.error}");
                }
            }
        }

        public async Task<Dictionary<EReportType, string>> CreateLabels(string boardId)
        {
            Dictionary<EReportType, string> labelIds = new();
            foreach (var entry in _labelColors)
            {
                string labelId = await CreateLabel(boardId, entry.Key.ToString(), entry.Value);
                if (!string.IsNullOrEmpty(labelId))
                {
                    labelIds[entry.Key] = labelId;
                }
            }

            return labelIds;
        }

        private async Task<string> CreateLabel(string boardId, string labelName, string color)
        {
            string url = $"https://api.trello.com/1/labels?key={API_KEY}&token={TOKEN}";

            WWWForm form = new WWWForm();
            form.AddField("name", labelName);
            form.AddField("color", color.ToLower()); // Trello accepts lowercase colors
            form.AddField("idBoard", boardId);

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Label '{labelName}' created successfully!");
                return JsonHelper.ExtractId(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"❌ Failed to create label '{labelName}': {request.error}");
                return null;
            }
        }
    }
}
