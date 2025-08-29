using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DebugToolkit.ReportForm.Utils
{
    public static class ReportUtils
    {
        public static string GetSystemSpecs()
        {
            return $"🖥️ **System Specs:**\n" +
                   $"• OS: {SystemInfo.operatingSystem}\n" +
                   $"• CPU: {SystemInfo.processorType} ({SystemInfo.processorCount} cores)\n" +
                   $"• RAM: {SystemInfo.systemMemorySize} MB\n" +
                   $"• GPU: {SystemInfo.graphicsDeviceName} ({SystemInfo.graphicsMemorySize} MB VRAM)\n";
        }

        public static string GetPlayerLogPath()
        {
#if UNITY_STANDALONE_WIN
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "..", "LocalLow", Application.companyName, Application.productName, "Player.log");
#elif UNITY_STANDALONE_OSX
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Logs", Application.companyName, Application.productName, "Player.log");
#elif UNITY_STANDALONE_LINUX
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".config", "unity3d", Application.companyName, Application.productName, "Player.log");
#elif UNITY_ANDROID
            return Path.Combine(Application.persistentDataPath, "Player.log");
#elif UNITY_IOS
            return Path.Combine(Application.persistentDataPath, "Player.log");
#else
            return null; 
#endif
        }

        public static byte[] CompressImage(Texture2D texture, int maxSize = 512, int jpegQuality = 75)
        {
            int width = texture.width;
            int height = texture.height;

            float scale = Mathf.Min((float)maxSize / width, (float)maxSize / height, 1f);
            int newWidth = Mathf.RoundToInt(width * scale);
            int newHeight = Mathf.RoundToInt(height * scale);

            RenderTexture rt = RenderTexture.GetTemporary(newWidth, newHeight);
            RenderTexture.active = rt;

            Graphics.Blit(texture, rt);

            Texture2D resized = new Texture2D(newWidth, newHeight, TextureFormat.RGB24, false);
            resized.ReadPixels(new Rect(0, 0, newWidth, newHeight), 0, 0);
            resized.Apply();

            RenderTexture.active = null;
            RenderTexture.ReleaseTemporary(rt);

            return resized.EncodeToJPG(jpegQuality);
        }

        private static Texture2D ResizeTexture(Texture2D original, int maxWidth, int maxHeight)
        {
            int width = original.width;
            int height = original.height;

            float scale = Mathf.Min((float)maxWidth / width, (float)maxHeight / height, 1f);
            int newWidth = Mathf.RoundToInt(width * scale);
            int newHeight = Mathf.RoundToInt(height * scale);

            Texture2D scaled = new Texture2D(newWidth, newHeight, original.format, false);
            Graphics.ConvertTexture(original, scaled);
            return scaled;
        }

        public static string ReadLastLines(string path, int maxLines = 500)
        {
            if (!File.Exists(path))
                return "[Log file not found]";

            var lines = File.ReadAllLines(path);
            int start = Mathf.Max(0, lines.Length - maxLines);
            return string.Join("\n", lines.Skip(start));
        }

        public static byte[] ReadTruncatedLog(string path, int maxBytes = 100_000)
        {
            if (!File.Exists(path))
                return Encoding.UTF8.GetBytes("[Log file not found]");

            var allBytes = File.ReadAllBytes(path);
            if (allBytes.Length <= maxBytes)
                return allBytes;

            var truncated = new byte[maxBytes];
            Array.Copy(allBytes, allBytes.Length - maxBytes, truncated, 0, maxBytes);
            return truncated;
        }
    }
}
