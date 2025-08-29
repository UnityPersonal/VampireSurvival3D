using DebugToolkit.ReportForm.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace DebugToolkit.ReportForm.Trello
{
    public class TrelloAPI 
    {
        public TrelloAPI(string apiKey, string token)
        {
            API_KEY = apiKey;
            TOKEN = token;

            _labelAPI = new LabelAPI(apiKey, token);
        }

        private string API_KEY;
        private string TOKEN;

        private LabelAPI _labelAPI;

        public async Task<string> CreateWorkspace(string workspaceName)
        {
            string url = $"https://api.trello.com/1/organizations?displayName={UnityWebRequest.EscapeURL(workspaceName)}&key={API_KEY}&token={TOKEN}";

            UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
            await request.SendWebRequest();

            string workspaceId = null;

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("\nWorkspace Created: " + request.downloadHandler.text);
                workspaceId = JsonHelper.ExtractId(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("\nError creating workspace: " + request.error);
            }

            return workspaceId;
        }

        public async Task<string> CreateBoard(string boardName, string workspaceId, Dictionary<EReportType, string> labelIds)
        {
            string url = $"https://api.trello.com/1/boards/?name={UnityWebRequest.EscapeURL(boardName)}&idOrganization={workspaceId}&key={API_KEY}&token={TOKEN}";

            UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
            await request.SendWebRequest();

            string boardId = null;

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("\nBoard Created: " + request.downloadHandler.text);
                boardId = JsonHelper.ExtractId(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("\nError creating board: " + request.error);
            }

            labelIds = await CreateLabels(boardId);

            return boardId;
        }

        public async Task<string> CreateList(string listName, string boardId)
        {
            string url = $"https://api.trello.com/1/lists?name={UnityWebRequest.EscapeURL(listName)}&idBoard={boardId}&key={API_KEY}&token={TOKEN}";

            UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
            await request.SendWebRequest();

            string listId = null;

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("\nList Created: " + request.downloadHandler.text);
                listId = JsonHelper.ExtractId(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("\nError creating list: " + request.error);
            }

            return listId;
        }

        public async Task<string> CreateCard(string title, string userDescription, EReportType reportType, string listId,
            Dictionary<EReportType, string> labelIds, string imagePath = null)
        {
            string url = $"https://api.trello.com/1/cards?idList={listId}&key={API_KEY}&token={TOKEN}";

            string systemSpecs = ReportUtils.GetSystemSpecs();

            string logPath = ReportUtils.GetPlayerLogPath();

            string fullDescription = $"📝 **User Description:**\n{userDescription}\n\n" +
                             systemSpecs;

            WWWForm form = new WWWForm();
            form.AddField("name", $"{title} - {DateTime.Now}");
            form.AddField("desc", fullDescription);


            if (labelIds.TryGetValue(reportType, out string labelId))
            {
                form.AddField("idLabels", labelId);
            }
            else
            {
                Debug.LogWarning($"⚠️ No label found for report type {reportType}");
            }

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            await request.SendWebRequest();

            string cardId = null;

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Log successfully sent to Trello!");
                if (!string.IsNullOrEmpty(imagePath))
                {
                    cardId = JsonHelper.ExtractId(request.downloadHandler.text);
                    UploadFileToCard(imagePath, cardId, true);
                    UploadFileToCard(logPath, cardId, false);
                }
            }
            else
            {
                Debug.LogError("Failed to send log to Trello: " + request.error);
            }

            return cardId;
        }

        private async void UploadFileToCard(string filePath, string cardId, bool isImage)
        {
            string url = $"https://api.trello.com/1/cards/{cardId}/attachments?key={API_KEY}&token={TOKEN}";

            byte[] fileData = File.ReadAllBytes(filePath);

            WWWForm form = new WWWForm();
            if (isImage)
            {
                form.AddBinaryData("file", fileData, Path.GetFileName(filePath), "image/png");
            }
            else
            {
                form.AddBinaryData("file", fileData, Path.GetFileName(filePath), "text/plain");
            }

            UnityWebRequest request = UnityWebRequest.Post(url, form);
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                if (isImage)
                {
                    Debug.Log("🖼️ Image successfully uploaded to Trello card!");
                }
                else
                {
                    Debug.Log("📂 Player.log successfully uploaded to Trello!");
                }
            }
            else
            {
                if (isImage)
                {
                    Debug.LogError("❌ Failed to upload image: " + request.error);
                }
                else
                {
                    Debug.LogError("❌ Failed to upload Player.log: " + request.error);
                }
            }
        }

        public async Task<Dictionary<EReportType, string>> CreateLabels(string boardId)
        {
            return await _labelAPI.CreateLabels(boardId);
        }

        public async Task<Dictionary<EReportType, string>> CheckIfLabelsExist(string boardId)
        {
            return await _labelAPI.CheckIfLabelsExist(boardId);
        }

        public async Task<List<string>> GetAllLabelIds(string boardId)
        {
            return await _labelAPI.GetAllLabelIds(boardId);
        }

        public async Task DeleteLabel(string labelId)
        {
            await _labelAPI.DeleteLabel(labelId);
        }
    }
}
