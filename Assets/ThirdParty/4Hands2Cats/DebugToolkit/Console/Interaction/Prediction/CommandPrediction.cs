using System;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;

namespace DebugToolkit.Interaction.Commands.Prediction
{
    public class CommandPrediction : MonoBehaviour
    {
        [Header("Ref")]
        [SerializeField] private TextMeshProUGUI _commandText;
        [Header("params")]
        [SerializeField] private Color selectedColor;
        private Color _defaultColor;

        private void Awake()
        {
            _defaultColor = _commandText.color;
        }

        public void SetCommandText(string command)
        {
            _commandText.text = command;
        }

        internal string GetValue()
        {
            return _commandText.text;
        }

        internal void Select()
        {
            _commandText.color = selectedColor;
        }

        internal void Deselect()
        {
            _commandText.color = _defaultColor;
        }
    }
}
