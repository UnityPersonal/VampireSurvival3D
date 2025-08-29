using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Security.Cryptography;
using System.Text;
using Unity.Profiling;

namespace DebugToolkit.Console.Log
{
    public class Logger : MonoBehaviour
    {
        [Header("Refs")]
        [SerializeField] private LogMessageControl consolLog;
        [SerializeField] private ScrollRect scrollRect;

        [Header("Filters")]
        [SerializeField] private List<LogContainer> logContainers;

        [Header("Dependencies")]
        [SerializeField] private ResearchBar researchBar;

        private int _index;
        private bool _collapsed;

        private void Awake()
        {
            for (int i = 0; i < logContainers.Count; i++)
            {
                logContainers[i].Init();
                logContainers[i].OnActivationChanged += FilterMessages;
            }

            DebugLog.OnLog += HandleLog;

            researchBar.Init(GetLogMessages);
            researchBar.OnResearch += (foundMessages) => 
            {
                consolLog.AppendBatch(foundMessages, true);
            };
            researchBar.OnCancel += () =>
            {
                FilterMessages();
            };
        }

        private void OnDestroy()
        {
            DebugLog.OnLog -= HandleLog;
        }

        public void Clear()
        {
            for (int i = 0; i < logContainers.Count; i++)
            {
                logContainers[i].Clear();
            }
            _index = 0;
            FilterMessages();
            researchBar.Clear();
        }

        [ContextMenu("collapse")]
        public void Collapse()
        {
            if (_collapsed)
            {
                _collapsed = false;
            }
            else
            {
                _collapsed = true;
            }
            FilterMessages();
        }

        private void FilterMessages()
        {

            List<string> messages = new List<string>();
            if (_collapsed)
            {
                List<CollapsedLog> collapsedLogs = GetCollapsedLogs();
                for (int i = 0; i <= _index; i++)
                {
                    for (int j = 0; j < collapsedLogs.Count; j++)
                    {
                        if (collapsedLogs[j].Order == i)
                        {
                            messages.Add($"{collapsedLogs[j].Amount}{collapsedLogs[j].Log.Message}");
                        }
                    }
                }
            }
            else
            {
                List<Log> logs = GetLogs();
                for (int i = 0; i <= _index; i++)
                {
                    for (int j = 0; j < logs.Count; j++)
                    {
                        if (logs[j].Order == i)
                        {
                            messages.Add(logs[j].Message);
                        }
                    }
                }
            }

            if (!researchBar.ReEval())
                consolLog.AppendBatch(messages, true);
        }

        private async void HandleLog(string condition, DebugLog.LogType type)
        {
            if (this == null) return;
            if (String.IsNullOrEmpty(condition)) return;
            for (int i = 0; i < logContainers.Count; i++)
            {
                if (logContainers[i].IsActive && logContainers[i].LogType == type)
                {
                    logContainers[i].AddLog(condition, _index);
                    if (logContainers[i].IsActive)
                        consolLog.AppendLog(condition);
                    break;
                }
            }
            _index++;

            await Awaitable.NextFrameAsync();
            if (this == null) return;

            researchBar.ReEval();
            scrollRect.normalizedPosition = new Vector2(0, 0);
        }

        internal void HandleApplicationLog(string condition, string stackTrace, LogType type)
        {
            if (condition.StartsWith(">_")) return;
            switch (type)
            {
                case LogType.Error:
                    DebugLog.Log(condition, DebugLog.LogColor.Red, DebugLog.LogType.Error);
                    break;
                case LogType.Assert:
                    DebugLog.Log(condition, DebugLog.LogColor.Red, DebugLog.LogType.Error);
                    break;
                case LogType.Warning:
                    DebugLog.Log(condition, DebugLog.LogColor.Yellow, DebugLog.LogType.Warning);
                    break;
                case LogType.Log:
                    DebugLog.Log(condition, DebugLog.LogColor.White, DebugLog.LogType.Log);
                    break;
                case LogType.Exception:
                    DebugLog.Log(condition, DebugLog.LogColor.Red, DebugLog.LogType.Error);
                    break;
            }
        }

        private List<string> GetLogMessages()
        {
            List<string> logs = new List<string>();
            for (int i = 0; i < logContainers.Count; i++)
            {
                if (logContainers[i].IsActive)
                {
                    for(int j = 0; j < logContainers[i].Logs.Count; j++)
                        logs.Add(logContainers[i].Logs[j].Message);
                }
            }

            return logs;
        }

        private List<Log> GetLogs()
        {
            List<Log> logs = new List<Log>();
            for (int i = 0; i < logContainers.Count; i++)
            {
                if (logContainers[i].IsActive)
                {
                    logs.AddRange(logContainers[i].Logs);
                }
            }

            return logs;
        }

        private List<CollapsedLog> GetCollapsedLogs()
        {
            List<CollapsedLog> logs = new List<CollapsedLog>();
            for (int i = 0; i < logContainers.Count; i++)
            {
                if (logContainers[i].IsActive)
                {
                    logs.AddRange(logContainers[i].CollapsedLogs);
                }
            }

            return logs;
        }

        [Serializable]
        public class LogContainer
        {
            [SerializeField] private LoggerFilterButton filterButton;
            [SerializeField] private DebugLog.LogType logType;
            public DebugLog.LogType LogType => logType;

            private List<CollapsedLog> _collapsedLogs = new();
            private List<Log> _logs = new List<Log>();

            public List<Log> Logs => _logs;
            public List<CollapsedLog> CollapsedLogs => _collapsedLogs;


            private bool isActive;
            public bool IsActive => isActive;

            public event Action OnActivationChanged;

            public void AddLog(string message, int order)
            {
                string hash = CalculateSHA256(message);
                bool _exist = false;

                for (int i = 0; i < _collapsedLogs.Count; i++)
                {
                    if (hash == _collapsedLogs[i].Hash)
                    {
                        _exist = true;
                        _collapsedLogs[i].Add(order);
                        break;
                    }
                }

                Log newLog = new(message, order);
                if (!_exist)
                {
                    _collapsedLogs.Add(new CollapsedLog(newLog, hash));
                }

                _logs.Add(newLog);
                if (_logs.Count > 999) filterButton.UpdateText("999+");
                else filterButton.UpdateText(_logs.Count.ToString());
            }

            public void SetActive(bool active)
            {
                isActive = active;
                OnActivationChanged?.Invoke();
            }

            internal void Init()
            {
                isActive = true;
                filterButton.OnActivation += SetActive;
            }

            internal void Clear()
            {
                _logs.Clear();
                _collapsedLogs.Clear();
                filterButton.UpdateText("0");
                isActive = true;
            }

            private string CalculateSHA256(string input)
            {
                using (var sha256 = SHA256.Create())
                {
                    byte[] data = Encoding.UTF8.GetBytes(input);
                    byte[] hashBytes = sha256.ComputeHash(data);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
                }
            }
        }

        public class CollapsedLog
        {
            public CollapsedLog(Log log, string hash)
            {
                _log = log;
                _hash = hash;
                _amount = 1;
                _order = _log.Order;
            }

            private string _hash;
            public string Hash => _hash;
            private Log _log;
            public Log Log => _log;
            private int _amount;
            public int Amount => _amount;
            private int _order;
            public int Order => _order;

            internal void Add(int order)
            {
                _order = (_amount * _order + order) / (_amount + 1);
                _amount++;
            }
        }

        [Serializable]
        public class Log
        {
            public Log(string message, int order)
            {
                this.message = message;
                this.order = order;
            }

            private string message;
            private int order;

            public string Message => message;   
            public int Order => order;
        }
    }
}

