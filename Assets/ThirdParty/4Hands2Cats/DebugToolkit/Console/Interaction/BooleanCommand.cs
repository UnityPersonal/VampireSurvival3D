using System;
using System.Collections.Generic;
using UnityEngine;
using DebugToolkit.Console.Log;
using UnityEngine.InputSystem;
using System.Reflection;

namespace DebugToolkit.Interaction.Commands
{
    [CreateAssetMenu(fileName = "BooleanCommand", menuName = "DebugToolkit/Command/BooleanCommand")]
    public class BooleanCommand : Command
    {
        [Header("KeyWord")]
        [Space]
        [SerializeField] private List<string> trueWords;
        [SerializeField] private List<string> falseWords;

        public event Action<bool> OnIsValid;
        public override event Action OnCommandValid;

        internal override CommandDto Init()
        {
            commandDto = new BooleanCommandDto(keyword, trueWords, falseWords);
            commandDto.OnCommandValid += () => OnCommandValid?.Invoke();
            ((BooleanCommandDto)commandDto).OnIsValid += (bool isValid) => OnIsValid?.Invoke(isValid);

            return commandDto;
        }

        public class BooleanCommandDto : CommandDto
        {
            private List<string> _trueWords;
            private List<string> _falseWords;

            public BooleanCommandDto(List<string> keyword) : base(keyword) { }

            public BooleanCommandDto(List<string> keyword, List<string> trueWords, List<string> falseWords) : base(keyword)
            {
                this._trueWords = trueWords;
                this._falseWords = falseWords;
            }

            public BooleanCommandDto(List<string> keyword, List<string> trueWords, List<string> falseWords, MethodInfo bindedMetod) : base(keyword, bindedMetod)
            {
                this._trueWords = trueWords;
                this._falseWords = falseWords;
            }

            public override string GetKeyword()
            {
                string baseString = base.GetKeyword();
                string trueString = baseString + " " + _trueWords[0];
                string falseString = baseString + " " + _falseWords[0];
                return trueString + "/" + falseString;
            }

            public event Action<bool> OnIsValid;
            public override bool Evaluate(string prompt)
            {
                string[] keywords = prompt.Split(" ");
                if (!CheckValidity(keywords)) return false;

                if (keywords.Length != GetRequestedLength() + 1)
                {
                    DebugLog.Log($"Arguments mismatch",
                            DebugLog.LogColor.White, DebugLog.LogType.Log
                    );
                    PrintHelp();
                }
                else
                {
                    EvalCommand(keywords);
                }

                return true;
            }

            public override void PrintHelp()
            {
                DebugLog.Log($"To Enable/Disable <b>{_keyword[0]}</b>, use : <b>{_keyword[0]}</b> " +
                    $"followed by <b>{_trueWords[0]}</b> or <b>{_falseWords[0]}</b>\n",
                        DebugLog.LogColor.White, DebugLog.LogType.Log
                );

            }

            private void EvalCommand(string[] keywords)
            {
                for (int i = 0; i < _trueWords.Count; i++)
                {
                    if (_trueWords[i].ToLower() == keywords[_keyword.Count].ToLower() || _trueWords[i].StartsWith(keywords[_keyword.Count]))
                    {
                        OnIsValid?.Invoke(true);
                        if (_bindedMethod != null)
                            _bindedMethod.Invoke(null, new object[] {true});
                        DebugLog.Log($"Successfully activated {_keyword[0]}",
                                DebugLog.LogColor.White, DebugLog.LogType.Log
                        );
                        return;
                    }
                }

                for (int i = 0; i < _falseWords.Count; i++)
                {
                    if (_falseWords[i].ToLower() == keywords[_keyword.Count].ToLower() || _trueWords[i].StartsWith(keywords[_keyword.Count]))
                    {
                        OnIsValid?.Invoke(false);
                        if (_bindedMethod != null)
                            _bindedMethod.Invoke(null, new object[] { false });
                        DebugLog.Log($"Successfully deactivated {_keyword[0]}",
                                DebugLog.LogColor.White, DebugLog.LogType.Log
                        );
                        return;
                    }
                }

                DebugLog.Log($"Something went Wrong",
                        DebugLog.LogColor.White, DebugLog.LogType.Log
                );
                PrintHelp();
            }
        }
    }  
}

