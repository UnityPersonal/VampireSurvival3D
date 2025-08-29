using System.Collections.Generic;
using System;
using UnityEngine;
using DebugToolkit.Console.Log;
using System.Text;

namespace DebugToolkit.Interaction.Commands
{
    [CreateAssetMenu(fileName = "EnumCommand", menuName = "DebugToolkit/Command/EnumCommand")]
    public class EnumCommand : Command
    {
        [Header("Enums args")]
        [SerializeField] private List<EnumArg> args = new List<EnumArg>();
        public override event Action OnCommandValid;

        internal override CommandDto Init()
        {
            commandDto = new EnumCommandDto(keyword, args);
            commandDto.OnCommandValid += () => OnCommandValid?.Invoke();
            ((EnumCommandDto)commandDto).OnIsValid += (val) => OnIsValid?.Invoke(val);

            return commandDto;
        }

        public event Action<int> OnIsValid;

        [Serializable]
        public class EnumArg
        {
            [SerializeField] private List<string> enumArg = new List<string>();
            [SerializeField] private int value;
            public int Value => value;
            public bool Eval(string arg)
            {
                for (int i = 0; i < enumArg.Count; i++)
                {
                    if (enumArg[i] == arg)
                        return true;
                }
                return false;
            }

            internal object GetName()
            {
                return enumArg[0];
            }
        }

        public class EnumCommandDto : CommandDto
        {
            private List<EnumArg> _args;
            public event Action<int> OnIsValid;

            public EnumCommandDto(List<string> keyword, List<EnumArg> args) : base(keyword)
            {
                _args = args;
            }

            public override string GetKeyword()
            {
                string baseString = base.GetKeyword() + " ";
                StringBuilder builder = new StringBuilder();

                for (int i = 0; i < _args.Count; i++)
                {
                    builder.Append(baseString).Append(_args[i].GetName());
                    
                    if (i != _args.Count - 1)                    
                        builder.Append("/");
                }

                return builder.ToString();
            }

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
                StringBuilder builder = new StringBuilder().Append("To change state of ")
                    .Append(_keyword[0]).Append("</b>, use : <b>").Append(_keyword[0])
                    .Append("followed by <b>");

                for (int i = 1; i < _args.Count; i++)
                {
                    builder.Append(_args[i].GetName()).Append(" ");
                    if (i != _args.Count - 1)
                        builder.Append("or ");
                }

                DebugLog.Log(builder.ToString(),
                        DebugLog.LogColor.White, DebugLog.LogType.Log
                );

            }

            private void EvalCommand(string[] keywords)
            {
                for (int i = 0; i < _args.Count; i++)
                {
                    if (_args[i].Eval(keywords[1]))
                    {
                        OnIsValid?.Invoke(_args[i].Value);
                        PrintSuccess(_args[i]);
                        return;
                    }
                }

                DebugLog.Log($"Something went Wrong",
                        DebugLog.LogColor.White, DebugLog.LogType.Log
                );
                PrintHelp();
            }

            private void PrintSuccess(EnumArg arg)
            {
                DebugLog.Log(message: $"Successfully set {_keyword[0]} to {arg.GetName()}",
                        DebugLog.LogColor.White, DebugLog.LogType.Log
                );
            }
        }
    }
}

