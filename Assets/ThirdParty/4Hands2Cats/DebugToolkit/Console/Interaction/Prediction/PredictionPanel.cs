using DebugToolkit.Interaction.Commands.Prediction;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Pool;
using UnityEngine.UI;
using static DebugToolkit.Interaction.Commands.Command;

namespace DebugToolkit.Console.Interaction.Prediction
{
    public class PredictionPanel : MonoBehaviour
    {
        [Header("prefabs")]
        [SerializeField] private GameObject predictionPrefab;

        private ObjectPool<CommandPrediction> _predictionPool;
        private List<CommandPrediction> _commandPredictions = new();
        private List<CommandDto> _commands;
        private CommandTree _commandTree = new();

        public event Action<string> OnCommandSelected;
        public bool IsPredicting => _commandPredictions.Count > 0;

        private int _currentlySelectedIndex = 0;

        private void Awake()
        {
            _predictionPool = new ObjectPool<CommandPrediction>(() =>
                {
                    var prediction = Instantiate(predictionPrefab, transform).GetComponent<CommandPrediction>();
                    prediction.transform.SetAsFirstSibling();
                    prediction.GetComponent<Button>().onClick.AddListener(() => OnCommandSelected?.Invoke(prediction.GetValue()));
                    return prediction;
                },
                prediction => 
                {
                    prediction.gameObject.SetActive(true);
                },
                prediction =>
                {
                    prediction.SetCommandText("");
                    prediction.gameObject.SetActive(false);
                    prediction.Deselect();
                }
            );
        }

        public void Init(List<CommandDto> commands)
        {
            _commands = commands;

            foreach (var command in commands)
            {
                List<string> commandVariants = command.GetKeyword().Split("/").ToList();
                foreach (var variant in commandVariants)
                    _commandTree.Insert(command, variant);
            }
        }

        public void NavUp()
        {
            if (_commandPredictions.Count <= 1)
                _currentlySelectedIndex = 0;
            else
                _currentlySelectedIndex = (_currentlySelectedIndex + 1) % (_commandPredictions.Count - 1);
            SelectUIUpdate();
        }

        public void NavDown()
        {
            if (_commandPredictions.Count <= 1)
                _currentlySelectedIndex = 0;
            else
                _currentlySelectedIndex--;

            if (_currentlySelectedIndex < 0) _currentlySelectedIndex = (_commandPredictions.Count - 1);
            SelectUIUpdate();
        }

        public void SelectUIUpdate()
        {
            foreach (var prediction in _commandPredictions)
                prediction.Deselect();

            _commandPredictions[_currentlySelectedIndex].Select();
        }

        public void Select()
        {
            Debug.Log("select");
            if (_commandPredictions.Count == 0) return;
            OnCommandSelected?.Invoke(_commandPredictions[_currentlySelectedIndex].GetValue());
        }

        public void Eval(string input)
        {
            foreach (var prediction in _commandPredictions)
                _predictionPool.Release(prediction);

            _commandPredictions.Clear();

            if (string.IsNullOrWhiteSpace(input))
                return;

            input = input.ToLower();

            List<CommandDto> matches = _commandTree.SearchPrefix(input).Distinct().ToList();

            foreach (var command in matches)
            {
                List<string> commandVariants = command.GetKeyword().Split("/").ToList();
                foreach (var variant in commandVariants)
                {
                    if (variant.StartsWith(input))
                    {
                        var prediction = _predictionPool.Get();
                        prediction.SetCommandText(variant);
                        _commandPredictions.Add(prediction);
                    }
                }
            }

            if (_commandPredictions.Count <= 1)
                _currentlySelectedIndex = 0;
            else
            {
                _currentlySelectedIndex %= (_commandPredictions.Count - 1);
            }
        }
    }

    public class TreeNode
    {
        public Dictionary<char, TreeNode> Children = new();
        public bool IsEndOfCommand;
        public CommandDto Command;
    }

    public class CommandTree
    {
        private readonly TreeNode _root = new();

        public void Insert(CommandDto command, string commandString)
        {
            if (command == null) return;

            commandString = commandString.Replace(" ", "*");
            TreeNode node = _root;
            string keyword = commandString;

            foreach (char c in keyword)
            {
                if (!node.Children.ContainsKey(c))
                    node.Children[c] = new TreeNode();

                node = node.Children[c];
            }
            node.IsEndOfCommand = true;
            node.Command = command;
        }

        public List<CommandDto> SearchPrefix(string prefix)
        {
            prefix = prefix.Replace(" ", "*");

            TreeNode node = _root;
            List<CommandDto> results = new();

            foreach (char c in prefix)
            {
                if (!node.Children.ContainsKey(c))
                    return results; 

                node = node.Children[c];
            }

            CollectAllCommands(node, results);
            return results;
        }

        private void CollectAllCommands(TreeNode node, List<CommandDto> results)
        {
            if (node.IsEndOfCommand)
                results.Add(node.Command);

            foreach (var child in node.Children.Values)
                CollectAllCommands(child, results);
        }
    }
}

