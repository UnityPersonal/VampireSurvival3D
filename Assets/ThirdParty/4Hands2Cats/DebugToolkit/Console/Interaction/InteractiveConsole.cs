using DebugToolkit.Interaction.Commands;
using DebugToolkit.Console.Log;
using DebugToolkit.Utils;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using DebugToolkit.Console.Interaction.AttributeSystem;
using DebugToolkit.Console.Interaction.Prediction;

namespace DebugToolkit.Console.Interaction
{
    public class InteractiveConsole : MonoBehaviour
    {
        [Header("Params")]
        [SerializeField] private int MaxShown;

        [Header("Refs")]
        [SerializeField] private TMP_InputField consoleInputField;
        [SerializeField] private ScrollRect scrollRect;
        [Space]
        [SerializeField] private PredictionPanel predictionPanel;

        [Header("Commands")]
        [SerializeField] private CommandMemory commandMemory;
        [SerializeField] private int maxCommandInMem;
        [Space]
        [SerializeField] private CommandContainer commandContainer;
        [SerializeField] private Command helpCommand;

        [Header("Inputs")]
        [SerializeField] private InputAction nextAction;
        [SerializeField] private InputAction previousAction;
        [SerializeField] private InputAction selectAction;

        private async void Awake()
        {
            consoleInputField.onSubmit.AddListener((text) => HandleCommand(text));
            consoleInputField.onValueChanged.AddListener((text) =>
            {
                predictionPanel.Eval(text);
            });
            helpCommand.OnCommandValid += SendHelp;
            commandMemory.Init(maxCommandInMem);

            nextAction.performed += HandleNext;
            previousAction.performed += HandlePrevious;
            selectAction.performed += i => predictionPanel.Select();

            commandContainer.Init();

            List<Command.CommandDto> commands = await commandContainer.GetCommands();
            predictionPanel.Init(commands);
            predictionPanel.OnCommandSelected += (text) =>
            {
                consoleInputField.ActivateInputField();
                consoleInputField.text = text;
                consoleInputField.caretPosition = consoleInputField.text.Length;
            };
        }

        private void OnDestroy()
        {
            helpCommand.OnCommandValid -= SendHelp;
        }

        private void OnEnable()
        {
            consoleInputField.ActivateInputField();
            nextAction.Enable();
            previousAction.Enable();
            selectAction.Enable();
        }

        private void OnDisable()
        {
            consoleInputField.DeactivateInputField();
            nextAction.Disable();
            previousAction.Disable();
            selectAction.Disable();
        }

        private async void HandlePrevious(InputAction.CallbackContext context)
        {
            if (predictionPanel.IsPredicting)
                predictionPanel.NavDown();
            else
                consoleInputField.text = commandMemory.GetPrevious();

            await Awaitable.NextFrameAsync();
            consoleInputField.caretPosition = consoleInputField.text.Length;
        }

        private async void HandleNext(InputAction.CallbackContext context)
        {
            if (predictionPanel.IsPredicting)
                predictionPanel.NavUp();
            else
                consoleInputField.text = commandMemory.GetNext();

            await Awaitable.NextFrameAsync();
            consoleInputField.caretPosition = consoleInputField.text.Length;
        }

        private async void HandleCommand(string text)
        {
            DebugLog.Log("<b>Com => </b>" + text, DebugLog.LogColor.Default, DebugLog.LogType.Command);

            commandContainer.Eval(text.TrimEnd());

            await Awaitable.NextFrameAsync();
            if (this == null) return;

            consoleInputField.text = "";
            consoleInputField.ActivateInputField();

            await Awaitable.NextFrameAsync();
            if (this == null) return;

            scrollRect.normalizedPosition = new Vector2(0, 0);
            commandMemory.Append(text);
        }

        private void SendHelp()
        {
            helpCommand.PrintHelp();
            commandContainer.PrintHelp(new List<Command.CommandDto> { helpCommand.DTO});
        }
    }
}

