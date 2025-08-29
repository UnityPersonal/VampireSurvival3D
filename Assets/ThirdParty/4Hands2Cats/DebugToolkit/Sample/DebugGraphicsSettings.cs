using DebugToolkit.Interaction.Commands;
using System;
using UnityEngine;

namespace DebugToolkit.Sample
{
    public class DebugGraphicsSettings : MonoBehaviour
    {
        [Header("Command")]
        [SerializeField] private EnumCommand graphicQualityCommand;

        private void Awake()
        {
            graphicQualityCommand.OnIsValid += HandleQualityChange;
        }

        private void OnDestroy()
        {
            graphicQualityCommand.OnIsValid -= HandleQualityChange;
        }

        private void HandleQualityChange(int newVal)
        {
            if (this == null) return;

            QualitySettings.SetQualityLevel(newVal);
        }
    }
}

