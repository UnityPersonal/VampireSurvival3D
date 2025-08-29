using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DebugToolkit.Console.Log
{
    public class ResearchBar : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private TMP_InputField researchBar;
        [SerializeField] private Button cancel;

        private Func<List<string>> getLogs;

        private bool researchActive = false;

        private void Awake()
        {
            researchBar.onSubmit.AddListener((txt) => ValidateInput(txt));
            cancel.onClick.AddListener(() => CancelResearch());
        }

        public void Init(Func<List<string>> getLogs)
        {
            this.getLogs = getLogs;
        }

        public bool ReEval()
        {
            if (researchActive)
                ValidateInput(researchBar.text);
            return researchActive;
        }

        public event Action<List<string>> OnResearch;
        public event Action OnCancel;
        private void ValidateInput(string txt)
        {
            researchActive = true;
            List<string> logs = getLogs?.Invoke();
            List<string> foundLogs = logs
                .Where(log => log.ToString().Contains(txt, StringComparison.OrdinalIgnoreCase))
                .ToList();

            OnResearch?.Invoke(foundLogs);
        }

        private void CancelResearch()
        {
            Clear();
            OnCancel?.Invoke();
        }

        internal void Clear()
        {
            researchBar.text = "";
            researchActive = false;
        }
    }
}

