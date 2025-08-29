using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugToolkit.Console.Log
{
    public class LogMessageControl : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private ContentSizeFitter fitter;

        public void ShowText(string text)
        {
            this.text.text = text;
        }

        public async void AppendBatch(List<string> logs, bool clearBefore = true)
        {
            if (this == null) return;
            text.text = clearBefore ? "" : text.text; 
            for (int i = 0; i < logs.Count; i++)
            {
                AppendLog(logs[i]);
            }

            fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
            await Awaitable.NextFrameAsync();

            if (this == null) return;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }

        internal async void AppendLog(string condition)
        {
            if (this == null) return;
            if (text.text.StartsWith("Console")) text.text = "";

            StringBuilder builder = new StringBuilder()
                .Append(text.text)
                .Append(condition)
                .Append("\n");
            text.text = builder.ToString();

            fitter.verticalFit = ContentSizeFitter.FitMode.MinSize;
            await Awaitable.NextFrameAsync();

            if (this == null) return;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
    }
}

