using DebugToolkit.Interaction.Commands;
using UnityEngine;
using UnityEngine.UI;

namespace DebugToolkit.Profiling
{
    public class MetricsManager : MonoBehaviour
    {
        [Header("Command")]
        [SerializeField] BooleanCommand metricCommand;

        [Header("Params")]
        [SerializeField] GameObject metricPrefab;
        private GameObject metric;

        [Header("UI")]
        [SerializeField] Button metricButton;
        bool metricEnabled = false;

        private void Awake()
        {
            metricCommand.OnIsValid += MetricCommand_OnIsValid;
            metricButton.onClick.AddListener(() =>
            {
                metricEnabled = !metricEnabled;
                MetricCommand_OnIsValid(metricEnabled);
            });
        }

        private void OnDestroy()
        {
            metricCommand.OnIsValid -= MetricCommand_OnIsValid;
            metricButton.onClick.RemoveAllListeners();
        }

        private void MetricCommand_OnIsValid(bool obj)
        {
            if (this == null) return;

            metricEnabled = obj;
            if (obj)
            {
                InstantiateMetric();
            }
            else
            {
                if(metric != null)
                    metric.GetComponent<Metrics>().Dispose();
            }
        }

        private void InstantiateMetric()
        {
            if (metric == null)
            {
                metric = Instantiate(metricPrefab);
            }
        }
    }
}

