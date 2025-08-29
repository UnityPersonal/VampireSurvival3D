using System;
using UnityEngine;
using DebugToolkit.Console.Log;
using System.Globalization;
using System.Collections.Generic;
using System.Reflection;

namespace DebugToolkit.Interaction.Commands
{
    [CreateAssetMenu(fileName = "VectorCommand", menuName = "DebugToolkit/Command/VectorCommand")]
    public class VectorCommand : Command
    {
        public event Action<Vector4> OnVector4Inputed;
        public event Action<Vector3> OnVector3Inputed;
        public event Action<Vector2> OnVector2Inputed;
        public event Action<float> OnVector1Inputed;
        public override event Action OnCommandValid;

        internal override CommandDto Init()
        {
            commandDto = new VectorCommandDto(keyword);
            commandDto.OnCommandValid += () => OnCommandValid?.Invoke();
            ((VectorCommandDto)commandDto).OnVector1Inputed += (val) => OnVector1Inputed?.Invoke(val);
            ((VectorCommandDto)commandDto).OnVector2Inputed += (val) => OnVector2Inputed?.Invoke(val);
            ((VectorCommandDto)commandDto).OnVector3Inputed += (val) => OnVector3Inputed?.Invoke(val);
            ((VectorCommandDto)commandDto).OnVector4Inputed += (val) => OnVector4Inputed?.Invoke(val);

            return commandDto;
        }

        public class VectorCommandDto : CommandDto
        {
            public VectorCommandDto(List<string> keyword) : base(keyword)
            {
            }

            public VectorCommandDto(List<string> keyword, MethodInfo bindedMethod) : base(keyword, bindedMethod)
            {
            }

            public event Action<Vector4> OnVector4Inputed;
            public event Action<Vector3> OnVector3Inputed;
            public event Action<Vector2> OnVector2Inputed;
            public event Action<float> OnVector1Inputed;

            public override bool Evaluate(string prompt)
            {
                string[] keywords = prompt.Split(" ");
                if (!CheckValidity(keywords)) return false;

                _requestedLength = GetRequestedLength();
                if (keywords.Length > _requestedLength + 4 || keywords.Length < _requestedLength + 1)
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
                DebugLog.Log($"To set a vector for <b>{_keyword[0]}</b>, use : <b>{_keyword[0]}</b> " +
                    $"followed by  digits in the for x or x.x or x.xf\n",
                        DebugLog.LogColor.White, DebugLog.LogType.Log
                );

            }

            private void EvalCommand(string[] keywords)
            {
                if (keywords.Length == _requestedLength + 1)
                {
                    float val = ParseArg(keywords[_requestedLength], 1);
                    OnVector1Inputed?.Invoke(val);
                    if (_bindedMethod != null)
                        _bindedMethod.Invoke(null, new object[] { val });
                    PrintSuccess(val.ToString());

                }
                else if (keywords.Length == _requestedLength + 2)
                {
                    Vector2 vec2 = new Vector2(
                            ParseArg(keywords[_requestedLength], 1),
                            ParseArg(keywords[_requestedLength + 1], 2)
                        );
                    OnVector2Inputed?.Invoke(
                        vec2
                    );
                    if (_bindedMethod != null)
                        _bindedMethod.Invoke(null, new object[] { vec2 });
                    PrintSuccess(vec2.ToString());
                }
                else if (keywords.Length == _requestedLength + 3)
                {
                    Vector3 vec3 = new Vector3(
                           ParseArg(keywords[_requestedLength], 1),
                           ParseArg(keywords[_requestedLength + 1], 2),
                           ParseArg(keywords[_requestedLength + 2], 3)
                       );
                    OnVector3Inputed?.Invoke(
                        vec3
                    );
                    if (_bindedMethod != null)
                        _bindedMethod.Invoke(null, new object[] { vec3 });
                    PrintSuccess(vec3.ToString());
                }
                else if (keywords.Length == _requestedLength + 4)
                {
                    Vector4 vec4 = new Vector4(
                           ParseArg(keywords[_requestedLength], 1),
                           ParseArg(keywords[_requestedLength + 1], 2),
                           ParseArg(keywords[_requestedLength + 2], 3),
                           ParseArg(keywords[_requestedLength + 3], 4)
                       );
                    OnVector4Inputed?.Invoke(
                        vec4
                    );
                    if (_bindedMethod != null)
                        _bindedMethod.Invoke(null, new object[] { vec4 });
                    PrintSuccess(vec4.ToString());
                }
            }

            private float ParseArg(string arg, int index)
            {
                try
                {
                    string sanitizedInput = arg.TrimEnd('f', 'F');
                    float result = float.Parse(sanitizedInput, CultureInfo.InvariantCulture);

                    return result;
                }
                catch (FormatException)
                {
                    DebugLog.Log($"The format of the digit n{index} is wrong");
                    PrintHelp();
                }
                catch (OverflowException)
                {
                    DebugLog.Log($"The digit n{index} is too big");
                    PrintHelp();
                }

                return 0.0f;
            }

            private void PrintSuccess(string val)
            {
                DebugLog.Log($"Value {val} for {_keyword[0]} successfully set",
                        DebugLog.LogColor.White, DebugLog.LogType.Log
                );
            }
        }
    }
}

