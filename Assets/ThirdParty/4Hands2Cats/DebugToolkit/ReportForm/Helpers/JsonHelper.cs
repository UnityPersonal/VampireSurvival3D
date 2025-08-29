using System.Collections.Generic;
using System;
using UnityEngine;

namespace DebugToolkit.ReportForm
{
    public class JsonHelper
    {

        public static List<T> FromJson<T>(string json)
        {
            string newJson = "{\"array\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [Serializable]
        private class Wrapper<T>
        {
            public List<T> array;
        }

        public static string ExtractId(string jsonResponse)
        {
            int start = jsonResponse.IndexOf("\"id\":\"") + 6;
            int end = jsonResponse.IndexOf("\"", start);
            return jsonResponse.Substring(start, end - start);
        }
    }

    public static class JsonUtilityWrapper
    {
        public static T FromJson<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }
    }
}

