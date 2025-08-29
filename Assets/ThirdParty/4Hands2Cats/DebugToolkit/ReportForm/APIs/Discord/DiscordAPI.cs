using DebugToolkit.ReportForm.Settings;
using DebugToolkit.ReportForm.Utils;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.CloudCode;
using Unity.Services.CloudCode.GeneratedBindings;
using UnityEngine;
using UnityEngine.Networking;

namespace DebugToolkit.ReportForm.APIs.Discord
{
    public class DiscordAPI
    {
        public DiscordAPI()
        {
            _discordThreadsBindings = new DiscordThreadsBindings(CloudCodeService.Instance);
        }

        private DiscordThreadsBindings _discordThreadsBindings;

        public async Task SendReport(string title, string userDescription, string screenShotPath)
        {
            string systemSpecs = ReportUtils.GetSystemSpecs();
            string logPath = ReportUtils.GetPlayerLogPath();
            string fullDescription = $"📝 **User Description:**\n{userDescription}\n\n" +
                             systemSpecs;
            string logText = System.IO.File.Exists(logPath)
                ? System.IO.File.ReadAllText(logPath)
                : "Aucun fichier log trouvé.";

            byte[] screenShotData = File.ReadAllBytes(screenShotPath);
            Texture2D texture = new Texture2D(2, 2);
            bool isLoaded = texture.LoadImage(screenShotData);
            if (!isLoaded) Debug.LogError("Failed to load image!");

            byte[] compressedScreenShot = ReportUtils.CompressImage(texture, 512, 75);

            byte[] logData = ReportUtils.ReadTruncatedLog(logPath, 25_000);

            Debug.Log($"Image size: {compressedScreenShot.Length / 1024f:F2} KB");
            Debug.Log($"Log size: {logData.Length / 1024f:F2} KB");

            var result = await _discordThreadsBindings.PostThread(title, fullDescription,
                "ScreenShot.jpg", "screenShotPath/jpg", compressedScreenShot,
                "logs.txt", "logs/plain", logData);

            Debug.Log("Discord API result: " + result);
        }
    }
}
