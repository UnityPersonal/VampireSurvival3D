using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Unity.Services.CloudCode.GeneratedBindings
{
    public class DiscordThreadsBindings
    {
        readonly ICloudCodeService k_Service;
        public DiscordThreadsBindings(ICloudCodeService service)
        {
            k_Service = service;
        }

        public async Task<object> PostThread(string title,
            string description,
            string imageFileName,
            string imageMimeType,
            byte[] imageBytes,
            string textFileName,
            string textMimeType,
            byte[] textBytes)
        {
            return await k_Service.CallModuleEndpointAsync<object>(
                "DiscordThreads",
                "PostToDiscordForum",
                new Dictionary<string, object>
                {
                    { "title", title },
                    { "description", description },
                    { "imageFileName", imageFileName },
                    { "imageMimeType", imageMimeType },
                    { "imageBase64", Convert.ToBase64String(imageBytes) },
                    { "textFileName", textFileName },
                    { "textFileMimeType", textMimeType },
                    { "textBase64", Convert.ToBase64String(textBytes) }
                });
        }
    }
}
